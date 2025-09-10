using System.ComponentModel.DataAnnotations;

namespace EMS.API.Models;

public class Attendance
{
    public int Id { get; set; }
    
    public int EmployeeId { get; set; }
    
    public Employee Employee { get; set; } = null!;
    
    public DateTime CheckInTime { get; set; }
    
    public DateTime? CheckOutTime { get; set; }
    
    public TimeSpan? TotalHours { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public DateTime Date { get; set; } = DateTime.Today;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
}
