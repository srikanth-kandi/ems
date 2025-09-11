using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Interfaces;
using EMS.API.Models;
using EMS.API.Common;

namespace EMS.API.Repositories;

public class EmployeeRepository : BaseRepository<Employee, EmployeeDto>, IEmployeeRepository
{
    public EmployeeRepository(EMSDbContext context, ILogger<EmployeeRepository> logger) 
        : base(context, logger)
    {
    }

    protected override IQueryable<Employee> GetBaseQuery()
    {
        return _context.Employees
            .Include(e => e.Department)
            .Where(e => e.IsActive);
    }

    protected override EmployeeDto MapToDto(Employee entity)
    {
        return EmployeeMapper.ToDto(entity);
    }

    protected override Employee MapToEntity(EmployeeDto dto)
    {
        throw new NotImplementedException("Use CreateEmployeeDto for creation");
    }

    protected override void UpdateEntity(Employee existing, EmployeeDto dto)
    {
        throw new NotImplementedException("Use UpdateEmployeeDto for updates");
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto createEmployeeDto)
    {
        var employee = EmployeeMapper.ToEntity(createEmployeeDto);
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

        EmployeeMapper.UpdateEntity(employee, updateEmployeeDto);
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
        return await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.DepartmentId == departmentId && e.IsActive)
            .Select(e => MapToDto(e))
            .ToListAsync();
    }

    public async Task<IEnumerable<EmployeeDto>> BulkCreateAsync(IEnumerable<CreateEmployeeDto> employees)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var employeeEntities = employees.Select(EmployeeMapper.ToEntity).ToList();

            // Validate all emails are unique before adding
            var emails = employeeEntities.Select(e => e.Email).ToList();
            var existingEmails = await _context.Employees
                .Where(e => emails.Contains(e.Email))
                .Select(e => e.Email)
                .ToListAsync();

            if (existingEmails.Any())
            {
                throw new InvalidOperationException($"The following emails already exist: {string.Join(", ", existingEmails)}");
            }

            _context.Employees.AddRange(employeeEntities);
            await _context.SaveChangesAsync();

            // Return the created employees
            var createdIds = employeeEntities.Select(e => e.Id).ToList();
            var result = await _context.Employees
                .Include(e => e.Department)
                .Where(e => createdIds.Contains(e.Id))
                .Select(e => MapToDto(e))
                .ToListAsync();

            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> BulkDeleteAsync(IEnumerable<int> employeeIds)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var employees = await _context.Employees
                .Where(e => employeeIds.Contains(e.Id))
                .ToListAsync();

            if (!employees.Any())
            {
                return false;
            }

            // Soft delete all employees
            foreach (var employee in employees)
            {
                employee.IsActive = false;
                employee.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PagedResult<EmployeeDto>> GetPagedAsync(PaginationRequest request)
    {
        var query = GetBaseQuery().AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(e => 
                e.FirstName.ToLower().Contains(searchTerm) ||
                e.LastName.ToLower().Contains(searchTerm) ||
                e.Email.ToLower().Contains(searchTerm) ||
                e.Position.ToLower().Contains(searchTerm) ||
                e.Department.Name.ToLower().Contains(searchTerm));
        }

        // Apply sorting
        query = ApplySorting(query, request);

        var totalCount = await query.CountAsync();

        var employees = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => MapToDto(e))
            .ToListAsync();

        return new PagedResult<EmployeeDto>
        {
            Data = employees,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PagedResult<EmployeeDto>> GetByDepartmentPagedAsync(int departmentId, PaginationRequest request)
    {
        var query = _context.Employees
            .Include(e => e.Department)
            .Where(e => e.DepartmentId == departmentId && e.IsActive)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(e => 
                e.FirstName.ToLower().Contains(searchTerm) ||
                e.LastName.ToLower().Contains(searchTerm) ||
                e.Email.ToLower().Contains(searchTerm) ||
                e.Position.ToLower().Contains(searchTerm));
        }

        // Apply sorting
        query = ApplySorting(query, request);

        var totalCount = await query.CountAsync();

        var employees = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => MapToDto(e))
            .ToListAsync();

        return new PagedResult<EmployeeDto>
        {
            Data = employees,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    private static IQueryable<Employee> ApplySorting(IQueryable<Employee> query, PaginationRequest request)
    {
        return request.SortBy?.ToLower() switch
        {
            "firstname" => request.SortDescending ? query.OrderByDescending(e => e.FirstName) : query.OrderBy(e => e.FirstName),
            "lastname" => request.SortDescending ? query.OrderByDescending(e => e.LastName) : query.OrderBy(e => e.LastName),
            "email" => request.SortDescending ? query.OrderByDescending(e => e.Email) : query.OrderBy(e => e.Email),
            "position" => request.SortDescending ? query.OrderByDescending(e => e.Position) : query.OrderBy(e => e.Position),
            "salary" => request.SortDescending ? query.OrderByDescending(e => e.Salary) : query.OrderBy(e => e.Salary),
            "department" => request.SortDescending ? query.OrderByDescending(e => e.Department.Name) : query.OrderBy(e => e.Department.Name),
            "dateofjoining" => request.SortDescending ? query.OrderByDescending(e => e.DateOfJoining) : query.OrderBy(e => e.DateOfJoining),
            _ => query.OrderBy(e => e.FirstName)
        };
    }
}