using Microsoft.AspNetCore.Mvc;
using EMS.API.DTOs;
using EMS.API.Interfaces;

namespace EMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IEmployeeRepository employeeRepository, ILogger<EmployeesController> logger)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
    {
        try
        {
            var employees = await _employeeRepository.GetAllAsync();
            return Ok(employees);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employees");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found");
            }
            return Ok(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving employee with ID {EmployeeId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee(CreateEmployeeDto createEmployeeDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = await _employeeRepository.CreateAsync(createEmployeeDto);
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeDto updateEmployeeDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = await _employeeRepository.UpdateAsync(id, updateEmployeeDto);
            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found");
            }

            return Ok(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating employee with ID {EmployeeId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        try
        {
            var result = await _employeeRepository.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"Employee with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting employee with ID {EmployeeId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
