using System.Text;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class AttendanceReportCsvGenerator : BaseReportGenerator
{
    public AttendanceReportCsvGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        return await GenerateAsync(null, null);
    }

    public override async Task<byte[]> GenerateAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var cacheKey = $"attendance_report_csv_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";
        return await GetCachedOrGenerateAsync(cacheKey, () => GenerateReport(startDate, endDate));
    }

    public override string GetContentType() => "text/csv";
    public override string GetFileExtension() => "csv";

    private async Task<byte[]> GenerateReport(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Attendances
            .Include(a => a.Employee)
            .ThenInclude(e => e.Department)
            .AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(a => a.Date >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(a => a.Date <= endDate.Value);
        }

        var attendanceData = await query
            .Select(a => new
            {
                a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FirstName + " " + a.Employee.LastName,
                Department = a.Employee.Department.Name,
                a.Date,
                a.CheckInTime,
                a.CheckOutTime,
                a.TotalHours,
                a.CreatedAt
            })
            .OrderBy(a => a.Date)
            .ThenBy(a => a.EmployeeName)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Id,EmployeeId,EmployeeName,Department,Date,CheckInTime,CheckOutTime,TotalHours,CreatedAt");
        
        foreach (var attendance in attendanceData)
        {
            sb.AppendLine($"{attendance.Id},{attendance.EmployeeId},{attendance.EmployeeName},{attendance.Department},{attendance.Date:yyyy-MM-dd},{attendance.CheckInTime:HH:mm:ss},{attendance.CheckOutTime:HH:mm:ss},{attendance.TotalHours},{attendance.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        }
        
        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
