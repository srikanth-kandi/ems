using System.Net;
using EMS.API.Tests;
using FluentAssertions;
using Xunit;

namespace EMS.API.Tests.Controllers;

public class ReportsControllerTests : TestBase
{
    public ReportsControllerTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetEmployeesReport_ReturnsCsvFile()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/employees");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/csv");
        response.Content.Headers.ContentDisposition?.FileName.Should().Be("employees.csv");
    }

    [Fact]
    public async Task GetDepartmentsReport_ReturnsCsvFile()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/departments");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/csv");
        response.Content.Headers.ContentDisposition?.FileName.Should().Be("departments.csv");
    }

    [Fact]
    public async Task GetAttendanceReport_ReturnsCsvFile()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/attendance");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/csv");
        response.Content.Headers.ContentDisposition?.FileName.Should().Be("attendance.csv");
    }

    [Fact]
    public async Task GetSalariesReport_ReturnsCsvFile()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/salaries");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/csv");
        response.Content.Headers.ContentDisposition?.FileName.Should().Be("salaries.csv");
    }

    [Fact]
    public async Task GetHiringTrendsReport_ReturnsCsvFile()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/hiring-trends");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/csv");
        response.Content.Headers.ContentDisposition?.FileName.Should().Be("hiring-trends.csv");
    }

    [Fact]
    public async Task GetDepartmentGrowthReport_ReturnsCsvFile()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/department-growth");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/csv");
        response.Content.Headers.ContentDisposition?.FileName.Should().Be("department-growth.csv");
    }

    [Fact]
    public async Task GetAttendancePatternsReport_ReturnsNotImplemented()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/attendance-patterns");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetPerformanceMetricsReport_ReturnsNotImplemented()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/performance-metrics");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetEmployeesPdf_ReturnsPdfFile()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/employees/pdf");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/pdf");
        response.Content.Headers.ContentDisposition?.FileName.Should().Be("employees.pdf");
    }

    [Fact]
    public async Task GetDepartmentsPdf_ReturnsNotImplemented()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/departments/pdf");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetAttendancePdf_ReturnsPdfFile()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/attendance/pdf");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/pdf");
        response.Content.Headers.ContentDisposition?.FileName.Should().Be("attendance.pdf");
    }

    [Fact]
    public async Task GetSalariesPdf_ReturnsNotImplemented()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/salaries/pdf");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetEmployeesExcel_ReturnsNotImplemented()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/employees/excel");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetDepartmentsExcel_ReturnsNotImplemented()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/departments/excel");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetAttendanceExcel_ReturnsExcelFile()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/attendance/excel");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        response.Content.Headers.ContentDisposition?.FileName.Should().Be("attendance.xlsx");
    }

    [Fact]
    public async Task GetSalariesExcel_ReturnsNotImplemented()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var response = await Client.GetAsync("/api/reports/salaries/excel");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
