using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class EmployeeDirectoryExcelGenerator : BaseReportGenerator
{
    public EmployeeDirectoryExcelGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "employee_directory_excel";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public override string GetFileExtension() => "xlsx";

    private async Task<byte[]> GenerateReport()
    {
        var data = await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.IsActive)
            .ToListAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Employee Directory");
        
        // Set header
        var headerRange = worksheet.Cells[1, 1, 1, 10];
        headerRange.Merge = true;
        headerRange.Value = "Employee Directory Report";
        headerRange.Style.Font.Size = 16;
        headerRange.Style.Font.Bold = true;
        headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        
        // Add generated date
        worksheet.Cells[2, 1, 2, 10].Merge = true;
        worksheet.Cells[2, 1].Value = $"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        worksheet.Cells[2, 1].Style.Font.Italic = true;
        worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        
        // Add headers starting from row 4
        var headers = new[] { "ID", "First Name", "Last Name", "Email", "Phone", "Department", "Position", "Salary", "Date of Birth", "Date of Joining" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[4, i + 1].Value = headers[i];
            worksheet.Cells[4, i + 1].Style.Font.Bold = true;
            worksheet.Cells[4, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[4, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            worksheet.Cells[4, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
        }
        
        // Add data starting from row 5
        for (int i = 0; i < data.Count; i++)
        {
            var employee = data[i];
            var row = i + 5;
            
            worksheet.Cells[row, 1].Value = employee.Id;
            worksheet.Cells[row, 2].Value = employee.FirstName;
            worksheet.Cells[row, 3].Value = employee.LastName;
            worksheet.Cells[row, 4].Value = employee.Email;
            worksheet.Cells[row, 5].Value = employee.PhoneNumber;
            worksheet.Cells[row, 6].Value = employee.Department?.Name ?? "N/A";
            worksheet.Cells[row, 7].Value = employee.Position ?? "N/A";
            worksheet.Cells[row, 8].Value = employee.Salary;
            worksheet.Cells[row, 8].Style.Numberformat.Format = "$#,##0.00";
            worksheet.Cells[row, 9].Value = employee.DateOfBirth.ToString("yyyy-MM-dd");
            worksheet.Cells[row, 10].Value = employee.DateOfJoining.ToString("yyyy-MM-dd");
        }
        
        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();
        
        // Add borders
        var dataRange = worksheet.Cells[4, 1, 4 + data.Count, 10];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        
        // Add summary sheet
        AddSummarySheet(package, data);
        
        return package.GetAsByteArray();
    }

    private static void AddSummarySheet(ExcelPackage package, List<Models.Employee> data)
    {
        var summarySheet = package.Workbook.Worksheets.Add("Summary");
        
        // Title
        var titleRange = summarySheet.Cells[1, 1, 1, 4];
        titleRange.Merge = true;
        titleRange.Value = "Employee Directory Summary";
        titleRange.Style.Font.Size = 16;
        titleRange.Style.Font.Bold = true;
        titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        titleRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        titleRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        
        // Summary data
        var summaryData = new[]
        {
            new { Metric = "Total Employees", Value = data.Count.ToString() },
            new { Metric = "Total Salary Cost", Value = data.Sum(e => e.Salary).ToString("C") },
            new { Metric = "Average Salary", Value = data.Any() ? data.Average(e => e.Salary).ToString("C") : "$0.00" },
            new { Metric = "Minimum Salary", Value = data.Any() ? data.Min(e => e.Salary).ToString("C") : "$0.00" },
            new { Metric = "Maximum Salary", Value = data.Any() ? data.Max(e => e.Salary).ToString("C") : "$0.00" }
        };
        
        summarySheet.Cells[3, 1].Value = "Metric";
        summarySheet.Cells[3, 2].Value = "Value";
        summarySheet.Cells[3, 1, 3, 2].Style.Font.Bold = true;
        summarySheet.Cells[3, 1, 3, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
        summarySheet.Cells[3, 1, 3, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
        summarySheet.Cells[3, 1, 3, 2].Style.Font.Color.SetColor(System.Drawing.Color.White);
        
        for (int i = 0; i < summaryData.Length; i++)
        {
            var row = i + 4;
            summarySheet.Cells[row, 1].Value = summaryData[i].Metric;
            summarySheet.Cells[row, 2].Value = summaryData[i].Value;
        }
        
        summarySheet.Cells.AutoFitColumns();
        
        // Department breakdown
        summarySheet.Cells[11, 1].Value = "Department Breakdown";
        summarySheet.Cells[11, 1].Style.Font.Bold = true;
        summarySheet.Cells[11, 1].Style.Font.Size = 12;
        
        summarySheet.Cells[12, 1].Value = "Department";
        summarySheet.Cells[12, 2].Value = "Employee Count";
        summarySheet.Cells[12, 3].Value = "Average Salary";
        summarySheet.Cells[12, 4].Value = "Total Salary";
        summarySheet.Cells[12, 1, 12, 4].Style.Font.Bold = true;
        summarySheet.Cells[12, 1, 12, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
        summarySheet.Cells[12, 1, 12, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
        summarySheet.Cells[12, 1, 12, 4].Style.Font.Color.SetColor(System.Drawing.Color.White);
        
        var deptBreakdown = data
            .GroupBy(e => e.Department?.Name ?? "No Department")
            .Select(g => new { 
                Department = g.Key, 
                Count = g.Count(), 
                AvgSalary = g.Average(e => e.Salary), 
                TotalSalary = g.Sum(e => e.Salary) 
            })
            .OrderByDescending(x => x.TotalSalary);
        
        int deptRow = 13;
        foreach (var dept in deptBreakdown)
        {
            summarySheet.Cells[deptRow, 1].Value = dept.Department;
            summarySheet.Cells[deptRow, 2].Value = dept.Count;
            summarySheet.Cells[deptRow, 3].Value = dept.AvgSalary;
            summarySheet.Cells[deptRow, 3].Style.Numberformat.Format = "$#,##0.00";
            summarySheet.Cells[deptRow, 4].Value = dept.TotalSalary;
            summarySheet.Cells[deptRow, 4].Style.Numberformat.Format = "$#,##0.00";
            deptRow++;
        }
        
        summarySheet.Cells.AutoFitColumns();
    }
}
