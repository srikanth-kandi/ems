using System.Text;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class DepartmentReportCsvGenerator : BaseReportGenerator
{
    public DepartmentReportCsvGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "department_report_csv";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "text/csv";
    public override string GetFileExtension() => "csv";

    private async Task<byte[]> GenerateReport()
    {
        var departmentData = await _context.Departments
            .Select(d => new
            {
                d.Id,
                d.Name,
                d.Description,
                d.ManagerName,
                d.CreatedAt,
                EmployeeCount = d.Employees.Count(e => e.IsActive),
                TotalSalary = d.Employees.Where(e => e.IsActive).Sum(e => e.Salary)
            })
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Id,Name,Description,ManagerName,CreatedAt,EmployeeCount,TotalSalary");
        
        foreach (var dept in departmentData)
        {
            sb.AppendLine($"{dept.Id},{dept.Name},{dept.Description},{dept.ManagerName},{dept.CreatedAt:yyyy-MM-dd},{dept.EmployeeCount},{dept.TotalSalary}");
        }
        
        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
