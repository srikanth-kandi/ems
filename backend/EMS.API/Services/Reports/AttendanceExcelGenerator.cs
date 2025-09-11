using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using EMS.API.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace EMS.API.Services.Reports;

public class AttendanceExcelGenerator : BaseReportGenerator
{
    public AttendanceExcelGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "attendance_excel";
        return await GetCachedOrGenerateAsync(cacheKey, async () => await GenerateReport());
    }

    public async Task<byte[]> GenerateAsync(DateTime? startDate, DateTime? endDate)
    {
        var cacheKey = $"attendance_excel_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";
        return await GetCachedOrGenerateAsync(cacheKey, async () => await GenerateReport(startDate, endDate));
    }

    public override string GetContentType() => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public override string GetFileExtension() => "xlsx";

    private async Task<byte[]> GenerateReport(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Attendances
            .Include(a => a.Employee)
            .ThenInclude(e => e.Department)
            .AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(a => a.Date >= startDate.Value.Date);
        }
        if (endDate.HasValue)
        {
            query = query.Where(a => a.Date <= endDate.Value.Date);
        }

        var attendanceData = await query
            .OrderByDescending(a => a.Date)
            .ThenBy(a => a.Employee.FirstName)
            .ToListAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Attendance Report");

        // Set up headers
        var headers = new[]
        {
            "Employee ID", "Employee Name", "Department", "Date", 
            "Check In Time", "Check Out Time", "Total Hours", "Status", "Notes"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
        }

        // Style the header row
        using (var range = worksheet.Cells[1, 1, 1, headers.Length])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        // Add data rows
        int row = 2;
        foreach (var attendance in attendanceData)
        {
            worksheet.Cells[row, 1].Value = attendance.EmployeeId;
            worksheet.Cells[row, 2].Value = $"{attendance.Employee.FirstName} {attendance.Employee.LastName}";
            worksheet.Cells[row, 3].Value = attendance.Employee.Department?.Name ?? "N/A";
            worksheet.Cells[row, 4].Value = attendance.Date.ToString("yyyy-MM-dd");
            worksheet.Cells[row, 5].Value = attendance.CheckInTime.ToString("HH:mm:ss");
            worksheet.Cells[row, 6].Value = attendance.CheckOutTime?.ToString("HH:mm:ss") ?? "Not checked out";
            worksheet.Cells[row, 7].Value = attendance.TotalHours?.ToString(@"hh\:mm") ?? "N/A";
            worksheet.Cells[row, 8].Value = attendance.CheckOutTime.HasValue ? "Completed" : "In Progress";
            worksheet.Cells[row, 9].Value = attendance.Notes ?? "";

            row++;
        }

        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();

        // Add summary section
        var summaryRow = row + 2;
        worksheet.Cells[summaryRow, 1].Value = "Summary";
        worksheet.Cells[summaryRow, 1].Style.Font.Bold = true;
        worksheet.Cells[summaryRow, 1].Style.Font.Size = 14;

        summaryRow++;
        worksheet.Cells[summaryRow, 1].Value = "Total Records:";
        worksheet.Cells[summaryRow, 2].Value = attendanceData.Count;

        summaryRow++;
        worksheet.Cells[summaryRow, 1].Value = "Completed Sessions:";
        worksheet.Cells[summaryRow, 2].Value = attendanceData.Count(a => a.CheckOutTime.HasValue);

        summaryRow++;
        worksheet.Cells[summaryRow, 1].Value = "In Progress Sessions:";
        worksheet.Cells[summaryRow, 2].Value = attendanceData.Count(a => !a.CheckOutTime.HasValue);

        if (attendanceData.Any())
        {
            summaryRow++;
            worksheet.Cells[summaryRow, 1].Value = "Date Range:";
            worksheet.Cells[summaryRow, 2].Value = $"{attendanceData.Min(a => a.Date):yyyy-MM-dd} to {attendanceData.Max(a => a.Date):yyyy-MM-dd}";
        }

        // Note: Conditional formatting can be added here if needed
        // For now, we'll keep it simple to ensure the basic functionality works

        return package.GetAsByteArray();
    }
}
