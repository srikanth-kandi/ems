namespace EMS.API.Interfaces;

public interface IReportService
{
    // CSV Reports
    Task<byte[]> GenerateEmployeeDirectoryReportAsync();
    Task<byte[]> GenerateDepartmentReportAsync();
    Task<byte[]> GenerateAttendanceReportAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<byte[]> GenerateSalaryReportAsync();
    Task<byte[]> GenerateHiringTrendReportAsync();
    Task<byte[]> GenerateDepartmentGrowthReportAsync();
    Task<byte[]> GenerateAttendancePatternReportAsync();
    Task<byte[]> GeneratePerformanceMetricsReportAsync(int? employeeId = null);
    
    // PDF Reports
    Task<byte[]> GenerateEmployeeDirectoryReportPdfAsync();
    Task<byte[]> GenerateDepartmentReportPdfAsync();
    Task<byte[]> GenerateAttendanceReportPdfAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<byte[]> GenerateSalaryReportPdfAsync();
    Task<byte[]> GenerateHiringTrendReportPdfAsync();
    Task<byte[]> GenerateDepartmentGrowthReportPdfAsync();
    Task<byte[]> GenerateAttendancePatternReportPdfAsync();
    Task<byte[]> GeneratePerformanceMetricsReportPdfAsync(int? employeeId = null);
    
    // Excel Reports
    Task<byte[]> GenerateEmployeeDirectoryReportExcelAsync();
    Task<byte[]> GenerateDepartmentReportExcelAsync();
    Task<byte[]> GenerateAttendanceReportExcelAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<byte[]> GenerateSalaryReportExcelAsync();
    Task<byte[]> GenerateHiringTrendReportExcelAsync();
    Task<byte[]> GenerateDepartmentGrowthReportExcelAsync();
    Task<byte[]> GenerateAttendancePatternReportExcelAsync();
    Task<byte[]> GeneratePerformanceMetricsReportExcelAsync(int? employeeId = null);
}
