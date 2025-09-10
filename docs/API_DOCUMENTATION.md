# Employee Management System - API Documentation

## Base URL
- **Development**: `http://localhost:5000/api`
- **Production**: `https://api.ems.srikanthkandi.tech/api`

## Authentication
All endpoints (except login/register) require JWT authentication. Include the token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

## API Endpoints

### Authentication

#### POST /auth/login
Authenticate user and get JWT token.

**Request Body:**
```json
{
  "username": "admin",
  "password": "admin123"
}
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

#### POST /auth/register
Register a new user.

**Request Body:**
```json
{
  "username": "newuser",
  "email": "user@example.com",
  "password": "password123",
  "role": "User"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "newuser",
  "email": "user@example.com",
  "role": "User",
  "expiresAt": "2024-01-01T12:00:00Z"
}
```

### Employee Management

#### GET /employees
Get all employees with pagination.

**Query Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Items per page (default: 10)
- `search` (optional): Search term for name or email
- `departmentId` (optional): Filter by department

**Response:**
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

#### GET /employees/{id}
Get employee by ID.

**Response:**
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

#### POST /employees
Create a new employee.

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

**Response:**
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

#### PUT /employees/{id}
Update an existing employee.

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

#### DELETE /employees/{id}
Delete an employee.

**Response:**
```json
{
  "message": "Employee deleted successfully"
}
```

#### POST /employees/bulk
Bulk import employees from CSV/Excel.

**Request Body:** (multipart/form-data)
- `file`: CSV or Excel file containing employee data

**Response:**
```json
{
  "message": "Bulk import completed successfully",
  "importedCount": 10,
  "failedCount": 0,
  "errors": []
}
```

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

### Bonus Features

#### GET /reports/hiring-trends
Generate hiring trend analysis report.

#### GET /reports/department-growth
Generate department growth tracking report.

#### GET /reports/attendance-patterns
Generate attendance pattern analysis report.

#### GET /reports/performance-metrics
Generate performance metrics report.

**Query Parameters:**
- `employeeId` (optional): Specific employee ID
- `format` (optional): "pdf" or "excel" (default: "pdf")

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
