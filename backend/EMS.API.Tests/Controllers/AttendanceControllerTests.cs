using System.Net;
using System.Text;
using System.Text.Json;
using EMS.API.DTOs;
using EMS.API.Tests;
using FluentAssertions;
using Xunit;

namespace EMS.API.Tests.Controllers;

public class AttendanceControllerTests : TestBase
{
    public AttendanceControllerTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CheckIn_WithValidData_ReturnsAttendance()
    {
        // Arrange
        await SeedTestDataAsync();
        var checkInDto = new CheckInDto
        {
            EmployeeId = 1,
            Notes = "Morning check-in"
        };
        var json = JsonSerializer.Serialize(checkInDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/attendance/check-in", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var attendance = JsonSerializer.Deserialize<AttendanceDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        attendance.Should().NotBeNull();
        attendance.EmployeeId.Should().Be(1);
        attendance.CheckInTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        attendance.CheckOutTime.Should().BeNull();
        attendance.Notes.Should().Be("Morning check-in");
    }

    [Fact]
    public async Task CheckIn_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var checkInDto = new CheckInDto
        {
            EmployeeId = 0, // Invalid: zero employee ID
            Notes = "Test notes"
        };
        var json = JsonSerializer.Serialize(checkInDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/attendance/check-in", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CheckOut_WithValidData_ReturnsAttendance()
    {
        // Arrange
        await SeedTestDataAsync();
        var checkOutDto = new CheckOutDto
        {
            EmployeeId = 1,
            Notes = "Evening check-out"
        };
        var json = JsonSerializer.Serialize(checkOutDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/attendance/check-out", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var attendance = JsonSerializer.Deserialize<AttendanceDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        attendance.Should().NotBeNull();
        attendance.EmployeeId.Should().Be(1);
        attendance.CheckOutTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        attendance.TotalHours.Should().NotBeNull();
    }

    [Fact]
    public async Task CheckOut_WithNoCheckIn_ReturnsNotFound()
    {
        // Arrange
        await SeedTestDataAsync();
        var checkOutDto = new CheckOutDto
        {
            EmployeeId = 3, // Carol has no check-in for today
            Notes = "Evening check-out"
        };
        var json = JsonSerializer.Serialize(checkOutDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/attendance/check-out", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CheckOut_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var checkOutDto = new CheckOutDto
        {
            EmployeeId = 0, // Invalid: zero employee ID
            Notes = "Test notes"
        };
        var json = JsonSerializer.Serialize(checkOutDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/attendance/check-out", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetEmployeeAttendance_WithValidEmployeeId_ReturnsAttendanceRecords()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/attendance/employee/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var attendances = JsonSerializer.Deserialize<List<AttendanceDto>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        attendances.Should().NotBeNull();
        attendances.Should().HaveCount(1);
        attendances.First().EmployeeId.Should().Be(1);
    }

    [Fact]
    public async Task GetEmployeeAttendance_WithDateRange_ReturnsFilteredRecords()
    {
        // Arrange
        await SeedTestDataAsync();
        var startDate = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
        var endDate = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd");

        // Act
        var response = await Client.GetAsync($"/api/attendance/employee/1?startDate={startDate}&endDate={endDate}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var attendances = JsonSerializer.Deserialize<List<AttendanceDto>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        attendances.Should().NotBeNull();
        attendances.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetTodayAttendance_WithValidEmployeeId_ReturnsTodayAttendance()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/attendance/employee/1/today");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var attendance = JsonSerializer.Deserialize<AttendanceDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        attendance.Should().NotBeNull();
        attendance.EmployeeId.Should().Be(1);
        attendance.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public async Task GetTodayAttendance_WithNoAttendance_ReturnsNotFound()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/attendance/employee/3/today");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllAttendance_ReturnsAllAttendanceRecords()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/attendance");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var attendances = JsonSerializer.Deserialize<List<AttendanceDto>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        attendances.Should().NotBeNull();
        attendances.Should().HaveCount(2); // Alice and Bob have attendance records
    }

    [Fact]
    public async Task GetAllAttendance_WithDateRange_ReturnsFilteredRecords()
    {
        // Arrange
        await SeedTestDataAsync();
        var startDate = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
        var endDate = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd");

        // Act
        var response = await Client.GetAsync($"/api/attendance?startDate={startDate}&endDate={endDate}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var attendances = JsonSerializer.Deserialize<List<AttendanceDto>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        attendances.Should().NotBeNull();
        attendances.Should().HaveCount(2);
    }
}
