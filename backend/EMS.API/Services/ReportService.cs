using System.Text;
using EMS.API.Data;
using EMS.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EMS.API.Services;

public class ReportService : IReportService
{
    private readonly EMSDbContext _context;

    public ReportService(EMSDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> GenerateEmployeeDirectoryReportAsync()
    {
        var data = await _context.Employees.Include(e => e.Department).Where(e => e.IsActive).ToListAsync();
        var sb = new StringBuilder();
        sb.AppendLine("Id,FirstName,LastName,Email,Department,Position,Salary");
        foreach (var e in data)
        {
            sb.AppendLine($"{e.Id},{e.FirstName},{e.LastName},{e.Email},{e.Department.Name},{e.Position},{e.Salary}");
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task<byte[]> GenerateDepartmentReportAsync()
    {
        var data = await _context.Departments.Include(d => d.Employees).ToListAsync();
        var sb = new StringBuilder();
        sb.AppendLine("Id,Name,EmployeesCount");
        foreach (var d in data)
        {
            sb.AppendLine($"{d.Id},{d.Name},{d.Employees.Count}");
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task<byte[]> GenerateAttendanceReportAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Attendances.Include(a => a.Employee).AsQueryable();
        if (startDate.HasValue) query = query.Where(a => a.Date >= startDate.Value.Date);
        if (endDate.HasValue) query = query.Where(a => a.Date <= endDate.Value.Date);
        var data = await query.OrderBy(a => a.Date).ToListAsync();
        var sb = new StringBuilder();
        sb.AppendLine("Date,Employee,CheckIn,CheckOut,TotalHours");
        foreach (var a in data)
        {
            sb.AppendLine($"{a.Date:yyyy-MM-dd},{a.Employee.FirstName} {a.Employee.LastName},{a.CheckInTime:o},{a.CheckOutTime:o},{a.TotalHours}");
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task<byte[]> GenerateSalaryReportAsync()
    {
        var data = await _context.Employees.Where(e => e.IsActive).ToListAsync();
        var sb = new StringBuilder();
        sb.AppendLine("Id,Name,Salary");
        foreach (var e in data)
        {
            sb.AppendLine($"{e.Id},{e.FirstName} {e.LastName},{e.Salary}");
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public Task<byte[]> GenerateHiringTrendReportAsync()
    {
        // Stub: group by month of DateOfJoining
        return GenerateEmployeeDirectoryReportAsync();
    }

    public Task<byte[]> GenerateDepartmentGrowthReportAsync()
    {
        // Stub: reuse department report
        return GenerateDepartmentReportAsync();
    }

    public Task<byte[]> GenerateAttendancePatternReportAsync()
    {
        // Stub: reuse attendance report for now
        return GenerateAttendanceReportAsync(null, null);
    }

    public Task<byte[]> GeneratePerformanceMetricsReportAsync(int? employeeId = null)
    {
        // Stub: return empty CSV
        return Task.FromResult(Encoding.UTF8.GetBytes("Id,Employee,Score,Comments\n"));
    }
}


