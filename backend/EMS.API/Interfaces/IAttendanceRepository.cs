using EMS.API.DTOs;

namespace EMS.API.Interfaces;

public interface IAttendanceRepository
{
    Task<AttendanceDto> CheckInAsync(CheckInDto checkInDto);
    Task<AttendanceDto?> CheckOutAsync(CheckOutDto checkOutDto);
    Task<IEnumerable<AttendanceDto>> GetEmployeeAttendanceAsync(int employeeId, DateTime? startDate = null, DateTime? endDate = null);
    Task<AttendanceDto?> GetTodayAttendanceAsync(int employeeId);
    Task<IEnumerable<AttendanceDto>> GetAllAttendanceAsync(DateTime? startDate = null, DateTime? endDate = null);
}
