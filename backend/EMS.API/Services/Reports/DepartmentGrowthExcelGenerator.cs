using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class DepartmentGrowthExcelGenerator : BaseReportGenerator
{
    public DepartmentGrowthExcelGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "department_growth_excel";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public override string GetFileExtension() => "xlsx";

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

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Department Growth");
        
        // Set header
        var headerRange = worksheet.Cells[1, 1, 1, 5];
        headerRange.Merge = true;
        headerRange.Value = "Department Growth Report";
        headerRange.Style.Font.Size = 16;
        headerRange.Style.Font.Bold = true;
        headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        
        // Add date range
        worksheet.Cells[2, 1, 2, 5].Merge = true;
        worksheet.Cells[2, 1].Value = $"Growth Analysis: {startDate:MMMM yyyy} - {endDate:MMMM yyyy}";
        worksheet.Cells[2, 1].Style.Font.Italic = true;
        worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        
        // Add generated date
        worksheet.Cells[3, 1, 3, 5].Merge = true;
        worksheet.Cells[3, 1].Value = $"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        worksheet.Cells[3, 1].Style.Font.Italic = true;
        worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        
        // Add summary statistics
        AddSummaryStats(worksheet, departmentGrowth.Cast<object>().ToList());
        
        // Add headers starting from row 7
        var headers = new[] { "Department", "Year", "Month", "Month Name", "New Hires" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[7, i + 1].Value = headers[i];
            worksheet.Cells[7, i + 1].Style.Font.Bold = true;
            worksheet.Cells[7, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[7, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            worksheet.Cells[7, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
        }
        
        // Add data starting from row 8
        for (int i = 0; i < departmentGrowth.Count; i++)
        {
            var item = departmentGrowth[i];
            var row = i + 8;
            var monthName = new DateTime(item.Year, item.Month, 1).ToString("MMMM");
            
            worksheet.Cells[row, 1].Value = item.Department;
            worksheet.Cells[row, 2].Value = item.Year;
            worksheet.Cells[row, 3].Value = item.Month;
            worksheet.Cells[row, 4].Value = monthName;
            worksheet.Cells[row, 5].Value = item.NewHires;
        }
        
        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();
        
        // Add borders
        var dataRange = worksheet.Cells[7, 1, 7 + departmentGrowth.Count, 5];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        
        // Add department summary sheet
        AddDepartmentSummarySheet(package, departmentGrowth.Cast<object>().ToList());
        
        // Add monthly trends sheet
        AddMonthlyTrendsSheet(package, departmentGrowth.Cast<object>().ToList());
        
        return package.GetAsByteArray();
    }

    private static void AddSummaryStats(ExcelWorksheet worksheet, List<object> data)
    {
        var totalHires = data.Sum(x => (int)((dynamic)x).NewHires);
        var departments = data.Select(x => (string)((dynamic)x).Department).Distinct().Count();
        var avgHiresPerMonth = data.GroupBy(x => new { ((dynamic)x).Year, ((dynamic)x).Month }).Average(g => g.Sum(x => (int)((dynamic)x).NewHires));
        var topDepartment = data.GroupBy(x => (string)((dynamic)x).Department)
            .OrderByDescending(g => g.Sum(x => (int)((dynamic)x).NewHires))
            .FirstOrDefault()?.Key ?? "N/A";
        
        // Summary section
        worksheet.Cells[5, 1].Value = "Summary Statistics:";
        worksheet.Cells[5, 1].Style.Font.Bold = true;
        worksheet.Cells[5, 1].Style.Font.Size = 12;
        
        var summaryData = new[]
        {
            $"Total New Hires: {totalHires}",
            $"Departments with Growth: {departments}",
            $"Average Hires per Month: {avgHiresPerMonth:F1}",
            $"Top Growing Department: {topDepartment}"
        };
        
        for (int i = 0; i < summaryData.Length; i++)
        {
            worksheet.Cells[5, i + 1].Value = summaryData[i];
        }
    }

    private static void AddDepartmentSummarySheet(ExcelPackage package, List<object> data)
    {
        var summarySheet = package.Workbook.Worksheets.Add("Department Summary");
        
        // Title
        var titleRange = summarySheet.Cells[1, 1, 1, 4];
        titleRange.Merge = true;
        titleRange.Value = "Department Growth Summary";
        titleRange.Style.Font.Size = 16;
        titleRange.Style.Font.Bold = true;
        titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        titleRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        titleRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        
        // Headers
        var headers = new[] { "Department", "Total Hires", "Average per Month", "Growth Rate" };
        for (int i = 0; i < headers.Length; i++)
        {
            summarySheet.Cells[3, i + 1].Value = headers[i];
            summarySheet.Cells[3, i + 1].Style.Font.Bold = true;
            summarySheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            summarySheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            summarySheet.Cells[3, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
        }
        
        // Calculate department summaries
        var departmentSummaries = data.GroupBy(x => (string)((dynamic)x).Department)
            .Select(g => new
            {
                Department = g.Key,
                TotalHires = g.Sum(x => (int)((dynamic)x).NewHires),
                AvgPerMonth = g.Average(x => (int)((dynamic)x).NewHires),
                GrowthRate = g.Count() > 0 ? (double)g.Sum(x => (int)((dynamic)x).NewHires) / g.Count() : 0
            })
            .OrderByDescending(x => x.TotalHires)
            .ToList();
        
        // Add data
        for (int i = 0; i < departmentSummaries.Count; i++)
        {
            var dept = departmentSummaries[i];
            var row = i + 4;
            
            summarySheet.Cells[row, 1].Value = dept.Department;
            summarySheet.Cells[row, 2].Value = dept.TotalHires;
            summarySheet.Cells[row, 3].Value = dept.AvgPerMonth;
            summarySheet.Cells[row, 3].Style.Numberformat.Format = "0.0";
            summarySheet.Cells[row, 4].Value = dept.GrowthRate;
            summarySheet.Cells[row, 4].Style.Numberformat.Format = "0.0%";
        }
        
        summarySheet.Cells.AutoFitColumns();
        
        // Add borders
        var dataRange = summarySheet.Cells[3, 1, 3 + departmentSummaries.Count, 4];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
    }

    private static void AddMonthlyTrendsSheet(ExcelPackage package, List<object> data)
    {
        var trendsSheet = package.Workbook.Worksheets.Add("Monthly Trends");
        
        // Title
        var titleRange = trendsSheet.Cells[1, 1, 1, 3];
        titleRange.Merge = true;
        titleRange.Value = "Monthly Hiring Trends";
        titleRange.Style.Font.Size = 16;
        titleRange.Style.Font.Bold = true;
        titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        titleRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        titleRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        
        // Headers
        var headers = new[] { "Month", "Total Hires", "Departments Active" };
        for (int i = 0; i < headers.Length; i++)
        {
            trendsSheet.Cells[3, i + 1].Value = headers[i];
            trendsSheet.Cells[3, i + 1].Style.Font.Bold = true;
            trendsSheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            trendsSheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            trendsSheet.Cells[3, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
        }
        
        // Calculate monthly trends
        var monthlyTrends = data.GroupBy(x => new { ((dynamic)x).Year, ((dynamic)x).Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                TotalHires = g.Sum(x => (int)((dynamic)x).NewHires),
                DepartmentsActive = g.Select(x => (string)((dynamic)x).Department).Distinct().Count()
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToList();
        
        // Add data
        for (int i = 0; i < monthlyTrends.Count; i++)
        {
            var trend = monthlyTrends[i];
            var row = i + 4;
            
            trendsSheet.Cells[row, 1].Value = trend.MonthName;
            trendsSheet.Cells[row, 2].Value = trend.TotalHires;
            trendsSheet.Cells[row, 3].Value = trend.DepartmentsActive;
        }
        
        trendsSheet.Cells.AutoFitColumns();
        
        // Add borders
        var dataRange = trendsSheet.Cells[3, 1, 3 + monthlyTrends.Count, 3];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
    }
}
