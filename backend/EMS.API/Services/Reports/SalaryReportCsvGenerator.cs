using System.Text;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class SalaryReportCsvGenerator : BaseReportGenerator
{
    public SalaryReportCsvGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "salary_report_csv";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "text/csv";
    public override string GetFileExtension() => "csv";

    private async Task<byte[]> GenerateReport()
    {
        var salaryData = await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.IsActive)
            .Select(e => new
            {
                e.Id,
                e.FirstName,
                e.LastName,
                e.Email,
                e.Department.Name,
                e.Position,
                e.Salary,
                e.DateOfJoining
            })
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Id,FirstName,LastName,Email,Department,Position,Salary,DateOfJoining");
        
        foreach (var employee in salaryData)
        {
            sb.AppendLine($"{employee.Id},{employee.FirstName},{employee.LastName},{employee.Email},{employee.Name},{employee.Position},{employee.Salary},{employee.DateOfJoining:yyyy-MM-dd}");
        }
        
        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
