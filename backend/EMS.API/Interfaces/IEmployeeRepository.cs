using EMS.API.DTOs;
using EMS.API.Models;

namespace EMS.API.Interfaces;

public interface IEmployeeRepository
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync();
    Task<EmployeeDto?> GetByIdAsync(int id);
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto createEmployeeDto);
    Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeDto updateEmployeeDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    Task<IEnumerable<EmployeeDto>> GetByDepartmentAsync(int departmentId);
    Task<IEnumerable<EmployeeDto>> BulkCreateAsync(IEnumerable<CreateEmployeeDto> employees);
    Task<bool> BulkDeleteAsync(IEnumerable<int> employeeIds);
    Task<PagedResult<EmployeeDto>> GetPagedAsync(PaginationRequest request);
    Task<PagedResult<EmployeeDto>> GetByDepartmentPagedAsync(int departmentId, PaginationRequest request);
}
