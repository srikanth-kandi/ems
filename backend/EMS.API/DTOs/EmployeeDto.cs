using System.ComponentModel.DataAnnotations;

namespace EMS.API.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime DateOfJoining { get; set; }
    public string? Position { get; set; }
    public decimal Salary { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateEmployeeDto
{
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
}

public class UpdateEmployeeDto
{
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
    
    public bool IsActive { get; set; }
}
