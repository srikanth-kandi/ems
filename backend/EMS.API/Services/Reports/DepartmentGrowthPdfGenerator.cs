using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class DepartmentGrowthPdfGenerator : BaseReportGenerator
{
    public DepartmentGrowthPdfGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "department_growth_pdf";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "application/pdf";
    public override string GetFileExtension() => "pdf";

    private async Task<byte[]> GenerateReport()
    {
        // Get department growth data by month for the last 12 months
        var startDate = DateTime.UtcNow.AddMonths(-12);
        var endDate = DateTime.UtcNow;

        var departmentGrowth = await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.DateOfJoining >= startDate && e.DateOfJoining <= endDate)
            .GroupBy(e => new { e.Department.Name, Year = e.DateOfJoining.Year, Month = e.DateOfJoining.Month })
            .Select(g => new
            {
                Department = g.Key.Name,
                Year = g.Key.Year,
                Month = g.Key.Month,
                NewHires = g.Count()
            })
            .OrderBy(x => x.Department)
            .ThenBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();

        using var memoryStream = new MemoryStream();
        var document = new Document(PageSize.A4, 25, 25, 30, 30);
        var writer = PdfWriter.GetInstance(document, memoryStream);
        
        document.Open();
        
        // Add title
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.DARK_GRAY);
        var title = new Paragraph("Department Growth Report", titleFont);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);
        
        // Add subtitle with date range
        var subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.GRAY);
        var subtitle = new Paragraph($"Growth Analysis: {startDate:MMMM yyyy} - {endDate:MMMM yyyy}", subtitleFont);
        subtitle.Alignment = Element.ALIGN_CENTER;
        document.Add(subtitle);
        
        document.Add(new Paragraph(" ")); // Spacing
        
        // Add summary statistics
        AddSummaryStats(document, departmentGrowth.Cast<object>().ToList());
        
        // Add table
        var table = new PdfPTable(5);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 2f, 1f, 1f, 2f, 1f });
        
        // Add headers
        AddTableHeaders(table);
        
        // Add data rows
        AddTableData(table, departmentGrowth.Cast<object>().ToList());
        
        document.Add(table);
        
        // Add charts section placeholder
        AddChartsSection(document, departmentGrowth.Cast<object>().ToList());
        
        // Add footer
        AddFooter(document);
        
        document.Close();
        return memoryStream.ToArray();
    }

    private static void AddSummaryStats(Document document, List<object> data)
    {
        var summaryFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.DARK_GRAY);
        var summary = new Paragraph("Summary Statistics", summaryFont);
        document.Add(summary);
        
        var totalHires = data.Sum(x => (int)((dynamic)x).NewHires);
        var departments = data.Select(x => (string)((dynamic)x).Department).Distinct().Count();
        var avgHiresPerMonth = data.GroupBy(x => new { ((dynamic)x).Year, ((dynamic)x).Month }).Average(g => g.Sum(x => (int)((dynamic)x).NewHires));
        var topDepartment = data.GroupBy(x => (string)((dynamic)x).Department)
            .OrderByDescending(g => g.Sum(x => (int)((dynamic)x).NewHires))
            .FirstOrDefault()?.Key ?? "N/A";
        
        var summaryData = new[]
        {
            $"Total New Hires: {totalHires}",
            $"Departments with Growth: {departments}",
            $"Average Hires per Month: {avgHiresPerMonth:F1}",
            $"Top Growing Department: {topDepartment}"
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
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
        var headers = new[] { "Department", "Year", "Month", "Month Name", "New Hires" };
        
        foreach (var header in headers)
        {
            var headerCell = new PdfPCell(new Phrase(header, headerFont));
            headerCell.BackgroundColor = BaseColor.DARK_GRAY;
            headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(headerCell);
        }
    }

    private static void AddTableData(PdfPTable table, List<object> data)
    {
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
        
        foreach (var item in data)
        {
            var dynamicItem = (dynamic)item;
            var monthName = new DateTime((int)dynamicItem.Year, (int)dynamicItem.Month, 1).ToString("MMMM");
            
            table.AddCell(new PdfPCell(new Phrase((string)dynamicItem.Department, dataFont)));
            table.AddCell(new PdfPCell(new Phrase(dynamicItem.Year.ToString(), dataFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(dynamicItem.Month.ToString(), dataFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(monthName, dataFont)));
            table.AddCell(new PdfPCell(new Phrase(dynamicItem.NewHires.ToString(), dataFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
        }
    }

    private static void AddChartsSection(Document document, List<object> data)
    {
        document.Add(new Paragraph(" "));
        
        var chartsFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.DARK_GRAY);
        var charts = new Paragraph("Growth Trends by Department", chartsFont);
        document.Add(charts);
        
        // Group by department and show monthly trends
        var departmentTrends = data.GroupBy(x => (string)((dynamic)x).Department)
            .OrderByDescending(g => g.Sum(x => (int)((dynamic)x).NewHires))
            .Take(5); // Top 5 departments
        
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        foreach (var dept in departmentTrends)
        {
            var totalHires = dept.Sum(x => (int)((dynamic)x).NewHires);
            var avgMonthly = dept.Average(x => (int)((dynamic)x).NewHires);
            document.Add(new Paragraph($"• {dept.Key}: {totalHires} total hires, {avgMonthly:F1} avg/month", dataFont));
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
