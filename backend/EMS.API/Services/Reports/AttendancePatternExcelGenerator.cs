using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class AttendancePatternExcelGenerator : BaseReportGenerator
{
    public AttendancePatternExcelGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "attendance_pattern_excel";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public override string GetFileExtension() => "xlsx";

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

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Attendance Patterns");
        
        // Set header
        var headerRange = worksheet.Cells[1, 1, 1, 8];
        headerRange.Merge = true;
        headerRange.Value = "Attendance Patterns Report";
        headerRange.Style.Font.Size = 16;
        headerRange.Style.Font.Bold = true;
        headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        
        // Add date range
        worksheet.Cells[2, 1, 2, 8].Merge = true;
        worksheet.Cells[2, 1].Value = $"Pattern Analysis: {startDate:MMMM dd, yyyy} - {endDate:MMMM dd, yyyy}";
        worksheet.Cells[2, 1].Style.Font.Italic = true;
        worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        
        // Add generated date
        worksheet.Cells[3, 1, 3, 8].Merge = true;
        worksheet.Cells[3, 1].Value = $"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        worksheet.Cells[3, 1].Style.Font.Italic = true;
        worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        
        // Add summary statistics
        AddSummaryStats(worksheet, attendancePatterns.Cast<object>().ToList());
        
        // Add headers starting from row 6
        var headers = new[] { "Employee ID", "Employee Name", "Department", "Day of Week", "Hour", "Attendance Count", "Avg Check-in Time", "Avg Total Hours" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[6, i + 1].Value = headers[i];
            worksheet.Cells[6, i + 1].Style.Font.Bold = true;
            worksheet.Cells[6, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[6, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            worksheet.Cells[6, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
        }
        
        // Add data starting from row 7
        for (int i = 0; i < attendancePatterns.Count; i++)
        {
            var pattern = attendancePatterns[i];
            var row = i + 7;
            var avgCheckInTime = TimeSpan.FromMinutes((double)pattern.AvgCheckInTime).ToString(@"hh\:mm");
            var avgTotalHours = pattern.AvgTotalHours;
            
            worksheet.Cells[row, 1].Value = pattern.EmployeeId;
            worksheet.Cells[row, 2].Value = pattern.EmployeeName;
            worksheet.Cells[row, 3].Value = pattern.Department;
            worksheet.Cells[row, 4].Value = pattern.DayOfWeek;
            worksheet.Cells[row, 5].Value = pattern.Hour;
            worksheet.Cells[row, 6].Value = pattern.AttendanceCount;
            worksheet.Cells[row, 7].Value = avgCheckInTime;
            worksheet.Cells[row, 8].Value = avgTotalHours;
            worksheet.Cells[row, 8].Style.Numberformat.Format = "0.0";
        }
        
        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();
        
        // Add borders
        var dataRange = worksheet.Cells[6, 1, 6 + attendancePatterns.Count, 8];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        
        // Add employee summary sheet
        AddEmployeeSummarySheet(package, attendancePatterns.Cast<object>().ToList());
        
        // Add hourly patterns sheet
        AddHourlyPatternsSheet(package, attendancePatterns.Cast<object>().ToList());
        
        return package.GetAsByteArray();
    }

    private static void AddSummaryStats(ExcelWorksheet worksheet, List<object> data)
    {
        var totalRecords = data.Sum(x => (int)((dynamic)x).AttendanceCount);
        var uniqueEmployees = data.Select(x => (int)((dynamic)x).EmployeeId).Distinct().Count();
        var departments = data.Select(x => (string)((dynamic)x).Department).Distinct().Count();
        var avgHoursPerRecord = data.Where(x => ((dynamic)x).AvgTotalHours.HasValue).Average(x => (double)((dynamic)x).AvgTotalHours);
        
        // Summary section
        worksheet.Cells[4, 1].Value = "Summary Statistics:";
        worksheet.Cells[4, 1].Style.Font.Bold = true;
        worksheet.Cells[4, 1].Style.Font.Size = 12;
        
        var summaryData = new[]
        {
            $"Total Records: {totalRecords}",
            $"Unique Employees: {uniqueEmployees}",
            $"Departments: {departments}",
            $"Avg Hours/Record: {avgHoursPerRecord:F1}"
        };
        
        for (int i = 0; i < summaryData.Length; i++)
        {
            worksheet.Cells[4, i + 1].Value = summaryData[i];
        }
    }

    private static void AddEmployeeSummarySheet(ExcelPackage package, List<object> data)
    {
        var summarySheet = package.Workbook.Worksheets.Add("Employee Summary");
        
        // Title
        var titleRange = summarySheet.Cells[1, 1, 1, 6];
        titleRange.Merge = true;
        titleRange.Value = "Employee Attendance Summary";
        titleRange.Style.Font.Size = 16;
        titleRange.Style.Font.Bold = true;
        titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        titleRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        titleRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        
        // Headers
        var headers = new[] { "Employee ID", "Employee Name", "Department", "Total Records", "Avg Hours", "Most Common Day" };
        for (int i = 0; i < headers.Length; i++)
        {
            summarySheet.Cells[3, i + 1].Value = headers[i];
            summarySheet.Cells[3, i + 1].Style.Font.Bold = true;
            summarySheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            summarySheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            summarySheet.Cells[3, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
        }
        
        // Calculate employee summaries
        var employeeSummaries = data.GroupBy(x => new { 
                EmployeeId = (int)((dynamic)x).EmployeeId, 
                EmployeeName = (string)((dynamic)x).EmployeeName, 
                Department = (string)((dynamic)x).Department 
            })
            .Select(g => new
            {
                EmployeeId = g.Key.EmployeeId,
                EmployeeName = g.Key.EmployeeName,
                Department = g.Key.Department,
                TotalRecords = g.Sum(x => (int)((dynamic)x).AttendanceCount),
                AvgHours = g.Average(x => (double)((dynamic)x).AvgTotalHours),
                MostCommonDay = g.GroupBy(x => (string)((dynamic)x).DayOfWeek)
                    .OrderByDescending(d => d.Sum(x => (int)((dynamic)x).AttendanceCount))
                    .FirstOrDefault()?.Key ?? "N/A"
            })
            .OrderByDescending(x => x.TotalRecords)
            .ToList();
        
        // Add data
        for (int i = 0; i < employeeSummaries.Count; i++)
        {
            var emp = employeeSummaries[i];
            var row = i + 4;
            
            summarySheet.Cells[row, 1].Value = emp.EmployeeId;
            summarySheet.Cells[row, 2].Value = emp.EmployeeName;
            summarySheet.Cells[row, 3].Value = emp.Department;
            summarySheet.Cells[row, 4].Value = emp.TotalRecords;
            summarySheet.Cells[row, 5].Value = emp.AvgHours;
            summarySheet.Cells[row, 5].Style.Numberformat.Format = "0.0";
            summarySheet.Cells[row, 6].Value = emp.MostCommonDay;
        }
        
        summarySheet.Cells.AutoFitColumns();
        
        // Add borders
        var dataRange = summarySheet.Cells[3, 1, 3 + employeeSummaries.Count, 6];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
    }

    private static void AddHourlyPatternsSheet(ExcelPackage package, List<object> data)
    {
        var hourlySheet = package.Workbook.Worksheets.Add("Hourly Patterns");
        
        // Title
        var titleRange = hourlySheet.Cells[1, 1, 1, 4];
        titleRange.Merge = true;
        titleRange.Value = "Hourly Attendance Patterns";
        titleRange.Style.Font.Size = 16;
        titleRange.Style.Font.Bold = true;
        titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        titleRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        titleRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        
        // Headers
        var headers = new[] { "Hour", "Total Records", "Unique Employees", "Avg Hours" };
        for (int i = 0; i < headers.Length; i++)
        {
            hourlySheet.Cells[3, i + 1].Value = headers[i];
            hourlySheet.Cells[3, i + 1].Style.Font.Bold = true;
            hourlySheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            hourlySheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            hourlySheet.Cells[3, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
        }
        
        // Calculate hourly patterns
        var hourlyPatterns = data.GroupBy(x => (int)((dynamic)x).Hour)
            .Select(g => new
            {
                Hour = g.Key,
                TotalRecords = g.Sum(x => (int)((dynamic)x).AttendanceCount),
                UniqueEmployees = g.Select(x => (int)((dynamic)x).EmployeeId).Distinct().Count(),
                AvgHours = g.Where(x => ((dynamic)x).AvgTotalHours.HasValue).Average(x => (double)((dynamic)x).AvgTotalHours)
            })
            .OrderBy(x => x.Hour)
            .ToList();
        
        // Add data
        for (int i = 0; i < hourlyPatterns.Count; i++)
        {
            var pattern = hourlyPatterns[i];
            var row = i + 4;
            
            hourlySheet.Cells[row, 1].Value = $"{pattern.Hour}:00";
            hourlySheet.Cells[row, 2].Value = pattern.TotalRecords;
            hourlySheet.Cells[row, 3].Value = pattern.UniqueEmployees;
            hourlySheet.Cells[row, 4].Value = pattern.AvgHours;
            hourlySheet.Cells[row, 4].Style.Numberformat.Format = "0.0";
        }
        
        hourlySheet.Cells.AutoFitColumns();
        
        // Add borders
        var dataRange = hourlySheet.Cells[3, 1, 3 + hourlyPatterns.Count, 4];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
    }
}
