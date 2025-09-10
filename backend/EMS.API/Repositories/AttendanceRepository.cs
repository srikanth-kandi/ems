using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EMS.API.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly EMSDbContext _context;

    public AttendanceRepository(EMSDbContext context)
    {
        _context = context;
    }

    public async Task<AttendanceDto> CheckInAsync(CheckInDto checkInDto)
    {
        var today = DateTime.Today;
        var existing = await _context.Attendances.FirstOrDefaultAsync(a => a.EmployeeId == checkInDto.EmployeeId && a.Date == today);
        if (existing != null)
        {
            // already checked in today
            return await MapAsync(existing.Id);
        }

        var entity = new EMS.API.Models.Attendance
        {
            EmployeeId = checkInDto.EmployeeId,
            CheckInTime = DateTime.UtcNow,
            Notes = checkInDto.Notes,
            Date = today,
            CreatedAt = DateTime.UtcNow
        };

        _context.Attendances.Add(entity);
        await _context.SaveChangesAsync();

        return await MapAsync(entity.Id);
    }

    public async Task<AttendanceDto?> CheckOutAsync(CheckOutDto checkOutDto)
    {
        var today = DateTime.Today;
        var entity = await _context.Attendances.FirstOrDefaultAsync(a => a.EmployeeId == checkOutDto.EmployeeId && a.Date == today);
        if (entity == null)
        {
            return null;
        }

        if (entity.CheckOutTime.HasValue)
        {
            return await MapAsync(entity.Id);
        }

        entity.CheckOutTime = DateTime.UtcNow;
        entity.TotalHours = entity.CheckOutTime - entity.CheckInTime;
        entity.Notes = string.IsNullOrWhiteSpace(checkOutDto.Notes) ? entity.Notes : checkOutDto.Notes;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await MapAsync(entity.Id);
    }

    public async Task<IEnumerable<AttendanceDto>> GetEmployeeAttendanceAsync(int employeeId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Attendances
            .Include(a => a.Employee)
            .Where(a => a.EmployeeId == employeeId)
            .AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(a => a.Date >= startDate.Value.Date);
        }
        if (endDate.HasValue)
        {
            query = query.Where(a => a.Date <= endDate.Value.Date);
        }

        return await query
            .OrderByDescending(a => a.Date)
            .Select(a => new AttendanceDto
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FirstName + " " + a.Employee.LastName,
                CheckInTime = a.CheckInTime,
                CheckOutTime = a.CheckOutTime,
                TotalHours = a.TotalHours,
                Notes = a.Notes,
                Date = a.Date,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<AttendanceDto?> GetTodayAttendanceAsync(int employeeId)
    {
        var today = DateTime.Today;
        var entity = await _context.Attendances
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == today);

        if (entity == null)
        {
            return null;
        }

        return new AttendanceDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            EmployeeName = entity.Employee.FirstName + " " + entity.Employee.LastName,
            CheckInTime = entity.CheckInTime,
            CheckOutTime = entity.CheckOutTime,
            TotalHours = entity.TotalHours,
            Notes = entity.Notes,
            Date = entity.Date,
            CreatedAt = entity.CreatedAt
        };
    }

    public async Task<IEnumerable<AttendanceDto>> GetAllAttendanceAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Attendances.Include(a => a.Employee).AsQueryable();
        if (startDate.HasValue)
        {
            query = query.Where(a => a.Date >= startDate.Value.Date);
        }
        if (endDate.HasValue)
        {
            query = query.Where(a => a.Date <= endDate.Value.Date);
        }

        return await query
            .OrderByDescending(a => a.Date)
            .Select(a => new AttendanceDto
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FirstName + " " + a.Employee.LastName,
                CheckInTime = a.CheckInTime,
                CheckOutTime = a.CheckOutTime,
                TotalHours = a.TotalHours,
                Notes = a.Notes,
                Date = a.Date,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
    }

    private async Task<AttendanceDto> MapAsync(int id)
    {
        var a = await _context.Attendances.Include(x => x.Employee).FirstAsync(x => x.Id == id);
        return new AttendanceDto
        {
            Id = a.Id,
            EmployeeId = a.EmployeeId,
            EmployeeName = a.Employee.FirstName + " " + a.Employee.LastName,
            CheckInTime = a.CheckInTime,
            CheckOutTime = a.CheckOutTime,
            TotalHours = a.TotalHours,
            Notes = a.Notes,
            Date = a.Date,
            CreatedAt = a.CreatedAt
        };
    }
}


