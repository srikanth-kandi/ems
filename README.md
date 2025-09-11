# Employee Management System (EMS)

A comprehensive Employee Management System built with ASP.NET Web API backend and React.js frontend.

## Project Structure

```
ems/
├── backend/                 # ASP.NET Web API
│   ├── EMS.API/            # Main API project
│   ├── EMS.Core/           # Domain models and interfaces
│   ├── EMS.Infrastructure/ # Data access and external services
│   ├── EMS.Application/    # Business logic and services
│   ├── database-management.ps1  # PowerShell database management script
│   ├── database-management.bat  # Windows batch database management script
│   └── DATABASE_SEEDING.md      # Comprehensive seeding documentation
├── frontend/               # React.js with TypeScript
│   ├── src/
│   ├── public/
│   └── package.json
├── docs/                   # Documentation
└── README.md
```

## Technology Stack

### Backend
- **ASP.NET Web API** (.NET 8)
- **Entity Framework Core** with MySQL
- **JWT Authentication**
- **AutoMapper** for object mapping
- **FluentValidation** for validation
- **Swagger/OpenAPI** for API documentation

### Frontend
- **React.js** with **TypeScript**
- **Vite** for build tooling
- **React Router** for navigation
- **Axios** for API calls
- **Material-UI** for UI components
- **React Hook Form** with **Yup** for form validation
- **Zustand** for state management

### Database
- **MySQL** (Local: MySQL Workbench, Production: Oracle Cloud)

## Features

### Core Features
- ✅ User Authentication & Authorization
- ✅ Employee CRUD Operations
- ✅ Bulk Employee Import
- ✅ Attendance Tracking
- ✅ Report Generation (PDF/Excel)
- ✅ Department Management

### Bonus Features
- 📊 Hiring Trend Analysis
- 📈 Department Growth Tracking
- 📋 Attendance Pattern Reports
- 🎯 Performance Metrics with PDF Export
- 🌱 Comprehensive Database Seeding with 200+ realistic test records
- 🔄 Database Management Scripts (PowerShell & Batch)
- 📊 Real-time Database Status Monitoring

## Getting Started

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- MySQL Server
- Visual Studio Code or Visual Studio

### Backend Setup
```bash
cd backend/EMS.API
dotnet restore
dotnet ef database update
dotnet run
```

### Database Seeding
The EMS API includes comprehensive seed data generation for development and testing:

#### Quick Start
```bash
# Using PowerShell (Recommended)
cd backend
.\database-management.ps1 -Action seed

# Using Batch File
cd backend
database-management.bat seed

# Using API directly (requires running API)
curl -X POST http://localhost:5000/api/seed/seed
```

#### Available Seeding Commands

**PowerShell Script (`database-management.ps1`):**
```powershell
# Show all available commands
.\database-management.ps1 -Action help

# Apply database migrations
.\database-management.ps1 -Action migrate

# Seed database with initial data
.\database-management.ps1 -Action seed

# Reseed database with fresh data (clears existing data)
.\database-management.ps1 -Action reseed -Force

# Clear all data from database
.\database-management.ps1 -Action clear

# Show current database status
.\database-management.ps1 -Action status

# Reset database (drop, recreate, migrate, seed)
.\database-management.ps1 -Action reset -Force
```

**Batch File (`database-management.bat`):**
```cmd
# Show all available commands
database-management.bat help

# Apply database migrations
database-management.bat migrate

# Seed database with initial data
database-management.bat seed

# Reseed database with fresh data
database-management.bat reseed

# Clear all data from database
database-management.bat clear

# Show current database status
database-management.bat status

# Reset database (drop, recreate, migrate, seed)
database-management.bat reset
```

#### Seed Data Overview
The seeding system generates realistic test data including:
- **10 Departments** with detailed descriptions and managers
- **200+ Employees** with realistic names, positions, and salaries
- **90 Days of Attendance Records** per employee with realistic work schedules
- **2 Years of Performance Metrics** with quarterly reviews
- **Multiple User Accounts** with different roles (Admin, HR, Manager)

#### API Seeding Endpoints
All seeding endpoints require Admin authentication:

```bash
# Seed database (only if empty)
POST /api/seed/seed

# Reseed database (clears existing data first)
POST /api/seed/reseed

# Clear all data
DELETE /api/seed/clear

# Get database status
GET /api/seed/status
```

**Example API Usage:**
```bash
# Get authentication token first
TOKEN=$(curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}' | jq -r '.token')

# Seed database
curl -X POST http://localhost:5000/api/seed/seed \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json"

# Check status
curl -X GET http://localhost:5000/api/seed/status \
  -H "Authorization: Bearer $TOKEN"
```

### Frontend Setup
```bash
cd frontend
npm install

# Configure environment variables
cp .env.example .env
# Edit .env to match your backend configuration

npm run dev
```

**Environment Configuration:**
- Copy `.env.example` to `.env` and configure the API URL
- Default API URL: `http://localhost:5001`
- See `frontend/README.md` for detailed configuration options

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration

### Employees
- `GET /api/employees` - Get all employees
- `GET /api/employees/{id}` - Get employee by ID
- `POST /api/employees` - Create employee
- `PUT /api/employees/{id}` - Update employee
- `DELETE /api/employees/{id}` - Delete employee
- `POST /api/employees/bulk` - Bulk import employees

### Reports
- `GET /api/reports/directory` - Employee directory report
- `GET /api/reports/departments` - Department report
- `GET /api/reports/attendance` - Attendance report
- `GET /api/reports/salary` - Salary report

### Attendance
- `POST /api/attendance/check-in` - Check in
- `POST /api/attendance/check-out` - Check out
- `GET /api/attendance/{employeeId}` - Get attendance history

### Database Seeding (Admin Only)
- `POST /api/seed/seed` - Seed database with initial data
- `POST /api/seed/reseed` - Clear and reseed with fresh data
- `DELETE /api/seed/clear` - Clear all data from database
- `GET /api/seed/status` - Get current database record counts

## Deployment

### Production URLs
- Frontend: `https://ems.srikanthkandi.tech`
- Backend API: `https://api.ems.srikanthkandi.tech`

### Oracle Cloud Setup
- Ubuntu Free Tier Instance
- MySQL Database Service (Always Free)
- Nginx reverse proxy
- SSL certificates with Let's Encrypt

## Development Flow

1. **Database Design** - Create models and migrations
2. **API Development** - Implement controllers and services
3. **Authentication** - JWT implementation
4. **Frontend Setup** - React components and routing
5. **Integration** - Connect frontend to backend
6. **Testing** - Unit and integration tests
7. **Deployment** - Oracle Cloud setup
8. **Documentation** - API docs and user guide

## Troubleshooting

### Database Seeding Issues

**Common Problems:**

1. **"API is not running" Error**
   ```bash
   # Make sure the API is running first
   cd backend/EMS.API
   dotnet run
   ```

2. **"Database already contains data" Error**
   ```bash
   # Use reseed instead of seed
   .\database-management.ps1 -Action reseed -Force
   ```

3. **Authentication Required**
   ```bash
   # Get admin token first
   curl -X POST http://localhost:5000/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{"username":"admin","password":"admin123"}'
   ```

4. **Database Connection Issues**
   - Verify connection string in `appsettings.json`
   - Ensure MySQL server is running
   - Check database permissions

**Performance Notes:**
- Full reseed takes 30-60 seconds
- Large datasets require adequate memory
- Full seed data creates ~50MB database

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License.
