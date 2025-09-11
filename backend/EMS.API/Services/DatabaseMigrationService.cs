using EMS.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EMS.API.Services;

public class DatabaseMigrationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseMigrationService> _logger;

    public DatabaseMigrationService(IServiceProvider serviceProvider, ILogger<DatabaseMigrationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task MigrateDatabaseAsync()
    {
        _logger.LogInformation("Starting database migration...");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<EMSDbContext>();

            // Apply pending migrations
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                _logger.LogInformation("Applying {Count} pending migrations: {Migrations}", 
                    pendingMigrations.Count(), string.Join(", ", pendingMigrations));
                
                await context.Database.MigrateAsync();
                _logger.LogInformation("Database migration completed successfully.");
            }
            else
            {
                _logger.LogInformation("No pending migrations found.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database migration.");
            throw;
        }
    }

    public async Task EnsureDatabaseCreatedAsync()
    {
        _logger.LogInformation("Ensuring database is created...");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<EMSDbContext>();

            await context.Database.EnsureCreatedAsync();
            _logger.LogInformation("Database creation check completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database creation check.");
            throw;
        }
    }

    public async Task DropDatabaseAsync()
    {
        _logger.LogInformation("Dropping database...");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<EMSDbContext>();

            await context.Database.EnsureDeletedAsync();
            _logger.LogInformation("Database dropped successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while dropping database.");
            throw;
        }
    }

    public async Task ResetDatabaseAsync()
    {
        _logger.LogInformation("Resetting database...");

        try
        {
            await DropDatabaseAsync();
            await MigrateDatabaseAsync();
            _logger.LogInformation("Database reset completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database reset.");
            throw;
        }
    }
}
