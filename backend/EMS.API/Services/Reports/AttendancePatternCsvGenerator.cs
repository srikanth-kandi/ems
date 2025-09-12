using System.Text;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class AttendancePatternCsvGenerator : BaseReportGenerator
{
    public AttendancePatternCsvGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "attendance_pattern_csv";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "text/csv";
    public override string GetFileExtension() => "csv";

    private async Task<byte[]> GenerateReport()
    {
        // Get attendance patterns for the last 30 days
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;

        var attendancePatterns = await _context.Attendances
            .Include(a => a.Employee)
            .ThenInclude(e => e.Department)
            .Where(a => a.Date >= startDate && a.Date <= endDate)
            .GroupBy(a => new { 
                a.EmployeeId, 
                EmployeeName = $"{a.Employee.FirstName} {a.Employee.LastName}",
                Department = a.Employee.Department.Name,
                DayOfWeek = a.Date.DayOfWeek,
                Hour = a.CheckInTime.Hour
            })
            .Select(g => new
            {
                EmployeeId = g.Key.EmployeeId,
                EmployeeName = g.Key.EmployeeName,
                Department = g.Key.Department,
                DayOfWeek = g.Key.DayOfWeek.ToString(),
                Hour = g.Key.Hour,
                AttendanceCount = g.Count(),
                AvgCheckInTime = g.Average(a => a.CheckInTime.Hour * 60 + a.CheckInTime.Minute),
                AvgTotalHours = g.Where(a => a.TotalHours.HasValue).Average(a => a.TotalHours!.Value.TotalHours)
            })
            .OrderBy(x => x.Department)
            .ThenBy(x => x.EmployeeName)
            .ThenBy(x => x.DayOfWeek)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("EmployeeId,EmployeeName,Department,DayOfWeek,Hour,AttendanceCount,AvgCheckInTime,AvgTotalHours");
        
        foreach (var pattern in attendancePatterns)
        {
            var avgCheckInTime = TimeSpan.FromMinutes((double)pattern.AvgCheckInTime).ToString(@"hh\:mm");
            var avgTotalHours = pattern.AvgTotalHours.ToString("F2");
            
            sb.AppendLine($"{pattern.EmployeeId},{pattern.EmployeeName},{pattern.Department},{pattern.DayOfWeek},{pattern.Hour},{pattern.AttendanceCount},{avgCheckInTime},{avgTotalHours}");
        }
        
        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
