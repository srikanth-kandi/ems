using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class PerformanceMetricsExcelGenerator : BaseReportGenerator
{
    public PerformanceMetricsExcelGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "performance_metrics_excel";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public override string GetFileExtension() => "xlsx";

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

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Performance Metrics");
        
        // Set header
        var headerRange = worksheet.Cells[1, 1, 1, 10];
        headerRange.Merge = true;
        headerRange.Value = "Performance Metrics Report";
        headerRange.Style.Font.Size = 16;
        headerRange.Style.Font.Bold = true;
        headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        
        // Add subtitle
        worksheet.Cells[2, 1, 2, 10].Merge = true;
        worksheet.Cells[2, 1].Value = $"Employee Performance Analysis - {DateTime.Now.Year}";
        worksheet.Cells[2, 1].Style.Font.Italic = true;
        worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        
        // Add generated date
        worksheet.Cells[3, 1, 3, 10].Merge = true;
        worksheet.Cells[3, 1].Value = $"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        worksheet.Cells[3, 1].Style.Font.Italic = true;
        worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        
        // Add summary statistics
        AddSummaryStats(worksheet, performanceMetrics);
        
        // Add headers starting from row 6
        var headers = new[] { "Employee ID", "Employee Name", "Department", "Year", "Quarter", "Performance Score", "Comments", "Goals", "Achievements", "Created Date" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[6, i + 1].Value = headers[i];
            worksheet.Cells[6, i + 1].Style.Font.Bold = true;
            worksheet.Cells[6, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[6, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            worksheet.Cells[6, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
        }
        
        // Add data starting from row 7
        for (int i = 0; i < performanceMetrics.Count; i++)
        {
            var metric = performanceMetrics[i];
            var row = i + 7;
            
            worksheet.Cells[row, 1].Value = metric.EmployeeId;
            worksheet.Cells[row, 2].Value = $"{metric.Employee.FirstName} {metric.Employee.LastName}";
            worksheet.Cells[row, 3].Value = metric.Employee.Department.Name;
            worksheet.Cells[row, 4].Value = metric.Year;
            worksheet.Cells[row, 5].Value = metric.Quarter;
            worksheet.Cells[row, 6].Value = metric.PerformanceScore;
            worksheet.Cells[row, 6].Style.Numberformat.Format = "0.0";
            worksheet.Cells[row, 7].Value = metric.Comments ?? "";
            worksheet.Cells[row, 8].Value = metric.Goals ?? "";
            worksheet.Cells[row, 9].Value = metric.Achievements ?? "";
            worksheet.Cells[row, 10].Value = metric.CreatedAt.ToString("yyyy-MM-dd");
            
            // Color code the performance score
            var scoreCell = worksheet.Cells[row, 6];
            if (metric.PerformanceScore >= 80)
            {
                scoreCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                scoreCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
            }
            else if (metric.PerformanceScore >= 60)
            {
                scoreCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                scoreCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
            }
            else
            {
                scoreCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                scoreCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCoral);
            }
        }
        
        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();
        
        // Add borders
        var dataRange = worksheet.Cells[6, 1, 6 + performanceMetrics.Count, 10];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        
        // Add employee summary sheet
        AddEmployeeSummarySheet(package, performanceMetrics);
        
        // Add department performance sheet
        AddDepartmentPerformanceSheet(package, performanceMetrics);
        
        return package.GetAsByteArray();
    }

    private static void AddSummaryStats(ExcelWorksheet worksheet, List<Models.PerformanceMetric> data)
    {
        var totalRecords = data.Count;
        var uniqueEmployees = data.Select(pm => pm.EmployeeId).Distinct().Count();
        var departments = data.Select(pm => pm.Employee.Department.Name).Distinct().Count();
        var avgScore = data.Average(pm => pm.PerformanceScore);
        var highPerformers = data.Count(pm => pm.PerformanceScore >= 80);
        
        // Summary section
        worksheet.Cells[4, 1].Value = "Summary Statistics:";
        worksheet.Cells[4, 1].Style.Font.Bold = true;
        worksheet.Cells[4, 1].Style.Font.Size = 12;
        
        var summaryData = new[]
        {
            $"Total Records: {totalRecords}",
            $"Unique Employees: {uniqueEmployees}",
            $"Departments: {departments}",
            $"Avg Score: {avgScore:F1}",
            $"High Performers: {highPerformers}"
        };
        
        for (int i = 0; i < summaryData.Length; i++)
        {
            worksheet.Cells[4, i + 1].Value = summaryData[i];
        }
    }

    private static void AddEmployeeSummarySheet(ExcelPackage package, List<Models.PerformanceMetric> data)
    {
        var summarySheet = package.Workbook.Worksheets.Add("Employee Summary");
        
        // Title
        var titleRange = summarySheet.Cells[1, 1, 1, 7];
        titleRange.Merge = true;
        titleRange.Value = "Employee Performance Summary";
        titleRange.Style.Font.Size = 16;
        titleRange.Style.Font.Bold = true;
        titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        titleRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        titleRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        
        // Headers
        var headers = new[] { "Employee ID", "Employee Name", "Department", "Records", "Avg Score", "Best Score", "Latest Score" };
        for (int i = 0; i < headers.Length; i++)
        {
            summarySheet.Cells[3, i + 1].Value = headers[i];
            summarySheet.Cells[3, i + 1].Style.Font.Bold = true;
            summarySheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            summarySheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            summarySheet.Cells[3, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
        }
        
        // Calculate employee summaries
        var employeeSummaries = data.GroupBy(pm => new { pm.EmployeeId, pm.Employee.FirstName, pm.Employee.LastName, pm.Employee.Department.Name })
            .Select(g => new
            {
                EmployeeId = g.Key.EmployeeId,
                EmployeeName = $"{g.Key.FirstName} {g.Key.LastName}",
                Department = g.Key.Name,
                Records = g.Count(),
                AvgScore = g.Average(pm => pm.PerformanceScore),
                BestScore = g.Max(pm => pm.PerformanceScore),
                LatestScore = g.OrderByDescending(pm => pm.CreatedAt).First().PerformanceScore
            })
            .OrderByDescending(x => x.AvgScore)
            .ToList();
        
        // Add data
        for (int i = 0; i < employeeSummaries.Count; i++)
        {
            var emp = employeeSummaries[i];
            var row = i + 4;
            
            summarySheet.Cells[row, 1].Value = emp.EmployeeId;
            summarySheet.Cells[row, 2].Value = emp.EmployeeName;
            summarySheet.Cells[row, 3].Value = emp.Department;
            summarySheet.Cells[row, 4].Value = emp.Records;
            summarySheet.Cells[row, 5].Value = emp.AvgScore;
            summarySheet.Cells[row, 5].Style.Numberformat.Format = "0.0";
            summarySheet.Cells[row, 6].Value = emp.BestScore;
            summarySheet.Cells[row, 6].Style.Numberformat.Format = "0.0";
            summarySheet.Cells[row, 7].Value = emp.LatestScore;
            summarySheet.Cells[row, 7].Style.Numberformat.Format = "0.0";
        }
        
        summarySheet.Cells.AutoFitColumns();
        
        // Add borders
        var dataRange = summarySheet.Cells[3, 1, 3 + employeeSummaries.Count, 7];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
    }

    private static void AddDepartmentPerformanceSheet(ExcelPackage package, List<Models.PerformanceMetric> data)
    {
        var deptSheet = package.Workbook.Worksheets.Add("Department Performance");
        
        // Title
        var titleRange = deptSheet.Cells[1, 1, 1, 6];
        titleRange.Merge = true;
        titleRange.Value = "Department Performance Analysis";
        titleRange.Style.Font.Size = 16;
        titleRange.Style.Font.Bold = true;
        titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        titleRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        titleRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        
        // Headers
        var headers = new[] { "Department", "Employees", "Records", "Avg Score", "High Performers", "Improvement Needed" };
        for (int i = 0; i < headers.Length; i++)
        {
            deptSheet.Cells[3, i + 1].Value = headers[i];
            deptSheet.Cells[3, i + 1].Style.Font.Bold = true;
            deptSheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            deptSheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            deptSheet.Cells[3, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
        }
        
        // Calculate department performance
        var deptPerformance = data.GroupBy(pm => pm.Employee.Department.Name)
            .Select(g => new
            {
                Department = g.Key,
                Employees = g.Select(pm => pm.EmployeeId).Distinct().Count(),
                Records = g.Count(),
                AvgScore = g.Average(pm => pm.PerformanceScore),
                HighPerformers = g.Count(pm => pm.PerformanceScore >= 80),
                ImprovementNeeded = g.Count(pm => pm.PerformanceScore < 50)
            })
            .OrderByDescending(x => x.AvgScore)
            .ToList();
        
        // Add data
        for (int i = 0; i < deptPerformance.Count; i++)
        {
            var dept = deptPerformance[i];
            var row = i + 4;
            
            deptSheet.Cells[row, 1].Value = dept.Department;
            deptSheet.Cells[row, 2].Value = dept.Employees;
            deptSheet.Cells[row, 3].Value = dept.Records;
            deptSheet.Cells[row, 4].Value = dept.AvgScore;
            deptSheet.Cells[row, 4].Style.Numberformat.Format = "0.0";
            deptSheet.Cells[row, 5].Value = dept.HighPerformers;
            deptSheet.Cells[row, 6].Value = dept.ImprovementNeeded;
        }
        
        deptSheet.Cells.AutoFitColumns();
        
        // Add borders
        var dataRange = deptSheet.Cells[3, 1, 3 + deptPerformance.Count, 6];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
    }
}
