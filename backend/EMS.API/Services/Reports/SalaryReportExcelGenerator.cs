using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class SalaryReportExcelGenerator : BaseReportGenerator
{
    public SalaryReportExcelGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "salary_report_excel";
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
        var worksheet = package.Workbook.Worksheets.Add("Salary Report");
        
        // Set header
        var headerRange = worksheet.Cells[1, 1, 1, 8];
        headerRange.Merge = true;
        headerRange.Value = "Salary Report";
        headerRange.Style.Font.Size = 16;
        headerRange.Style.Font.Bold = true;
        headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        
        // Add generated date
        worksheet.Cells[2, 1, 2, 8].Merge = true;
        worksheet.Cells[2, 1].Value = $"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        worksheet.Cells[2, 1].Style.Font.Italic = true;
        worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        
        // Add headers starting from row 4
        var headers = new[] { "Employee", "Department", "Position", "Salary", "Experience", "Joining Date", "Age", "Years at Company" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[4, i + 1].Value = headers[i];
            worksheet.Cells[4, i + 1].Style.Font.Bold = true;
            worksheet.Cells[4, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[4, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            worksheet.Cells[4, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
        }
        
        // Add data starting from row 5 (sorted by salary descending)
        var sortedData = data.OrderByDescending(e => e.Salary).ThenBy(e => e.LastName).ToList();
        for (int i = 0; i < sortedData.Count; i++)
        {
            var employee = sortedData[i];
            var row = i + 5;
            
            var fullName = $"{employee.FirstName} {employee.LastName}";
            var experience = DateTime.Now.Year - employee.DateOfJoining.Year;
            var age = DateTime.Now.Year - employee.DateOfBirth.Year;
            var yearsAtCompany = DateTime.Now.Year - employee.DateOfJoining.Year;
            
            worksheet.Cells[row, 1].Value = fullName;
            worksheet.Cells[row, 2].Value = employee.Department?.Name ?? "N/A";
            worksheet.Cells[row, 3].Value = employee.Position ?? "N/A";
            worksheet.Cells[row, 4].Value = employee.Salary;
            worksheet.Cells[row, 4].Style.Numberformat.Format = "$#,##0.00";
            worksheet.Cells[row, 5].Value = experience;
            worksheet.Cells[row, 6].Value = employee.DateOfJoining.ToString("yyyy-MM-dd");
            worksheet.Cells[row, 7].Value = age;
            worksheet.Cells[row, 8].Value = yearsAtCompany;
        }
        
        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();
        
        // Add borders
        var dataRange = worksheet.Cells[4, 1, 4 + sortedData.Count, 8];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        
        // Add analysis sheets
        AddSalaryAnalysisSheet(package, data);
        AddDepartmentSalarySheet(package, data);
        
        return package.GetAsByteArray();
    }

    private static void AddSalaryAnalysisSheet(ExcelPackage package, List<Models.Employee> data)
    {
        var analysisSheet = package.Workbook.Worksheets.Add("Salary Analysis");
        
        // Title
        var titleRange = analysisSheet.Cells[1, 1, 1, 4];
        titleRange.Merge = true;
        titleRange.Value = "Salary Analysis";
        titleRange.Style.Font.Size = 16;
        titleRange.Style.Font.Bold = true;
        titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        titleRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        titleRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        
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
                new { Metric = "Total Employees", Value = data.Count.ToString() },
                new { Metric = "Total Payroll", Value = totalPayroll.ToString("C") },
                new { Metric = "Average Salary", Value = avgSalary.ToString("C") },
                new { Metric = "Median Salary", Value = medianSalary.ToString("C") },
                new { Metric = "Minimum Salary", Value = minSalary.ToString("C") },
                new { Metric = "Maximum Salary", Value = maxSalary.ToString("C") },
                new { Metric = "Salary Range", Value = (maxSalary - minSalary).ToString("C") }
            };
            
            analysisSheet.Cells[3, 1].Value = "Metric";
            analysisSheet.Cells[3, 2].Value = "Value";
            analysisSheet.Cells[3, 1, 3, 2].Style.Font.Bold = true;
            analysisSheet.Cells[3, 1, 3, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            analysisSheet.Cells[3, 1, 3, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            analysisSheet.Cells[3, 1, 3, 2].Style.Font.Color.SetColor(System.Drawing.Color.White);
            
            for (int i = 0; i < analysisData.Length; i++)
            {
                var row = i + 4;
                analysisSheet.Cells[row, 1].Value = analysisData[i].Metric;
                analysisSheet.Cells[row, 2].Value = analysisData[i].Value;
            }
            
            // Add salary brackets
            analysisSheet.Cells[12, 1].Value = "Salary Brackets";
            analysisSheet.Cells[12, 1].Style.Font.Bold = true;
            analysisSheet.Cells[12, 1].Style.Font.Size = 12;
            
            var brackets = new[]
            {
                new { Range = "< $50,000", Count = data.Count(e => e.Salary < 50000) },
                new { Range = "$50,000 - $75,000", Count = data.Count(e => e.Salary >= 50000 && e.Salary < 75000) },
                new { Range = "$75,000 - $100,000", Count = data.Count(e => e.Salary >= 75000 && e.Salary < 100000) },
                new { Range = "$100,000 - $125,000", Count = data.Count(e => e.Salary >= 100000 && e.Salary < 125000) },
                new { Range = "> $125,000", Count = data.Count(e => e.Salary >= 125000) }
            };
            
            analysisSheet.Cells[13, 1].Value = "Salary Range";
            analysisSheet.Cells[13, 2].Value = "Employee Count";
            analysisSheet.Cells[13, 3].Value = "Percentage";
            analysisSheet.Cells[13, 1, 13, 3].Style.Font.Bold = true;
            analysisSheet.Cells[13, 1, 13, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
            analysisSheet.Cells[13, 1, 13, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            analysisSheet.Cells[13, 1, 13, 3].Style.Font.Color.SetColor(System.Drawing.Color.White);
            
            for (int i = 0; i < brackets.Length; i++)
            {
                var row = i + 14;
                var percentage = data.Count > 0 ? (double)brackets[i].Count / data.Count * 100 : 0;
                analysisSheet.Cells[row, 1].Value = brackets[i].Range;
                analysisSheet.Cells[row, 2].Value = brackets[i].Count;
                analysisSheet.Cells[row, 3].Value = percentage;
                analysisSheet.Cells[row, 3].Style.Numberformat.Format = "0.0%";
            }
        }
        
        analysisSheet.Cells.AutoFitColumns();
    }

    private static void AddDepartmentSalarySheet(ExcelPackage package, List<Models.Employee> data)
    {
        var deptSheet = package.Workbook.Worksheets.Add("Department Analysis");
        
        // Title
        var titleRange = deptSheet.Cells[1, 1, 1, 6];
        titleRange.Merge = true;
        titleRange.Value = "Salary Analysis by Department";
        titleRange.Style.Font.Size = 16;
        titleRange.Style.Font.Bold = true;
        titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        titleRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        titleRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        
        // Headers
        var headers = new[] { "Department", "Employee Count", "Total Salary", "Average Salary", "Min Salary", "Max Salary" };
        for (int i = 0; i < headers.Length; i++)
        {
            deptSheet.Cells[3, i + 1].Value = headers[i];
            deptSheet.Cells[3, i + 1].Style.Font.Bold = true;
            deptSheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            deptSheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            deptSheet.Cells[3, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
        }
        
        var deptAnalysis = data
            .GroupBy(e => e.Department?.Name ?? "No Department")
            .Select(g => new { 
                Department = g.Key, 
                Count = g.Count(), 
                TotalSalary = g.Sum(e => e.Salary), 
                AvgSalary = g.Average(e => e.Salary),
                MinSalary = g.Min(e => e.Salary),
                MaxSalary = g.Max(e => e.Salary)
            })
            .OrderByDescending(x => x.AvgSalary);
        
        int row = 4;
        foreach (var dept in deptAnalysis)
        {
            deptSheet.Cells[row, 1].Value = dept.Department;
            deptSheet.Cells[row, 2].Value = dept.Count;
            deptSheet.Cells[row, 3].Value = dept.TotalSalary;
            deptSheet.Cells[row, 3].Style.Numberformat.Format = "$#,##0.00";
            deptSheet.Cells[row, 4].Value = dept.AvgSalary;
            deptSheet.Cells[row, 4].Style.Numberformat.Format = "$#,##0.00";
            deptSheet.Cells[row, 5].Value = dept.MinSalary;
            deptSheet.Cells[row, 5].Style.Numberformat.Format = "$#,##0.00";
            deptSheet.Cells[row, 6].Value = dept.MaxSalary;
            deptSheet.Cells[row, 6].Style.Numberformat.Format = "$#,##0.00";
            row++;
        }
        
        deptSheet.Cells.AutoFitColumns();
    }
}
