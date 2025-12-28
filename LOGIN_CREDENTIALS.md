# EMS Login Credentials - Seeded Database

## 🔐 **Default Login Credentials**

### **Primary Accounts (Main Roles)**

| Role | Username | Password | Email | Access Level |
|------|----------|----------|-------|--------------|
| **Admin** | `admin` | `admin123` | admin@ems.com | Full system access, database seeding, user management |
| **HR Manager** | `hr_manager` | `hr123` | hr@ems.com | Employee management, salary reports, department management |
| **Manager** | `manager` | `manager123` | manager@ems.com | Employee directory, attendance reports, department reports |

---

## 📊 **Role-Based Access Control**

### **Admin Role**
**Username:** `admin`  
**Password:** `admin123`

**Permissions:**
- ✅ Full system access
- ✅ Database seeding operations (`/api/seed/*`)
- ✅ User management
- ✅ All employee operations (Create, Read, Update, Delete)
- ✅ All department operations
- ✅ All reports (including salary reports)
- ✅ Attendance management
- ✅ Performance metrics access

### **HR Role**
**Username:** `hr_manager`  
**Password:** `hr123`

**Permissions:**
- ✅ Employee management (Create, Read, Update)
- ✅ Department management (Create, Read, Update)
- ✅ Salary reports (confidential access)
- ✅ Employee directory reports
- ✅ Attendance reports
- ✅ Performance metrics
- ❌ Database seeding (Admin only)
- ❌ Employee deletion (Admin only)

### **Manager Role**
**Username:** `manager`  
**Password:** `manager123`

**Permissions:**
- ✅ View employee directory
- ✅ View department information
- ✅ Attendance reports
- ✅ Department reports
- ✅ Hiring trends
- ✅ Department growth reports
- ❌ Employee creation/editing (HR/Admin only)
- ❌ Salary reports (HR/Admin only)
- ❌ Database operations

---

## 👥 **Additional Test Users (50 Users Seeded)**

The database also contains 50 additional test users with the following pattern:

**Username Format:** `firstname.lastname` (e.g., `john.smith`, `mary.johnson`)  
**Password:** `password123` (same for all test users)  
**Email Format:** `firstname.lastname@ems.com`  
**Roles:** Randomly assigned as Employee, Manager, or HR

### **Example Additional Users:**
- `james.smith` / `password123`
- `mary.johnson` / `password123`
- `john.williams` / `password123`
- `patricia.brown` / `password123`

**Note:** These users are randomly generated, so exact usernames may vary. Use the primary accounts above for testing.

---

## 🎯 **Quick Login Guide for Interview**

### **For Demonstrating Admin Features:**
```
Username: admin
Password: admin123
```
Use this to show:
- Database seeding
- Full CRUD operations
- All report types
- System administration

### **For Demonstrating HR Features:**
```
Username: hr_manager
Password: hr123
```
Use this to show:
- Employee management
- Salary reports
- Department management
- HR-specific workflows

### **For Demonstrating Manager Features:**
```
Username: manager
Password: manager123
```
Use this to show:
- Read-only employee access
- Department reports
- Attendance tracking
- Manager-level permissions

---

## 🔑 **Testing Different Scenarios**

### **Scenario 1: Full System Access**
```
Login: admin / admin123
Navigate to: All sections available
Test: Create employee, generate salary report, seed database
```

### **Scenario 2: HR Workflow**
```
Login: hr_manager / hr123
Navigate to: Employees, Departments, Reports
Test: Add new employee, view salary report, manage departments
```

### **Scenario 3: Manager View**
```
Login: manager / manager123
Navigate to: Dashboard, Reports, Attendance
Test: View employee directory, generate reports (no salary access)
```

### **Scenario 4: Permission Testing**
```
1. Login as manager / manager123
2. Try to create employee → Should be blocked
3. Try to view salary report → Should be blocked
4. Logout and login as admin / admin123
5. Same actions → Should work
```

---

## 🚀 **Quick Test Commands**

### **Get Auth Token (PowerShell)**
```powershell
# Admin token
$response = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" -Method Post -ContentType "application/json" -Body '{"username":"admin","password":"admin123"}'
$adminToken = $response.token
Write-Host "Admin Token: $adminToken"

# HR token
$response = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" -Method Post -ContentType "application/json" -Body '{"username":"hr_manager","password":"hr123"}'
$hrToken = $response.token
Write-Host "HR Token: $hrToken"

# Manager token
$response = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" -Method Post -ContentType "application/json" -Body '{"username":"manager","password":"manager123"}'
$managerToken = $response.token
Write-Host "Manager Token: $managerToken"
```

### **Test API with Token (PowerShell)**
```powershell
# Get employees (works for all roles)
$headers = @{"Authorization"="Bearer $adminToken"}
Invoke-RestMethod -Uri "http://localhost:5000/api/employees" -Method Get -Headers $headers

# Seed database (Admin only)
$headers = @{"Authorization"="Bearer $adminToken"}
Invoke-RestMethod -Uri "http://localhost:5000/api/seed/reseed" -Method Post -Headers $headers

# Try salary report with Manager (should fail)
$headers = @{"Authorization"="Bearer $managerToken"}
Invoke-RestMethod -Uri "http://localhost:5000/api/reports/salaries" -Method Get -Headers $headers
```

---

## 📝 **Interview Demonstration Tips**

### **Tip 1: Show Role-Based Access**
1. Login as `manager` → Try to create employee → Show error
2. Logout → Login as `admin` → Create employee → Success
3. Explain: "This demonstrates our role-based authorization system"

### **Tip 2: Show Different Dashboards**
1. Login as each role and show different available features
2. Point out menu items that appear/disappear based on role
3. Explain: "UI adapts based on user permissions"

### **Tip 3: Show Report Access Control**
1. Login as `manager` → Try salary report → Access denied
2. Login as `hr_manager` → Salary report → Success
3. Explain: "Sensitive data is protected by role-based access"

---

## ⚠️ **Important Notes**

1. **Default Credentials**: These are for development/testing only. In production, enforce strong password policies.

2. **Token Expiration**: JWT tokens expire after 60 minutes. You'll need to login again.

3. **Password Hashing**: All passwords are hashed with BCrypt before storage (never stored in plain text).

4. **Active Status**: All primary accounts are active by default. Additional test users have 90% active rate.

5. **Last Login**: Admin shows recent login, others may show older login times (seeded data).

---

## 🎯 **Quick Reference Card (Print This!)**

```
┌─────────────────────────────────────────────────┐
│         EMS LOGIN CREDENTIALS                   │
├─────────────────────────────────────────────────┤
│                                                 │
│  ADMIN:                                         │
│  Username: admin                                │
│  Password: admin123                             │
│  Access:   Full system (including seeding)     │
│                                                 │
│  HR MANAGER:                                    │
│  Username: hr_manager                           │
│  Password: hr123                                │
│  Access:   Employee & salary management        │
│                                                 │
│  MANAGER:                                       │
│  Username: manager                              │
│  Password: manager123                           │
│  Access:   View-only, reports (no salary)      │
│                                                 │
│  TEST USERS:                                    │
│  Username: firstname.lastname                   │
│  Password: password123                          │
│  Access:   Varies (Employee/Manager/HR)        │
│                                                 │
└─────────────────────────────────────────────────┘
```

---

## 🔐 **Security Implementation Details**

### **Password Hashing**
```csharp
// From DatabaseSeedingService.cs
PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123")
```

### **JWT Token Generation**
- Algorithm: HS256
- Expiration: 60 minutes
- Claims: Username, Role, Email
- Validation: Issuer, Audience, Lifetime

### **Authorization Attributes**
```csharp
[Authorize]                           // All authenticated users
[Authorize(Roles = "Admin")]          // Admin only
[Authorize(Roles = "Admin,HR")]       // Admin or HR
[Authorize(Roles = "Admin,HR,Manager")] // Multiple roles
```

---

**Use these credentials for your interview demonstration tomorrow! 🚀**
