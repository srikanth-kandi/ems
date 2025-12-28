# Docker Commands for Windows - Live Coding Interview
## EMS Development Environment - Windows PowerShell Guide

---

## 🚀 **QUICK START (WINDOWS)**

### **Option 1: Using PowerShell Script (EASIEST)**
```powershell
# Start development environment
.\dev.ps1 start

# View logs
.\dev.ps1 logs

# Stop environment
.\dev.ps1 stop
```

### **Option 2: Direct Docker Commands**
```powershell
# Start development environment with hot reload
docker-compose -f docker-compose.dev.yml up -d

# Stop environment
docker-compose -f docker-compose.dev.yml down
```

---

## 📋 **ESSENTIAL COMMANDS FOR INTERVIEW (WINDOWS)**

### **1. Start Development Environment**
```powershell
# Using PowerShell script
.\dev.ps1 start

# OR using docker-compose directly
docker-compose -f docker-compose.dev.yml up -d

# With database seeding (200+ employees)
docker-compose -f docker-compose.dev.yml -f docker-compose.dev-seed.yml up -d
```

**What starts:**
- ✅ MySQL database (port 3306)
- ✅ Backend API (port 5000) - http://localhost:5000
- ✅ Frontend React (port 3000) - http://localhost:3000 with **HOT RELOAD**

### **2. View Logs (Real-time Monitoring)**
```powershell
# All logs (follow mode)
.\dev.ps1 logs
# OR
docker-compose -f docker-compose.dev.yml logs -f

# Backend logs only
.\dev.ps1 logs-backend
# OR
docker-compose -f docker-compose.dev.yml logs -f backend

# Frontend logs only
.\dev.ps1 logs-frontend
# OR
docker-compose -f docker-compose.dev.yml logs -f frontend

# Database logs
.\dev.ps1 logs-db
# OR
docker-compose -f docker-compose.dev.yml logs -f mysql

# Exit logs: Press Ctrl+C
```

### **3. Check Status**
```powershell
# Using script
.\dev.ps1 status

# OR direct command
docker-compose -f docker-compose.dev.yml ps

# Check all Docker containers
docker ps
```

### **4. Stop Services**
```powershell
# Using script
.\dev.ps1 stop

# OR direct command
docker-compose -f docker-compose.dev.yml down

# Stop but keep containers
docker-compose -f docker-compose.dev.yml stop
```

### **5. Restart Services**
```powershell
# Restart all services
.\dev.ps1 restart
# OR
docker-compose -f docker-compose.dev.yml restart

# Restart backend only
.\dev.ps1 restart-backend
# OR
docker-compose -f docker-compose.dev.yml restart backend

# Restart frontend only
.\dev.ps1 restart-frontend
# OR
docker-compose -f docker-compose.dev.yml restart frontend
```

### **6. Rebuild After Code Changes**
```powershell
# Rebuild backend (MOST COMMON during interview)
.\dev.ps1 rebuild-backend
# OR
docker-compose -f docker-compose.dev.yml up -d --build backend

# Rebuild all services
.\dev.ps1 rebuild
# OR
docker-compose -f docker-compose.dev.yml up -d --build

# Force rebuild (no cache)
docker-compose -f docker-compose.dev.yml build --no-cache backend
docker-compose -f docker-compose.dev.yml up -d backend
```

---

## 🔥 **HOT RELOAD FOR WINDOWS**

### **Frontend Changes (Automatic - No Rebuild)**
```powershell
# 1. Start dev environment
.\dev.ps1 start

# 2. Edit any file in: frontend\src\
# Example: frontend\src\components\employees\EmployeeList.tsx

# 3. Save the file (Ctrl+S)

# 4. Browser automatically refreshes in 1-2 seconds
# NO REBUILD NEEDED! ✅
```

### **Backend Changes (Requires Rebuild)**
```powershell
# 1. Edit file in: backend\EMS.API\
# Example: backend\EMS.API\Controllers\EmployeesController.cs

# 2. Rebuild backend
.\dev.ps1 rebuild-backend
# OR
docker-compose -f docker-compose.dev.yml up -d --build backend

# 3. Wait ~30 seconds for rebuild

# 4. Test your changes
curl http://localhost:5000/api/employees
# OR open http://localhost:5000/swagger
```

---

## 💻 **LIVE CODING SCENARIOS (WINDOWS)**

### **Scenario 1: Add New Frontend Component**
```powershell
# 1. Start environment
.\dev.ps1 start

# 2. Open VS Code or your editor
code frontend\src\components\

# 3. Create/edit component file

# 4. Save file - HOT RELOAD happens automatically!

# 5. Check browser - changes appear instantly

# 6. View logs if needed
.\dev.ps1 logs-frontend
```

### **Scenario 2: Add New Backend API Endpoint**
```powershell
# 1. Edit controller
code backend\EMS.API\Controllers\EmployeesController.cs

# 2. Add your new endpoint

# 3. Rebuild backend
.\dev.ps1 rebuild-backend

# 4. Check logs for errors
.\dev.ps1 logs-backend

# 5. Test endpoint in browser
# http://localhost:5000/swagger

# 6. Or test with curl
curl http://localhost:5000/api/employees
```

### **Scenario 3: Fix Error During Interview**
```powershell
# 1. Check logs to see error
.\dev.ps1 logs-backend

# 2. Fix the code

# 3. Rebuild
.\dev.ps1 rebuild-backend

# 4. Verify fix
.\dev.ps1 status

# 5. If still broken, restart
.\dev.ps1 restart
```

### **Scenario 4: Database Changes**
```powershell
# 1. Edit model
code backend\EMS.API\Models\Employee.cs

# 2. Access backend container
docker exec -it ems-backend-dev bash

# 3. Create migration (inside container)
dotnet ef migrations add YourMigrationName

# 4. Apply migration
dotnet ef database update

# 5. Exit container
exit

# 6. Rebuild backend
.\dev.ps1 rebuild-backend
```

---

## 🗄️ **DATABASE COMMANDS (WINDOWS)**

### **Access Database Shell**
```powershell
# Using script
.\dev.ps1 db-shell

# OR direct command
docker exec -it ems-mysql-dev mysql -u ems_user -pems_password123 EMS

# Inside MySQL shell, run:
SHOW TABLES;
SELECT COUNT(*) FROM Employees;
SELECT * FROM Employees LIMIT 10;
DESCRIBE Employees;
EXIT;
```

### **Seed Database with Test Data**
```powershell
# Using script (easiest)
.\dev.ps1 seed

# OR manually with curl
# 1. Get auth token
$response = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" -Method Post -ContentType "application/json" -Body '{"username":"admin","password":"admin123"}'
$token = $response.token

# 2. Seed database
$headers = @{"Authorization"="Bearer $token"}
Invoke-RestMethod -Uri "http://localhost:5000/api/seed/reseed" -Method Post -Headers $headers

# 3. Check status
Invoke-RestMethod -Uri "http://localhost:5000/api/seed/status" -Method Get -Headers $headers
```

---

## 🔧 **TROUBLESHOOTING (WINDOWS)**

### **Port Already in Use**
```powershell
# Check what's using port 3000
netstat -ano | findstr :3000

# Check port 5000
netstat -ano | findstr :5000

# Kill process by PID
taskkill /PID <PID_NUMBER> /F

# Example:
taskkill /PID 12345 /F
```

### **Docker Not Running**
```powershell
# Check if Docker Desktop is running
docker ps

# If error, start Docker Desktop from Start Menu
# Wait for Docker to fully start (whale icon in system tray)
```

### **Container Issues**
```powershell
# Check running containers
docker ps

# Check all containers (including stopped)
docker ps -a

# Restart specific container
docker restart ems-backend-dev
docker restart ems-frontend-dev
docker restart ems-mysql-dev

# Remove and recreate
docker-compose -f docker-compose.dev.yml up -d --force-recreate backend
```

### **Clean Start (Nuclear Option)**
```powershell
# Using script
.\dev.ps1 clean-all

# OR manually
docker-compose -f docker-compose.dev.yml down -v
docker system prune -f
docker-compose -f docker-compose.dev.yml up -d --build
```

### **Permission Issues**
```powershell
# Run PowerShell as Administrator
# Right-click PowerShell -> Run as Administrator

# Enable script execution (if needed)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Then run your commands
.\dev.ps1 start
```

---

## 🎯 **COMPLETE INTERVIEW WORKFLOW (WINDOWS)**

### **Before Interview (3:55 PM)**
```powershell
# 1. Open PowerShell in project directory
cd "D:\github issues\ems"

# 2. Start development environment
.\dev.ps1 start

# 3. Wait 60 seconds for everything to start

# 4. Open another PowerShell window for logs
.\dev.ps1 logs

# 5. Verify in browser
# Frontend: http://localhost:3000
# Backend: http://localhost:5000/swagger
# Login: admin / admin123

# 6. Keep both PowerShell windows open during interview
```

### **During Live Coding (4:00 PM - 5:00 PM)**

**For Frontend Changes:**
```powershell
# 1. Edit file in: frontend\src\
# 2. Save (Ctrl+S)
# 3. Browser auto-refreshes
# NO REBUILD NEEDED! ✅
```

**For Backend Changes:**
```powershell
# 1. Edit file in: backend\EMS.API\
# 2. Rebuild:
.\dev.ps1 rebuild-backend
# 3. Wait 30 seconds
# 4. Test changes
```

**Monitor Everything:**
```powershell
# Keep logs window open
.\dev.ps1 logs

# Or specific service
.\dev.ps1 logs-backend
```

### **After Interview (5:00 PM)**
```powershell
# Stop everything
.\dev.ps1 stop

# Or complete cleanup
.\dev.ps1 clean
```

---

## 📊 **QUICK REFERENCE (WINDOWS)**

| Task | PowerShell Script | Direct Command |
|------|-------------------|----------------|
| Start dev | `.\dev.ps1 start` | `docker-compose -f docker-compose.dev.yml up -d` |
| Stop dev | `.\dev.ps1 stop` | `docker-compose -f docker-compose.dev.yml down` |
| View logs | `.\dev.ps1 logs` | `docker-compose -f docker-compose.dev.yml logs -f` |
| Status | `.\dev.ps1 status` | `docker-compose -f docker-compose.dev.yml ps` |
| Rebuild backend | `.\dev.ps1 rebuild-backend` | `docker-compose -f docker-compose.dev.yml up -d --build backend` |
| Restart | `.\dev.ps1 restart` | `docker-compose -f docker-compose.dev.yml restart` |
| DB shell | `.\dev.ps1 db-shell` | `docker exec -it ems-mysql-dev mysql -u ems_user -pems_password123 EMS` |
| Seed DB | `.\dev.ps1 seed` | (See Database Commands section) |

---

## 🎯 **5 MOST IMPORTANT COMMANDS (MEMORIZE)**

```powershell
# 1. Start everything
.\dev.ps1 start

# 2. View logs
.\dev.ps1 logs

# 3. Rebuild backend after code changes
.\dev.ps1 rebuild-backend

# 4. Check status
.\dev.ps1 status

# 5. Stop everything
.\dev.ps1 stop
```

---

## ✅ **PRE-INTERVIEW CHECKLIST (WINDOWS)**

- [ ] Docker Desktop is running (check system tray)
- [ ] PowerShell is open in project directory: `D:\github issues\ems`
- [ ] Run: `.\dev.ps1 start`
- [ ] Wait 60 seconds
- [ ] Verify frontend: http://localhost:3000
- [ ] Verify backend: http://localhost:5000/swagger
- [ ] Test login: admin / admin123
- [ ] Open second PowerShell window for logs: `.\dev.ps1 logs`
- [ ] Have this cheat sheet open
- [ ] Have VS Code ready

---

## 🚀 **TEST NOW**

```powershell
# 1. Open PowerShell in project directory
cd "D:\github issues\ems"

# 2. Start dev environment
.\dev.ps1 start

# 3. Wait 60 seconds

# 4. Check status
.\dev.ps1 status

# 5. Open browser
# http://localhost:3000

# 6. When done testing
.\dev.ps1 stop
```

---

## 💡 **WINDOWS-SPECIFIC TIPS**

1. **Use PowerShell, not CMD** - Better support for Docker commands
2. **Run as Administrator** - If you get permission errors
3. **Keep Docker Desktop running** - Check system tray for whale icon
4. **Use `.\dev.ps1`** - Much easier than typing full docker-compose commands
5. **Two PowerShell windows** - One for commands, one for logs
6. **Ctrl+C to exit logs** - Don't close window, just stop following

---

## 🎬 **YOU'RE READY!**

**Frontend changes** = Just save file (hot reload) ✅
**Backend changes** = `.\dev.ps1 rebuild-backend` ✅
**View logs** = `.\dev.ps1 logs` ✅
**Fix errors** = Check logs, rebuild, restart ✅

**Good luck with your interview tomorrow! 🚀**
