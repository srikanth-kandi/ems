using System.Net;
using System.Text;
using System.Text.Json;
using EMS.API.DTOs;
using EMS.API.Tests;
using FluentAssertions;
using Xunit;

namespace EMS.API.Tests.Controllers;

public class EmployeesControllerTests : TestBase
{
    public EmployeesControllerTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetEmployees_ReturnsAllEmployees()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/employees");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var employees = JsonSerializer.Deserialize<List<EmployeeDto>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        employees.Should().NotBeNull();
        employees.Should().HaveCount(3);
        employees.Should().Contain(e => e.FirstName == "Alice");
        employees.Should().Contain(e => e.FirstName == "Bob");
        employees.Should().Contain(e => e.FirstName == "Carol");
    }

    [Fact]
    public async Task GetEmployee_WithValidId_ReturnsEmployee()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/employees/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var employee = JsonSerializer.Deserialize<EmployeeDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        employee.Should().NotBeNull();
        employee.Id.Should().Be(1);
        employee.FirstName.Should().Be("Alice");
        employee.Email.Should().Be("alice@test.com");
    }

    [Fact]
    public async Task GetEmployee_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/employees/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateEmployee_WithValidData_ReturnsCreatedEmployee()
    {
        // Arrange
        await SeedTestDataAsync();
        var createEmployeeDto = new CreateEmployeeDto
        {
            FirstName = "David",
            LastName = "Wilson",
            Email = "david@test.com",
            PhoneNumber = "123-456-7893",
            Address = "321 Test Rd",
            DateOfBirth = new DateTime(1988, 3, 10),
            DateOfJoining = new DateTime(2022, 1, 1),
            Position = "QA Engineer",
            Salary = 70000,
            DepartmentId = 1
        };
        var json = JsonSerializer.Serialize(createEmployeeDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/employees", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseContent = await response.Content.ReadAsStringAsync();
        var employee = JsonSerializer.Deserialize<EmployeeDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        employee.Should().NotBeNull();
        employee.FirstName.Should().Be("David");
        employee.Email.Should().Be("david@test.com");
        employee.DepartmentId.Should().Be(1);
    }

    [Fact]
    public async Task CreateEmployee_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var createEmployeeDto = new CreateEmployeeDto
        {
            FirstName = "", // Invalid: empty first name
            LastName = "Wilson",
            Email = "invalid-email", // Invalid email format
            DateOfBirth = new DateTime(1988, 3, 10),
            DateOfJoining = new DateTime(2022, 1, 1),
            Salary = 70000,
            DepartmentId = 1
        };
        var json = JsonSerializer.Serialize(createEmployeeDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/employees", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateEmployee_WithValidData_ReturnsUpdatedEmployee()
    {
        // Arrange
        await SeedTestDataAsync();
        var updateEmployeeDto = new UpdateEmployeeDto
        {
            FirstName = "Alice",
            LastName = "Johnson-Updated",
            Email = "alice.updated@test.com",
            PhoneNumber = "123-456-7890",
            Address = "123 Updated Test St",
            DateOfBirth = new DateTime(1990, 1, 1),
            DateOfJoining = new DateTime(2020, 1, 1),
            Position = "Senior Software Engineer",
            Salary = 85000,
            DepartmentId = 1,
            IsActive = true
        };
        var json = JsonSerializer.Serialize(updateEmployeeDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PutAsync("/api/employees/1", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var employee = JsonSerializer.Deserialize<EmployeeDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        employee.Should().NotBeNull();
        employee.LastName.Should().Be("Johnson-Updated");
        employee.Email.Should().Be("alice.updated@test.com");
        employee.Salary.Should().Be(85000);
    }

    [Fact]
    public async Task UpdateEmployee_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        await SeedTestDataAsync();
        var updateEmployeeDto = new UpdateEmployeeDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@test.com",
            DateOfBirth = new DateTime(1990, 1, 1),
            DateOfJoining = new DateTime(2020, 1, 1),
            Salary = 50000,
            DepartmentId = 1,
            IsActive = true
        };
        var json = JsonSerializer.Serialize(updateEmployeeDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PutAsync("/api/employees/999", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteEmployee_WithValidId_ReturnsNoContent()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.DeleteAsync("/api/employees/3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteEmployee_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.DeleteAsync("/api/employees/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetEmployeesPaged_ReturnsPagedResults()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/employees/paged?pageNumber=1&pageSize=2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var pagedResult = JsonSerializer.Deserialize<PagedResult<EmployeeDto>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        pagedResult.Should().NotBeNull();
        pagedResult.Data.Should().HaveCount(2);
        pagedResult.TotalCount.Should().Be(3);
        pagedResult.PageNumber.Should().Be(1);
        pagedResult.PageSize.Should().Be(2);
        pagedResult.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task GetEmployeesByDepartmentPaged_ReturnsPagedResults()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/employees/department/1/paged?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var pagedResult = JsonSerializer.Deserialize<PagedResult<EmployeeDto>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        pagedResult.Should().NotBeNull();
        pagedResult.Data.Should().HaveCount(2); // Alice and Bob are in department 1
        pagedResult.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task BulkCreateEmployees_WithValidData_ReturnsCreatedEmployees()
    {
        // Arrange
        await SeedTestDataAsync();
        var employees = new List<CreateEmployeeDto>
        {
            new()
            {
                FirstName = "Eve",
                LastName = "Brown",
                Email = "eve@test.com",
                DateOfBirth = new DateTime(1991, 4, 15),
                DateOfJoining = new DateTime(2023, 1, 1),
                Salary = 65000,
                DepartmentId = 2
            },
            new()
            {
                FirstName = "Frank",
                LastName = "Miller",
                Email = "frank@test.com",
                DateOfBirth = new DateTime(1987, 7, 22),
                DateOfJoining = new DateTime(2023, 2, 1),
                Salary = 72000,
                DepartmentId = 3
            }
        };
        var json = JsonSerializer.Serialize(employees);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/employees/bulk", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var createdEmployees = JsonSerializer.Deserialize<List<EmployeeDto>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        createdEmployees.Should().NotBeNull();
        createdEmployees.Should().HaveCount(2);
        createdEmployees.Should().Contain(e => e.FirstName == "Eve");
        createdEmployees.Should().Contain(e => e.FirstName == "Frank");
    }

    [Fact]
    public async Task BulkDeleteEmployees_WithValidIds_ReturnsNoContent()
    {
        // Arrange
        await SeedTestDataAsync();
        var employeeIds = new List<int> { 2, 3 };
        var json = JsonSerializer.Serialize(employeeIds);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/employees/bulk")
        {
            Content = content
        };
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task BulkDeleteEmployees_WithInvalidIds_ReturnsNotFound()
    {
        // Arrange
        await SeedTestDataAsync();
        var employeeIds = new List<int> { 999, 998 };
        var json = JsonSerializer.Serialize(employeeIds);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/employees/bulk")
        {
            Content = content
        };
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
