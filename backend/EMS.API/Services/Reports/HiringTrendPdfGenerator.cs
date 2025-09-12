using System.Text;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace EMS.API.Services.Reports;

public class HiringTrendPdfGenerator : BaseReportGenerator
{
    public HiringTrendPdfGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "hiring_trend_pdf";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "application/pdf";
    public override string GetFileExtension() => "pdf";

    private async Task<byte[]> GenerateReport()
    {
        // Get hiring data by month for the last 12 months
        var startDate = DateTime.UtcNow.AddMonths(-12);
        var endDate = DateTime.UtcNow;

        var hiringData = await _context.Employees
            .Where(e => e.DateOfJoining >= startDate && e.DateOfJoining <= endDate)
            .GroupBy(e => new { e.DateOfJoining.Year, e.DateOfJoining.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Count = g.Count(),
                Department = g.First().Department.Name
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();

        using var memoryStream = new MemoryStream();
        var document = new Document(PageSize.A4, 50, 50, 25, 25);
        var writer = PdfWriter.GetInstance(document, memoryStream);
        
        document.Open();

        // Title
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.DARK_GRAY);
        var title = new Paragraph("Hiring Trends Report", titleFont)
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 20
        };
        document.Add(title);

        // Report date
        var dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY);
        var reportDate = new Paragraph($"Generated on: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC", dateFont)
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 20
        };
        document.Add(reportDate);

        // Create table
        var table = new PdfPTable(5) { WidthPercentage = 100 };
        table.SetWidths(new float[] { 1f, 1f, 2f, 1f, 2f });

        // Headers
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
        var headerCell = new PdfPCell(new Phrase("Year", headerFont))
        {
            BackgroundColor = new BaseColor(102, 126, 234),
            HorizontalAlignment = Element.ALIGN_CENTER,
            Padding = 8
        };
        table.AddCell(headerCell);

        headerCell = new PdfPCell(new Phrase("Month", headerFont))
        {
            BackgroundColor = new BaseColor(102, 126, 234),
            HorizontalAlignment = Element.ALIGN_CENTER,
            Padding = 8
        };
        table.AddCell(headerCell);

        headerCell = new PdfPCell(new Phrase("Month Name", headerFont))
        {
            BackgroundColor = new BaseColor(102, 126, 234),
            HorizontalAlignment = Element.ALIGN_CENTER,
            Padding = 8
        };
        table.AddCell(headerCell);

        headerCell = new PdfPCell(new Phrase("Hires", headerFont))
        {
            BackgroundColor = new BaseColor(102, 126, 234),
            HorizontalAlignment = Element.ALIGN_CENTER,
            Padding = 8
        };
        table.AddCell(headerCell);

        headerCell = new PdfPCell(new Phrase("Department", headerFont))
        {
            BackgroundColor = new BaseColor(102, 126, 234),
            HorizontalAlignment = Element.ALIGN_CENTER,
            Padding = 8
        };
        table.AddCell(headerCell);

        // Data rows
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK);
        foreach (var item in hiringData)
        {
            var monthName = new DateTime(item.Year, item.Month, 1).ToString("MMMM");
            
            table.AddCell(new PdfPCell(new Phrase(item.Year.ToString(), dataFont)) { Padding = 6 });
            table.AddCell(new PdfPCell(new Phrase(item.Month.ToString(), dataFont)) { Padding = 6 });
            table.AddCell(new PdfPCell(new Phrase(monthName, dataFont)) { Padding = 6 });
            table.AddCell(new PdfPCell(new Phrase(item.Count.ToString(), dataFont)) { Padding = 6, HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(item.Department, dataFont)) { Padding = 6 });
        }

        document.Add(table);
        document.Close();

        return memoryStream.ToArray();
    }
}
