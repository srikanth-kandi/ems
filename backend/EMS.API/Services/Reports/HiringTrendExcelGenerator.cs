using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;
using OfficeOpenXml;

namespace EMS.API.Services.Reports;

public class HiringTrendExcelGenerator : BaseReportGenerator
{
    public HiringTrendExcelGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "hiring_trend_excel";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public override string GetFileExtension() => "xlsx";

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

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Hiring Trends");

        // Headers
        worksheet.Cells[1, 1].Value = "Year";
        worksheet.Cells[1, 2].Value = "Month";
        worksheet.Cells[1, 3].Value = "Month Name";
        worksheet.Cells[1, 4].Value = "Hires";
        worksheet.Cells[1, 5].Value = "Department";

        // Style headers
        using (var range = worksheet.Cells[1, 1, 1, 5])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(102, 126, 234));
            range.Style.Font.Color.SetColor(System.Drawing.Color.White);
            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        }

        // Data
        for (int i = 0; i < hiringData.Count; i++)
        {
            var item = hiringData[i];
            var row = i + 2;
            var monthName = new DateTime(item.Year, item.Month, 1).ToString("MMMM");
            
            worksheet.Cells[row, 1].Value = item.Year;
            worksheet.Cells[row, 2].Value = item.Month;
            worksheet.Cells[row, 3].Value = monthName;
            worksheet.Cells[row, 4].Value = item.Count;
            worksheet.Cells[row, 5].Value = item.Department;
        }

        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();

        return package.GetAsByteArray();
    }
}
