using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EMS.API.DTOs;
using EMS.API.Interfaces;

namespace EMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<DepartmentsController> _logger;

    public DepartmentsController(IDepartmentRepository departmentRepository, ILogger<DepartmentsController> logger)
    {
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
    {
        try
        {
            var departments = await _departmentRepository.GetAllAsync();
            return Ok(departments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving departments");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DepartmentDto>> GetDepartment(int id)
    {
        try
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
            {
                return NotFound($"Department with ID {id} not found");
            }
            return Ok(department);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving department with ID {DepartmentId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HR")]
    public async Task<ActionResult<DepartmentDto>> CreateDepartment(CreateDepartmentDto createDepartmentDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if department name already exists
            var nameExists = await _departmentRepository.NameExistsAsync(createDepartmentDto.Name);
            if (nameExists)
            {
                return Conflict("Department with this name already exists");
            }

            var department = await _departmentRepository.CreateAsync(createDepartmentDto);
            return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, department);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating department");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> UpdateDepartment(int id, UpdateDepartmentDto updateDepartmentDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if department name already exists (excluding current department)
            var nameExists = await _departmentRepository.NameExistsAsync(updateDepartmentDto.Name, id);
            if (nameExists)
            {
                return Conflict("Department with this name already exists");
            }

            var department = await _departmentRepository.UpdateAsync(id, updateDepartmentDto);
            if (department == null)
            {
                return NotFound($"Department with ID {id} not found");
            }

            return Ok(department);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating department with ID {DepartmentId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        try
        {
            var result = await _departmentRepository.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"Department with ID {id} not found");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting department with ID {DepartmentId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("with-employee-count")]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartmentsWithEmployeeCount()
    {
        try
        {
            var departments = await _departmentRepository.GetDepartmentsWithEmployeeCountAsync();
            return Ok(departments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving departments with employee count");
            return StatusCode(500, "Internal server error");
        }
    }
}
