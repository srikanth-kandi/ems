using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class AttendancePdfGenerator : BaseReportGenerator
{
    public AttendancePdfGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "attendance_pdf";
        return await GetCachedOrGenerateAsync(cacheKey, () => GenerateReport());
    }

    public async Task<byte[]> GenerateAsync(DateTime? startDate, DateTime? endDate)
    {
        var cacheKey = $"attendance_pdf_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";
        return await GetCachedOrGenerateAsync(cacheKey, () => GenerateReport(startDate, endDate));
    }

    public override string GetContentType() => "application/pdf";
    public override string GetFileExtension() => "pdf";

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

        using var memoryStream = new MemoryStream();
        var document = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30); // Landscape for better table display
        var writer = PdfWriter.GetInstance(document, memoryStream);
        
        document.Open();
        
        // Add title
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.DARK_GRAY);
        var title = new Paragraph("Attendance Report", titleFont);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);
        
        // Add date range if specified
        if (startDate.HasValue || endDate.HasValue)
        {
            var dateRangeFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.GRAY);
            var dateRange = "Date Range: ";
            if (startDate.HasValue && endDate.HasValue)
            {
                dateRange += $"{startDate.Value:yyyy-MM-dd} to {endDate.Value:yyyy-MM-dd}";
            }
            else if (startDate.HasValue)
            {
                dateRange += $"From {startDate.Value:yyyy-MM-dd}";
            }
            else if (endDate.HasValue)
            {
                dateRange += $"Until {endDate.Value:yyyy-MM-dd}";
            }
            
            var dateRangePara = new Paragraph(dateRange, dateRangeFont);
            dateRangePara.Alignment = Element.ALIGN_CENTER;
            document.Add(dateRangePara);
        }
        
        document.Add(new Paragraph(" ")); // Spacing
        
        // Add table
        var table = new PdfPTable(8);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 1f, 2f, 2f, 2f, 1.5f, 1.5f, 1.5f, 1.5f });
        
        // Add headers
        AddTableHeaders(table);
        
        // Add data rows
        AddTableData(table, attendanceData);
        
        document.Add(table);
        
        // Add summary
        AddSummary(document, attendanceData);
        
        // Add footer
        AddFooter(document);
        
        document.Close();
        return memoryStream.ToArray();
    }

    private static void AddTableHeaders(PdfPTable table)
    {
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE);
        var headers = new[] { "ID", "Employee", "Department", "Date", "Check In", "Check Out", "Hours", "Status" };
        
        foreach (var header in headers)
        {
            var headerCell = new PdfPCell(new Phrase(header, headerFont));
            headerCell.BackgroundColor = BaseColor.DARK_GRAY;
            headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(headerCell);
        }
    }

    private static void AddTableData(PdfPTable table, List<Models.Attendance> data)
    {
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
        
        foreach (var attendance in data)
        {
            table.AddCell(new PdfPCell(new Phrase(attendance.EmployeeId.ToString(), dataFont)));
            table.AddCell(new PdfPCell(new Phrase($"{attendance.Employee.FirstName} {attendance.Employee.LastName}", dataFont)));
            table.AddCell(new PdfPCell(new Phrase(attendance.Employee.Department?.Name ?? "N/A", dataFont)));
            table.AddCell(new PdfPCell(new Phrase(attendance.Date.ToString("yyyy-MM-dd"), dataFont)));
            table.AddCell(new PdfPCell(new Phrase(attendance.CheckInTime.ToString("HH:mm"), dataFont)));
            table.AddCell(new PdfPCell(new Phrase(attendance.CheckOutTime?.ToString("HH:mm") ?? "N/A", dataFont)));
            table.AddCell(new PdfPCell(new Phrase(attendance.TotalHours?.ToString(@"hh\:mm") ?? "N/A", dataFont)));
            
            var status = attendance.CheckOutTime.HasValue ? "Completed" : "In Progress";
            var statusColor = attendance.CheckOutTime.HasValue ? BaseColor.GREEN : BaseColor.ORANGE;
            var statusCell = new PdfPCell(new Phrase(status, dataFont));
            statusCell.BackgroundColor = statusColor;
            table.AddCell(statusCell);
        }
    }

    private static void AddSummary(Document document, List<Models.Attendance> data)
    {
        document.Add(new Paragraph(" "));
        
        var summaryFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.DARK_GRAY);
        var summaryTitle = new Paragraph("Summary", summaryFont);
        document.Add(summaryTitle);
        
        var summaryDataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        
        var totalRecords = data.Count;
        var completedSessions = data.Count(a => a.CheckOutTime.HasValue);
        var inProgressSessions = data.Count(a => !a.CheckOutTime.HasValue);
        
        document.Add(new Paragraph($"Total Records: {totalRecords}", summaryDataFont));
        document.Add(new Paragraph($"Completed Sessions: {completedSessions}", summaryDataFont));
        document.Add(new Paragraph($"In Progress Sessions: {inProgressSessions}", summaryDataFont));
        
        if (data.Any())
        {
            var dateRange = $"Date Range: {data.Min(a => a.Date):yyyy-MM-dd} to {data.Max(a => a.Date):yyyy-MM-dd}";
            document.Add(new Paragraph(dateRange, summaryDataFont));
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
