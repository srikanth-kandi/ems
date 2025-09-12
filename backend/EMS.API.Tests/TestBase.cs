using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EMS.API.Data;
using EMS.API.Services;
using EMS.API.Interfaces;
using EMS.API.Repositories;
using Xunit;

namespace EMS.API.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly string _testDatabaseName = $"TestDatabase_{Guid.NewGuid()}";
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the real database
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<EMSDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add in-memory database with shared name for all tests in this factory instance
            services.AddDbContext<EMSDbContext>(options =>
            {
                options.UseInMemoryDatabase(_testDatabaseName);
                options.EnableSensitiveDataLogging();
                options.EnableServiceProviderCaching();
            });

            // Register test services
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IReportService, RefactoredReportService>();
            
            // Register all report generators
            services.AddScoped<EMS.API.Services.Reports.EmployeeDirectoryCsvGenerator>();
            services.AddScoped<EMS.API.Services.Reports.EmployeeDirectoryPdfGenerator>();
            services.AddScoped<EMS.API.Services.Reports.EmployeeDirectoryExcelGenerator>();
            services.AddScoped<EMS.API.Services.Reports.AttendanceExcelGenerator>();
            services.AddScoped<EMS.API.Services.Reports.AttendancePdfGenerator>();
            services.AddScoped<EMS.API.Services.Reports.HiringTrendCsvGenerator>();
            services.AddScoped<EMS.API.Services.Reports.DepartmentGrowthCsvGenerator>();
            services.AddScoped<EMS.API.Services.Reports.SalaryReportCsvGenerator>();
            services.AddScoped<EMS.API.Services.Reports.SalaryReportPdfGenerator>();
            services.AddScoped<EMS.API.Services.Reports.SalaryReportExcelGenerator>();
            services.AddScoped<EMS.API.Services.Reports.DepartmentReportCsvGenerator>();
            services.AddScoped<EMS.API.Services.Reports.DepartmentReportPdfGenerator>();
            services.AddScoped<EMS.API.Services.Reports.DepartmentReportExcelGenerator>();
            services.AddScoped<EMS.API.Services.Reports.AttendanceReportCsvGenerator>();
        });
    }
}

public class TestBase : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
{
    protected readonly TestWebApplicationFactory Factory;
    protected readonly HttpClient Client;
    private static readonly Dictionary<string, bool> _seededDatabases = new();

    public TestBase(TestWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await SeedTestDataAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected async Task SeedTestDataAsync()
    {
        using var scope = Factory.Services.CreateScope();
        
        var databaseName = scope.ServiceProvider.GetRequiredService<DbContextOptions<EMSDbContext>>()
            .FindExtension<Microsoft.EntityFrameworkCore.InMemory.Infrastructure.Internal.InMemoryOptionsExtension>()?.StoreName ?? "TestDatabase";
        
        if (_seededDatabases.ContainsKey(databaseName)) return;
        
        var context = scope.ServiceProvider.GetRequiredService<EMSDbContext>();
        
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed test data
        var departments = new List<EMS.API.Models.Department>
        {
            new() { Name = "Engineering", Description = "Software Development", ManagerName = "John Doe", CreatedAt = DateTime.UtcNow },
            new() { Name = "HR", Description = "Human Resources", ManagerName = "Jane Smith", CreatedAt = DateTime.UtcNow },
            new() { Name = "Finance", Description = "Financial Management", ManagerName = "Bob Johnson", CreatedAt = DateTime.UtcNow }
        };

        // Add departments first
        context.Departments.AddRange(departments);
        await context.SaveChangesAsync();

        var employees = new List<EMS.API.Models.Employee>
        {
            new() 
            { 
                FirstName = "Alice", 
                LastName = "Johnson", 
                Email = "alice@test.com", 
                PhoneNumber = "123-456-7890",
                Address = "123 Test St",
                DateOfBirth = new DateTime(1990, 1, 1),
                DateOfJoining = new DateTime(2020, 1, 1),
                Position = "Software Engineer",
                Salary = 75000,
                DepartmentId = departments[0].Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new() 
            { 
                FirstName = "Bob", 
                LastName = "Smith", 
                Email = "bob@test.com", 
                PhoneNumber = "123-456-7891",
                Address = "456 Test Ave",
                DateOfBirth = new DateTime(1985, 5, 15),
                DateOfJoining = new DateTime(2019, 6, 1),
                Position = "Senior Developer",
                Salary = 95000,
                DepartmentId = departments[0].Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new() 
            { 
                FirstName = "Carol", 
                LastName = "Davis", 
                Email = "carol@test.com", 
                PhoneNumber = "123-456-7892",
                Address = "789 Test Blvd",
                DateOfBirth = new DateTime(1992, 8, 20),
                DateOfJoining = new DateTime(2021, 3, 15),
                Position = "HR Manager",
                Salary = 80000,
                DepartmentId = departments[1].Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        // Add employees
        context.Employees.AddRange(employees);
        await context.SaveChangesAsync();

        var attendances = new List<EMS.API.Models.Attendance>
        {
            new() 
            { 
                EmployeeId = employees[0].Id, 
                CheckInTime = DateTime.UtcNow.AddHours(-8), 
                CheckOutTime = DateTime.UtcNow.AddHours(-1),
                TotalHours = TimeSpan.FromHours(7),
                Date = DateTime.UtcNow.Date,
                CreatedAt = DateTime.UtcNow
            },
            new() 
            { 
                EmployeeId = employees[1].Id, 
                CheckInTime = DateTime.UtcNow.AddHours(-9), 
                CheckOutTime = DateTime.UtcNow.AddHours(-2),
                TotalHours = TimeSpan.FromHours(7),
                Date = DateTime.UtcNow.Date,
                CreatedAt = DateTime.UtcNow
            }
        };

        var users = new List<EMS.API.Models.User>
        {
            new() 
            { 
                Username = "admin", 
                Email = "admin@test.com", 
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            },
            new() 
            { 
                Username = "user", 
                Email = "user@test.com", 
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = "User",
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Attendances.AddRange(attendances);
        context.Users.AddRange(users);
        await context.SaveChangesAsync();
        
        _seededDatabases[databaseName] = true;
    }
}
