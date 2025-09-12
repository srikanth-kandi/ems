# Database Seeding for EMS

This directory contains scripts and configurations for seeding the EMS database with comprehensive test data.

## Overview

The EMS application includes extensive seed data generation capabilities that create realistic test data for:
- **10 Departments** with detailed descriptions and managers
- **200+ Employees** with realistic names, positions, salaries, and contact information
- **Attendance Records** for the last 90 days with realistic check-in/out times
- **Performance Metrics** for the last 2 years with quarterly reviews
- **Multiple User Accounts** with different roles (Admin, HR, Manager)

## Files

### SQL Scripts
- `01-init-database.sql` - Database initialization script that runs when MySQL container starts
- `02-seed-data.sh` - Basic seeding script (Linux/Unix)
- `seeding-service.sh` - Comprehensive seeding service with multiple options

### Docker Compose Files
- `docker-compose.seed.yml` - Production seeding override
- `docker-compose.dev-seed.yml` - Development seeding override

## Usage

### 1. Basic Seeding (Automatic)
The application automatically seeds the database on startup if it's empty. This happens in both production and development environments.

```bash
# Production
docker-compose up

# Development
docker-compose -f docker-compose.dev.yml up
```

### 2. Comprehensive Seeding
For comprehensive seeding with fresh data, use the seeding override files:

```bash
# Production with comprehensive seeding
docker-compose -f docker-compose.yml -f docker-compose.seed.yml up

# Development with comprehensive seeding
docker-compose -f docker-compose.dev.yml -f docker-compose.dev-seed.yml up
```

### 3. Custom Seeding Type
You can specify the type of seeding operation:

```bash
# Seed only if database is empty
SEED_TYPE=seed docker-compose -f docker-compose.yml -f docker-compose.seed.yml up

# Reseed with fresh data (default)
SEED_TYPE=reseed docker-compose -f docker-compose.yml -f docker-compose.seed.yml up

# Clear all data
SEED_TYPE=clear docker-compose -f docker-compose.yml -f docker-compose.seed.yml up
```

### 4. Manual Seeding
You can also seed the database manually using the API endpoints:

```bash
# Get authentication token
TOKEN=$(curl -s -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}' | \
  grep -o '"token":"[^"]*"' | cut -d'"' -f4)

# Seed database
curl -X POST http://localhost:5000/api/seed/seed \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN"

# Reseed database
curl -X POST http://localhost:5000/api/seed/reseed \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN"

# Clear database
curl -X DELETE http://localhost:5000/api/seed/clear \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN"

# Get database status
curl -X GET http://localhost:5000/api/seed/status \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN"
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

## Default User Accounts

The seeding process creates the following default user accounts:

| Username | Password | Role | Description |
|----------|----------|------|-------------|
| admin | admin123 | Admin | Full system access |
| hr_manager | hr123 | HR Manager | HR department access |
| manager | manager123 | Manager | Department management access |

## Troubleshooting

### Common Issues

1. **API Not Running**: Ensure the backend API is running and accessible
2. **Authentication Failed**: Check that the default user accounts exist
3. **Database Connection**: Verify the database connection string
4. **Permissions**: Ensure the database user has appropriate permissions

### Logs

Check the container logs for detailed information:

```bash
# Check seeder container logs
docker logs ems-seeder

# Check backend logs
docker logs ems-backend

# Check database logs
docker logs ems-mysql
```

### Reset Everything

To completely reset the database and reseed:

```bash
# Stop all containers
docker-compose down

# Remove volumes (this will delete all data)
docker-compose down -v

# Start with fresh seeding
docker-compose -f docker-compose.yml -f docker-compose.seed.yml up
```

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
