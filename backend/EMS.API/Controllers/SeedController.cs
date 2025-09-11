using EMS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class SeedController : ControllerBase
{
    private readonly SeedDataService _seedDataService;
    private readonly ILogger<SeedController> _logger;

    public SeedController(SeedDataService seedDataService, ILogger<SeedController> logger)
    {
        _seedDataService = seedDataService;
        _logger = logger;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> SeedData()
    {
        try
        {
            _logger.LogInformation("Starting database seeding process");
            await _seedDataService.SeedAllDataAsync();
            _logger.LogInformation("Database seeding completed successfully");
            
            return Ok(new { message = "Database seeded successfully", timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database seeding");
            return StatusCode(500, new { message = "Error occurred during seeding", error = ex.Message });
        }
    }

    [HttpPost("reseed")]
    public async Task<IActionResult> ReseedData()
    {
        try
        {
            _logger.LogInformation("Starting database reseeding process");
            await _seedDataService.ReseedAllDataAsync();
            _logger.LogInformation("Database reseeding completed successfully");
            
            return Ok(new { message = "Database reseeded successfully", timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database reseeding");
            return StatusCode(500, new { message = "Error occurred during reseeding", error = ex.Message });
        }
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearData()
    {
        try
        {
            _logger.LogInformation("Starting database clearing process");
            await _seedDataService.ClearAllDataAsync();
            _logger.LogInformation("Database cleared successfully");
            
            return Ok(new { message = "Database cleared successfully", timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database clearing");
            return StatusCode(500, new { message = "Error occurred during clearing", error = ex.Message });
        }
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetSeedStatus()
    {
        try
        {
            var status = new
            {
                Departments = await _seedDataService.GetDepartmentCountAsync(),
                Employees = await _seedDataService.GetEmployeeCountAsync(),
                Attendances = await _seedDataService.GetAttendanceCountAsync(),
                PerformanceMetrics = await _seedDataService.GetPerformanceMetricCountAsync(),
                Users = await _seedDataService.GetUserCountAsync(),
                Timestamp = DateTime.UtcNow
            };
            
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting seed status");
            return StatusCode(500, new { message = "Error occurred while getting status", error = ex.Message });
        }
    }
}
