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
}


