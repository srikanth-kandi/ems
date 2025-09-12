using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services.Reports;

public class DepartmentReportExcelGenerator : BaseReportGenerator
{
    public DepartmentReportExcelGenerator(EMSDbContext context, IMemoryCache cache) 
        : base(context, cache)
    {
    }

    public override async Task<byte[]> GenerateAsync()
    {
        const string cacheKey = "department_report_excel";
        return await GetCachedOrGenerateAsync(cacheKey, GenerateReport);
    }

    public override string GetContentType() => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public override string GetFileExtension() => "xlsx";

    private async Task<byte[]> GenerateReport()
    {
        var data = await _context.Departments
            .Include(d => d.Employees.Where(e => e.IsActive))
            .ToListAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Department Report");
        
        // Set header
        var headerRange = worksheet.Cells[1, 1, 1, 6];
        headerRange.Merge = true;
        headerRange.Value = "Department Report";
        headerRange.Style.Font.Size = 16;
        headerRange.Style.Font.Bold = true;
        headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        
        // Add generated date
        worksheet.Cells[2, 1, 2, 6].Merge = true;
        worksheet.Cells[2, 1].Value = $"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        worksheet.Cells[2, 1].Style.Font.Italic = true;
        worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        
        // Add headers starting from row 4
        var headers = new[] { "ID", "Name", "Description", "Manager", "Employee Count", "Created Date" };
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
            var department = data[i];
            var row = i + 5;
            
            worksheet.Cells[row, 1].Value = department.Id;
            worksheet.Cells[row, 2].Value = department.Name;
            worksheet.Cells[row, 3].Value = department.Description ?? "";
            worksheet.Cells[row, 4].Value = department.ManagerName ?? "";
            worksheet.Cells[row, 5].Value = department.Employees.Count;
            worksheet.Cells[row, 6].Value = department.CreatedAt.ToString("yyyy-MM-dd");
        }
        
        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();
        
        // Add borders
        var dataRange = worksheet.Cells[4, 1, 4 + data.Count, 6];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        
        // Add employee details sheet
        AddEmployeeDetailsSheet(package, data);
        
        return package.GetAsByteArray();
    }

    private static void AddEmployeeDetailsSheet(ExcelPackage package, List<Models.Department> data)
    {
        var employeeSheet = package.Workbook.Worksheets.Add("Employee Details");
        
        // Title
        var titleRange = employeeSheet.Cells[1, 1, 1, 8];
        titleRange.Merge = true;
        titleRange.Value = "Employee Details by Department";
        titleRange.Style.Font.Size = 16;
        titleRange.Style.Font.Bold = true;
        titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        titleRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        titleRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        
        // Headers
        var headers = new[] { "Department", "Employee ID", "Name", "Email", "Position", "Salary", "Joining Date", "Experience (Years)" };
        for (int i = 0; i < headers.Length; i++)
        {
            employeeSheet.Cells[3, i + 1].Value = headers[i];
            employeeSheet.Cells[3, i + 1].Style.Font.Bold = true;
            employeeSheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            employeeSheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
            employeeSheet.Cells[3, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
        }
        
        // Add employee data
        int currentRow = 4;
        foreach (var department in data.OrderBy(d => d.Name))
        {
            foreach (var employee in department.Employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName))
            {
                employeeSheet.Cells[currentRow, 1].Value = department.Name;
                employeeSheet.Cells[currentRow, 2].Value = employee.Id;
                employeeSheet.Cells[currentRow, 3].Value = $"{employee.FirstName} {employee.LastName}";
                employeeSheet.Cells[currentRow, 4].Value = employee.Email;
                employeeSheet.Cells[currentRow, 5].Value = employee.Position ?? "";
                employeeSheet.Cells[currentRow, 6].Value = employee.Salary;
                employeeSheet.Cells[currentRow, 6].Style.Numberformat.Format = "$#,##0.00";
                employeeSheet.Cells[currentRow, 7].Value = employee.DateOfJoining.ToString("yyyy-MM-dd");
                
                var experience = DateTime.Now.Year - employee.DateOfJoining.Year;
                employeeSheet.Cells[currentRow, 8].Value = experience;
                
                currentRow++;
            }
        }
        
        employeeSheet.Cells.AutoFitColumns();
        
        // Add borders
        var dataRange = employeeSheet.Cells[3, 1, currentRow - 1, 8];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
    }
}
