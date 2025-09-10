# Employee Management System (EMS)

A comprehensive Employee Management System built with ASP.NET Web API backend and React.js frontend.

## Project Structure

```
ems/
├── backend/                 # ASP.NET Web API
│   ├── EMS.API/            # Main API project
│   ├── EMS.Core/           # Domain models and interfaces
│   ├── EMS.Infrastructure/ # Data access and external services
│   └── EMS.Application/    # Business logic and services
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

### Frontend Setup
```bash
cd frontend
npm install
npm run dev
```

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

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License.
