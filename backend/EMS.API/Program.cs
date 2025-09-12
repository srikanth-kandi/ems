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
        new MySqlServerVersion(new Version(8, 0, 21))
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
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "https://ems.srikanthkandi.tech")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Setup database on startup
using (var scope = app.Services.CreateScope())
{
    var migrationService = scope.ServiceProvider.GetRequiredService<DatabaseMigrationService>();
    var seedingService = scope.ServiceProvider.GetRequiredService<DatabaseSeedingService>();
    
    try
    {
        // Apply migrations
        await migrationService.MigrateDatabaseAsync();
        
        // Seed database if empty
        await seedingService.SeedDatabaseAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while setting up the database.");
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
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck")
   .WithOpenApi();

app.Run();

// Make Program class accessible for testing
public partial class Program { }