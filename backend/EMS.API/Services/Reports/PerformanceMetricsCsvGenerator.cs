using System.Text;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class PerformanceMetricsCsvGenerator : BaseReportGenerator
{
    public PerformanceMetricsCsvGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "performance_metrics_csv";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "text/csv";
    public override string GetFileExtension() => "csv";

    private async Task<byte[]> GenerateReport()
    {
        var performanceMetrics = await _context.PerformanceMetrics
            .Include(pm => pm.Employee)
            .ThenInclude(e => e.Department)
            .OrderBy(pm => pm.Employee.Department.Name)
            .ThenBy(pm => pm.Employee.LastName)
            .ThenBy(pm => pm.Year)
            .ThenBy(pm => pm.Quarter)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("EmployeeId,EmployeeName,Department,Year,Quarter,PerformanceScore,Comments,Goals,Achievements,CreatedAt");
        
        foreach (var metric in performanceMetrics)
        {
            var comments = metric.Comments?.Replace("\"", "\"\"") ?? "";
            var goals = metric.Goals?.Replace("\"", "\"\"") ?? "";
            var achievements = metric.Achievements?.Replace("\"", "\"\"") ?? "";
            
            sb.AppendLine($"\"{metric.EmployeeId}\",\"{metric.Employee.FirstName} {metric.Employee.LastName}\",\"{metric.Employee.Department.Name}\",\"{metric.Year}\",\"{metric.Quarter}\",\"{metric.PerformanceScore}\",\"{comments}\",\"{goals}\",\"{achievements}\",\"{metric.CreatedAt:yyyy-MM-dd}\"");
        }
        
        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
