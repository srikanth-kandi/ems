# Docker Commands Cheat Sheet for Live Coding Interview
## EMS Development Environment - Quick Reference

---

## 🚀 **QUICK START - DEVELOPMENT WITH HOT RELOAD**

### **Start Development Environment (Recommended for Interview)**
```bash
# Option 1: Using Makefile (Easiest)
make dev

# Option 2: Using docker-compose directly
docker-compose -f docker-compose.dev.yml up -d

# Option 3: With database seeding (200+ employees)
docker-compose -f docker-compose.dev.yml -f docker-compose.dev-seed.yml up -d
```

**What this does:**
- ✅ Starts MySQL database
- ✅ Starts backend API (ASP.NET) on http://localhost:5000
- ✅ Starts frontend (React) on http://localhost:3000 with **HOT RELOAD**
- ✅ Frontend auto-reloads when you change code
- ✅ Backend requires rebuild for changes (see below)

---

## 📋 **ESSENTIAL COMMANDS FOR INTERVIEW**

### **1. View Logs (Monitor What's Happening)**
```bash
# View all logs (follow mode - shows real-time updates)
docker-compose -f docker-compose.dev.yml logs -f

# View specific service logs
docker-compose -f docker-compose.dev.yml logs -f frontend
docker-compose -f docker-compose.dev.yml logs -f backend
docker-compose -f docker-compose.dev.yml logs -f mysql

# View last 100 lines
docker-compose -f docker-compose.dev.yml logs --tail=100

# Exit logs: Press Ctrl+C
```

### **2. Stop Services**
```bash
# Stop all services (keeps containers)
docker-compose -f docker-compose.dev.yml stop

# Stop and remove containers
docker-compose -f docker-compose.dev.yml down

# Using Makefile
make dev-down
```

### **3. Restart Services**
```bash
# Restart all services
docker-compose -f docker-compose.dev.yml restart

# Restart specific service
docker-compose -f docker-compose.dev.yml restart backend
docker-compose -f docker-compose.dev.yml restart frontend
docker-compose -f docker-compose.dev.yml restart mysql
```

### **4. Check Service Status**
```bash
# View running containers
docker-compose -f docker-compose.dev.yml ps

# View detailed status
docker ps

# Check health status
docker inspect ems-backend-dev | grep -A 10 Health
```

### **5. Rebuild After Code Changes**
```bash
# Rebuild and restart (when you change backend code)
docker-compose -f docker-compose.dev.yml up -d --build

# Rebuild specific service
docker-compose -f docker-compose.dev.yml up -d --build backend

# Force rebuild (no cache)
docker-compose -f docker-compose.dev.yml build --no-cache backend
docker-compose -f docker-compose.dev.yml up -d backend
```

---

## 🔥 **HOT RELOAD EXPLAINED**

### **Frontend Hot Reload (Automatic)**
- **Location**: `frontend/src/` directory
- **How it works**: Changes are detected automatically
- **No rebuild needed**: Just save your file
- **Refresh browser**: Changes appear in 1-2 seconds

**Example:**
```bash
# 1. Start dev environment
make dev

# 2. Edit any file in frontend/src/
# Example: frontend/src/components/employees/EmployeeList.tsx

# 3. Save the file
# 4. Browser automatically refreshes - NO REBUILD NEEDED!
```

### **Backend Changes (Requires Rebuild)**
- **Location**: `backend/EMS.API/` directory
- **How it works**: Need to rebuild Docker image
- **Rebuild command**: See section below

**Example:**
```bash
# 1. Edit backend file
# Example: backend/EMS.API/Controllers/EmployeesController.cs

# 2. Rebuild and restart backend
docker-compose -f docker-compose.dev.yml up -d --build backend

# 3. Wait ~30 seconds for rebuild
# 4. Test your changes
```

---

## 💻 **LIVE CODING SCENARIOS**

### **Scenario 1: Add New Frontend Feature**
```bash
# 1. Start dev environment
make dev

# 2. Edit frontend file
# Example: Add new component in frontend/src/components/

# 3. Save file - HOT RELOAD happens automatically

# 4. View logs if needed
docker-compose -f docker-compose.dev.yml logs -f frontend
```

### **Scenario 2: Add New Backend API Endpoint**
```bash
# 1. Edit backend controller
# Example: backend/EMS.API/Controllers/EmployeesController.cs

# 2. Rebuild backend
docker-compose -f docker-compose.dev.yml up -d --build backend

# 3. Check logs for errors
docker-compose -f docker-compose.dev.yml logs -f backend

# 4. Test endpoint
curl http://localhost:5000/api/employees
```

### **Scenario 3: Database Changes**
```bash
# 1. Edit model or add migration
# Example: backend/EMS.API/Models/Employee.cs

# 2. Access backend container
docker exec -it ems-backend-dev bash

# 3. Create migration
dotnet ef migrations add YourMigrationName

# 4. Apply migration
dotnet ef database update

# 5. Exit container
exit

# 6. Rebuild backend
docker-compose -f docker-compose.dev.yml up -d --build backend
```

### **Scenario 4: Fix Errors During Interview**
```bash
# 1. Check logs for error details
docker-compose -f docker-compose.dev.yml logs -f backend

# 2. If backend crashed, restart it
docker-compose -f docker-compose.dev.yml restart backend

# 3. If database issue, restart database
docker-compose -f docker-compose.dev.yml restart mysql

# 4. If everything broken, restart all
docker-compose -f docker-compose.dev.yml restart
```

---

## 🗄️ **DATABASE COMMANDS**

### **Access Database Shell**
```bash
# Using docker exec
docker exec -it ems-mysql-dev mysql -u ems_user -pems_password123 EMS

# Inside MySQL shell
SHOW TABLES;
SELECT COUNT(*) FROM Employees;
SELECT * FROM Employees LIMIT 10;
DESCRIBE Employees;
EXIT;
```

### **Database Seeding**
```bash
# Seed database with 200+ employees
curl -X POST http://localhost:5000/api/seed/seed \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN"

# Reseed (clear and seed fresh)
curl -X POST http://localhost:5000/api/seed/reseed \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN"

# Check database status
curl -X GET http://localhost:5000/api/seed/status \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### **Get Auth Token**
```bash
# Login to get token
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'

# Response will contain token - copy it for other commands
```

---

## 🔧 **TROUBLESHOOTING COMMANDS**

### **Container Issues**
```bash
# Check if containers are running
docker ps

# Check all containers (including stopped)
docker ps -a

# Restart specific container
docker restart ems-backend-dev
docker restart ems-frontend-dev
docker restart ems-mysql-dev

# Remove and recreate container
docker-compose -f docker-compose.dev.yml up -d --force-recreate backend
```

### **Port Conflicts**
```bash
# Check what's using port 3000
netstat -ano | findstr :3000  # Windows
lsof -i :3000                 # Mac/Linux

# Kill process on port (Windows)
taskkill /PID <PID> /F

# Kill process on port (Mac/Linux)
kill -9 <PID>
```

### **Clean Start (Nuclear Option)**
```bash
# Stop everything
docker-compose -f docker-compose.dev.yml down

# Remove volumes (deletes database data)
docker-compose -f docker-compose.dev.yml down -v

# Remove all images
docker-compose -f docker-compose.dev.yml down --rmi all

# Clean Docker system
docker system prune -f

# Start fresh
docker-compose -f docker-compose.dev.yml up -d --build
```

### **View Container Details**
```bash
# Inspect container
docker inspect ems-backend-dev

# Check container resource usage
docker stats

# View container processes
docker top ems-backend-dev
```

---

## 🎯 **INTERVIEW-SPECIFIC COMMANDS**

### **Quick Demo Setup**
```bash
# 1. Start everything with seeded data
docker-compose -f docker-compose.dev.yml -f docker-compose.dev-seed.yml up -d

# 2. Wait 60 seconds for everything to start

# 3. Open browser
# Frontend: http://localhost:3000
# Backend API: http://localhost:5000/swagger
# Login: admin / admin123

# 4. Show logs in separate terminal
docker-compose -f docker-compose.dev.yml logs -f
```

### **Quick Code Change Demo**
```bash
# 1. Show current running state
docker-compose -f docker-compose.dev.yml ps

# 2. Make frontend change (auto hot-reload)
# Edit: frontend/src/components/dashboard/Dashboard.tsx

# 3. Show logs to demonstrate hot reload
docker-compose -f docker-compose.dev.yml logs -f frontend

# 4. Make backend change (requires rebuild)
# Edit: backend/EMS.API/Controllers/EmployeesController.cs

# 5. Rebuild backend
docker-compose -f docker-compose.dev.yml up -d --build backend

# 6. Show it's working
curl http://localhost:5000/api/employees
```

### **Access Running Containers**
```bash
# Access backend container shell
docker exec -it ems-backend-dev bash

# Access frontend container shell
docker exec -it ems-frontend-dev sh

# Access database container shell
docker exec -it ems-mysql-dev bash

# Run commands inside container without entering
docker exec ems-backend-dev dotnet --version
docker exec ems-frontend-dev node --version
```

---

## 📊 **HEALTH CHECK COMMANDS**

### **Check API Health**
```bash
# Health endpoint
curl http://localhost:5000/health

# Check if API is responding
curl http://localhost:5000/api/employees

# Check Swagger documentation
# Open: http://localhost:5000/swagger
```

### **Check Frontend**
```bash
# Check if frontend is running
curl http://localhost:3000

# Open in browser
# http://localhost:3000
```

### **Check Database Connection**
```bash
# Test database connection
docker exec ems-mysql-dev mysqladmin ping -h localhost -u ems_user -pems_password123

# Check database size
docker exec ems-mysql-dev mysql -u ems_user -pems_password123 -e "SELECT table_schema AS 'Database', ROUND(SUM(data_length + index_length) / 1024 / 1024, 2) AS 'Size (MB)' FROM information_schema.tables WHERE table_schema = 'EMS' GROUP BY table_schema;"
```

---

## 🎬 **COMPLETE WORKFLOW FOR INTERVIEW**

### **Before Interview Starts**
```bash
# 1. Start development environment
make dev

# 2. Verify everything is running
docker-compose -f docker-compose.dev.yml ps

# 3. Check logs for errors
docker-compose -f docker-compose.dev.yml logs --tail=50

# 4. Test frontend: http://localhost:3000
# 5. Test backend: http://localhost:5000/swagger
# 6. Login with: admin / admin123
```

### **During Live Coding**
```bash
# Frontend changes (automatic hot reload)
# 1. Edit file in frontend/src/
# 2. Save
# 3. Browser auto-refreshes

# Backend changes (requires rebuild)
# 1. Edit file in backend/EMS.API/
# 2. Run: docker-compose -f docker-compose.dev.yml up -d --build backend
# 3. Wait 30 seconds
# 4. Test changes

# View logs in separate terminal
docker-compose -f docker-compose.dev.yml logs -f
```

### **After Interview / Cleanup**
```bash
# Stop services
make dev-down

# Or complete cleanup
docker-compose -f docker-compose.dev.yml down -v
```

---

## 🔑 **KEY POINTS FOR INTERVIEW**

1. **Frontend has HOT RELOAD** - No rebuild needed for React changes
2. **Backend needs rebuild** - Use `up -d --build backend` after changes
3. **Always check logs** - Use `logs -f` to see what's happening
4. **Database persists** - Data survives container restarts
5. **Port 3000** - Frontend, **Port 5000** - Backend, **Port 3306** - MySQL

---

## 📝 **QUICK REFERENCE TABLE**

| Task | Command |
|------|---------|
| Start dev environment | `make dev` |
| Stop dev environment | `make dev-down` |
| View logs | `docker-compose -f docker-compose.dev.yml logs -f` |
| Restart all | `docker-compose -f docker-compose.dev.yml restart` |
| Rebuild backend | `docker-compose -f docker-compose.dev.yml up -d --build backend` |
| Check status | `docker-compose -f docker-compose.dev.yml ps` |
| Access database | `docker exec -it ems-mysql-dev mysql -u ems_user -pems_password123 EMS` |
| Clean restart | `docker-compose -f docker-compose.dev.yml down && make dev` |

---

## 🎯 **MEMORIZE THESE 5 COMMANDS**

```bash
# 1. Start everything
make dev

# 2. View logs
docker-compose -f docker-compose.dev.yml logs -f

# 3. Rebuild backend after changes
docker-compose -f docker-compose.dev.yml up -d --build backend

# 4. Restart service
docker-compose -f docker-compose.dev.yml restart backend

# 5. Stop everything
make dev-down
```

---

## ✅ **PRE-INTERVIEW CHECKLIST**

- [ ] Docker Desktop is running
- [ ] Run `make dev` to start environment
- [ ] Verify frontend: http://localhost:3000
- [ ] Verify backend: http://localhost:5000/swagger
- [ ] Test login: admin / admin123
- [ ] Have this cheat sheet open in separate window
- [ ] Have logs running in separate terminal
- [ ] Know how to rebuild backend quickly

**You're ready for live coding! Good luck! 🚀**
