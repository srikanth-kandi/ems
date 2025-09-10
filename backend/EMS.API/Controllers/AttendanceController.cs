using EMS.API.DTOs;
using EMS.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceRepository _attendanceRepository;

    public AttendanceController(IAttendanceRepository attendanceRepository)
    {
        _attendanceRepository = attendanceRepository;
    }

    [HttpPost("check-in")]
    public async Task<ActionResult<AttendanceDto>> CheckIn([FromBody] CheckInDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _attendanceRepository.CheckInAsync(dto);
        return Ok(result);
    }

    [HttpPost("check-out")]
    public async Task<ActionResult<AttendanceDto>> CheckOut([FromBody] CheckOutDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _attendanceRepository.CheckOutAsync(dto);
        if (result == null)
        {
            return NotFound("No check-in found for today");
        }
        return Ok(result);
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetEmployeeAttendance(int employeeId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var result = await _attendanceRepository.GetEmployeeAttendanceAsync(employeeId, startDate, endDate);
        return Ok(result);
    }

    [HttpGet("employee/{employeeId}/today")]
    public async Task<ActionResult<AttendanceDto>> GetTodayAttendance(int employeeId)
    {
        var result = await _attendanceRepository.GetTodayAttendanceAsync(employeeId);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetAll([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var result = await _attendanceRepository.GetAllAttendanceAsync(startDate, endDate);
        return Ok(result);
    }
}


