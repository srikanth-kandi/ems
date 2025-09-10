using EMS.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("employees")] 
    public async Task<IActionResult> Employees()
    {
        var bytes = await _reportService.GenerateEmployeeDirectoryReportAsync();
        return File(bytes, "text/csv", "employees.csv");
    }

    [HttpGet("departments")]
    public async Task<IActionResult> Departments()
    {
        var bytes = await _reportService.GenerateDepartmentReportAsync();
        return File(bytes, "text/csv", "departments.csv");
    }

    [HttpGet("attendance")]
    public async Task<IActionResult> Attendance([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var bytes = await _reportService.GenerateAttendanceReportAsync(startDate, endDate);
        return File(bytes, "text/csv", "attendance.csv");
    }

    [HttpGet("salaries")]
    public async Task<IActionResult> Salaries()
    {
        var bytes = await _reportService.GenerateSalaryReportAsync();
        return File(bytes, "text/csv", "salaries.csv");
    }

    [HttpGet("hiring-trends")]
    public async Task<IActionResult> HiringTrends()
    {
        var bytes = await _reportService.GenerateHiringTrendReportAsync();
        return File(bytes, "text/csv", "hiring-trends.csv");
    }

    [HttpGet("department-growth")]
    public async Task<IActionResult> DepartmentGrowth()
    {
        var bytes = await _reportService.GenerateDepartmentGrowthReportAsync();
        return File(bytes, "text/csv", "department-growth.csv");
    }

    [HttpGet("attendance-patterns")]
    public async Task<IActionResult> AttendancePatterns()
    {
        var bytes = await _reportService.GenerateAttendancePatternReportAsync();
        return File(bytes, "text/csv", "attendance-patterns.csv");
    }

    [HttpGet("performance-metrics")]
    public async Task<IActionResult> PerformanceMetrics([FromQuery] int? employeeId = null)
    {
        var bytes = await _reportService.GeneratePerformanceMetricsReportAsync(employeeId);
        return File(bytes, "text/csv", "performance-metrics.csv");
    }
}


