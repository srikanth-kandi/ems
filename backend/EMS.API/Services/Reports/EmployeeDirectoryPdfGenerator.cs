using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class EmployeeDirectoryPdfGenerator : BaseReportGenerator
{
    public EmployeeDirectoryPdfGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "employee_directory_pdf";
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
        var title = new Paragraph("Employee Directory Report", titleFont);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);
        
        document.Add(new Paragraph(" ")); // Spacing
        
        // Add table
        var table = new PdfPTable(7);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 1f, 2f, 2f, 3f, 2f, 2f, 2f });
        
        // Add headers
        AddTableHeaders(table);
        
        // Add data rows
        AddTableData(table, data);
        
        document.Add(table);
        
        // Add footer
        AddFooter(document);
        
        document.Close();
        return memoryStream.ToArray();
    }

    private static void AddTableHeaders(PdfPTable table)
    {
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
        var headers = new[] { "ID", "First Name", "Last Name", "Email", "Department", "Position", "Salary" };
        
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
        
        foreach (var employee in data)
        {
            table.AddCell(new PdfPCell(new Phrase(employee.Id.ToString(), dataFont)));
            table.AddCell(new PdfPCell(new Phrase(employee.FirstName, dataFont)));
            table.AddCell(new PdfPCell(new Phrase(employee.LastName, dataFont)));
            table.AddCell(new PdfPCell(new Phrase(employee.Email, dataFont)));
            table.AddCell(new PdfPCell(new Phrase(employee.Department.Name, dataFont)));
            table.AddCell(new PdfPCell(new Phrase(employee.Position ?? "", dataFont)));
            table.AddCell(new PdfPCell(new Phrase(employee.Salary.ToString("C"), dataFont)));
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
