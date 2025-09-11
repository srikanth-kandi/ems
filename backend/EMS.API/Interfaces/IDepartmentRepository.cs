using EMS.API.DTOs;

namespace EMS.API.Interfaces;

public interface IDepartmentRepository
{
    Task<IEnumerable<DepartmentDto>> GetAllAsync();
    Task<DepartmentDto?> GetByIdAsync(int id);
    Task<DepartmentDto> CreateAsync(CreateDepartmentDto createDepartmentDto);
    Task<DepartmentDto?> UpdateAsync(int id, UpdateDepartmentDto updateDepartmentDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> NameExistsAsync(string name, int? excludeId = null);
    Task<IEnumerable<DepartmentDto>> GetDepartmentsWithEmployeeCountAsync();
}
