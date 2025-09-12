# Employee Management System (EMS)

A comprehensive, full-stack Employee Management System built with modern technologies, featuring a robust ASP.NET Web API backend and a responsive React.js frontend. This system provides complete HR management capabilities including employee tracking, attendance management, department organization, and comprehensive reporting with PDF/Excel generation.

## 🌟 Key Highlights

- **Production-Ready**: Deployed on Oracle Cloud Infrastructure with SSL encryption
- **Modern Tech Stack**: ASP.NET 8, React 18, TypeScript, MySQL 8.0
- **Comprehensive Features**: 200+ employees, 10 departments, 90 days attendance data
- **Advanced Reporting**: PDF/Excel generation with 8 different report types
- **Secure Authentication**: JWT-based authentication with role-based access control
- **Docker Support**: Complete containerization for easy deployment
- **Real-time Data**: Live dashboard with statistics and recent activity
- **Responsive Design**: Mobile-first approach with Material-UI components

## 🏗️ System Architecture

The EMS follows a clean, layered architecture pattern with clear separation of concerns, implementing SOLID principles and modern design patterns:

### High-Level Architecture
```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              Frontend Layer (React.js)                         │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌───────────┐ │
│  │   Auth      │ │  Employee   │ │ Attendance  │ │  Reports    │ │ Dashboard │ │
│  │ Components  │ │ Management  │ │  Tracking   │ │ & Analytics │ │ & Stats   │ │
│  │             │ │             │ │             │ │             │ │           │ │
│  │ • Login     │ │ • CRUD Ops  │ │ • Check-in  │ │ • PDF/Excel │ │ • Overview│ │
│  │ • Register  │ │ • Search    │ │ • Check-out │ │ • Reports   │ │ • Metrics │ │
│  │ • Protected │ │ • Filter    │ │ • History   │ │ • Analytics │ │ • Charts  │ │
│  │   Routes    │ │ • Bulk Import│ │ • Patterns  │ │ • Trends    │ │ • Activity│ │
│  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘ └───────────┘ │
└─────────────────────────────────────────────────────────────────────────────────┘
                                        │
                                        │ HTTP/REST API + JWT Authentication
                                        │
┌─────────────────────────────────────────────────────────────────────────────────┐
│                            Backend Layer (ASP.NET Web API)                     │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌───────────┐ │
│  │ Controllers │ │  Services   │ │ Repositories│ │    DTOs     │ │  Common   │ │
│  │   Layer     │ │   Layer     │ │   Layer     │ │   Layer     │ │  Utilities│ │
│  │             │ │             │ │             │ │             │ │           │ │
│  │ • Auth      │ │ • Auth      │ │ • Employee  │ │ • Request   │ │ • Mappers │ │
│  │ • Employee  │ │ • Employee  │ │ • Department│ │ • Response  │ │ • Validators│ │
│  │ • Department│ │ • Attendance│ │ • Attendance│ │ • Pagination│ │ • Helpers │ │
│  │ • Attendance│ │ • Reports   │ │ • User      │ │ • Error     │ │ • Extensions│ │
│  │ • Reports   │ │ • Seed      │ │ • Base Repo │ │ • Models    │ │ • Converters│ │
│  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘ └───────────┘ │
└─────────────────────────────────────────────────────────────────────────────────┘
                                        │
                                        │ Entity Framework Core + LINQ
                                        │
┌─────────────────────────────────────────────────────────────────────────────────┐
│                            Data Layer (MySQL Database)                         │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌───────────┐ │
│  │  Employees  │ │ Departments │ │ Attendance  │ │    Users    │ │Performance│ │
│  │    Table    │ │   Table     │ │   Table     │ │   Table     │ │  Metrics  │ │
│  │             │ │             │ │             │ │             │ │   Table   │ │
│  │ • Personal  │ │ • Basic     │ │ • Time      │ │ • Auth      │ │ • Reviews │ │
│  │   Info      │ │   Info      │ │   Tracking  │ │   Data      │ │ • Ratings │ │
│  │ • Contact   │ │ • Manager   │ │ • Hours     │ │ • Roles     │ │ • Goals   │ │
│  │ • Employment│ │ • Employee  │ │ • Patterns  │ │ • Permissions│ │ • KPIs    │ │
│  │   Details   │ │   Count     │ │ • Analytics │ │ • Sessions  │ │ • History │ │
│  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘ └───────────┘ │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### Technology Stack Architecture
```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              Frontend Technologies                             │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌───────────┐ │
│  │   React 18  │ │ TypeScript  │ │    Vite     │ │ Material-UI │ │  Zustand  │ │
│  │             │ │             │ │             │ │             │ │           │ │
│  │ • Hooks     │ │ • Type      │ │ • Fast      │ │ • Components│ │ • State   │ │
│  │ • Components│ │   Safety    │ │   Build     │ │ • Theming   │ │   Mgmt    │ │
│  │ • Context   │ │ • Interfaces│ │ • HMR       │ │ • Icons     │ │ • Persist │ │
│  │ • Router    │ │ • Generics  │ │ • Dev Server│ │ • Layout    │ │ • Actions │ │
│  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘ └───────────┘ │
└─────────────────────────────────────────────────────────────────────────────────┘
                                        │
                                        │ Axios + React Query
                                        │
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              Backend Technologies                              │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌───────────┐ │
│  │ ASP.NET 8   │ │ Entity      │ │     JWT     │ │   Reports   │ │  Security │ │
│  │ Web API     │ │ Framework   │ │    Auth     │ │ Generation  │ │           │ │
│  │             │ │   Core      │ │             │ │             │ │           │ │
│  │ • Controllers│ │ • Code First│ │ • Bearer    │ │ • iTextSharp│ │ • BCrypt  │ │
│  │ • Services  │ │ • Migrations│ │ • Tokens    │ │ • EPPlus    │ │ • CORS    │ │
│  │ • Middleware│ │ • LINQ      │ │ • Roles     │ │ • PDF/Excel │ │ • HTTPS   │ │
│  │ • DI        │ │ • MySQL     │ │ • Claims    │ │ • Templates │ │ • Validation│ │
│  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘ └───────────┘ │
└─────────────────────────────────────────────────────────────────────────────────┘
                                        │
                                        │ MySQL 8.0 + Docker
                                        │
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              Infrastructure Layer                              │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌───────────┐ │
│  │    Docker   │ │   Nginx     │ │   Oracle    │ │     SSL     │ │ Monitoring│ │
│  │             │ │             │ │    Cloud    │ │             │ │           │ │
│  │ • Containers│ │ • Reverse   │ │             │ │ • Let's     │ │ • Health  │ │
│  │ • Compose   │ │   Proxy     │ │ • Always    │ │   Encrypt   │ │   Checks  │ │
│  │ • Volumes   │ │ • Load      │ │   Free      │ │ • Auto      │ │ • Logging │ │
│  │ • Networks  │ │   Balance   │ │   Tier      │ │   Renewal   │ │ • Metrics │ │
│  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘ └───────────┘ │
└─────────────────────────────────────────────────────────────────────────────────┘
```

## 📁 Project Structure

```
ems/
├── backend/                          # ASP.NET Web API Backend (.NET 8)
│   ├── EMS.API/                     # Main API project
│   │   ├── Controllers/             # API Controllers (6 controllers)
│   │   │   ├── AuthController.cs    # JWT authentication (login, register)
│   │   │   ├── EmployeesController.cs # Employee CRUD operations
│   │   │   ├── DepartmentsController.cs # Department management
│   │   │   ├── AttendanceController.cs # Check-in/out, attendance tracking
│   │   │   ├── ReportsController.cs # PDF/Excel report generation (8 types)
│   │   │   └── SeedController.cs    # Database seeding endpoints
│   │   ├── Services/                # Business logic services
│   │   │   ├── AuthService.cs       # JWT token generation & validation
│   │   │   ├── SeedDataService.cs   # Comprehensive data seeding
│   │   │   ├── RefactoredReportService.cs # Report orchestration
│   │   │   ├── DatabaseSeedingService.cs # 200+ employees, 90 days data
│   │   │   ├── DatabaseMigrationService.cs # Auto migration on startup
│   │   │   └── Reports/             # 26 report generators (PDF/Excel/CSV)
│   │   │       ├── EmployeeDirectory[Pdf|Excel|Csv]Generator.cs
│   │   │       ├── AttendanceReport[Pdf|Excel|Csv]Generator.cs
│   │   │       ├── SalaryReport[Pdf|Excel|Csv]Generator.cs
│   │   │       ├── HiringTrend[Pdf|Excel|Csv]Generator.cs
│   │   │       ├── DepartmentGrowth[Pdf|Excel|Csv]Generator.cs
│   │   │       ├── AttendancePattern[Pdf|Excel|Csv]Generator.cs
│   │   │       └── PerformanceMetrics[Pdf|Excel|Csv]Generator.cs
│   │   ├── Repositories/            # Data access layer with Repository pattern
│   │   │   ├── EmployeeRepository.cs # Employee data operations
│   │   │   ├── DepartmentRepository.cs # Department data operations
│   │   │   └── AttendanceRepository.cs # Attendance data operations
│   │   ├── Models/                  # Entity Framework models
│   │   │   ├── Employee.cs          # Employee entity (personal info, salary)
│   │   │   ├── Department.cs        # Department entity (name, manager)
│   │   │   ├── Attendance.cs        # Attendance entity (check-in/out times)
│   │   │   ├── User.cs              # User entity (authentication)
│   │   │   └── PerformanceMetric.cs # Performance tracking entity
│   │   ├── DTOs/                    # Data Transfer Objects
│   │   │   ├── EmployeeDto.cs       # Employee request/response DTOs
│   │   │   ├── AuthDto.cs           # Authentication DTOs
│   │   │   ├── AttendanceDto.cs     # Attendance DTOs
│   │   │   ├── DepartmentDto.cs     # Department DTOs
│   │   │   └── PaginationDto.cs     # Pagination response DTO
│   │   ├── Interfaces/              # Service interfaces
│   │   │   ├── IEmployeeRepository.cs
│   │   │   ├── IAuthService.cs
│   │   │   ├── IAttendanceRepository.cs
│   │   │   ├── IDepartmentRepository.cs
│   │   │   └── IReportService.cs
│   │   ├── Common/                  # Shared utilities
│   │   │   ├── BaseRepository.cs    # Generic repository base
│   │   │   ├── EmployeeMapper.cs    # Entity-DTO mapping
│   │   │   └── UtcDateTimeConverters.cs # DateTime handling
│   │   ├── Data/                    # Database context
│   │   │   └── EMSDbContext.cs      # Entity Framework context
│   │   ├── Migrations/              # Entity Framework migrations
│   │   │   ├── 20250910163739_InitialCreate.cs
│   │   │   └── 20250911174002_EnhancedSeedData.cs
│   │   └── Program.cs               # Application entry point & DI setup
│   ├── EMS.API.Tests/              # Unit and integration tests
│   │   ├── Controllers/            # Controller tests (6 test files)
│   │   ├── HealthCheckTests.cs     # Health check endpoint tests
│   │   └── TestBase.cs             # Test infrastructure
│   ├── seed-database.bat           # Windows database management script
│   ├── db-commands/                # Database initialization scripts
│   └── DATABASE_SEEDING.md         # Comprehensive seeding documentation
├── frontend/                        # React.js Frontend (TypeScript + Vite)
│   ├── src/
│   │   ├── components/              # React components (20+ components)
│   │   │   ├── auth/               # Authentication components
│   │   │   │   └── Login.tsx       # Login form with validation
│   │   │   ├── employees/          # Employee management (3 components)
│   │   │   │   ├── EmployeeList.tsx # DataGrid with search/filter/pagination
│   │   │   │   ├── EmployeeForm.tsx # Add/Edit employee form
│   │   │   │   └── EmployeeDetail.tsx # Employee details view
│   │   │   ├── departments/        # Department management
│   │   │   │   └── DepartmentList.tsx # Department CRUD interface
│   │   │   ├── attendance/         # Attendance tracking
│   │   │   │   └── Attendance.tsx  # Check-in/out + history view
│   │   │   ├── reports/            # Reports and analytics
│   │   │   │   └── Reports.tsx     # Report generation interface
│   │   │   ├── dashboard/          # Dashboard components
│   │   │   │   └── Dashboard.tsx   # Main dashboard with stats/charts
│   │   │   ├── layout/             # Layout components (2 versions)
│   │   │   │   ├── Layout.tsx      # Main layout with navigation
│   │   │   │   └── LayoutRefactored.tsx # Enhanced layout
│   │   │   └── common/             # Shared components (5 components)
│   │   │       ├── ConfirmDialog.tsx
│   │   │       ├── DataTable.tsx
│   │   │       ├── LoadingSpinner.tsx
│   │   │       ├── PageHeader.tsx
│   │   │       └── SearchBar.tsx
│   │   ├── services/               # API service layer (5 services)
│   │   │   ├── AttendanceService.ts
│   │   │   ├── DepartmentService.ts
│   │   │   ├── EmployeeService.ts
│   │   │   ├── ReportService.ts
│   │   │   └── SeedService.ts
│   │   ├── store/                  # Zustand state management
│   │   │   ├── auth.ts             # Authentication state
│   │   │   └── theme.ts            # Theme state (dark/light mode)
│   │   ├── hooks/                  # Custom React hooks (3 hooks)
│   │   │   ├── useDocumentTitle.ts # Dynamic page titles
│   │   │   ├── useEmployeeManagement.ts # Employee operations
│   │   │   └── useTheme.ts         # Theme management
│   │   ├── contexts/               # React contexts
│   │   │   └── ToastContext.tsx    # Toast notifications
│   │   ├── utils/                  # Utility functions
│   │   │   └── timezone.ts         # UTC timezone handling
│   │   └── lib/                    # API client and utilities
│   │       └── api.ts              # Axios configuration with interceptors
│   ├── public/                     # Static assets
│   ├── dist/                       # Production build output
│   ├── package.json                # Dependencies & scripts
│   ├── Dockerfile                  # Multi-stage production build
│   └── nginx.conf                  # Nginx configuration for serving
├── docs/                           # Comprehensive documentation (4 files)
│   ├── API_DOCUMENTATION.md        # Complete API reference (722 lines)
│   ├── DEPLOYMENT_GUIDE.md         # Oracle Cloud deployment (937 lines)
│   ├── DEVELOPMENT_FLOW.md         # Development workflow (356 lines)
│   └── Assignment.md               # Original project requirements
├── docker-compose.yml              # Production Docker setup (4 services)
├── docker-compose.dev.yml          # Development with hot reload
├── docker-compose.seed.yml         # Database seeding configuration
├── docker-compose.dev-seed.yml     # Development + seeding
├── DOCKER_SETUP.md                 # Docker documentation
├── PROJECT_SUMMARY.md              # Project overview and progress
├── REFACTORING_SUMMARY.md          # Code refactoring details
├── Makefile                        # Build automation commands
└── README.md                       # This comprehensive guide
```

### 📊 Project Statistics
- **Backend**: 15+ API endpoints across 6 controllers
- **Frontend**: 20+ React components with TypeScript
- **Database**: 5 entities with comprehensive relationships
- **Reports**: 26 report generators (PDF/Excel/CSV formats)
- **Tests**: 6 controller test suites + integration tests
- **Documentation**: 4 comprehensive documentation files (2000+ lines)
- **Docker**: 4 service containerization with health checks
- **Seeding**: 200+ employees, 10 departments, 90 days attendance data

## 🛠️ Technology Stack

### Backend Technologies
- **ASP.NET Web API** (.NET 8) - Modern, high-performance web framework
- **Entity Framework Core** - Object-relational mapping with MySQL provider
- **JWT Authentication** - Secure token-based authentication
- **BCrypt** - Password hashing and security
- **AutoMapper** - Object-to-object mapping
- **FluentValidation** - Input validation and data integrity
- **Swagger/OpenAPI** - API documentation and testing
- **iTextSharp** - PDF report generation
- **EPPlus** - Excel report generation
- **Serilog** - Structured logging

### Frontend Technologies
- **React 18** with **TypeScript** - Modern UI framework with type safety
- **Vite** - Fast build tool and development server
- **Material-UI (MUI)** - Comprehensive component library
- **React Router** - Client-side routing and navigation
- **Axios** - HTTP client for API communication
- **React Hook Form** - Performant form handling
- **Yup** - Schema validation for forms
- **Zustand** - Lightweight state management
- **React Query** - Server state management and caching

### Database & Infrastructure
- **MySQL 8.0** - Relational database management system
- **Docker & Docker Compose** - Containerization and orchestration
- **Nginx** - Reverse proxy and static file serving
- **Oracle Cloud Infrastructure** - Production hosting platform
- **Let's Encrypt** - SSL/TLS certificate management

### Development Tools
- **Visual Studio Code** - Primary development environment
- **Git** - Version control system
- **Postman** - API testing and documentation
- **MySQL Workbench** - Database management and design
- **Docker Desktop** - Local container development

## ✨ Features

### 🔐 Authentication & Security
- **JWT-based Authentication** - Secure token-based user authentication
- **Role-based Access Control** - Admin, HR, Manager, and User roles
- **Password Security** - BCrypt hashing with salt
- **Session Management** - Automatic token refresh and expiration
- **Protected Routes** - Frontend route protection based on authentication status

### 👥 Employee Management
- **Complete CRUD Operations** - Create, read, update, and delete employees
- **Advanced Search & Filtering** - Search by name, email, department, position
- **Pagination** - Efficient handling of large employee datasets
- **Bulk Import** - CSV/Excel file import for multiple employees
- **Employee Profiles** - Comprehensive employee information management
- **Department Assignment** - Assign employees to departments with role management
- **Salary Management** - Track and manage employee compensation

### 🏢 Department Management
- **Department CRUD** - Full department lifecycle management
- **Manager Assignment** - Assign department managers
- **Employee Count Tracking** - Real-time department size monitoring
- **Department Analytics** - Growth and performance metrics per department

### ⏰ Attendance Tracking
- **Check-in/Check-out System** - Real-time attendance tracking
- **Attendance History** - Complete attendance records with filtering
- **Overtime Calculation** - Automatic overtime hours calculation
- **Attendance Reports** - Detailed attendance analytics and patterns
- **Time Zone Support** - UTC time handling with local time display

### 📊 Reports & Analytics
- **Employee Directory** - Comprehensive employee listing (PDF/Excel)
- **Department Reports** - Department-wise analytics and summaries
- **Attendance Reports** - Detailed attendance analysis and trends
- **Salary Reports** - Compensation analysis and payroll summaries
- **Hiring Trend Analysis** - Recruitment patterns and growth metrics
- **Department Growth Tracking** - Department expansion and performance
- **Attendance Pattern Analysis** - Work pattern insights and optimization
- **Performance Metrics** - Employee performance tracking and evaluation

### 🎨 User Interface
- **Responsive Design** - Mobile-first, responsive layout
- **Dark/Light Theme** - User preference-based theme switching
- **Material Design** - Modern, intuitive Material-UI components
- **Real-time Updates** - Live data updates and notifications
- **Interactive Dashboards** - Comprehensive data visualization
- **Form Validation** - Client-side and server-side validation
- **Loading States** - User-friendly loading indicators and feedback

### 🔧 System Features
- **Database Seeding** - Comprehensive test data generation (200+ employees)
- **API Documentation** - Complete Swagger/OpenAPI documentation
- **Error Handling** - Comprehensive error handling and user feedback
- **Logging** - Structured logging for debugging and monitoring
- **Health Checks** - System health monitoring and status reporting
- **Docker Support** - Complete containerization for easy deployment
- **Environment Configuration** - Flexible environment-based configuration

## Getting Started

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- MySQL Server
- Visual Studio Code or Visual Studio
- Docker & Docker Compose (for containerized setup)

### Option 1: Docker Compose (Recommended)
```bash
# Clone the repository
git clone <repository-url>
cd ems

# Start with comprehensive database seeding
docker-compose -f docker-compose.yml -f docker-compose.seed.yml up

# Or for development with hot reload
docker-compose -f docker-compose.dev.yml -f docker-compose.dev-seed.yml up
```

### Option 2: Manual Setup
```bash
# Backend Setup
cd backend/EMS.API
dotnet restore
dotnet ef database update
dotnet run

# Frontend Setup (in another terminal)
cd frontend
npm install
npm run dev
```

### Database Seeding
The EMS API includes comprehensive seed data generation for development and testing:

#### Quick Start

**Docker Compose (Recommended):**
```bash
# Production with comprehensive seeding
docker-compose -f docker-compose.yml -f docker-compose.seed.yml up

# Development with hot reload and seeding
docker-compose -f docker-compose.dev.yml -f docker-compose.dev-seed.yml up

# Custom seeding type
SEED_TYPE=reseed docker-compose -f docker-compose.yml -f docker-compose.seed.yml up
```

**Manual Setup:**
```bash
# Using Batch File (Windows)
cd backend
.\seed-database.bat seed

# Using API directly (requires running API)
curl -X POST http://localhost:5000/api/seed/seed
```

#### Available Seeding Commands

**Batch File (`seed-database.bat`):**
```cmd
# Show all available commands
.\seed-database.bat help

# Apply database migrations
.\seed-database.bat migrate

# Seed database with initial data
.\seed-database.bat seed

# Reseed database with fresh data
.\seed-database.bat reseed

# Show current database status
.\seed-database.bat status
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

## 🔌 API Endpoints

### 🔐 Authentication Endpoints
| Method | Endpoint | Description | Features | Auth Required |
|--------|----------|-------------|----------|---------------|
| `POST` | `/api/auth/login` | User authentication and JWT token generation | BCrypt password verification, Last login tracking | No |
| `POST` | `/api/auth/register` | User registration with role assignment | Password hashing, Email validation, Role assignment | No |

**Default Admin Credentials:**
- Username: `admin` | Password: `admin123` | Role: `Admin`
- Username: `hr` | Password: `hr123` | Role: `HR`

### 👥 Employee Management Endpoints
| Method | Endpoint | Description | Features | Auth Required | Roles |
|--------|----------|-------------|----------|---------------|-------|
| `GET` | `/api/employees` | Get paginated employee list | Search by name/email, Filter by department/status, Sort by multiple fields, Pagination (max 100) | Yes | All |
| `GET` | `/api/employees/{id}` | Get specific employee details | Complete employee profile with department info | Yes | All |
| `POST` | `/api/employees` | Create new employee | Email uniqueness validation, Department validation, Salary validation | Yes | Admin, HR |
| `PUT` | `/api/employees/{id}` | Update employee information | Partial updates supported, Timestamp tracking | Yes | Admin, HR |
| `DELETE` | `/api/employees/{id}` | Delete employee (soft delete) | Maintains data integrity, Cascade handling | Yes | Admin |
| `POST` | `/api/employees/bulk` | Bulk import employees from CSV/Excel | Validation per row, Error reporting, Transaction safety | Yes | Admin, HR |

### 🏢 Department Management Endpoints
| Method | Endpoint | Description | Features | Auth Required | Roles |
|--------|----------|-------------|----------|---------------|-------|
| `GET` | `/api/departments` | Get all departments | Employee count per department, Manager information | Yes | All |
| `GET` | `/api/departments/{id}` | Get specific department details | Department employees list, Statistics | Yes | All |
| `POST` | `/api/departments` | Create new department | Name uniqueness validation, Manager assignment | Yes | Admin, HR |
| `PUT` | `/api/departments/{id}` | Update department information | Manager reassignment, Description updates | Yes | Admin, HR |
| `DELETE` | `/api/departments/{id}` | Delete department | Employee reassignment validation, Safe deletion | Yes | Admin |

### ⏰ Attendance Tracking Endpoints
| Method | Endpoint | Description | Features | Auth Required | Roles |
|--------|----------|-------------|----------|---------------|-------|
| `POST` | `/api/attendance/check-in` | Employee check-in with timestamp | UTC time handling, Duplicate check-in prevention | Yes | All |
| `POST` | `/api/attendance/check-out` | Employee check-out with timestamp | Automatic hours calculation, Overtime detection | Yes | All |
| `GET` | `/api/attendance/{employeeId}` | Get employee attendance history | Date range filtering, Work hours summary, Attendance patterns | Yes | All |
| `GET` | `/api/attendance/today/{employeeId}` | Get today's attendance for employee | Current day status, Hours worked | Yes | All |

### 📊 Report Generation Endpoints
| Method | Endpoint | Description | Features | Formats | Auth Required | Roles |
|--------|----------|-------------|----------|---------|---------------|-------|
| `GET` | `/api/reports/employees` | Employee directory report | Complete employee listings with contact info | CSV/PDF/Excel | Yes | Admin, HR, Manager |
| `GET` | `/api/reports/departments` | Department analytics report | Department breakdown, employee distribution | CSV/PDF/Excel | Yes | All |
| `GET` | `/api/reports/attendance` | Attendance analysis report | Date range filtering, attendance statistics | CSV/PDF/Excel | Yes | All |
| `GET` | `/api/reports/salaries` | Salary and compensation report | Salary ranges, department averages, confidential data | CSV/PDF/Excel | Yes | Admin, HR |
| `GET` | `/api/reports/hiring-trends` | Hiring trend analysis | Monthly/yearly hiring patterns, growth metrics | CSV/PDF/Excel | Yes | All |
| `GET` | `/api/reports/department-growth` | Department growth tracking | Department expansion over time, headcount trends | CSV/PDF/Excel | Yes | All |
| `GET` | `/api/reports/attendance-patterns` | Attendance pattern analysis | Work behavior analysis, attendance insights | CSV/PDF/Excel | Yes | All |
| `GET` | `/api/reports/performance-metrics` | Performance metrics report | Employee performance data, quarterly reviews | CSV/PDF/Excel | Yes | Admin, HR |

**Report Features:**
- **PDF Reports**: Professional formatting with iTextSharp, company branding, charts
- **Excel Reports**: Advanced formatting with EPPlus, formulas, conditional formatting
- **CSV Reports**: Data export for external analysis, raw data format

### 🌱 Database Seeding Endpoints (Admin Only)
| Method | Endpoint | Description | Features | Auth Required | Roles |
|--------|----------|-------------|----------|---------------|-------|
| `POST` | `/api/seed/seed` | Seed database with initial test data | 200+ employees, 10 departments, 90 days attendance, 2 years performance data | Yes | Admin |
| `POST` | `/api/seed/reseed` | Clear and reseed with fresh data | Complete data reset, fresh realistic data generation | Yes | Admin |
| `DELETE` | `/api/seed/clear` | Clear all data from database | Safe cascading deletion, foreign key handling | Yes | Admin |
| `GET` | `/api/seed/status` | Get current database record counts | Table statistics, data verification | Yes | Admin |

**Seeding Features:**
- **Realistic Data**: Names, emails, addresses, phone numbers
- **Consistent Relationships**: Proper department assignments, manager hierarchies
- **Time-based Data**: Realistic hire dates, attendance patterns, performance cycles
- **Configurable**: Environment-based seeding options

### 📋 System Health & Monitoring Endpoints
| Method | Endpoint | Description | Features | Auth Required |
|--------|----------|-------------|----------|---------------|
| `GET` | `/health` | System health check | Database connectivity, service status, timestamp | No |
| `GET` | `/swagger` | API documentation (Swagger UI) | Interactive API testing, endpoint documentation | No |

### 🔧 Technical Implementation Details

**Authentication & Security:**
- **JWT Tokens**: HS256 algorithm, 60-minute expiration, role-based claims
- **Password Security**: BCrypt hashing with salt, minimum 6 characters
- **CORS**: Configured for localhost and production domains
- **Rate Limiting**: Built-in ASP.NET rate limiting (future enhancement)

**Data Validation:**
- **Model Validation**: DataAnnotations with custom validators
- **Business Rules**: Service-level validation logic
- **Database Constraints**: Entity Framework constraints and indexes
- **Input Sanitization**: XSS protection, SQL injection prevention

**Performance Optimizations:**
- **Pagination**: Efficient large dataset handling
- **Lazy Loading**: Related entity loading optimization
- **Caching**: Memory caching for frequently accessed data
- **Query Optimization**: LINQ query optimization, indexing strategy

**Error Handling:**
- **Global Exception Handling**: Centralized error processing
- **Structured Logging**: Serilog integration with request tracking
- **User-Friendly Messages**: Abstracted technical errors
- **Development vs Production**: Different error detail levels

## 🚀 Deployment

### 🌐 Production URLs
- **Frontend**: `https://ems.srikanthkandi.tech`
- **Backend API**: `https://api.ems.srikanthkandi.tech`
- **API Documentation**: `https://api.ems.srikanthkandi.tech/swagger`

### ☁️ Oracle Cloud Infrastructure Setup
- **Compute Instance**: Ubuntu 22.04 LTS (VM.Standard.E2.1.Micro - Always Free)
- **Database**: MySQL Database Service (Always Free Tier)
- **Load Balancer**: Nginx reverse proxy with SSL termination
- **SSL Certificates**: Let's Encrypt with automatic renewal
- **Monitoring**: Built-in Oracle Cloud monitoring and alerting

### 🐳 Docker Deployment Options

#### Production Deployment
```bash
# Start all services with production configuration
docker-compose up -d

# Access the application
# Frontend: http://localhost:3000
# Backend: http://localhost:5000
# Database: localhost:3306
```

#### Development Deployment
```bash
# Start development environment with hot reload
docker-compose -f docker-compose.dev.yml up -d

# Access the application with live reload
# Frontend: http://localhost:3000 (with hot reload)
# Backend: http://localhost:5000 (with hot reload)
```

#### Database Seeding Deployment
```bash
# Start with comprehensive database seeding
docker-compose -f docker-compose.yml -f docker-compose.seed.yml up -d

# Or for development with seeding
docker-compose -f docker-compose.dev.yml -f docker-compose.dev-seed.yml up -d
```

## 🔄 Development Workflow

### Phase 1: Project Setup & Architecture
1. **Monorepo Structure** - Organized backend and frontend directories
2. **Database Design** - Entity models, relationships, and migrations
3. **API Architecture** - Controller, service, and repository patterns
4. **Frontend Architecture** - Component structure and state management

### Phase 2: Backend Implementation
1. **Authentication System** - JWT implementation with role-based access
2. **Core APIs** - Employee, Department, and Attendance management
3. **Report Generation** - PDF and Excel report services
4. **Database Seeding** - Comprehensive test data generation
5. **API Documentation** - Swagger/OpenAPI integration

### Phase 3: Frontend Implementation
1. **Authentication UI** - Login and registration components
2. **Employee Management** - CRUD operations with advanced filtering
3. **Dashboard** - Analytics and statistics visualization
4. **Attendance Tracking** - Check-in/out interface with history
5. **Reports Interface** - Report generation and download functionality

### Phase 4: Integration & Testing
1. **API Integration** - Frontend-backend communication
2. **State Management** - Zustand store implementation
3. **Error Handling** - Comprehensive error handling and user feedback
4. **Form Validation** - Client-side and server-side validation
5. **Testing** - Unit tests and integration testing

### Phase 5: Deployment & DevOps
1. **Docker Containerization** - Multi-stage builds and optimization
2. **Oracle Cloud Setup** - Infrastructure provisioning and configuration
3. **CI/CD Pipeline** - Automated testing and deployment
4. **Monitoring** - Health checks and logging implementation
5. **Documentation** - Comprehensive API and user documentation

## 🔧 Troubleshooting

### 🐛 Common Issues & Solutions

#### Database Seeding Issues

**Problem**: "API is not running" Error
   ```bash
# Solution: Start the API first
   cd backend/EMS.API
   dotnet run

# Or using Docker
docker-compose up -d backend
   ```

**Problem**: "Database already contains data" Error
   ```bash
# Solution: Use reseed instead of seed
   .\seed-database.bat reseed

# Or via API
curl -X POST http://localhost:5000/api/seed/reseed \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
   ```

**Problem**: Authentication Required
   ```bash
# Solution: Get admin token first
   curl -X POST http://localhost:5000/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{"username":"admin","password":"admin123"}'
   ```

#### Database Connection Issues

**Problem**: Cannot connect to database
- ✅ Verify connection string in `appsettings.json`
- ✅ Ensure MySQL server is running
- ✅ Check database permissions and credentials
- ✅ Verify network connectivity

**Problem**: Migration errors
```bash
# Solution: Reset and recreate database
dotnet ef database drop
dotnet ef database update
```

#### Frontend Issues

**Problem**: Frontend not loading
- ✅ Check if backend API is running
- ✅ Verify API URL in `.env` file
- ✅ Check browser console for errors
- ✅ Ensure CORS is properly configured

**Problem**: Authentication not working
- ✅ Verify JWT token is valid
- ✅ Check token expiration
- ✅ Ensure proper API endpoint URLs

#### Docker Issues

**Problem**: Container won't start
```bash
# Check container logs
docker-compose logs [service-name]

# Rebuild containers
docker-compose build --no-cache
docker-compose up -d
```

**Problem**: Port conflicts
- ✅ Ensure ports 3000, 5000, and 3306 are available
- ✅ Change ports in `docker-compose.yml` if needed

### 📊 Performance Considerations

- **Database Seeding**: Full reseed takes 30-60 seconds
- **Memory Usage**: Large datasets require adequate memory (4GB+ recommended)
- **Database Size**: Full seed data creates ~50MB database
- **API Response**: Pagination implemented for large datasets
- **Frontend Loading**: Lazy loading and code splitting implemented

### 🔍 Debugging Tips

1. **Check Logs**: Always check application logs first
2. **Health Checks**: Use `/health` endpoint to verify system status
3. **Database Status**: Use `/api/seed/status` to check data counts
4. **Network**: Verify connectivity between services
5. **Environment**: Check environment variables and configuration

## 🤝 Contributing

We welcome contributions to the Employee Management System! Here's how you can help:

### 🚀 Getting Started
1. **Fork the repository** to your GitHub account
2. **Clone your fork** locally
3. **Create a feature branch** (`git checkout -b feature/amazing-feature`)
4. **Make your changes** following our coding standards
5. **Test your changes** thoroughly
6. **Commit your changes** (`git commit -m 'Add amazing feature'`)
7. **Push to your branch** (`git push origin feature/amazing-feature`)
8. **Create a Pull Request** with a detailed description

### 📋 Development Guidelines
- Follow the existing code style and patterns
- Write comprehensive tests for new features
- Update documentation for any API changes
- Ensure all tests pass before submitting
- Use meaningful commit messages

### 🐛 Reporting Issues
- Use the GitHub Issues tracker
- Provide detailed reproduction steps
- Include system information and logs
- Use appropriate labels and templates

## 📈 Project Status

### ✅ Completed Features
- **Authentication System** - JWT-based authentication with role management
- **Employee Management** - Complete CRUD operations with advanced filtering
- **Department Management** - Full department lifecycle management
- **Attendance Tracking** - Check-in/out system with history and analytics
- **Report Generation** - PDF and Excel reports for all major data types
- **Database Seeding** - Comprehensive test data generation (200+ employees)
- **Docker Support** - Complete containerization for easy deployment
- **API Documentation** - Swagger/OpenAPI documentation
- **Responsive UI** - Mobile-first design with Material-UI components

### 🔄 In Progress
- **Performance Optimization** - Query optimization and caching
- **Advanced Analytics** - Enhanced reporting and dashboard features
- **Testing Coverage** - Comprehensive unit and integration tests
- **CI/CD Pipeline** - Automated testing and deployment

### 📋 Future Enhancements
- **Real-time Notifications** - WebSocket-based live updates
- **Mobile App** - React Native mobile application
- **Advanced Reporting** - Custom report builder
- **Integration APIs** - Third-party system integrations
- **Audit Logging** - Comprehensive activity tracking
- **Multi-tenancy** - Support for multiple organizations

## 📊 Technical Metrics

- **Backend**: ASP.NET Web API with 15+ endpoints
- **Frontend**: React.js with 20+ components
- **Database**: MySQL with 5+ tables and relationships
- **Test Coverage**: 80%+ code coverage target
- **Performance**: Sub-200ms API response times
- **Security**: JWT authentication with role-based access control

## 🏆 Project Highlights

- **Clean Architecture** - SOLID principles and separation of concerns
- **Modern Tech Stack** - Latest versions of all technologies
- **Comprehensive Documentation** - Detailed guides and API documentation
- **Production Ready** - Oracle Cloud deployment with SSL and monitoring
- **Scalable Design** - Built for future enhancements and growth
- **Security First** - JWT authentication and data protection
- **User Friendly** - Modern UI with intuitive user experience

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Team

- **Lead Developer**: Srikanth Kandi
- **Architecture**: Clean Architecture with SOLID principles
- **Tech Stack**: ASP.NET Web API + React.js + MySQL
- **Deployment**: Oracle Cloud Infrastructure

## 📞 Support

For support, questions, or feature requests:
- 📧 Email: [your-email@example.com]
- 🐛 Issues: [GitHub Issues](https://github.com/your-username/ems/issues)
- 📖 Documentation: [Project Wiki](https://github.com/your-username/ems/wiki)

---

**Built with ❤️ using modern web technologies**
