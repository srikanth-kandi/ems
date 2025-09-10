using System.ComponentModel.DataAnnotations;

namespace EMS.API.Models;

public class PerformanceMetric
{
    public int Id { get; set; }
    
    public int EmployeeId { get; set; }
    
    public Employee Employee { get; set; } = null!;
    
    public int Year { get; set; }
    
    public int Quarter { get; set; }
    
    [Range(0, 100)]
    public decimal PerformanceScore { get; set; }
    
    [MaxLength(1000)]
    public string? Comments { get; set; }
    
    [MaxLength(100)]
    public string? Goals { get; set; }
    
    [MaxLength(100)]
    public string? Achievements { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
}
