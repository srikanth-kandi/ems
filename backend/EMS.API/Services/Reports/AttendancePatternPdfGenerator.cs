using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class AttendancePatternPdfGenerator : BaseReportGenerator
{
    public AttendancePatternPdfGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "attendance_pattern_pdf";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "application/pdf";
    public override string GetFileExtension() => "pdf";

    private async Task<byte[]> GenerateReport()
    {
        // Get attendance patterns for the last 30 days
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;

        var attendancePatterns = _context.Attendances
            .Include(a => a.Employee)
            .ThenInclude(e => e.Department)
            .Where(a => a.Date >= startDate && a.Date <= endDate)
            .AsEnumerable()
            .GroupBy(a => new { 
                a.EmployeeId, 
                FirstName = a.Employee.FirstName,
                LastName = a.Employee.LastName,
                Department = a.Employee.Department.Name,
                DayOfWeek = a.Date.DayOfWeek,
                Hour = a.CheckInTime.Hour
            })
            .Select(g => new
            {
                EmployeeId = g.Key.EmployeeId,
                EmployeeName = g.Key.FirstName + " " + g.Key.LastName,
                Department = g.Key.Department,
                DayOfWeek = g.Key.DayOfWeek,
                Hour = g.Key.Hour,
                AttendanceCount = g.Count(),
                AvgCheckInTime = g.Average(a => a.CheckInTime.Hour * 60 + a.CheckInTime.Minute),
                AvgTotalHours = g.Where(a => a.TotalHours.HasValue).Average(a => a.TotalHours!.Value.TotalHours)
            })
            .OrderBy(x => x.Department)
            .ThenBy(x => x.EmployeeName)
            .ThenBy(x => x.DayOfWeek)
            .ToList();

        // Convert DayOfWeek to string on the client side
        var attendancePatternsWithStringDayOfWeek = attendancePatterns.Select(p => new
        {
            p.EmployeeId,
            p.EmployeeName,
            p.Department,
            DayOfWeek = p.DayOfWeek.ToString(),
            p.Hour,
            p.AttendanceCount,
            p.AvgCheckInTime,
            p.AvgTotalHours
        }).ToList();

        using var memoryStream = new MemoryStream();
        var document = new Document(PageSize.A4, 25, 25, 30, 30);
        var writer = PdfWriter.GetInstance(document, memoryStream);
        
        document.Open();
        
        // Add title
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.DARK_GRAY);
        var title = new Paragraph("Attendance Patterns Report", titleFont);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);
        
        // Add subtitle with date range
        var subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.GRAY);
        var subtitle = new Paragraph($"Pattern Analysis: {startDate:MMMM dd, yyyy} - {endDate:MMMM dd, yyyy}", subtitleFont);
        subtitle.Alignment = Element.ALIGN_CENTER;
        document.Add(subtitle);
        
        document.Add(new Paragraph(" ")); // Spacing
        
        // Add summary statistics
        AddSummaryStats(document, attendancePatternsWithStringDayOfWeek.Cast<object>().ToList());
        
        // Add table
        var table = new PdfPTable(8);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 1f, 2f, 2f, 1.5f, 1f, 1.5f, 2f, 1.5f });
        
        // Add headers
        AddTableHeaders(table);
        
        // Add data rows
        AddTableData(table, attendancePatternsWithStringDayOfWeek.Cast<object>().ToList());
        
        document.Add(table);
        
        // Add patterns analysis
        AddPatternsAnalysis(document, attendancePatternsWithStringDayOfWeek.Cast<object>().ToList());
        
        // Add footer
        AddFooter(document);
        
        document.Close();
        return memoryStream.ToArray();
    }

    private static void AddSummaryStats(Document document, List<dynamic> data)
    {
        var summaryFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.DARK_GRAY);
        var summary = new Paragraph("Summary Statistics", summaryFont);
        document.Add(summary);
        
        var totalRecords = data.Sum(x => (int)x.AttendanceCount);
        var uniqueEmployees = data.Select(x => (int)x.EmployeeId).Distinct().Count();
        var departments = data.Select(x => (string)x.Department).Distinct().Count();
        var avgHoursPerRecord = data.Where(x => x.AvgTotalHours > 0).Average(x => (double)x.AvgTotalHours);
        
        var summaryData = new[]
        {
            $"Total Attendance Records: {totalRecords}",
            $"Unique Employees: {uniqueEmployees}",
            $"Departments Covered: {departments}",
            $"Average Hours per Record: {avgHoursPerRecord:F1}"
        };
        
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        foreach (var item in summaryData)
        {
            document.Add(new Paragraph($"• {item}", dataFont));
        }
        
        document.Add(new Paragraph(" ")); // Spacing
    }

    private static void AddTableHeaders(PdfPTable table)
    {
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE);
        var headers = new[] { "Emp ID", "Employee", "Department", "Day", "Hour", "Count", "Avg Check-in", "Avg Hours" };
        
        foreach (var header in headers)
        {
            var headerCell = new PdfPCell(new Phrase(header, headerFont));
            headerCell.BackgroundColor = BaseColor.DARK_GRAY;
            headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(headerCell);
        }
    }

    private static void AddTableData(PdfPTable table, List<dynamic> data)
    {
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
        
        foreach (var item in data)
        {
            var avgCheckInTime = TimeSpan.FromMinutes((double)item.AvgCheckInTime).ToString(@"hh\:mm");
            var avgTotalHours = item.AvgTotalHours > 0 ? item.AvgTotalHours.ToString("F1") : "0.0";
            
            table.AddCell(new PdfPCell(new Phrase(item.EmployeeId.ToString(), dataFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase((string)item.EmployeeName, dataFont)));
            table.AddCell(new PdfPCell(new Phrase((string)item.Department, dataFont)));
            table.AddCell(new PdfPCell(new Phrase((string)item.DayOfWeek, dataFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(item.Hour.ToString(), dataFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(item.AttendanceCount.ToString(), dataFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(avgCheckInTime, dataFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(avgTotalHours, dataFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
        }
    }

    private static void AddPatternsAnalysis(Document document, List<dynamic> data)
    {
        document.Add(new Paragraph(" "));
        
        var analysisFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.DARK_GRAY);
        var analysis = new Paragraph("Pattern Analysis", analysisFont);
        document.Add(analysis);
        
        // Most common check-in hours
        var commonHours = data.GroupBy(x => (int)x.Hour)
            .OrderByDescending(g => g.Sum(x => (int)x.AttendanceCount))
            .Take(3);
        
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        document.Add(new Paragraph("Most Common Check-in Hours:", dataFont));
        foreach (var hour in commonHours)
        {
            var totalCount = hour.Sum(x => (int)x.AttendanceCount);
            document.Add(new Paragraph($"• {hour.Key}:00 - {totalCount} records", dataFont));
        }
        
        document.Add(new Paragraph(" "));
        
        // Department patterns
        var deptPatterns = data.GroupBy(x => (string)x.Department)
            .OrderByDescending(g => g.Sum(x => (int)x.AttendanceCount))
            .Take(3);
        
        document.Add(new Paragraph("Top Active Departments:", dataFont));
        foreach (var dept in deptPatterns)
        {
            var totalCount = dept.Sum(x => (int)x.AttendanceCount);
            var avgHours = dept.Where(x => x.AvgTotalHours > 0).Average(x => (double)x.AvgTotalHours);
            document.Add(new Paragraph($"• {dept.Key}: {totalCount} records, {avgHours:F1} avg hours", dataFont));
        }
    }

    private static void AddFooter(Document document)
    {
        document.Add(new Paragraph(" "));
        var footerFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, BaseColor.GRAY);
        var footer = new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", footerFont);
        footer.Alignment = Element.ALIGN_RIGHT;
        document.Add(footer);
    }
}
