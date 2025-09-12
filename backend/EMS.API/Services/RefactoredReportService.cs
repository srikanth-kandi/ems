using EMS.API.Interfaces;
using EMS.API.Services.Reports;

namespace EMS.API.Services;

public class RefactoredReportService : IReportService
{
    private readonly EmployeeDirectoryCsvGenerator _employeeCsvGenerator;
    private readonly EmployeeDirectoryPdfGenerator _employeePdfGenerator;
    private readonly EmployeeDirectoryExcelGenerator _employeeExcelGenerator;
    private readonly AttendanceExcelGenerator _attendanceExcelGenerator;
    private readonly AttendancePdfGenerator _attendancePdfGenerator;
    private readonly HiringTrendCsvGenerator _hiringTrendCsvGenerator;
    private readonly HiringTrendPdfGenerator _hiringTrendPdfGenerator;
    private readonly HiringTrendExcelGenerator _hiringTrendExcelGenerator;
    private readonly DepartmentGrowthCsvGenerator _departmentGrowthCsvGenerator;
    private readonly DepartmentGrowthPdfGenerator _departmentGrowthPdfGenerator;
    private readonly DepartmentGrowthExcelGenerator _departmentGrowthExcelGenerator;
    private readonly AttendancePatternCsvGenerator _attendancePatternCsvGenerator;
    private readonly AttendancePatternPdfGenerator _attendancePatternPdfGenerator;
    private readonly AttendancePatternExcelGenerator _attendancePatternExcelGenerator;
    private readonly PerformanceMetricsCsvGenerator _performanceMetricsCsvGenerator;
    private readonly PerformanceMetricsPdfGenerator _performanceMetricsPdfGenerator;
    private readonly PerformanceMetricsExcelGenerator _performanceMetricsExcelGenerator;
    private readonly SalaryReportCsvGenerator _salaryReportCsvGenerator;
    private readonly SalaryReportPdfGenerator _salaryReportPdfGenerator;
    private readonly SalaryReportExcelGenerator _salaryReportExcelGenerator;
    private readonly DepartmentReportCsvGenerator _departmentReportCsvGenerator;
    private readonly DepartmentReportPdfGenerator _departmentReportPdfGenerator;
    private readonly DepartmentReportExcelGenerator _departmentReportExcelGenerator;
    private readonly AttendanceReportCsvGenerator _attendanceReportCsvGenerator;

    public RefactoredReportService(
        EmployeeDirectoryCsvGenerator employeeCsvGenerator,
        EmployeeDirectoryPdfGenerator employeePdfGenerator,
        EmployeeDirectoryExcelGenerator employeeExcelGenerator,
        AttendanceExcelGenerator attendanceExcelGenerator,
        AttendancePdfGenerator attendancePdfGenerator,
        HiringTrendCsvGenerator hiringTrendCsvGenerator,
        HiringTrendPdfGenerator hiringTrendPdfGenerator,
        HiringTrendExcelGenerator hiringTrendExcelGenerator,
        DepartmentGrowthCsvGenerator departmentGrowthCsvGenerator,
        DepartmentGrowthPdfGenerator departmentGrowthPdfGenerator,
        DepartmentGrowthExcelGenerator departmentGrowthExcelGenerator,
        AttendancePatternCsvGenerator attendancePatternCsvGenerator,
        AttendancePatternPdfGenerator attendancePatternPdfGenerator,
        AttendancePatternExcelGenerator attendancePatternExcelGenerator,
        PerformanceMetricsCsvGenerator performanceMetricsCsvGenerator,
        PerformanceMetricsPdfGenerator performanceMetricsPdfGenerator,
        PerformanceMetricsExcelGenerator performanceMetricsExcelGenerator,
        SalaryReportCsvGenerator salaryReportCsvGenerator,
        SalaryReportPdfGenerator salaryReportPdfGenerator,
        SalaryReportExcelGenerator salaryReportExcelGenerator,
        DepartmentReportCsvGenerator departmentReportCsvGenerator,
        DepartmentReportPdfGenerator departmentReportPdfGenerator,
        DepartmentReportExcelGenerator departmentReportExcelGenerator,
        AttendanceReportCsvGenerator attendanceReportCsvGenerator)
    {
        _employeeCsvGenerator = employeeCsvGenerator;
        _employeePdfGenerator = employeePdfGenerator;
        _employeeExcelGenerator = employeeExcelGenerator;
        _attendanceExcelGenerator = attendanceExcelGenerator;
        _attendancePdfGenerator = attendancePdfGenerator;
        _hiringTrendCsvGenerator = hiringTrendCsvGenerator;
        _hiringTrendPdfGenerator = hiringTrendPdfGenerator;
        _hiringTrendExcelGenerator = hiringTrendExcelGenerator;
        _departmentGrowthCsvGenerator = departmentGrowthCsvGenerator;
        _departmentGrowthPdfGenerator = departmentGrowthPdfGenerator;
        _departmentGrowthExcelGenerator = departmentGrowthExcelGenerator;
        _attendancePatternCsvGenerator = attendancePatternCsvGenerator;
        _attendancePatternPdfGenerator = attendancePatternPdfGenerator;
        _attendancePatternExcelGenerator = attendancePatternExcelGenerator;
        _performanceMetricsCsvGenerator = performanceMetricsCsvGenerator;
        _performanceMetricsPdfGenerator = performanceMetricsPdfGenerator;
        _performanceMetricsExcelGenerator = performanceMetricsExcelGenerator;
        _salaryReportCsvGenerator = salaryReportCsvGenerator;
        _salaryReportPdfGenerator = salaryReportPdfGenerator;
        _salaryReportExcelGenerator = salaryReportExcelGenerator;
        _departmentReportCsvGenerator = departmentReportCsvGenerator;
        _departmentReportPdfGenerator = departmentReportPdfGenerator;
        _departmentReportExcelGenerator = departmentReportExcelGenerator;
        _attendanceReportCsvGenerator = attendanceReportCsvGenerator;
    }

    // CSV Reports
    public async Task<byte[]> GenerateEmployeeDirectoryReportAsync()
    {
        return await _employeeCsvGenerator.GenerateAsync();
    }

    public async Task<byte[]> GenerateDepartmentReportAsync()
    {
        return await _departmentReportCsvGenerator.GenerateAsync();
    }

    public async Task<byte[]> GenerateAttendanceReportAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        return await _attendanceReportCsvGenerator.GenerateAsync(startDate, endDate);
    }

    public async Task<byte[]> GenerateSalaryReportAsync()
    {
        return await _salaryReportCsvGenerator.GenerateAsync();
    }

    public async Task<byte[]> GenerateHiringTrendReportAsync()
    {
        return await _hiringTrendCsvGenerator.GenerateAsync();
    }

    public async Task<byte[]> GenerateDepartmentGrowthReportAsync()
    {
        return await _departmentGrowthCsvGenerator.GenerateAsync();
    }

    public async Task<byte[]> GenerateAttendancePatternReportAsync()
    {
        return await _attendancePatternCsvGenerator.GenerateAsync();
    }

    public async Task<byte[]> GeneratePerformanceMetricsReportAsync(int? employeeId = null)
    {
        return await _performanceMetricsCsvGenerator.GenerateAsync();
    }

    // PDF Reports
    public async Task<byte[]> GenerateEmployeeDirectoryReportPdfAsync()
    {
        return await _employeePdfGenerator.GenerateAsync();
    }

    public async Task<byte[]> GenerateDepartmentReportPdfAsync()
    {
        return await _departmentReportPdfGenerator.GenerateAsync();
    }

    public async Task<byte[]> GenerateAttendanceReportPdfAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        return await _attendancePdfGenerator.GenerateAsync(startDate, endDate);
    }

    public async Task<byte[]> GenerateSalaryReportPdfAsync()
    {
        return await _salaryReportPdfGenerator.GenerateAsync();
    }

    public async Task<byte[]> GenerateHiringTrendReportPdfAsync()
    {
        return await _hiringTrendPdfGenerator.GenerateAsync();
    }

    public async Task<byte[]> GenerateDepartmentGrowthReportPdfAsync()
    {
        return await _departmentGrowthPdfGenerator.GenerateAsync();
    }

    public async Task<byte[]> GenerateAttendancePatternReportPdfAsync()
    {
        return await _attendancePatternPdfGenerator.GenerateAsync();
    }

    public async Task<byte[]> GeneratePerformanceMetricsReportPdfAsync(int? employeeId = null)
    {
        return await _performanceMetricsPdfGenerator.GenerateAsync();
    }

    // Excel Reports
    public async Task<byte[]> GenerateEmployeeDirectoryReportExcelAsync()
    {
        return await _employeeExcelGenerator.GenerateAsync();
    }

    public async Task<byte[]> GenerateDepartmentReportExcelAsync()
    {
        return await _departmentReportExcelGenerator.GenerateAsync();
    }

    public async Task<byte[]> GenerateAttendanceReportExcelAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        return await _attendanceExcelGenerator.GenerateAsync(startDate, endDate);
    }

    public async Task<byte[]> GenerateSalaryReportExcelAsync()
    {
        return await _salaryReportExcelGenerator.GenerateAsync();
    }

    public async Task<byte[]> GenerateHiringTrendReportExcelAsync()
    {
        return await _hiringTrendExcelGenerator.GenerateAsync();
    }

    public async Task<byte[]> GenerateDepartmentGrowthReportExcelAsync()
    {
        return await _departmentGrowthExcelGenerator.GenerateAsync();
    }

    public async Task<byte[]> GenerateAttendancePatternReportExcelAsync()
    {
        return await _attendancePatternExcelGenerator.GenerateAsync();
    }

    public async Task<byte[]> GeneratePerformanceMetricsReportExcelAsync(int? employeeId = null)
    {
        return await _performanceMetricsExcelGenerator.GenerateAsync();
    }
}
