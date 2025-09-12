using System.Net;
using System.Text;
using System.Text.Json;
using EMS.API.DTOs;
using EMS.API.Tests;
using FluentAssertions;
using Xunit;

namespace EMS.API.Tests.Controllers;

public class DepartmentsControllerTests : TestBase
{
    public DepartmentsControllerTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetDepartments_ReturnsAllDepartments()
    {
        // Act
        var response = await Client.GetAsync("/api/departments");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var departments = JsonSerializer.Deserialize<List<DepartmentDto>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        departments.Should().NotBeNull();
        departments.Should().HaveCount(3);
        departments.Should().Contain(d => d.Name == "Engineering");
        departments.Should().Contain(d => d.Name == "HR");
        departments.Should().Contain(d => d.Name == "Finance");
    }

    [Fact]
    public async Task GetDepartment_WithValidId_ReturnsDepartment()
    {
        // Act
        var response = await Client.GetAsync("/api/departments/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var department = JsonSerializer.Deserialize<DepartmentDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        department.Should().NotBeNull();
        department.Id.Should().Be(1);
        department.Name.Should().Be("Engineering");
        department.ManagerName.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetDepartment_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await Client.GetAsync("/api/departments/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateDepartment_WithValidData_ReturnsCreatedDepartment()
    {
        // Arrange
        var createDepartmentDto = new CreateDepartmentDto
        {
            Name = "Marketing",
            Description = "Marketing and Sales",
            ManagerName = "Sarah Wilson"
        };
        var json = JsonSerializer.Serialize(createDepartmentDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/departments", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseContent = await response.Content.ReadAsStringAsync();
        var department = JsonSerializer.Deserialize<DepartmentDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        department.Should().NotBeNull();
        department.Name.Should().Be("Marketing");
        department.Description.Should().Be("Marketing and Sales");
        department.ManagerName.Should().Be("Sarah Wilson");
    }

    [Fact]
    public async Task CreateDepartment_WithDuplicateName_ReturnsConflict()
    {
        // Arrange
        var createDepartmentDto = new CreateDepartmentDto
        {
            Name = "Engineering", // Already exists
            Description = "Software Development",
            ManagerName = "Jane Doe"
        };
        var json = JsonSerializer.Serialize(createDepartmentDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/departments", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateDepartment_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var createDepartmentDto = new CreateDepartmentDto
        {
            Name = "", // Invalid: empty name
            Description = "Test Department"
        };
        var json = JsonSerializer.Serialize(createDepartmentDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PostAsync("/api/departments", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateDepartment_WithValidData_ReturnsUpdatedDepartment()
    {
        // Arrange
        var updateDepartmentDto = new UpdateDepartmentDto
        {
            Name = "Engineering-Updated",
            Description = "Updated Software Development",
            ManagerName = "John Smith"
        };
        var json = JsonSerializer.Serialize(updateDepartmentDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PutAsync("/api/departments/1", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var department = JsonSerializer.Deserialize<DepartmentDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        department.Should().NotBeNull();
        department.Name.Should().Be("Engineering-Updated");
        department.Description.Should().Be("Updated Software Development");
        department.ManagerName.Should().Be("John Smith");
    }

    [Fact]
    public async Task UpdateDepartment_WithDuplicateName_ReturnsConflict()
    {
        // Arrange
        var updateDepartmentDto = new UpdateDepartmentDto
        {
            Name = "HR", // Already exists (different department)
            Description = "Updated Engineering",
            ManagerName = "John Smith"
        };
        var json = JsonSerializer.Serialize(updateDepartmentDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PutAsync("/api/departments/1", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateDepartment_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDepartmentDto = new UpdateDepartmentDto
        {
            Name = "Test Department",
            Description = "Test Description",
            ManagerName = "Test Manager"
        };
        var json = JsonSerializer.Serialize(updateDepartmentDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await Client.PutAsync("/api/departments/999", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteDepartment_WithValidId_ReturnsNoContent()
    {
        // Act
        var response = await Client.DeleteAsync("/api/departments/3"); // Finance department (no employees)

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteDepartment_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await Client.DeleteAsync("/api/departments/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetDepartmentsWithEmployeeCount_ReturnsDepartmentsWithCounts()
    {
        // Act
        var response = await Client.GetAsync("/api/departments/with-employee-count");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var departments = JsonSerializer.Deserialize<List<DepartmentDto>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        departments.Should().NotBeNull();
        departments.Should().HaveCount(3);
        
        var engineeringDept = departments.First(d => d.Name == "Engineering");
        engineeringDept.EmployeeCount.Should().Be(2); // Alice and Bob
        
        var hrDept = departments.First(d => d.Name == "HR");
        hrDept.EmployeeCount.Should().Be(1); // Carol
        
        var financeDept = departments.First(d => d.Name == "Finance");
        financeDept.EmployeeCount.Should().Be(0); // No employees
    }
}
