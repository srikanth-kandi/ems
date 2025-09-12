using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class DepartmentReportPdfGenerator : BaseReportGenerator
{
    public DepartmentReportPdfGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "department_report_pdf";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "application/pdf";
    public override string GetFileExtension() => "pdf";

    private async Task<byte[]> GenerateReport()
    {
        var data = await _context.Departments
            .Include(d => d.Employees.Where(e => e.IsActive))
            .ToListAsync();

        using var memoryStream = new MemoryStream();
        var document = new Document(PageSize.A4, 25, 25, 30, 30);
        var writer = PdfWriter.GetInstance(document, memoryStream);
        
        document.Open();
        
        // Add title
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.DARK_GRAY);
        var title = new Paragraph("Department Report", titleFont);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);
        
        document.Add(new Paragraph(" ")); // Spacing
        
        // Add table
        var table = new PdfPTable(6);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 1f, 3f, 3f, 2f, 2f, 2f });
        
        // Add headers
        AddTableHeaders(table);
        
        // Add data rows
        AddTableData(table, data);
        
        document.Add(table);
        
        // Add summary
        AddSummary(document, data);
        
        // Add footer
        AddFooter(document);
        
        document.Close();
        return memoryStream.ToArray();
    }

    private static void AddTableHeaders(PdfPTable table)
    {
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
        var headers = new[] { "ID", "Name", "Description", "Manager", "Employees", "Created" };
        
        foreach (var header in headers)
        {
            var headerCell = new PdfPCell(new Phrase(header, headerFont));
            headerCell.BackgroundColor = BaseColor.DARK_GRAY;
            table.AddCell(headerCell);
        }
    }

    private static void AddTableData(PdfPTable table, List<Models.Department> data)
    {
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
        
        foreach (var department in data)
        {
            table.AddCell(new PdfPCell(new Phrase(department.Id.ToString(), dataFont)));
            table.AddCell(new PdfPCell(new Phrase(department.Name, dataFont)));
            table.AddCell(new PdfPCell(new Phrase(department.Description ?? "", dataFont)));
            table.AddCell(new PdfPCell(new Phrase(department.ManagerName ?? "", dataFont)));
            table.AddCell(new PdfPCell(new Phrase(department.Employees.Count.ToString(), dataFont)));
            table.AddCell(new PdfPCell(new Phrase(department.CreatedAt.ToString("yyyy-MM-dd"), dataFont)));
        }
    }

    private static void AddSummary(Document document, List<Models.Department> data)
    {
        document.Add(new Paragraph(" "));
        
        var summaryFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.DARK_GRAY);
        var summary = new Paragraph("Summary", summaryFont);
        document.Add(summary);
        
        var totalDepartments = data.Count;
        var totalEmployees = data.Sum(d => d.Employees.Count);
        var avgEmployeesPerDept = totalDepartments > 0 ? (double)totalEmployees / totalDepartments : 0;
        
        var summaryData = new[]
        {
            $"Total Departments: {totalDepartments}",
            $"Total Active Employees: {totalEmployees}",
            $"Average Employees per Department: {avgEmployeesPerDept:F1}"
        };
        
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        foreach (var item in summaryData)
        {
            document.Add(new Paragraph($"• {item}", dataFont));
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
