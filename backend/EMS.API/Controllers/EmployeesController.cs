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

    [HttpPost("bulk")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> BulkCreateEmployees([FromBody] IEnumerable<CreateEmployeeDto> employees)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdEmployees = await _employeeRepository.BulkCreateAsync(employees);
            return Ok(createdEmployees);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk creating employees");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("bulk")]
    public async Task<IActionResult> BulkDeleteEmployees([FromBody] IEnumerable<int> employeeIds)
    {
        try
        {
            var result = await _employeeRepository.BulkDeleteAsync(employeeIds);
            if (!result)
            {
                return NotFound("No employees found with the provided IDs");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk deleting employees");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<EmployeeDto>>> GetEmployeesPaged([FromQuery] PaginationRequest request)
    {
        try
        {
            // Validate pagination parameters
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1 || request.PageSize > 100) request.PageSize = 10;

            var result = await _employeeRepository.GetPagedAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged employees");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("department/{departmentId}/paged")]
    public async Task<ActionResult<PagedResult<EmployeeDto>>> GetEmployeesByDepartmentPaged(int departmentId, [FromQuery] PaginationRequest request)
    {
        try
        {
            // Validate pagination parameters
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1 || request.PageSize > 100) request.PageSize = 10;

            var result = await _employeeRepository.GetByDepartmentPagedAsync(departmentId, request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged employees for department {DepartmentId}", departmentId);
            return StatusCode(500, "Internal server error");
        }
    }
}
