using System.ComponentModel.DataAnnotations;

namespace EMS.API.DTOs;

public class AttendanceDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public TimeSpan? TotalHours { get; set; }
    public string? Notes { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CheckInDto
{
    [Required]
    public int EmployeeId { get; set; }
    
    public string? Notes { get; set; }
}

public class CheckOutDto
{
    [Required]
    public int EmployeeId { get; set; }
    
    public string? Notes { get; set; }
}
