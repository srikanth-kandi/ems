# EMS Backend - Single Project Structure

## Project Structure
```
EMS.API/
├── Controllers/          # API Controllers
│   └── EmployeesController.cs
├── Services/            # Business Logic Services
├── Repositories/        # Data Access Layer
│   └── EmployeeRepository.cs
├── Models/              # Entity Models
│   ├── Employee.cs
│   ├── Department.cs
│   ├── User.cs
│   ├── Attendance.cs
│   └── PerformanceMetric.cs
├── DTOs/                # Data Transfer Objects
│   ├── EmployeeDto.cs
│   ├── AuthDto.cs
│   └── AttendanceDto.cs
├── Interfaces/          # Service Interfaces
│   ├── IEmployeeRepository.cs
│   ├── IAuthService.cs
│   ├── IAttendanceRepository.cs
│   └── IReportService.cs
├── Data/                # Database Context
│   └── EMSDbContext.cs
├── Program.cs           # Application Entry Point
├── appsettings.json     # Configuration
└── EMS.API.csproj      # Project File
```

## Benefits of Single Project Structure

### ✅ **Simplified Management**
- All code in one project
- Easier to navigate and understand
- Reduced complexity for small to medium applications

### ✅ **Faster Development**
- No need to manage multiple project references
- Simpler build process
- Easier debugging and testing

### ✅ **Clear Organization**
- Logical folder structure
- Separation of concerns maintained
- Easy to find related files

## Folder Responsibilities

### **Controllers/**
- Handle HTTP requests and responses
- Input validation
- Call appropriate services
- Return HTTP status codes

### **Services/**
- Business logic implementation
- Complex operations
- Integration with external services
- Report generation

### **Repositories/**
- Data access operations
- Database queries
- Entity mapping
- CRUD operations

### **Models/**
- Entity models for database
- Data annotations
- Relationships between entities

### **DTOs/**
- Data Transfer Objects
- Input/Output models for APIs
- Validation attributes

### **Interfaces/**
- Service contracts
- Repository interfaces
- Dependency injection contracts

### **Data/**
- Database context
- Entity configurations
- Seed data
- Migrations

## Development Commands

```bash
# Restore packages
dotnet restore

# Build project
dotnet build

# Run the API
dotnet run

# Create migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update

# Run with specific environment
dotnet run --environment Development
```

## API Endpoints

### Employees
- `GET /api/employees` - Get all employees
- `GET /api/employees/{id}` - Get employee by ID
- `POST /api/employees` - Create employee
- `PUT /api/employees/{id}` - Update employee
- `DELETE /api/employees/{id}` - Delete employee

### Health Check
- `GET /health` - Application health status

## Configuration

### Database Connection
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EMS;Uid=root;Pwd=yourpassword;"
  }
}
```

### JWT Settings
```json
{
  "Jwt": {
    "Key": "your-super-secret-jwt-key-here-make-it-long-and-random",
    "Issuer": "https://localhost:5000",
    "Audience": "https://localhost:3000",
    "ExpiryMinutes": 60
  }
}
```

## Dependencies

### Core Packages
- **Microsoft.EntityFrameworkCore** - ORM
- **Pomelo.EntityFrameworkCore.MySql** - MySQL provider
- **Microsoft.AspNetCore.Authentication.JwtBearer** - JWT authentication

### Report Generation
- **iTextSharp** - PDF generation
- **EPPlus** - Excel generation

### Utilities
- **BCrypt.Net-Next** - Password hashing
- **Swashbuckle.AspNetCore** - API documentation

## Next Steps

1. **Add Authentication Controller** - Login/Register endpoints
2. **Implement JWT Service** - Token generation and validation
3. **Add Department Controller** - Department management
4. **Create Attendance Controller** - Check-in/Check-out functionality
5. **Implement Report Services** - PDF/Excel generation
6. **Add Validation** - Input validation and error handling
7. **Create Database Migrations** - Set up database schema

This single project structure provides a clean, maintainable, and efficient approach for the Employee Management System backend.
