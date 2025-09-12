# Employee Management System (EMS)

A comprehensive, full-stack Employee Management System built with modern technologies, featuring a robust ASP.NET Web API backend and a responsive React.js frontend. This system provides complete HR management capabilities including employee tracking, attendance management, department organization, and comprehensive reporting.

## 🏗️ System Architecture

The EMS follows a clean, layered architecture pattern with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                    Frontend (React.js)                     │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌───────┐ │
│  │   Auth      │ │  Employee   │ │ Attendance  │ │Reports│ │
│  │ Components  │ │ Management  │ │  Tracking   │ │ &Analytics│ │
│  └─────────────┘ └─────────────┘ └─────────────┘ └───────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
                              │ HTTP/REST API
                              │
┌─────────────────────────────────────────────────────────────┐
│                 Backend (ASP.NET Web API)                  │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌───────┐ │
│  │ Controllers │ │  Services   │ │ Repositories│ │  DTOs │ │
│  │   Layer     │ │   Layer     │ │   Layer     │ │ Layer │ │
│  └─────────────┘ └─────────────┘ └─────────────┘ └───────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
                              │ Entity Framework Core
                              │
┌─────────────────────────────────────────────────────────────┐
│                    Database (MySQL)                        │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌───────┐ │
│  │  Employees  │ │ Departments │ │ Attendance  │ │ Users │ │
│  │    Table    │ │   Table     │ │   Table     │ │ Table │ │
│  └─────────────┘ └─────────────┘ └─────────────┘ └───────┘ │
└─────────────────────────────────────────────────────────────┘
```

## 📁 Project Structure

```
ems/
├── backend/                          # ASP.NET Web API Backend
│   ├── EMS.API/                     # Main API project
│   │   ├── Controllers/             # API Controllers
│   │   │   ├── AuthController.cs    # Authentication endpoints
│   │   │   ├── EmployeesController.cs # Employee management
│   │   │   ├── DepartmentsController.cs # Department management
│   │   │   ├── AttendanceController.cs # Attendance tracking
│   │   │   ├── ReportsController.cs # Report generation
│   │   │   └── SeedController.cs    # Database seeding
│   │   ├── Services/                # Business logic services
│   │   │   ├── AuthService.cs       # Authentication service
│   │   │   ├── SeedDataService.cs   # Database seeding
│   │   │   ├── RefactoredReportService.cs # Report generation
│   │   │   └── Reports/             # Report generators
│   │   ├── Repositories/            # Data access layer
│   │   │   ├── EmployeeRepository.cs
│   │   │   ├── DepartmentRepository.cs
│   │   │   └── AttendanceRepository.cs
│   │   ├── Models/                  # Entity models
│   │   │   ├── Employee.cs
│   │   │   ├── Department.cs
│   │   │   ├── Attendance.cs
│   │   │   ├── User.cs
│   │   │   └── PerformanceMetric.cs
│   │   ├── DTOs/                    # Data Transfer Objects
│   │   ├── Common/                  # Shared utilities
│   │   ├── Data/                    # Database context
│   │   └── Migrations/              # Entity Framework migrations
│   ├── EMS.API.Tests/              # Unit and integration tests
│   ├── seed-database.bat           # Database management script
│   └── DATABASE_SEEDING.md         # Seeding documentation
├── frontend/                        # React.js Frontend
│   ├── src/
│   │   ├── components/              # React components
│   │   │   ├── auth/               # Authentication components
│   │   │   ├── employees/          # Employee management
│   │   │   ├── departments/        # Department management
│   │   │   ├── attendance/         # Attendance tracking
│   │   │   ├── reports/            # Reports and analytics
│   │   │   ├── dashboard/          # Dashboard components
│   │   │   ├── layout/             # Layout components
│   │   │   └── common/             # Shared components
│   │   ├── services/               # API service layer
│   │   ├── store/                  # State management (Zustand)
│   │   ├── hooks/                  # Custom React hooks
│   │   ├── contexts/               # React contexts
│   │   ├── utils/                  # Utility functions
│   │   └── lib/                    # API client and utilities
│   ├── public/                     # Static assets
│   └── package.json
├── docs/                           # Comprehensive documentation
│   ├── API_DOCUMENTATION.md        # API reference
│   ├── DEPLOYMENT_GUIDE.md         # Deployment instructions
│   ├── DEVELOPMENT_FLOW.md         # Development workflow
│   └── Assignment.md               # Project requirements
├── docker-compose.yml              # Production Docker setup
├── docker-compose.dev.yml          # Development Docker setup
├── docker-compose.seed.yml         # Database seeding setup
├── DOCKER_SETUP.md                 # Docker documentation
└── README.md                       # This file
```

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
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `POST` | `/api/auth/login` | User authentication and JWT token generation | No |
| `POST` | `/api/auth/register` | User registration with role assignment | No |

### 👥 Employee Management Endpoints
| Method | Endpoint | Description | Auth Required | Roles |
|--------|----------|-------------|---------------|-------|
| `GET` | `/api/employees` | Get paginated employee list with search/filter | Yes | All |
| `GET` | `/api/employees/{id}` | Get specific employee details | Yes | All |
| `POST` | `/api/employees` | Create new employee | Yes | Admin, HR |
| `PUT` | `/api/employees/{id}` | Update employee information | Yes | Admin, HR |
| `DELETE` | `/api/employees/{id}` | Delete employee | Yes | Admin |
| `POST` | `/api/employees/bulk` | Bulk import employees from CSV/Excel | Yes | Admin, HR |

### 🏢 Department Management Endpoints
| Method | Endpoint | Description | Auth Required | Roles |
|--------|----------|-------------|---------------|-------|
| `GET` | `/api/departments` | Get all departments | Yes | All |
| `GET` | `/api/departments/{id}` | Get specific department details | Yes | All |
| `POST` | `/api/departments` | Create new department | Yes | Admin, HR |
| `PUT` | `/api/departments/{id}` | Update department information | Yes | Admin, HR |
| `DELETE` | `/api/departments/{id}` | Delete department | Yes | Admin |

### ⏰ Attendance Tracking Endpoints
| Method | Endpoint | Description | Auth Required | Roles |
|--------|----------|-------------|---------------|-------|
| `POST` | `/api/attendance/check-in` | Employee check-in with timestamp | Yes | All |
| `POST` | `/api/attendance/check-out` | Employee check-out with timestamp | Yes | All |
| `GET` | `/api/attendance/{employeeId}` | Get employee attendance history | Yes | All |
| `GET` | `/api/attendance/today/{employeeId}` | Get today's attendance for employee | Yes | All |

### 📊 Report Generation Endpoints
| Method | Endpoint | Description | Auth Required | Roles | Format |
|--------|----------|-------------|---------------|-------|--------|
| `GET` | `/api/reports/directory` | Employee directory report | Yes | All | PDF/Excel |
| `GET` | `/api/reports/departments` | Department analytics report | Yes | All | PDF/Excel |
| `GET` | `/api/reports/attendance` | Attendance analysis report | Yes | All | PDF/Excel |
| `GET` | `/api/reports/salary` | Salary and compensation report | Yes | Admin, HR | PDF/Excel |
| `GET` | `/api/reports/hiring-trends` | Hiring trend analysis | Yes | Admin, HR | PDF/Excel |
| `GET` | `/api/reports/department-growth` | Department growth tracking | Yes | Admin, HR | PDF/Excel |
| `GET` | `/api/reports/attendance-patterns` | Attendance pattern analysis | Yes | Admin, HR | PDF/Excel |
| `GET` | `/api/reports/performance-metrics` | Performance metrics report | Yes | Admin, HR | PDF/Excel |

### 🌱 Database Seeding Endpoints (Admin Only)
| Method | Endpoint | Description | Auth Required | Roles |
|--------|----------|-------------|---------------|-------|
| `POST` | `/api/seed/seed` | Seed database with initial test data | Yes | Admin |
| `POST` | `/api/seed/reseed` | Clear and reseed with fresh data | Yes | Admin |
| `DELETE` | `/api/seed/clear` | Clear all data from database | Yes | Admin |
| `GET` | `/api/seed/status` | Get current database record counts | Yes | Admin |

### 📋 System Health Endpoints
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/health` | System health check | No |
| `GET` | `/swagger` | API documentation (Swagger UI) | No |

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
