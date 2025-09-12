# Employee Management System - API Documentation

## 📋 Overview

The Employee Management System (EMS) API provides a comprehensive RESTful interface for managing employees, departments, attendance, and generating reports. Built with ASP.NET Web API (.NET 8), it offers secure, scalable, and well-documented endpoints for all HR management operations.

### 🚀 API Highlights
- **15+ Endpoints**: Comprehensive coverage of HR operations
- **JWT Authentication**: Secure token-based authentication with role-based access
- **Advanced Reporting**: PDF/Excel generation with 8 different report types
- **Data Seeding**: 200+ employees with 90 days of attendance data
- **Production Ready**: Deployed on Oracle Cloud with SSL encryption
- **Interactive Documentation**: Swagger UI for API testing and exploration

## 🌐 Base URLs
- **Development**: `http://localhost:5000/api`
- **Production**: `https://api.ems.srikanthkandi.tech/api`
- **Swagger UI**: `https://api.ems.srikanthkandi.tech/swagger`

## 🔐 Authentication

The API uses JWT (JSON Web Token) based authentication. All endpoints except login and register require authentication.

### Authentication Header
Include the JWT token in the Authorization header for all protected endpoints:
```
Authorization: Bearer <your-jwt-token>
```

### Token Information
- **Token Type**: JWT (JSON Web Token)
- **Algorithm**: HS256
- **Expiration**: 60 minutes (configurable)
- **Refresh**: Automatic token refresh on frontend
- **Roles**: Admin, HR, Manager, User

### Getting a Token
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "email": "admin@ems.com",
  "role": "Admin",
  "expiresAt": "2024-01-01T12:00:00Z"
}
```

## 📚 API Endpoints

### 🔐 Authentication Endpoints

#### POST /auth/login
Authenticate user and get JWT token.

**Description:** Authenticates a user with username and password, returning a JWT token for subsequent API calls.

**Request Body:**
```json
{
  "username": "admin",
  "password": "admin123"
}
```

**Validation Rules:**
- `username`: Required, string, max 50 characters
- `password`: Required, string, min 6 characters

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "email": "admin@ems.com",
  "role": "Admin",
  "expiresAt": "2024-01-01T12:00:00Z"
}
```

**Error Responses:**
- `400 Bad Request`: Invalid request body or validation errors
- `401 Unauthorized`: Invalid credentials
- `500 Internal Server Error`: Server error

**Example Usage:**
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'
```

#### POST /auth/register
Register a new user account.

**Description:** Creates a new user account with the specified credentials and role.

**Request Body:**
```json
{
  "username": "newuser",
  "email": "user@example.com",
  "password": "password123",
  "role": "User"
}
```

**Validation Rules:**
- `username`: Required, string, max 50 characters, unique
- `email`: Required, valid email format, max 255 characters, unique
- `password`: Required, string, min 6 characters
- `role`: Optional, string, default "User", values: Admin, HR, Manager, User

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "newuser",
  "email": "user@example.com",
  "role": "User",
  "expiresAt": "2024-01-01T12:00:00Z"
}
```

**Error Responses:**
- `400 Bad Request`: Invalid request body or validation errors
- `409 Conflict`: Username or email already exists
- `500 Internal Server Error`: Server error

**Example Usage:**
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"newuser","email":"user@example.com","password":"password123","role":"User"}'
```

### 👥 Employee Management Endpoints

#### GET /employees
Get paginated list of employees with search and filtering capabilities.

**Description:** Retrieves a paginated list of employees with optional search and filtering options.

**Authentication:** Required (All roles)

**Query Parameters:**
| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `page` | integer | No | 1 | Page number (1-based) |
| `pageSize` | integer | No | 10 | Items per page (max 100) |
| `search` | string | No | - | Search term for name or email |
| `departmentId` | integer | No | - | Filter by department ID |
| `isActive` | boolean | No | - | Filter by active status |
| `sortBy` | string | No | "firstName" | Sort field (firstName, lastName, email, dateOfJoining) |
| `sortOrder` | string | No | "asc" | Sort order (asc, desc) |

**Response (200 OK):**
```json
{
  "data": [
    {
      "id": 1,
      "firstName": "John",
      "lastName": "Doe",
      "email": "john.doe@company.com",
      "phoneNumber": "+1234567890",
      "address": "123 Main St, City, State",
      "dateOfBirth": "1990-01-01T00:00:00Z",
      "dateOfJoining": "2020-01-01T00:00:00Z",
      "position": "Software Developer",
      "salary": 75000.00,
      "departmentId": 1,
      "departmentName": "Information Technology",
      "isActive": true,
      "createdAt": "2024-01-01T00:00:00Z",
      "updatedAt": null
    }
  ],
  "totalCount": 1,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

**Error Responses:**
- `401 Unauthorized`: Missing or invalid authentication token
- `500 Internal Server Error`: Server error

**Example Usage:**
```bash
# Get first page of employees
curl -X GET "http://localhost:5000/api/employees" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

# Search employees by name
curl -X GET "http://localhost:5000/api/employees?search=john" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

# Filter by department with pagination
curl -X GET "http://localhost:5000/api/employees?departmentId=1&page=2&pageSize=20" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

#### GET /employees/{id}
Get specific employee by ID.

**Description:** Retrieves detailed information for a specific employee by their unique identifier.

**Authentication:** Required (All roles)

**Path Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | integer | Yes | Employee ID |

**Response (200 OK):**
```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@company.com",
  "phoneNumber": "+1234567890",
  "address": "123 Main St, City, State",
  "dateOfBirth": "1990-01-01T00:00:00Z",
  "dateOfJoining": "2020-01-01T00:00:00Z",
  "position": "Software Developer",
  "salary": 75000.00,
  "departmentId": 1,
  "departmentName": "Information Technology",
  "isActive": true,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": null
}
```

**Error Responses:**
- `401 Unauthorized`: Missing or invalid authentication token
- `404 Not Found`: Employee with specified ID not found
- `500 Internal Server Error`: Server error

**Example Usage:**
```bash
curl -X GET "http://localhost:5000/api/employees/1" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

#### POST /employees
Create a new employee.

**Description:** Creates a new employee record with the provided information.

**Authentication:** Required (Admin, HR roles)

**Request Body:**
```json
{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@company.com",
  "phoneNumber": "+1234567891",
  "address": "456 Oak Ave, City, State",
  "dateOfBirth": "1992-05-15T00:00:00Z",
  "dateOfJoining": "2024-01-01T00:00:00Z",
  "position": "Marketing Manager",
  "salary": 65000.00,
  "departmentId": 4
}
```

**Validation Rules:**
- `firstName`: Required, string, max 100 characters
- `lastName`: Required, string, max 100 characters
- `email`: Required, valid email format, max 255 characters, unique
- `phoneNumber`: Optional, string, max 20 characters
- `address`: Optional, string, max 500 characters
- `dateOfBirth`: Required, valid date
- `dateOfJoining`: Required, valid date
- `position`: Optional, string, max 100 characters
- `salary`: Required, positive decimal
- `departmentId`: Required, valid department ID

**Response (201 Created):**
```json
{
  "id": 2,
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@company.com",
  "phoneNumber": "+1234567891",
  "address": "456 Oak Ave, City, State",
  "dateOfBirth": "1992-05-15T00:00:00Z",
  "dateOfJoining": "2024-01-01T00:00:00Z",
  "position": "Marketing Manager",
  "salary": 65000.00,
  "departmentId": 4,
  "departmentName": "Marketing",
  "isActive": true,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": null
}
```

**Error Responses:**
- `400 Bad Request`: Invalid request body or validation errors
- `401 Unauthorized`: Missing or invalid authentication token
- `403 Forbidden`: Insufficient permissions
- `409 Conflict`: Email already exists
- `500 Internal Server Error`: Server error

**Example Usage:**
```bash
curl -X POST "http://localhost:5000/api/employees" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Jane",
    "lastName": "Smith",
    "email": "jane.smith@company.com",
    "phoneNumber": "+1234567891",
    "address": "456 Oak Ave, City, State",
    "dateOfBirth": "1992-05-15T00:00:00Z",
    "dateOfJoining": "2024-01-01T00:00:00Z",
    "position": "Marketing Manager",
    "salary": 65000.00,
    "departmentId": 4
  }'
```

#### PUT /employees/{id}
Update an existing employee.

**Description:** Updates an existing employee record with the provided information.

**Authentication:** Required (Admin, HR roles)

**Path Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | integer | Yes | Employee ID to update |

**Request Body:**
```json
{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@company.com",
  "phoneNumber": "+1234567891",
  "address": "456 Oak Ave, City, State",
  "dateOfBirth": "1992-05-15T00:00:00Z",
  "dateOfJoining": "2024-01-01T00:00:00Z",
  "position": "Senior Marketing Manager",
  "salary": 75000.00,
  "departmentId": 4,
  "isActive": true
}
```

**Response (200 OK):**
```json
{
  "id": 2,
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@company.com",
  "phoneNumber": "+1234567891",
  "address": "456 Oak Ave, City, State",
  "dateOfBirth": "1992-05-15T00:00:00Z",
  "dateOfJoining": "2024-01-01T00:00:00Z",
  "position": "Senior Marketing Manager",
  "salary": 75000.00,
  "departmentId": 4,
  "departmentName": "Marketing",
  "isActive": true,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T12:00:00Z"
}
```

#### DELETE /employees/{id}
Delete an employee.

**Description:** Permanently deletes an employee record from the system.

**Authentication:** Required (Admin role only)

**Path Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | integer | Yes | Employee ID to delete |

**Response (200 OK):**
```json
{
  "message": "Employee deleted successfully"
}
```

#### POST /employees/bulk
Bulk import employees from CSV/Excel file.

**Description:** Imports multiple employees from a CSV or Excel file.

**Authentication:** Required (Admin, HR roles)

**Request Body:** (multipart/form-data)
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `file` | file | Yes | CSV or Excel file containing employee data |

**Response (200 OK):**
```json
{
  "message": "Bulk import completed successfully",
  "importedCount": 10,
  "failedCount": 0,
  "errors": []
}
```

**Error Responses:**
- `400 Bad Request`: Invalid file format or validation errors
- `401 Unauthorized`: Missing or invalid authentication token
- `403 Forbidden`: Insufficient permissions
- `500 Internal Server Error`: Server error

### Department Management

#### GET /departments
Get all departments.

**Response:**
```json
[
  {
    "id": 1,
    "name": "Human Resources",
    "description": "HR Department",
    "managerName": "John Smith",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": null
  }
]
```

#### GET /departments/{id}
Get department by ID.

#### POST /departments
Create a new department.

**Request Body:**
```json
{
  "name": "Research & Development",
  "description": "R&D Department",
  "managerName": "Dr. Sarah Johnson"
}
```

#### PUT /departments/{id}
Update department.

#### DELETE /departments/{id}
Delete department.

### Attendance Management

#### POST /attendance/check-in
Employee check-in.

**Request Body:**
```json
{
  "employeeId": 1,
  "notes": "Starting work day"
}
```

**Response:**
```json
{
  "id": 1,
  "employeeId": 1,
  "employeeName": "John Doe",
  "checkInTime": "2024-01-01T09:00:00Z",
  "checkOutTime": null,
  "totalHours": null,
  "notes": "Starting work day",
  "date": "2024-01-01T00:00:00Z",
  "createdAt": "2024-01-01T09:00:00Z"
}
```

#### POST /attendance/check-out
Employee check-out.

**Request Body:**
```json
{
  "employeeId": 1,
  "notes": "Ending work day"
}
```

**Response:**
```json
{
  "id": 1,
  "employeeId": 1,
  "employeeName": "John Doe",
  "checkInTime": "2024-01-01T09:00:00Z",
  "checkOutTime": "2024-01-01T17:00:00Z",
  "totalHours": "08:00:00",
  "notes": "Ending work day",
  "date": "2024-01-01T00:00:00Z",
  "createdAt": "2024-01-01T09:00:00Z"
}
```

#### GET /attendance/{employeeId}
Get employee attendance history.

**Query Parameters:**
- `startDate` (optional): Start date filter
- `endDate` (optional): End date filter

**Response:**
```json
[
  {
    "id": 1,
    "employeeId": 1,
    "employeeName": "John Doe",
    "checkInTime": "2024-01-01T09:00:00Z",
    "checkOutTime": "2024-01-01T17:00:00Z",
    "totalHours": "08:00:00",
    "notes": "Full day",
    "date": "2024-01-01T00:00:00Z",
    "createdAt": "2024-01-01T09:00:00Z"
  }
]
```

#### GET /attendance/today/{employeeId}
Get today's attendance for employee.

### Report Generation

#### GET /reports/directory
Generate employee directory report.

**Query Parameters:**
- `format` (optional): "pdf" or "excel" (default: "pdf")

**Response:** File download (PDF or Excel)

#### GET /reports/departments
Generate department report.

**Query Parameters:**
- `format` (optional): "pdf" or "excel" (default: "pdf")

#### GET /reports/attendance
Generate attendance report.

**Query Parameters:**
- `startDate` (optional): Start date
- `endDate` (optional): End date
- `format` (optional): "pdf" or "excel" (default: "pdf")

#### GET /reports/salary
Generate salary report.

**Query Parameters:**
- `format` (optional): "pdf" or "excel" (default: "pdf")

### 🌟 Advanced Analytics Reports

#### GET /reports/hiring-trends
Generate comprehensive hiring trend analysis report.

**Description:** Analyzes hiring patterns over time, providing insights into recruitment trends, seasonal variations, and growth metrics.

**Query Parameters:**
- `format` (optional): "pdf", "excel", or "csv" (default: "csv")
- `startDate` (optional): Start date for analysis
- `endDate` (optional): End date for analysis

**Features:**
- Monthly and yearly hiring trends
- Department-wise hiring patterns
- Growth rate calculations
- Visual charts in PDF format
- Raw data export in CSV/Excel

**Response:** File download with hiring trend analysis

#### GET /reports/department-growth
Generate department growth tracking report.

**Description:** Tracks department expansion over time, showing headcount changes, growth rates, and organizational development.

**Query Parameters:**
- `format` (optional): "pdf", "excel", or "csv" (default: "csv")
- `departmentId` (optional): Specific department analysis

**Features:**
- Department size evolution
- Growth percentage calculations
- Comparative analysis between departments
- Timeline visualization
- Headcount predictions

**Response:** File download with department growth metrics

#### GET /reports/attendance-patterns
Generate comprehensive attendance pattern analysis report.

**Description:** Analyzes employee work behavior patterns, identifying trends in check-in times, work hours, and attendance regularity.

**Query Parameters:**
- `format` (optional): "pdf", "excel", or "csv" (default: "csv")
- `employeeId` (optional): Specific employee analysis
- `startDate` (optional): Analysis start date
- `endDate` (optional): Analysis end date

**Features:**
- Average work hours analysis
- Check-in/check-out time patterns
- Attendance regularity scoring
- Overtime trend analysis
- Department-wise comparisons

**Response:** File download with attendance behavior insights

#### GET /reports/performance-metrics
Generate detailed performance metrics report.

**Description:** Comprehensive employee performance evaluation report with quarterly reviews, performance scores, and goal tracking.

**Query Parameters:**
- `employeeId` (optional): Specific employee ID
- `format` (optional): "pdf", "excel", or "csv" (default: "csv")
- `year` (optional): Performance year
- `quarter` (optional): Specific quarter (1-4)

**Features:**
- Performance score tracking
- Goal achievement analysis
- Quarterly review summaries
- Performance trend visualization
- Department performance comparisons
- Individual vs team performance metrics

**Response:** File download with performance evaluation data

## Error Responses

### 400 Bad Request
```json
{
  "error": "Validation failed",
  "details": [
    {
      "field": "email",
      "message": "Email is required"
    }
  ]
}
```

### 401 Unauthorized
```json
{
  "error": "Unauthorized",
  "message": "Invalid or expired token"
}
```

### 403 Forbidden
```json
{
  "error": "Forbidden",
  "message": "Insufficient permissions"
}
```

### 404 Not Found
```json
{
  "error": "Not Found",
  "message": "Employee with ID 999 not found"
}
```

### 500 Internal Server Error
```json
{
  "error": "Internal Server Error",
  "message": "An unexpected error occurred"
}
```

## Rate Limiting
- **Authentication endpoints**: 5 requests per minute
- **Other endpoints**: 100 requests per minute

## CORS Configuration
- **Allowed Origins**: 
  - `http://localhost:3000` (development)
  - `https://ems.srikanthkandi.tech` (production)
- **Allowed Methods**: GET, POST, PUT, DELETE, OPTIONS
- **Allowed Headers**: Content-Type, Authorization

## Data Validation

### Employee Validation Rules
- **FirstName**: Required, max 100 characters
- **LastName**: Required, max 100 characters
- **Email**: Required, valid email format, max 255 characters, unique
- **PhoneNumber**: Optional, max 20 characters
- **Address**: Optional, max 500 characters
- **DateOfBirth**: Required, valid date
- **DateOfJoining**: Required, valid date
- **Position**: Optional, max 100 characters
- **Salary**: Required, positive decimal
- **DepartmentId**: Required, valid department ID

### User Validation Rules
- **Username**: Required, max 50 characters, unique
- **Email**: Required, valid email format, max 255 characters, unique
- **Password**: Required, min 6 characters
- **Role**: Optional, max 50 characters, default "User"

## Pagination
All list endpoints support pagination with the following parameters:
- `page`: Page number (1-based)
- `pageSize`: Items per page (max 100)

Response includes pagination metadata:
```json
{
  "data": [...],
  "totalCount": 100,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 10
}
```

## File Upload
Bulk import endpoints accept CSV and Excel files with the following specifications:

### CSV Format
```csv
FirstName,LastName,Email,PhoneNumber,Address,DateOfBirth,DateOfJoining,Position,Salary,DepartmentId
John,Doe,john.doe@company.com,+1234567890,123 Main St,1990-01-01,2020-01-01,Developer,75000,1
```

### Excel Format
Same columns as CSV, with headers in the first row.

## WebSocket Support (Future Enhancement)
Real-time features for:
- Live attendance updates
- Notification system
- Dashboard updates

## API Versioning
Current version: v1
- Base URL: `/api/v1/`
- Future versions will be backward compatible

## SDKs and Libraries
- **JavaScript/TypeScript**: Axios, Fetch API
- **C#/.NET**: HttpClient, RestSharp
- **Python**: Requests, httpx
- **Java**: OkHttp, Retrofit

## Testing
- **Postman Collection**: Available in `/docs/postman/`
- **Swagger UI**: Available at `/swagger` (development only)
- **OpenAPI Spec**: Available at `/swagger/v1/swagger.json`
