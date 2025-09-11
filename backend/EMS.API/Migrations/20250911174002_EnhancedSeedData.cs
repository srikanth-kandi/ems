using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EMS.API.Migrations
{
    /// <inheritdoc />
    public partial class EnhancedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "ManagerName" },
                values: new object[] { new DateTime(2025, 9, 11, 17, 39, 55, 955, DateTimeKind.Utc).AddTicks(9502), "Manages employee relations, recruitment, and benefits", "Sarah Johnson" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "ManagerName" },
                values: new object[] { new DateTime(2025, 9, 11, 17, 39, 55, 955, DateTimeKind.Utc).AddTicks(9506), "Handles all technology infrastructure and software development", "Michael Chen" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description", "ManagerName" },
                values: new object[] { new DateTime(2025, 9, 11, 17, 39, 55, 955, DateTimeKind.Utc).AddTicks(9508), "Manages financial planning, accounting, and budgeting", "Robert Williams" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Description", "ManagerName" },
                values: new object[] { new DateTime(2025, 9, 11, 17, 39, 55, 955, DateTimeKind.Utc).AddTicks(9511), "Responsible for brand management and customer acquisition", "Emily Davis" });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "CreatedAt", "Description", "ManagerName", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 5, new DateTime(2025, 9, 11, 17, 39, 55, 955, DateTimeKind.Utc).AddTicks(9513), "Handles customer relationships and revenue generation", "David Martinez", "Sales", null },
                    { 6, new DateTime(2025, 9, 11, 17, 39, 55, 955, DateTimeKind.Utc).AddTicks(9515), "Manages day-to-day business operations and logistics", "Lisa Anderson", "Operations", null },
                    { 7, new DateTime(2025, 9, 11, 17, 39, 55, 955, DateTimeKind.Utc).AddTicks(9518), "Provides customer service and technical support", "James Wilson", "Customer Support", null },
                    { 8, new DateTime(2025, 9, 11, 17, 39, 55, 955, DateTimeKind.Utc).AddTicks(9520), "Conducts product research and innovation", "Dr. Jennifer Taylor", "Research & Development", null },
                    { 9, new DateTime(2025, 9, 11, 17, 39, 55, 955, DateTimeKind.Utc).AddTicks(9522), "Handles legal compliance and contract management", "Attorney Mark Brown", "Legal", null },
                    { 10, new DateTime(2025, 9, 11, 17, 39, 55, 955, DateTimeKind.Utc).AddTicks(9524), "Ensures product and service quality standards", "Patricia Garcia", "Quality Assurance", null }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 9, 11, 17, 39, 56, 416, DateTimeKind.Utc).AddTicks(9074), "$2a$11$zXj9morSGFoT4/eCD3.dHejBR/RGVbMmgGMN82amCUQ/dqfvRk75a" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "LastLoginAt", "PasswordHash", "Role", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { 2, new DateTime(2025, 9, 11, 17, 39, 56, 767, DateTimeKind.Utc).AddTicks(8756), "hr@ems.com", true, null, "$2a$11$oq2RP3YX4qn9mZRTnyGfj.sZ6BA1QWqqOx.qni5C4pgDHcG1jo/4.", "HR", null, "hr_manager" },
                    { 3, new DateTime(2025, 9, 11, 17, 39, 57, 164, DateTimeKind.Utc).AddTicks(1847), "manager@ems.com", true, null, "$2a$11$a1bXbjMSaeotLQ92hh7gf.CELQ3LJp8d1rlVAuBg0UBUTxV8qGbw6", "Manager", null, "manager" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceMetrics_EmployeeId_Year_Quarter",
                table: "PerformanceMetrics",
                columns: new[] { "EmployeeId", "Year", "Quarter" });

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceMetrics_PerformanceScore",
                table: "PerformanceMetrics",
                column: "PerformanceScore");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceMetrics_Year_Quarter",
                table: "PerformanceMetrics",
                columns: new[] { "Year", "Quarter" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DateOfJoining",
                table: "Employees",
                column: "DateOfJoining");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DateOfJoining_IsActive",
                table: "Employees",
                columns: new[] { "DateOfJoining", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId_IsActive",
                table: "Employees",
                columns: new[] { "DepartmentId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_FirstName_LastName",
                table: "Employees",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_IsActive",
                table: "Employees",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_CheckOutTime",
                table: "Attendances",
                column: "CheckOutTime");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_Date",
                table: "Attendances",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_Date_CheckInTime",
                table: "Attendances",
                columns: new[] { "Date", "CheckInTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_EmployeeId_Date",
                table: "Attendances",
                columns: new[] { "EmployeeId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PerformanceMetrics_EmployeeId_Year_Quarter",
                table: "PerformanceMetrics");

            migrationBuilder.DropIndex(
                name: "IX_PerformanceMetrics_PerformanceScore",
                table: "PerformanceMetrics");

            migrationBuilder.DropIndex(
                name: "IX_PerformanceMetrics_Year_Quarter",
                table: "PerformanceMetrics");

            migrationBuilder.DropIndex(
                name: "IX_Employees_DateOfJoining",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_DateOfJoining_IsActive",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_DepartmentId_IsActive",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_FirstName_LastName",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_IsActive",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_CheckOutTime",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_Date",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_Date_CheckInTime",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_EmployeeId_Date",
                table: "Attendances");

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "ManagerName" },
                values: new object[] { new DateTime(2025, 9, 10, 16, 37, 31, 717, DateTimeKind.Utc).AddTicks(4255), "HR Department", "John Smith" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "ManagerName" },
                values: new object[] { new DateTime(2025, 9, 10, 16, 37, 31, 717, DateTimeKind.Utc).AddTicks(4258), "IT Department", "Jane Doe" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description", "ManagerName" },
                values: new object[] { new DateTime(2025, 9, 10, 16, 37, 31, 717, DateTimeKind.Utc).AddTicks(4261), "Finance Department", "Bob Johnson" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Description", "ManagerName" },
                values: new object[] { new DateTime(2025, 9, 10, 16, 37, 31, 717, DateTimeKind.Utc).AddTicks(4264), "Marketing Department", "Alice Brown" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 9, 10, 16, 37, 31, 967, DateTimeKind.Utc).AddTicks(9590), "$2a$11$CcLtyopmHEQL2rFXcSbAi.TQ290MCLp7/o2PetqDjvRzlwqbGhcc2" });
        }
    }
}
