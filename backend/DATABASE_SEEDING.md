# Database Seeding Guide

This guide explains how to use the comprehensive database seeding functionality in the EMS API.

## Overview

The EMS API includes extensive seed data generation capabilities that create realistic test data for:
- **10 Departments** with detailed descriptions and managers
- **200+ Employees** with realistic names, positions, salaries, and contact information
- **Attendance Records** for the last 90 days with realistic check-in/out times
- **Performance Metrics** for the last 2 years with quarterly reviews
- **Multiple User Accounts** with different roles (Admin, HR, Manager)

## Seed Data Services

### 1. SeedDataService
A comprehensive service that generates large amounts of realistic test data.

**Features:**
- Generates 200 employees across 10 departments
- Creates 90 days of attendance records per employee
- Generates 2 years of performance metrics
- Includes realistic salary ranges based on department
- Creates diverse employee profiles with varied experience levels

### 2. DatabaseSeedingService
The original seeding service with extensive data arrays and realistic generation.

**Features:**
- 500 employees with comprehensive profiles
- 1 year of attendance records
- 2 years of performance metrics
- 50+ user accounts with different roles
- Realistic names, addresses, and contact information

## API Endpoints

### Seed Controller (`/api/seed`)

All endpoints require Admin authentication.

#### POST `/api/seed/seed`
Seeds the database with initial data if it's empty.

**Response:**
```json
{
  "message": "Database seeded successfully",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

#### POST `/api/seed/reseed`
Clears all existing data and reseeds with fresh data.

**Response:**
```json
{
  "message": "Database reseeded successfully",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

#### DELETE `/api/seed/clear`
Clears all data from the database.

**Response:**
```json
{
  "message": "Database cleared successfully",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

#### GET `/api/seed/status`
Returns the current count of records in each table.

**Response:**
```json
{
  "departments": 10,
  "employees": 200,
  "attendances": 18000,
  "performanceMetrics": 1600,
  "users": 3,
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## Management Scripts

### PowerShell Script (`database-management.ps1`)

A comprehensive PowerShell script for database management.

**Usage:**
```powershell
# Show help
.\database-management.ps1 -Action help

# Apply migrations
.\database-management.ps1 -Action migrate

# Seed database
.\database-management.ps1 -Action seed

# Reseed database
.\database-management.ps1 -Action reseed -Force

# Clear database
.\database-management.ps1 -Action clear

# Show status
.\database-management.ps1 -Action status

# Reset database (drop, recreate, migrate, seed)
.\database-management.ps1 -Action reset -Force
```

### Batch Script (`database-management.bat`)

A Windows batch file for database management.

**Usage:**
```cmd
# Show help
database-management.bat help

# Apply migrations
database-management.bat migrate

# Seed database
database-management.bat seed

# Reseed database
database-management.bat reseed

# Clear database
database-management.bat clear

# Show status
database-management.bat status

# Reset database
database-management.bat reset
```

## Manual Seeding

### Using Entity Framework CLI

```bash
# Navigate to the API project
cd backend/EMS.API

# Apply migrations
dotnet ef database update

# The database will be automatically seeded on first run
```

### Using the API Directly

```bash
# Start the API
dotnet run --project backend/EMS.API

# In another terminal, seed the database
curl -X POST http://localhost:5000/api/seed/seed \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Seed Data Details

### Departments
- Human Resources
- Information Technology
- Finance
- Marketing
- Sales
- Operations
- Customer Support
- Research & Development
- Legal
- Quality Assurance

### Employee Data
- **Names:** 50+ first names and 50+ last names for variety
- **Positions:** 50+ realistic job titles
- **Salaries:** Department-appropriate salary ranges
- **Contact Info:** Realistic phone numbers and addresses
- **Experience:** 2-10 years of experience
- **Status:** 95% active employees

### Attendance Data
- **Duration:** 90 days of records
- **Schedule:** Monday-Friday workdays
- **Times:** 7:30 AM - 9:00 AM check-in, 4:00 PM - 7:00 PM check-out
- **Overtime:** 30% of employees work overtime
- **Attendance Rate:** 95% attendance rate

### Performance Metrics
- **Duration:** 2 years of quarterly reviews
- **Scores:** 60-100 performance scores
- **Comments:** Realistic performance feedback
- **Goals:** Professional development goals
- **Achievements:** Career accomplishments

## Database Migration

The seed data is included in the `EnhancedSeedData` migration, which adds:
- 10 departments with detailed information
- 3 default user accounts (admin, hr_manager, manager)
- Comprehensive data generation on first run

## Best Practices

1. **Development:** Use the reseed functionality to get fresh test data
2. **Testing:** Clear data between test runs for consistency
3. **Production:** Never use seeding in production environments
4. **Backup:** Always backup production data before any operations

## Troubleshooting

### Common Issues

1. **API Not Running:** Ensure the API is running on `http://localhost:5000`
2. **Authentication:** Use valid JWT tokens for authenticated endpoints
3. **Database Connection:** Verify the connection string in `appsettings.json`
4. **Permissions:** Ensure database user has appropriate permissions

### Error Messages

- **"Database already contains data"**: Use reseed instead of seed
- **"Error occurred during seeding"**: Check database connection and permissions
- **"API is not running"**: Start the API before using seeding endpoints

## Performance Considerations

- **Seeding Time:** Full reseed takes 30-60 seconds
- **Memory Usage:** Large datasets require adequate memory
- **Database Size:** Full seed data creates ~50MB database
- **Network:** API calls may timeout for very large datasets

## Security Notes

- All seeding endpoints require Admin authentication
- Never expose seeding endpoints in production
- Use environment-specific connection strings
- Regularly rotate database credentials
