using System.ComponentModel.DataAnnotations;

namespace EMS.API.Models;

public class Employee
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    
    [MaxLength(500)]
    public string? Address { get; set; }
    
    public DateTime DateOfBirth { get; set; }
    
    public DateTime DateOfJoining { get; set; }
    
    [MaxLength(100)]
    public string? Position { get; set; }
    
    public decimal Salary { get; set; }
    
    public int DepartmentId { get; set; }
    
    public Department Department { get; set; } = null!;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<PerformanceMetric> PerformanceMetrics { get; set; } = new List<PerformanceMetric>();
}
