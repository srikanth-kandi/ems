using EMS.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("employees")]
    [Authorize(Roles = "Admin,HR,Manager")]
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
    [Authorize(Roles = "Admin,HR")]
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

    // PDF Report Endpoints
    [HttpGet("employees/pdf")]
    public async Task<IActionResult> GetEmployeesPdf()
    {
        var bytes = await _reportService.GenerateEmployeeDirectoryReportPdfAsync();
        return File(bytes, "application/pdf", "employees.pdf");
    }

    [HttpGet("departments/pdf")]
    public async Task<IActionResult> GetDepartmentsPdf()
    {
        var bytes = await _reportService.GenerateDepartmentReportPdfAsync();
        return File(bytes, "application/pdf", "departments.pdf");
    }

    [HttpGet("attendance/pdf")]
    public async Task<IActionResult> GetAttendancePdf([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var bytes = await _reportService.GenerateAttendanceReportPdfAsync(startDate, endDate);
        return File(bytes, "application/pdf", "attendance.pdf");
    }

    [HttpGet("salaries/pdf")]
    public async Task<IActionResult> GetSalariesPdf()
    {
        var bytes = await _reportService.GenerateSalaryReportPdfAsync();
        return File(bytes, "application/pdf", "salaries.pdf");
    }

    [HttpGet("hiring-trends/pdf")]
    public async Task<IActionResult> GetHiringTrendsPdf()
    {
        var bytes = await _reportService.GenerateHiringTrendReportPdfAsync();
        return File(bytes, "application/pdf", "hiring-trends.pdf");
    }

    [HttpGet("department-growth/pdf")]
    public async Task<IActionResult> GetDepartmentGrowthPdf()
    {
        var bytes = await _reportService.GenerateDepartmentGrowthReportPdfAsync();
        return File(bytes, "application/pdf", "department-growth.pdf");
    }

    [HttpGet("attendance-patterns/pdf")]
    public async Task<IActionResult> GetAttendancePatternsPdf()
    {
        var bytes = await _reportService.GenerateAttendancePatternReportPdfAsync();
        return File(bytes, "application/pdf", "attendance-patterns.pdf");
    }

    [HttpGet("performance-metrics/pdf")]
    public async Task<IActionResult> GetPerformanceMetricsPdf([FromQuery] int? employeeId = null)
    {
        var bytes = await _reportService.GeneratePerformanceMetricsReportPdfAsync(employeeId);
        return File(bytes, "application/pdf", "performance-metrics.pdf");
    }

    // Excel Report Endpoints
    [HttpGet("employees/excel")]
    public async Task<IActionResult> GetEmployeesExcel()
    {
        var bytes = await _reportService.GenerateEmployeeDirectoryReportExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "employees.xlsx");
    }

    [HttpGet("departments/excel")]
    public async Task<IActionResult> GetDepartmentsExcel()
    {
        var bytes = await _reportService.GenerateDepartmentReportExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "departments.xlsx");
    }

    [HttpGet("attendance/excel")]
    public async Task<IActionResult> GetAttendanceExcel([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var bytes = await _reportService.GenerateAttendanceReportExcelAsync(startDate, endDate);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "attendance.xlsx");
    }

    [HttpGet("salaries/excel")]
    public async Task<IActionResult> GetSalariesExcel()
    {
        var bytes = await _reportService.GenerateSalaryReportExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "salaries.xlsx");
    }

    [HttpGet("hiring-trends/excel")]
    public async Task<IActionResult> GetHiringTrendsExcel()
    {
        var bytes = await _reportService.GenerateHiringTrendReportExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "hiring-trends.xlsx");
    }

    [HttpGet("department-growth/excel")]
    public async Task<IActionResult> GetDepartmentGrowthExcel()
    {
        var bytes = await _reportService.GenerateDepartmentGrowthReportExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "department-growth.xlsx");
    }

    [HttpGet("attendance-patterns/excel")]
    public async Task<IActionResult> GetAttendancePatternsExcel()
    {
        var bytes = await _reportService.GenerateAttendancePatternReportExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "attendance-patterns.xlsx");
    }

    [HttpGet("performance-metrics/excel")]
    public async Task<IActionResult> GetPerformanceMetricsExcel([FromQuery] int? employeeId = null)
    {
        var bytes = await _reportService.GeneratePerformanceMetricsReportExcelAsync(employeeId);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "performance-metrics.xlsx");
    }
}


