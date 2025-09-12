using System.Text;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class HiringTrendCsvGenerator : BaseReportGenerator
{
    public HiringTrendCsvGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "hiring_trend_csv";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "text/csv";
    public override string GetFileExtension() => "csv";

    private async Task<byte[]> GenerateReport()
    {
        // Get hiring data by month for the last 12 months
        var startDate = DateTime.UtcNow.AddMonths(-12);
        var endDate = DateTime.UtcNow;

        var hiringData = await _context.Employees
            .Where(e => e.DateOfJoining >= startDate && e.DateOfJoining <= endDate)
            .GroupBy(e => new { e.DateOfJoining.Year, e.DateOfJoining.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Count = g.Count(),
                Department = g.First().Department.Name
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Year,Month,MonthName,Hires,Department");
        
        foreach (var item in hiringData)
        {
            var monthName = new DateTime(item.Year, item.Month, 1).ToString("MMMM");
            sb.AppendLine($"{item.Year},{item.Month},{monthName},{item.Count},{item.Department}");
        }
        
        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
