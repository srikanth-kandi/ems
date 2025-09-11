using System.Text;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class EmployeeDirectoryCsvGenerator : BaseReportGenerator
{
    public EmployeeDirectoryCsvGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "employee_directory_csv";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "text/csv";
    public override string GetFileExtension() => "csv";

    private async Task<byte[]> GenerateReport()
    {
        var data = await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.IsActive)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Id,FirstName,LastName,Email,Department,Position,Salary");
        
        foreach (var employee in data)
        {
            sb.AppendLine($"{employee.Id},{employee.FirstName},{employee.LastName},{employee.Email},{employee.Department.Name},{employee.Position},{employee.Salary}");
        }
        
        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
