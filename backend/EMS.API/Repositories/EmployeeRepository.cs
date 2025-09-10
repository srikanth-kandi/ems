using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Interfaces;
using EMS.API.Models;

namespace EMS.API.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly EMSDbContext _context;
    private readonly ILogger<EmployeeRepository> _logger;

    public EmployeeRepository(EMSDbContext context, ILogger<EmployeeRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
    {
        var employees = await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.IsActive)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                Address = e.Address,
                DateOfBirth = e.DateOfBirth,
                DateOfJoining = e.DateOfJoining,
                Position = e.Position,
                Salary = e.Salary,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.Name,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            })
            .ToListAsync();

        return employees;
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.Id == id && e.IsActive)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                Address = e.Address,
                DateOfBirth = e.DateOfBirth,
                DateOfJoining = e.DateOfJoining,
                Position = e.Position,
                Salary = e.Salary,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.Name,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            })
            .FirstOrDefaultAsync();

        return employee;
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto createEmployeeDto)
    {
        var employee = new Employee
        {
            FirstName = createEmployeeDto.FirstName,
            LastName = createEmployeeDto.LastName,
            Email = createEmployeeDto.Email,
            PhoneNumber = createEmployeeDto.PhoneNumber,
            Address = createEmployeeDto.Address,
            DateOfBirth = createEmployeeDto.DateOfBirth,
            DateOfJoining = createEmployeeDto.DateOfJoining,
            Position = createEmployeeDto.Position,
            Salary = createEmployeeDto.Salary,
            DepartmentId = createEmployeeDto.DepartmentId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(employee.Id) ?? throw new InvalidOperationException("Failed to retrieve created employee");
    }

    public async Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeDto updateEmployeeDto)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return null;
        }

        employee.FirstName = updateEmployeeDto.FirstName;
        employee.LastName = updateEmployeeDto.LastName;
        employee.Email = updateEmployeeDto.Email;
        employee.PhoneNumber = updateEmployeeDto.PhoneNumber;
        employee.Address = updateEmployeeDto.Address;
        employee.DateOfBirth = updateEmployeeDto.DateOfBirth;
        employee.DateOfJoining = updateEmployeeDto.DateOfJoining;
        employee.Position = updateEmployeeDto.Position;
        employee.Salary = updateEmployeeDto.Salary;
        employee.DepartmentId = updateEmployeeDto.DepartmentId;
        employee.IsActive = updateEmployeeDto.IsActive;
        employee.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return false;
        }

        // Soft delete - mark as inactive
        employee.IsActive = false;
        employee.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Employees.AnyAsync(e => e.Id == id && e.IsActive);
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        var query = _context.Employees.Where(e => e.Email == email);
        
        if (excludeId.HasValue)
        {
            query = query.Where(e => e.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<EmployeeDto>> GetByDepartmentAsync(int departmentId)
    {
        var employees = await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.DepartmentId == departmentId && e.IsActive)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                Address = e.Address,
                DateOfBirth = e.DateOfBirth,
                DateOfJoining = e.DateOfJoining,
                Position = e.Position,
                Salary = e.Salary,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.Name,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            })
            .ToListAsync();

        return employees;
    }

    public async Task<IEnumerable<EmployeeDto>> BulkCreateAsync(IEnumerable<CreateEmployeeDto> employees)
    {
        var employeeEntities = employees.Select(dto => new Employee
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
        }).ToList();

        _context.Employees.AddRange(employeeEntities);
        await _context.SaveChangesAsync();

        // Return the created employees
        var createdIds = employeeEntities.Select(e => e.Id).ToList();
        return await _context.Employees
            .Include(e => e.Department)
            .Where(e => createdIds.Contains(e.Id))
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                Address = e.Address,
                DateOfBirth = e.DateOfBirth,
                DateOfJoining = e.DateOfJoining,
                Position = e.Position,
                Salary = e.Salary,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.Name,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            })
            .ToListAsync();
    }
}
