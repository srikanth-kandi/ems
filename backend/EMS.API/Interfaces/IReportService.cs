namespace EMS.API.Interfaces;

public interface IReportService
{
    Task<byte[]> GenerateEmployeeDirectoryReportAsync();
    Task<byte[]> GenerateDepartmentReportAsync();
    Task<byte[]> GenerateAttendanceReportAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<byte[]> GenerateSalaryReportAsync();
    Task<byte[]> GenerateHiringTrendReportAsync();
    Task<byte[]> GenerateDepartmentGrowthReportAsync();
    Task<byte[]> GenerateAttendancePatternReportAsync();
    Task<byte[]> GeneratePerformanceMetricsReportAsync(int? employeeId = null);
}
