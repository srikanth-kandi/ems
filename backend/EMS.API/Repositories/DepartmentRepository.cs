using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Interfaces;
using EMS.API.Models;

namespace EMS.API.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly EMSDbContext _context;
    private readonly ILogger<DepartmentRepository> _logger;

    public DepartmentRepository(EMSDbContext context, ILogger<DepartmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
    {
        var departments = await _context.Departments
            .Include(d => d.Employees)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                ManagerName = d.ManagerName,
                EmployeeCount = d.Employees.Count(e => e.IsActive),
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .ToListAsync();

        return departments;
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id)
    {
        var department = await _context.Departments
            .Include(d => d.Employees)
            .Where(d => d.Id == id)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                ManagerName = d.ManagerName,
                EmployeeCount = d.Employees.Count(e => e.IsActive),
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .FirstOrDefaultAsync();

        return department;
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto createDepartmentDto)
    {
        var department = new Department
        {
            Name = createDepartmentDto.Name,
            Description = createDepartmentDto.Description,
            ManagerName = createDepartmentDto.ManagerName,
            CreatedAt = DateTime.UtcNow
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(department.Id) ?? throw new InvalidOperationException("Failed to retrieve created department");
    }

    public async Task<DepartmentDto?> UpdateAsync(int id, UpdateDepartmentDto updateDepartmentDto)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
        {
            return null;
        }

        department.Name = updateDepartmentDto.Name;
        department.Description = updateDepartmentDto.Description;
        department.ManagerName = updateDepartmentDto.ManagerName;
        department.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
        {
            return false;
        }

        // Check if department has active employees
        var hasActiveEmployees = await _context.Employees
            .AnyAsync(e => e.DepartmentId == id && e.IsActive);

        if (hasActiveEmployees)
        {
            throw new InvalidOperationException("Cannot delete department with active employees");
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Departments.AnyAsync(d => d.Id == id);
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
    {
        var query = _context.Departments.Where(d => d.Name == name);
        
        if (excludeId.HasValue)
        {
            query = query.Where(d => d.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<DepartmentDto>> GetDepartmentsWithEmployeeCountAsync()
    {
        var departments = await _context.Departments
            .Include(d => d.Employees)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                ManagerName = d.ManagerName,
                EmployeeCount = d.Employees.Count(e => e.IsActive),
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .OrderByDescending(d => d.EmployeeCount)
            .ToListAsync();

        return departments;
    }
}
