using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class PerformanceMetricsPdfGenerator : BaseReportGenerator
{
    public PerformanceMetricsPdfGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "performance_metrics_pdf";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "application/pdf";
    public override string GetFileExtension() => "pdf";

    private async Task<byte[]> GenerateReport()
    {
        var performanceMetrics = await _context.PerformanceMetrics
            .Include(pm => pm.Employee)
            .ThenInclude(e => e.Department)
            .OrderBy(pm => pm.Employee.Department.Name)
            .ThenBy(pm => pm.Employee.LastName)
            .ThenBy(pm => pm.Year)
            .ThenBy(pm => pm.Quarter)
            .ToListAsync();

        using var memoryStream = new MemoryStream();
        var document = new Document(PageSize.A4, 25, 25, 30, 30);
        var writer = PdfWriter.GetInstance(document, memoryStream);
        
        document.Open();
        
        // Add title
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.DARK_GRAY);
        var title = new Paragraph("Performance Metrics Report", titleFont);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);
        
        // Add subtitle
        var subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.GRAY);
        var subtitle = new Paragraph($"Employee Performance Analysis - {DateTime.Now.Year}", subtitleFont);
        subtitle.Alignment = Element.ALIGN_CENTER;
        document.Add(subtitle);
        
        document.Add(new Paragraph(" ")); // Spacing
        
        // Add summary statistics
        AddSummaryStats(document, performanceMetrics);
        
        // Add table
        var table = new PdfPTable(6);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 1f, 2.5f, 2f, 1f, 1f, 1.5f });
        
        // Add headers
        AddTableHeaders(table);
        
        // Add data rows
        AddTableData(table, performanceMetrics);
        
        document.Add(table);
        
        // Add performance analysis
        AddPerformanceAnalysis(document, performanceMetrics);
        
        // Add footer
        AddFooter(document);
        
        document.Close();
        return memoryStream.ToArray();
    }

    private static void AddSummaryStats(Document document, List<Models.PerformanceMetric> data)
    {
        var summaryFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.DARK_GRAY);
        var summary = new Paragraph("Summary Statistics", summaryFont);
        document.Add(summary);
        
        var totalRecords = data.Count;
        var uniqueEmployees = data.Select(pm => pm.EmployeeId).Distinct().Count();
        var departments = data.Select(pm => pm.Employee.Department.Name).Distinct().Count();
        var avgScore = data.Average(pm => pm.PerformanceScore);
        var highPerformers = data.Count(pm => pm.PerformanceScore >= 80);
        
        var summaryData = new[]
        {
            $"Total Performance Records: {totalRecords}",
            $"Unique Employees: {uniqueEmployees}",
            $"Departments Covered: {departments}",
            $"Average Performance Score: {avgScore:F1}",
            $"High Performers (≥80): {highPerformers}"
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
        var headers = new[] { "Emp ID", "Employee", "Department", "Year", "Q", "Score" };
        
        foreach (var header in headers)
        {
            var headerCell = new PdfPCell(new Phrase(header, headerFont));
            headerCell.BackgroundColor = BaseColor.DARK_GRAY;
            headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(headerCell);
        }
    }

    private static void AddTableData(PdfPTable table, List<Models.PerformanceMetric> data)
    {
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
        
        foreach (var metric in data)
        {
            table.AddCell(new PdfPCell(new Phrase(metric.EmployeeId.ToString(), dataFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase($"{metric.Employee.FirstName} {metric.Employee.LastName}", dataFont)));
            table.AddCell(new PdfPCell(new Phrase(metric.Employee.Department.Name, dataFont)));
            table.AddCell(new PdfPCell(new Phrase(metric.Year.ToString(), dataFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase($"Q{metric.Quarter}", dataFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
            
            var scoreCell = new PdfPCell(new Phrase(metric.PerformanceScore.ToString("F1"), dataFont));
            scoreCell.HorizontalAlignment = Element.ALIGN_CENTER;
            
            // Color code the performance score
            if (metric.PerformanceScore >= 80)
                scoreCell.BackgroundColor = BaseColor.GREEN;
            else if (metric.PerformanceScore >= 60)
                scoreCell.BackgroundColor = BaseColor.YELLOW;
            else
                scoreCell.BackgroundColor = BaseColor.RED;
                
            table.AddCell(scoreCell);
        }
    }

    private static void AddPerformanceAnalysis(Document document, List<Models.PerformanceMetric> data)
    {
        document.Add(new Paragraph(" "));
        
        var analysisFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.DARK_GRAY);
        var analysis = new Paragraph("Performance Analysis", analysisFont);
        document.Add(analysis);
        
        // Top performers
        var topPerformers = data.OrderByDescending(pm => pm.PerformanceScore).Take(5);
        
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        document.Add(new Paragraph("Top Performers:", dataFont));
        foreach (var performer in topPerformers)
        {
            document.Add(new Paragraph($"• {performer.Employee.FirstName} {performer.Employee.LastName} ({performer.Employee.Department.Name}) - {performer.PerformanceScore:F1} (Q{performer.Quarter} {performer.Year})", dataFont));
        }
        
        document.Add(new Paragraph(" "));
        
        // Department performance
        var deptPerformance = data.GroupBy(pm => pm.Employee.Department.Name)
            .Select(g => new
            {
                Department = g.Key,
                AvgScore = g.Average(pm => pm.PerformanceScore),
                Count = g.Count()
            })
            .OrderByDescending(x => x.AvgScore)
            .ToList();
        
        document.Add(new Paragraph("Department Performance (Average Score):", dataFont));
        foreach (var dept in deptPerformance)
        {
            document.Add(new Paragraph($"• {dept.Department}: {dept.AvgScore:F1} ({dept.Count} records)", dataFont));
        }
        
        document.Add(new Paragraph(" "));
        
        // Performance distribution
        var excellent = data.Count(pm => pm.PerformanceScore >= 90);
        var good = data.Count(pm => pm.PerformanceScore >= 70 && pm.PerformanceScore < 90);
        var average = data.Count(pm => pm.PerformanceScore >= 50 && pm.PerformanceScore < 70);
        var needsImprovement = data.Count(pm => pm.PerformanceScore < 50);
        
        document.Add(new Paragraph("Performance Distribution:", dataFont));
        document.Add(new Paragraph($"• Excellent (90+): {excellent} employees", dataFont));
        document.Add(new Paragraph($"• Good (70-89): {good} employees", dataFont));
        document.Add(new Paragraph($"• Average (50-69): {average} employees", dataFont));
        document.Add(new Paragraph($"• Needs Improvement (<50): {needsImprovement} employees", dataFont));
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
