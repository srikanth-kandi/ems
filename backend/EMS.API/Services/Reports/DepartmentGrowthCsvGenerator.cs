using System.Text;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class DepartmentGrowthCsvGenerator : BaseReportGenerator
{
    public DepartmentGrowthCsvGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "department_growth_csv";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "text/csv";
    public override string GetFileExtension() => "csv";

    private async Task<byte[]> GenerateReport()
    {
        // Get department growth data by month for the last 12 months
        var startDate = DateTime.UtcNow.AddMonths(-12);
        var endDate = DateTime.UtcNow;

        var departmentGrowth = await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.DateOfJoining >= startDate && e.DateOfJoining <= endDate)
            .GroupBy(e => new { e.Department.Name, Year = e.DateOfJoining.Year, Month = e.DateOfJoining.Month })
            .Select(g => new
            {
                Department = g.Key.Name,
                Year = g.Key.Year,
                Month = g.Key.Month,
                NewHires = g.Count()
            })
            .OrderBy(x => x.Department)
            .ThenBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Department,Year,Month,MonthName,NewHires");
        
        foreach (var item in departmentGrowth)
        {
            var monthName = new DateTime(item.Year, item.Month, 1).ToString("MMMM");
            sb.AppendLine($"{item.Department},{item.Year},{item.Month},{monthName},{item.NewHires}");
        }
        
        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
