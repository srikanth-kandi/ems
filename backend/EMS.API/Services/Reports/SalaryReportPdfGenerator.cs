using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class SalaryReportPdfGenerator : BaseReportGenerator
{
    public SalaryReportPdfGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "salary_report_pdf";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "application/pdf";
    public override string GetFileExtension() => "pdf";

    private async Task<byte[]> GenerateReport()
    {
        var data = await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.IsActive)
            .ToListAsync();

        using var memoryStream = new MemoryStream();
        var document = new Document(PageSize.A4, 25, 25, 30, 30);
        var writer = PdfWriter.GetInstance(document, memoryStream);
        
        document.Open();
        
        // Add title
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.DARK_GRAY);
        var title = new Paragraph("Salary Report", titleFont);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);
        
        document.Add(new Paragraph(" ")); // Spacing
        
        // Add table
        var table = new PdfPTable(6);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 2f, 2f, 2f, 2f, 2f, 2f });
        
        // Add headers
        AddTableHeaders(table);
        
        // Add data rows
        AddTableData(table, data);
        
        document.Add(table);
        
        // Add salary analysis
        AddSalaryAnalysis(document, data);
        
        // Add footer
        AddFooter(document);
        
        document.Close();
        return memoryStream.ToArray();
    }

    private static void AddTableHeaders(PdfPTable table)
    {
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
        var headers = new[] { "Employee", "Department", "Position", "Salary", "Experience", "Joining Date" };
        
        foreach (var header in headers)
        {
            var headerCell = new PdfPCell(new Phrase(header, headerFont));
            headerCell.BackgroundColor = BaseColor.DARK_GRAY;
            table.AddCell(headerCell);
        }
    }

    private static void AddTableData(PdfPTable table, List<Models.Employee> data)
    {
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
        
        foreach (var employee in data.OrderBy(e => e.Salary).ThenBy(e => e.LastName))
        {
            var fullName = $"{employee.FirstName} {employee.LastName}";
            var experience = DateTime.Now.Year - employee.DateOfJoining.Year;
            
            table.AddCell(new PdfPCell(new Phrase(fullName, dataFont)));
            table.AddCell(new PdfPCell(new Phrase(employee.Department?.Name ?? "N/A", dataFont)));
            table.AddCell(new PdfPCell(new Phrase(employee.Position ?? "N/A", dataFont)));
            table.AddCell(new PdfPCell(new Phrase(employee.Salary.ToString("C"), dataFont)));
            table.AddCell(new PdfPCell(new Phrase($"{experience} years", dataFont)));
            table.AddCell(new PdfPCell(new Phrase(employee.DateOfJoining.ToString("yyyy-MM-dd"), dataFont)));
        }
    }

    private static void AddSalaryAnalysis(Document document, List<Models.Employee> data)
    {
        document.Add(new Paragraph(" "));
        
        var analysisFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.DARK_GRAY);
        var analysis = new Paragraph("Salary Analysis", analysisFont);
        document.Add(analysis);
        
        if (data.Any())
        {
            var salaries = data.Select(e => e.Salary).ToList();
            var minSalary = salaries.Min();
            var maxSalary = salaries.Max();
            var avgSalary = salaries.Average();
            var medianSalary = salaries.OrderBy(x => x).Skip(salaries.Count / 2).First();
            var totalPayroll = salaries.Sum();
            
            var analysisData = new[]
            {
                $"Total Employees: {data.Count}",
                $"Minimum Salary: {minSalary:C}",
                $"Maximum Salary: {maxSalary:C}",
                $"Average Salary: {avgSalary:C}",
                $"Median Salary: {medianSalary:C}",
                $"Total Payroll: {totalPayroll:C}"
            };
            
            var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            foreach (var item in analysisData)
            {
                document.Add(new Paragraph($"• {item}", dataFont));
            }
            
            // Add department breakdown
            document.Add(new Paragraph(" "));
            var deptFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.DARK_GRAY);
            var deptHeader = new Paragraph("Salary by Department", deptFont);
            document.Add(deptHeader);
            
            var deptSalaries = data
                .GroupBy(e => e.Department?.Name ?? "No Department")
                .Select(g => new { Department = g.Key, AvgSalary = g.Average(e => e.Salary), Count = g.Count() })
                .OrderByDescending(x => x.AvgSalary);
            
            foreach (var dept in deptSalaries)
            {
                document.Add(new Paragraph($"• {dept.Department}: {dept.AvgSalary:C} (avg) - {dept.Count} employees", dataFont));
            }
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
