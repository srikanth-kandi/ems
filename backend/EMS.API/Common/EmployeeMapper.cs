using EMS.API.DTOs;
using EMS.API.Models;

namespace EMS.API.Common;

public static class EmployeeMapper
{
    public static EmployeeDto ToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber,
            Address = employee.Address,
            DateOfBirth = employee.DateOfBirth,
            DateOfJoining = employee.DateOfJoining,
            Position = employee.Position,
            Salary = employee.Salary,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department?.Name ?? string.Empty,
            IsActive = employee.IsActive,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt
        };
    }

    public static Employee ToEntity(CreateEmployeeDto dto)
    {
        return new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            DateOfBirth = dto.DateOfBirth,
            DateOfJoining = dto.DateOfJoining,
            Position = dto.Position,
            Salary = dto.Salary,
            DepartmentId = dto.DepartmentId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(Employee existing, UpdateEmployeeDto dto)
    {
        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.Email = dto.Email;
        existing.PhoneNumber = dto.PhoneNumber;
        existing.Address = dto.Address;
        existing.DateOfBirth = dto.DateOfBirth;
        existing.DateOfJoining = dto.DateOfJoining;
        existing.Position = dto.Position;
        existing.Salary = dto.Salary;
        existing.DepartmentId = dto.DepartmentId;
        existing.IsActive = dto.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;
    }
}
