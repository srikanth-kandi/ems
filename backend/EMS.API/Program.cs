using EMS.API.Data;
using EMS.API.Interfaces;
using EMS.API.Repositories;
using EMS.API.Services;
using EMS.API.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Set EPPlus license context
OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure DateTime serialization to use UTC format with Z suffix
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        // Add custom DateTime converters to ensure UTC format with Z suffix
        options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
        options.JsonSerializerOptions.Converters.Add(new UtcNullableDateTimeConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

// Register repositories
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register report generators
builder.Services.AddScoped<EMS.API.Services.Reports.EmployeeDirectoryCsvGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.EmployeeDirectoryPdfGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.EmployeeDirectoryExcelGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.AttendanceExcelGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.AttendancePdfGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.HiringTrendCsvGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.HiringTrendPdfGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.HiringTrendExcelGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.DepartmentGrowthCsvGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.DepartmentGrowthPdfGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.DepartmentGrowthExcelGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.AttendancePatternCsvGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.AttendancePatternPdfGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.AttendancePatternExcelGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.PerformanceMetricsCsvGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.PerformanceMetricsPdfGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.PerformanceMetricsExcelGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.SalaryReportCsvGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.SalaryReportPdfGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.SalaryReportExcelGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.DepartmentReportCsvGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.DepartmentReportPdfGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.DepartmentReportExcelGenerator>();
builder.Services.AddScoped<EMS.API.Services.Reports.AttendanceReportCsvGenerator>();

// Register report service
builder.Services.AddScoped<IReportService, RefactoredReportService>();

// Register database services
builder.Services.AddScoped<DatabaseMigrationService>();
builder.Services.AddScoped<DatabaseSeedingService>();
builder.Services.AddScoped<SeedDataService>();

// Add Entity Framework
builder.Services.AddDbContext<EMSDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)
    ));

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!)),
        ValidateIssuer = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSection["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "https://ems.srikanthkandi.dev")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Setup database on startup (with retry logic)
using (var scope = app.Services.CreateScope())
{
    var migrationService = scope.ServiceProvider.GetRequiredService<DatabaseMigrationService>();
    var seedingService = scope.ServiceProvider.GetRequiredService<DatabaseSeedingService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    // Retry database setup with exponential backoff
    var maxRetries = 10;
    var retryDelay = TimeSpan.FromSeconds(5);
    
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            logger.LogInformation("Attempting database setup (attempt {Attempt}/{MaxRetries})", attempt, maxRetries);
            
            // Apply migrations
            await migrationService.MigrateDatabaseAsync();
            
            // Seed database if empty
            await seedingService.SeedDatabaseAsync();
            
            logger.LogInformation("Database setup completed successfully.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Database setup failed on attempt {Attempt}/{MaxRetries}. Retrying in {Delay} seconds...", 
                attempt, maxRetries, retryDelay.TotalSeconds);
            
            if (attempt == maxRetries)
            {
                logger.LogError(ex, "Database setup failed after {MaxRetries} attempts. Application will continue but database operations may fail.", maxRetries);
                break;
            }
            
            await Task.Delay(retryDelay);
            retryDelay = TimeSpan.FromSeconds(Math.Min(retryDelay.TotalSeconds * 1.5, 30)); // Exponential backoff, max 30 seconds
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", async (EMSDbContext context) =>
{
    try
    {
        // Check if database is accessible
        await context.Database.CanConnectAsync();
        return Results.Ok(new { Status = "Healthy", Database = "Connected", Timestamp = DateTime.UtcNow });
    }
    catch (Exception ex)
    {
        return Results.Ok(new { Status = "Degraded", Database = "Disconnected", Error = ex.Message, Timestamp = DateTime.UtcNow });
    }
})
   .WithName("HealthCheck")
   .WithOpenApi();

app.Run();

// Make Program class accessible for testing
public partial class Program { }