using System.Text;
using EMS.API.Data;
using EMS.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using Microsoft.Extensions.Caching.Memory;

namespace EMS.API.Services;

public class ReportService : IReportService
{
    private readonly EMSDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

    public ReportService(EMSDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<byte[]> GenerateEmployeeDirectoryReportAsync()
    {
        const string cacheKey = "employee_directory_report";
        
        if (_cache.TryGetValue(cacheKey, out byte[]? cachedData))
        {
            return cachedData!;
        }

        var data = await _context.Employees.Include(e => e.Department).Where(e => e.IsActive).ToListAsync();
        var sb = new StringBuilder();
        sb.AppendLine("Id,FirstName,LastName,Email,Department,Position,Salary");
        foreach (var e in data)
        {
            sb.AppendLine($"{e.Id},{e.FirstName},{e.LastName},{e.Email},{e.Department.Name},{e.Position},{e.Salary}");
        }
        
        var result = Encoding.UTF8.GetBytes(sb.ToString());
        _cache.Set(cacheKey, result, _cacheExpiration);
        
        return result;
    }

    public async Task<byte[]> GenerateDepartmentReportAsync()
    {
        const string cacheKey = "department_report";
        
        if (_cache.TryGetValue(cacheKey, out byte[]? cachedData))
        {
            return cachedData!;
        }

        var data = await _context.Departments.Include(d => d.Employees).ToListAsync();
        var sb = new StringBuilder();
        sb.AppendLine("Id,Name,EmployeesCount");
        foreach (var d in data)
        {
            sb.AppendLine($"{d.Id},{d.Name},{d.Employees.Count}");
        }
        
        var result = Encoding.UTF8.GetBytes(sb.ToString());
        _cache.Set(cacheKey, result, _cacheExpiration);
        
        return result;
    }

    public async Task<byte[]> GenerateAttendanceReportAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Attendances.Include(a => a.Employee).AsQueryable();
        if (startDate.HasValue) query = query.Where(a => a.Date >= startDate.Value.Date);
        if (endDate.HasValue) query = query.Where(a => a.Date <= endDate.Value.Date);
        var data = await query.OrderBy(a => a.Date).ToListAsync();
        var sb = new StringBuilder();
        sb.AppendLine("Date,Employee,CheckIn,CheckOut,TotalHours");
        foreach (var a in data)
        {
            sb.AppendLine($"{a.Date:yyyy-MM-dd},{a.Employee.FirstName} {a.Employee.LastName},{a.CheckInTime:o},{a.CheckOutTime:o},{a.TotalHours}");
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task<byte[]> GenerateSalaryReportAsync()
    {
        var data = await _context.Employees.Where(e => e.IsActive).ToListAsync();
        var sb = new StringBuilder();
        sb.AppendLine("Id,Name,Salary");
        foreach (var e in data)
        {
            sb.AppendLine($"{e.Id},{e.FirstName} {e.LastName},{e.Salary}");
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task<byte[]> GenerateHiringTrendReportAsync()
    {
        var data = await _context.Employees
            .Where(e => e.IsActive)
            .GroupBy(e => new { e.DateOfJoining.Year, e.DateOfJoining.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthName = g.Key.Month,
                Count = g.Count(),
                Employees = g.Select(e => new { e.FirstName, e.LastName, e.Position, e.Department.Name })
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Year,Month,MonthName,NewHires,EmployeeDetails");
        
        foreach (var item in data)
        {
            var monthName = new DateTime(item.Year, item.Month, 1).ToString("MMMM");
            var employeeDetails = string.Join("; ", item.Employees.Select(e => $"{e.FirstName} {e.LastName} ({e.Position}) - {e.Name}"));
            sb.AppendLine($"{item.Year},{item.Month},{monthName},{item.Count},\"{employeeDetails}\"");
        }
        
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task<byte[]> GenerateDepartmentGrowthReportAsync()
    {
        var data = await _context.Departments
            .Include(d => d.Employees)
            .Select(d => new
            {
                DepartmentId = d.Id,
                DepartmentName = d.Name,
                TotalEmployees = d.Employees.Count(e => e.IsActive),
                NewHiresThisYear = d.Employees.Count(e => e.IsActive && e.DateOfJoining.Year == DateTime.Now.Year),
                NewHiresLastYear = d.Employees.Count(e => e.IsActive && e.DateOfJoining.Year == DateTime.Now.Year - 1),
                AverageTenure = d.Employees.Where(e => e.IsActive).Average(e => (DateTime.Now - e.DateOfJoining).TotalDays / 365.25),
                GrowthRate = d.Employees.Count(e => e.IsActive && e.DateOfJoining.Year == DateTime.Now.Year) > 0 ? 
                    (double)d.Employees.Count(e => e.IsActive && e.DateOfJoining.Year == DateTime.Now.Year) / 
                    Math.Max(1, d.Employees.Count(e => e.IsActive && e.DateOfJoining.Year == DateTime.Now.Year - 1)) * 100 : 0
            })
            .OrderByDescending(d => d.GrowthRate)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("DepartmentId,DepartmentName,TotalEmployees,NewHiresThisYear,NewHiresLastYear,AverageTenureYears,GrowthRatePercentage");
        
        foreach (var item in data)
        {
            sb.AppendLine($"{item.DepartmentId},{item.DepartmentName},{item.TotalEmployees},{item.NewHiresThisYear},{item.NewHiresLastYear},{item.AverageTenure:F2},{item.GrowthRate:F2}");
        }
        
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task<byte[]> GenerateAttendancePatternReportAsync()
    {
        var data = await _context.Attendances
            .Include(a => a.Employee)
            .Where(a => a.CheckOutTime.HasValue)
            .Select(a => new
            {
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FirstName + " " + a.Employee.LastName,
                Date = a.Date,
                CheckInTime = a.CheckInTime,
                CheckOutTime = a.CheckOutTime.Value,
                TotalHours = a.TotalHours.Value,
                DayOfWeek = a.Date.DayOfWeek,
                IsLateArrival = a.CheckInTime.TimeOfDay > TimeSpan.FromHours(9), // Late if after 9 AM
                IsEarlyDeparture = a.CheckOutTime.Value.TimeOfDay < TimeSpan.FromHours(17), // Early if before 5 PM
                IsOvertime = a.TotalHours.Value > TimeSpan.FromHours(8) // Overtime if more than 8 hours
            })
            .ToListAsync();

        var patterns = data
            .GroupBy(a => a.EmployeeId)
            .Select(g => new
            {
                EmployeeId = g.Key,
                EmployeeName = g.First().EmployeeName,
                TotalDays = g.Count(),
                AverageHours = g.Average(a => a.TotalHours.TotalHours),
                LateArrivals = g.Count(a => a.IsLateArrival),
                EarlyDepartures = g.Count(a => a.IsEarlyDeparture),
                OvertimeDays = g.Count(a => a.IsOvertime),
                LateArrivalRate = (double)g.Count(a => a.IsLateArrival) / g.Count() * 100,
                EarlyDepartureRate = (double)g.Count(a => a.IsEarlyDeparture) / g.Count() * 100,
                OvertimeRate = (double)g.Count(a => a.IsOvertime) / g.Count() * 100,
                MostFrequentDay = g.GroupBy(a => a.DayOfWeek)
                    .OrderByDescending(d => d.Count())
                    .First().Key.ToString()
            })
            .OrderByDescending(p => p.AverageHours)
            .ToList();

        var sb = new StringBuilder();
        sb.AppendLine("EmployeeId,EmployeeName,TotalDays,AverageHours,LateArrivals,EarlyDepartures,OvertimeDays,LateArrivalRate,EarlyDepartureRate,OvertimeRate,MostFrequentDay");
        
        foreach (var pattern in patterns)
        {
            sb.AppendLine($"{pattern.EmployeeId},{pattern.EmployeeName},{pattern.TotalDays},{pattern.AverageHours:F2},{pattern.LateArrivals},{pattern.EarlyDepartures},{pattern.OvertimeDays},{pattern.LateArrivalRate:F2},{pattern.EarlyDepartureRate:F2},{pattern.OvertimeRate:F2},{pattern.MostFrequentDay}");
        }
        
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task<byte[]> GeneratePerformanceMetricsReportAsync(int? employeeId = null)
    {
        var query = _context.PerformanceMetrics
            .Include(p => p.Employee)
            .AsQueryable();

        if (employeeId.HasValue)
        {
            query = query.Where(p => p.EmployeeId == employeeId.Value);
        }

        var data = await query
            .Select(p => new
            {
                Id = p.Id,
                EmployeeId = p.EmployeeId,
                EmployeeName = p.Employee.FirstName + " " + p.Employee.LastName,
                Year = p.Year,
                Quarter = p.Quarter,
                PerformanceScore = p.PerformanceScore,
                Goals = p.Goals ?? "",
                Achievements = p.Achievements ?? "",
                Comments = p.Comments ?? "",
                CreatedAt = p.CreatedAt
            })
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Quarter)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Id,EmployeeId,EmployeeName,Year,Quarter,PerformanceScore,Goals,Achievements,Comments,CreatedAt");
        
        foreach (var item in data)
        {
            sb.AppendLine($"{item.Id},{item.EmployeeId},{item.EmployeeName},{item.Year},{item.Quarter},{item.PerformanceScore:F2},\"{item.Goals}\",\"{item.Achievements}\",\"{item.Comments}\",{item.CreatedAt:yyyy-MM-dd}");
        }
        
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    // PDF Generation Methods
    public async Task<byte[]> GenerateEmployeeDirectoryReportPdfAsync()
    {
        var data = await _context.Employees.Include(e => e.Department).Where(e => e.IsActive).ToListAsync();
        
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
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
        var headerCell = new PdfPCell(new Phrase("ID", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        headerCell = new PdfPCell(new Phrase("First Name", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        headerCell = new PdfPCell(new Phrase("Last Name", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        headerCell = new PdfPCell(new Phrase("Email", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        headerCell = new PdfPCell(new Phrase("Department", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        headerCell = new PdfPCell(new Phrase("Position", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        headerCell = new PdfPCell(new Phrase("Salary", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        // Add data rows
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
        
        document.Add(table);
        
        // Add footer
        document.Add(new Paragraph(" "));
        var footerFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, BaseColor.GRAY);
        var footer = new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", footerFont);
        footer.Alignment = Element.ALIGN_RIGHT;
        document.Add(footer);
        
        document.Close();
        return memoryStream.ToArray();
    }

    public async Task<byte[]> GenerateDepartmentReportPdfAsync()
    {
        var data = await _context.Departments.Include(d => d.Employees).ToListAsync();
        
        using var memoryStream = new MemoryStream();
        var document = new Document(PageSize.A4, 25, 25, 30, 30);
        var writer = PdfWriter.GetInstance(document, memoryStream);
        
        document.Open();
        
        // Add title
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.DARK_GRAY);
        var title = new Paragraph("Department Report", titleFont);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);
        
        document.Add(new Paragraph(" "));
        
        // Add table
        var table = new PdfPTable(4);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 1f, 3f, 2f, 2f });
        
        // Add headers
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
        var headerCell = new PdfPCell(new Phrase("ID", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        headerCell = new PdfPCell(new Phrase("Department Name", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        headerCell = new PdfPCell(new Phrase("Manager", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        headerCell = new PdfPCell(new Phrase("Employee Count", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        // Add data rows
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
        foreach (var dept in data)
        {
            table.AddCell(new PdfPCell(new Phrase(dept.Id.ToString(), dataFont)));
            table.AddCell(new PdfPCell(new Phrase(dept.Name, dataFont)));
            table.AddCell(new PdfPCell(new Phrase(dept.ManagerName ?? "N/A", dataFont)));
            table.AddCell(new PdfPCell(new Phrase(dept.Employees.Count.ToString(), dataFont)));
        }
        
        document.Add(table);
        
        // Add footer
        document.Add(new Paragraph(" "));
        var footerFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, BaseColor.GRAY);
        var footer = new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", footerFont);
        footer.Alignment = Element.ALIGN_RIGHT;
        document.Add(footer);
        
        document.Close();
        return memoryStream.ToArray();
    }

    public async Task<byte[]> GenerateAttendanceReportPdfAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Attendances.Include(a => a.Employee).AsQueryable();
        if (startDate.HasValue) query = query.Where(a => a.Date >= startDate.Value.Date);
        if (endDate.HasValue) query = query.Where(a => a.Date <= endDate.Value.Date);
        var data = await query.OrderBy(a => a.Date).ToListAsync();
        
        using var memoryStream = new MemoryStream();
        var document = new Document(PageSize.A4, 25, 25, 30, 30);
        var writer = PdfWriter.GetInstance(document, memoryStream);
        
        document.Open();
        
        // Add title
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.DARK_GRAY);
        var title = new Paragraph("Attendance Report", titleFont);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);
        
        if (startDate.HasValue || endDate.HasValue)
        {
            var dateRangeFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.GRAY);
            var dateRange = $"Date Range: {startDate?.ToString("yyyy-MM-dd") ?? "Start"} to {endDate?.ToString("yyyy-MM-dd") ?? "End"}";
            var dateRangePara = new Paragraph(dateRange, dateRangeFont);
            dateRangePara.Alignment = Element.ALIGN_CENTER;
            document.Add(dateRangePara);
        }
        
        document.Add(new Paragraph(" "));
        
        // Add table
        var table = new PdfPTable(5);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 2f, 3f, 2f, 2f, 2f });
        
        // Add headers
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
        var headerCell = new PdfPCell(new Phrase("Date", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        headerCell = new PdfPCell(new Phrase("Employee", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        headerCell = new PdfPCell(new Phrase("Check In", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        headerCell = new PdfPCell(new Phrase("Check Out", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        headerCell = new PdfPCell(new Phrase("Total Hours", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        // Add data rows
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
        foreach (var attendance in data)
        {
            table.AddCell(new PdfPCell(new Phrase(attendance.Date.ToString("yyyy-MM-dd"), dataFont)));
            table.AddCell(new PdfPCell(new Phrase($"{attendance.Employee.FirstName} {attendance.Employee.LastName}", dataFont)));
            table.AddCell(new PdfPCell(new Phrase(attendance.CheckInTime.ToString("HH:mm"), dataFont)));
            table.AddCell(new PdfPCell(new Phrase(attendance.CheckOutTime?.ToString("HH:mm") ?? "N/A", dataFont)));
            table.AddCell(new PdfPCell(new Phrase(attendance.TotalHours?.ToString(@"hh\:mm") ?? "N/A", dataFont)));
        }
        
        document.Add(table);
        
        // Add footer
        document.Add(new Paragraph(" "));
        var footerFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, BaseColor.GRAY);
        var footer = new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", footerFont);
        footer.Alignment = Element.ALIGN_RIGHT;
        document.Add(footer);
        
        document.Close();
        return memoryStream.ToArray();
    }

    public async Task<byte[]> GenerateSalaryReportPdfAsync()
    {
        var data = await _context.Employees.Where(e => e.IsActive).ToListAsync();
        
        using var memoryStream = new MemoryStream();
        var document = new Document(PageSize.A4, 25, 25, 30, 30);
        var writer = PdfWriter.GetInstance(document, memoryStream);
        
        document.Open();
        
        // Add title
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.DARK_GRAY);
        var title = new Paragraph("Salary Report", titleFont);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);
        
        document.Add(new Paragraph(" "));
        
        // Add table
        var table = new PdfPTable(3);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 1f, 4f, 2f });
        
        // Add headers
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
        var headerCell = new PdfPCell(new Phrase("ID", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        headerCell = new PdfPCell(new Phrase("Employee Name", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        headerCell = new PdfPCell(new Phrase("Salary", headerFont));
        headerCell.BackgroundColor = BaseColor.DARK_GRAY;
        table.AddCell(headerCell);
        
        // Add data rows
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
        foreach (var employee in data)
        {
            table.AddCell(new PdfPCell(new Phrase(employee.Id.ToString(), dataFont)));
            table.AddCell(new PdfPCell(new Phrase($"{employee.FirstName} {employee.LastName}", dataFont)));
            table.AddCell(new PdfPCell(new Phrase(employee.Salary.ToString("C"), dataFont)));
        }
        
        document.Add(table);
        
        // Add summary
        document.Add(new Paragraph(" "));
        var summaryFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.DARK_GRAY);
        var totalSalary = data.Sum(e => e.Salary);
        var avgSalary = data.Average(e => e.Salary);
        var summary = new Paragraph($"Total Employees: {data.Count} | Total Salary: {totalSalary:C} | Average Salary: {avgSalary:C}", summaryFont);
        summary.Alignment = Element.ALIGN_CENTER;
        document.Add(summary);
        
        // Add footer
        document.Add(new Paragraph(" "));
        var footerFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, BaseColor.GRAY);
        var footer = new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", footerFont);
        footer.Alignment = Element.ALIGN_RIGHT;
        document.Add(footer);
        
        document.Close();
        return memoryStream.ToArray();
    }

    // Excel Generation Methods
    public async Task<byte[]> GenerateEmployeeDirectoryReportExcelAsync()
    {
        var data = await _context.Employees.Include(e => e.Department).Where(e => e.IsActive).ToListAsync();
        
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Employee Directory");
        
        // Add headers
        worksheet.Cells[1, 1].Value = "ID";
        worksheet.Cells[1, 2].Value = "First Name";
        worksheet.Cells[1, 3].Value = "Last Name";
        worksheet.Cells[1, 4].Value = "Email";
        worksheet.Cells[1, 5].Value = "Department";
        worksheet.Cells[1, 6].Value = "Position";
        worksheet.Cells[1, 7].Value = "Salary";
        worksheet.Cells[1, 8].Value = "Date of Joining";
        
        // Style headers
        using (var range = worksheet.Cells[1, 1, 1, 8])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkGray);
            range.Style.Font.Color.SetColor(System.Drawing.Color.White);
        }
        
        // Add data
        for (int i = 0; i < data.Count; i++)
        {
            var employee = data[i];
            var row = i + 2;
            
            worksheet.Cells[row, 1].Value = employee.Id;
            worksheet.Cells[row, 2].Value = employee.FirstName;
            worksheet.Cells[row, 3].Value = employee.LastName;
            worksheet.Cells[row, 4].Value = employee.Email;
            worksheet.Cells[row, 5].Value = employee.Department.Name;
            worksheet.Cells[row, 6].Value = employee.Position ?? "";
            worksheet.Cells[row, 7].Value = employee.Salary;
            worksheet.Cells[row, 8].Value = employee.DateOfJoining.ToString("yyyy-MM-dd");
        }
        
        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();
        
        // Add summary
        var summaryRow = data.Count + 3;
        worksheet.Cells[summaryRow, 1].Value = "Total Employees:";
        worksheet.Cells[summaryRow, 2].Value = data.Count;
        worksheet.Cells[summaryRow, 6].Value = "Total Salary:";
        worksheet.Cells[summaryRow, 7].Value = data.Sum(e => e.Salary);
        worksheet.Cells[summaryRow, 7].Style.Numberformat.Format = "$#,##0.00";
        
        return package.GetAsByteArray();
    }

    public async Task<byte[]> GenerateDepartmentReportExcelAsync()
    {
        var data = await _context.Departments.Include(d => d.Employees).ToListAsync();
        
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Department Report");
        
        // Add headers
        worksheet.Cells[1, 1].Value = "ID";
        worksheet.Cells[1, 2].Value = "Department Name";
        worksheet.Cells[1, 3].Value = "Manager";
        worksheet.Cells[1, 4].Value = "Employee Count";
        worksheet.Cells[1, 5].Value = "Description";
        
        // Style headers
        using (var range = worksheet.Cells[1, 1, 1, 5])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkGray);
            range.Style.Font.Color.SetColor(System.Drawing.Color.White);
        }
        
        // Add data
        for (int i = 0; i < data.Count; i++)
        {
            var dept = data[i];
            var row = i + 2;
            
            worksheet.Cells[row, 1].Value = dept.Id;
            worksheet.Cells[row, 2].Value = dept.Name;
            worksheet.Cells[row, 3].Value = dept.ManagerName ?? "N/A";
            worksheet.Cells[row, 4].Value = dept.Employees.Count;
            worksheet.Cells[row, 5].Value = dept.Description ?? "";
        }
        
        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();
        
        return package.GetAsByteArray();
    }

    public async Task<byte[]> GenerateAttendanceReportExcelAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Attendances.Include(a => a.Employee).AsQueryable();
        if (startDate.HasValue) query = query.Where(a => a.Date >= startDate.Value.Date);
        if (endDate.HasValue) query = query.Where(a => a.Date <= endDate.Value.Date);
        var data = await query.OrderBy(a => a.Date).ToListAsync();
        
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Attendance Report");
        
        // Add headers
        worksheet.Cells[1, 1].Value = "Date";
        worksheet.Cells[1, 2].Value = "Employee";
        worksheet.Cells[1, 3].Value = "Check In";
        worksheet.Cells[1, 4].Value = "Check Out";
        worksheet.Cells[1, 5].Value = "Total Hours";
        worksheet.Cells[1, 6].Value = "Notes";
        
        // Style headers
        using (var range = worksheet.Cells[1, 1, 1, 6])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkGray);
            range.Style.Font.Color.SetColor(System.Drawing.Color.White);
        }
        
        // Add data
        for (int i = 0; i < data.Count; i++)
        {
            var attendance = data[i];
            var row = i + 2;
            
            worksheet.Cells[row, 1].Value = attendance.Date.ToString("yyyy-MM-dd");
            worksheet.Cells[row, 2].Value = $"{attendance.Employee.FirstName} {attendance.Employee.LastName}";
            worksheet.Cells[row, 3].Value = attendance.CheckInTime.ToString("HH:mm");
            worksheet.Cells[row, 4].Value = attendance.CheckOutTime?.ToString("HH:mm") ?? "N/A";
            worksheet.Cells[row, 5].Value = attendance.TotalHours?.ToString(@"hh\:mm") ?? "N/A";
            worksheet.Cells[row, 6].Value = attendance.Notes ?? "";
        }
        
        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();
        
        return package.GetAsByteArray();
    }

    public async Task<byte[]> GenerateSalaryReportExcelAsync()
    {
        var data = await _context.Employees.Where(e => e.IsActive).ToListAsync();
        
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Salary Report");
        
        // Add headers
        worksheet.Cells[1, 1].Value = "ID";
        worksheet.Cells[1, 2].Value = "Employee Name";
        worksheet.Cells[1, 3].Value = "Position";
        worksheet.Cells[1, 4].Value = "Department";
        worksheet.Cells[1, 5].Value = "Salary";
        worksheet.Cells[1, 6].Value = "Date of Joining";
        
        // Style headers
        using (var range = worksheet.Cells[1, 1, 1, 6])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkGray);
            range.Style.Font.Color.SetColor(System.Drawing.Color.White);
        }
        
        // Add data
        for (int i = 0; i < data.Count; i++)
        {
            var employee = data[i];
            var row = i + 2;
            
            worksheet.Cells[row, 1].Value = employee.Id;
            worksheet.Cells[row, 2].Value = $"{employee.FirstName} {employee.LastName}";
            worksheet.Cells[row, 3].Value = employee.Position ?? "";
            worksheet.Cells[row, 4].Value = employee.Department.Name;
            worksheet.Cells[row, 5].Value = employee.Salary;
            worksheet.Cells[row, 6].Value = employee.DateOfJoining.ToString("yyyy-MM-dd");
        }
        
        // Format salary column
        using (var range = worksheet.Cells[2, 5, data.Count + 1, 5])
        {
            range.Style.Numberformat.Format = "$#,##0.00";
        }
        
        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();
        
        // Add summary
        var summaryRow = data.Count + 3;
        worksheet.Cells[summaryRow, 1].Value = "Total Employees:";
        worksheet.Cells[summaryRow, 2].Value = data.Count;
        worksheet.Cells[summaryRow, 4].Value = "Total Salary:";
        worksheet.Cells[summaryRow, 5].Value = data.Sum(e => e.Salary);
        worksheet.Cells[summaryRow, 5].Style.Numberformat.Format = "$#,##0.00";
        
        var avgRow = summaryRow + 1;
        worksheet.Cells[avgRow, 4].Value = "Average Salary:";
        worksheet.Cells[avgRow, 5].Value = data.Average(e => e.Salary);
        worksheet.Cells[avgRow, 5].Style.Numberformat.Format = "$#,##0.00";
        
        return package.GetAsByteArray();
    }
}


