# EMS Development Environment - PowerShell Script
# Quick commands for Windows users

param(
    [Parameter(Position=0)]
    [string]$Command = "help"
)

function Show-Help {
    Write-Host "EMS Docker Management Commands (Windows)" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage: .\dev.ps1 [command]" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Development Commands:" -ForegroundColor Green
    Write-Host "  .\dev.ps1 start        - Start development environment with hot reload"
    Write-Host "  .\dev.ps1 stop         - Stop development environment"
    Write-Host "  .\dev.ps1 restart      - Restart all services"
    Write-Host "  .\dev.ps1 logs         - Show all logs (follow mode)"
    Write-Host "  .\dev.ps1 status       - Show service status"
    Write-Host ""
    Write-Host "Service-Specific Commands:" -ForegroundColor Green
    Write-Host "  .\dev.ps1 logs-backend  - Show backend logs"
    Write-Host "  .\dev.ps1 logs-frontend - Show frontend logs"
    Write-Host "  .\dev.ps1 logs-db       - Show database logs"
    Write-Host "  .\dev.ps1 restart-backend  - Restart backend only"
    Write-Host "  .\dev.ps1 restart-frontend - Restart frontend only"
    Write-Host ""
    Write-Host "Build Commands:" -ForegroundColor Green
    Write-Host "  .\dev.ps1 rebuild       - Rebuild and restart all services"
    Write-Host "  .\dev.ps1 rebuild-backend  - Rebuild backend only"
    Write-Host ""
    Write-Host "Database Commands:" -ForegroundColor Green
    Write-Host "  .\dev.ps1 db-shell      - Access database shell"
    Write-Host "  .\dev.ps1 seed          - Seed database with test data"
    Write-Host ""
    Write-Host "Cleanup Commands:" -ForegroundColor Green
    Write-Host "  .\dev.ps1 clean         - Stop and remove containers"
    Write-Host "  .\dev.ps1 clean-all     - Remove everything including volumes"
    Write-Host ""
    Write-Host "Quick Access:" -ForegroundColor Green
    Write-Host "  Frontend: http://localhost:3000"
    Write-Host "  Backend:  http://localhost:5000/swagger"
    Write-Host "  Login:    admin / admin123"
}

function Start-Dev {
    Write-Host "Starting EMS Development Environment..." -ForegroundColor Green
    docker-compose -f docker-compose.dev.yml up -d
    Write-Host ""
    Write-Host "Services starting... Please wait 30 seconds" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Frontend: http://localhost:3000" -ForegroundColor Cyan
    Write-Host "Backend:  http://localhost:5000/swagger" -ForegroundColor Cyan
    Write-Host "Login:    admin / admin123" -ForegroundColor Cyan
}

function Stop-Dev {
    Write-Host "Stopping EMS Development Environment..." -ForegroundColor Yellow
    docker-compose -f docker-compose.dev.yml down
}

function Restart-Dev {
    Write-Host "Restarting all services..." -ForegroundColor Yellow
    docker-compose -f docker-compose.dev.yml restart
}

function Show-Logs {
    Write-Host "Showing all logs (Press Ctrl+C to exit)..." -ForegroundColor Cyan
    docker-compose -f docker-compose.dev.yml logs -f
}

function Show-Status {
    Write-Host "Service Status:" -ForegroundColor Cyan
    docker-compose -f docker-compose.dev.yml ps
}

function Show-BackendLogs {
    Write-Host "Showing backend logs (Press Ctrl+C to exit)..." -ForegroundColor Cyan
    docker-compose -f docker-compose.dev.yml logs -f backend
}

function Show-FrontendLogs {
    Write-Host "Showing frontend logs (Press Ctrl+C to exit)..." -ForegroundColor Cyan
    docker-compose -f docker-compose.dev.yml logs -f frontend
}

function Show-DbLogs {
    Write-Host "Showing database logs (Press Ctrl+C to exit)..." -ForegroundColor Cyan
    docker-compose -f docker-compose.dev.yml logs -f mysql
}

function Restart-Backend {
    Write-Host "Restarting backend service..." -ForegroundColor Yellow
    docker-compose -f docker-compose.dev.yml restart backend
}

function Restart-Frontend {
    Write-Host "Restarting frontend service..." -ForegroundColor Yellow
    docker-compose -f docker-compose.dev.yml restart frontend
}

function Rebuild-All {
    Write-Host "Rebuilding all services..." -ForegroundColor Yellow
    docker-compose -f docker-compose.dev.yml up -d --build
}

function Rebuild-Backend {
    Write-Host "Rebuilding backend service..." -ForegroundColor Yellow
    docker-compose -f docker-compose.dev.yml up -d --build backend
    Write-Host "Backend rebuilt successfully!" -ForegroundColor Green
}

function Access-DbShell {
    Write-Host "Accessing database shell..." -ForegroundColor Cyan
    Write-Host "Password: ems_password123" -ForegroundColor Yellow
    docker exec -it ems-mysql-dev mysql -u ems_user -pems_password123 EMS
}

function Seed-Database {
    Write-Host "Seeding database with test data..." -ForegroundColor Yellow
    Write-Host "Getting auth token..." -ForegroundColor Cyan
    
    $loginResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" -Method Post -ContentType "application/json" -Body '{"username":"admin","password":"admin123"}'
    $token = $loginResponse.token
    
    if ($token) {
        Write-Host "Seeding database..." -ForegroundColor Cyan
        $headers = @{
            "Authorization" = "Bearer $token"
            "Content-Type" = "application/json"
        }
        Invoke-RestMethod -Uri "http://localhost:5000/api/seed/reseed" -Method Post -Headers $headers
        Write-Host "Database seeded successfully!" -ForegroundColor Green
    } else {
        Write-Host "Failed to get auth token. Make sure backend is running." -ForegroundColor Red
    }
}

function Clean-Dev {
    Write-Host "Cleaning up containers and networks..." -ForegroundColor Yellow
    docker-compose -f docker-compose.dev.yml down
    Write-Host "Cleanup complete!" -ForegroundColor Green
}

function Clean-All {
    Write-Host "Removing everything including volumes..." -ForegroundColor Red
    docker-compose -f docker-compose.dev.yml down -v
    Write-Host "Complete cleanup done!" -ForegroundColor Green
}

# Command routing
switch ($Command.ToLower()) {
    "start" { Start-Dev }
    "stop" { Stop-Dev }
    "restart" { Restart-Dev }
    "logs" { Show-Logs }
    "status" { Show-Status }
    "logs-backend" { Show-BackendLogs }
    "logs-frontend" { Show-FrontendLogs }
    "logs-db" { Show-DbLogs }
    "restart-backend" { Restart-Backend }
    "restart-frontend" { Restart-Frontend }
    "rebuild" { Rebuild-All }
    "rebuild-backend" { Rebuild-Backend }
    "db-shell" { Access-DbShell }
    "seed" { Seed-Database }
    "clean" { Clean-Dev }
    "clean-all" { Clean-All }
    "help" { Show-Help }
    default { 
        Write-Host "Unknown command: $Command" -ForegroundColor Red
        Write-Host ""
        Show-Help 
    }
}
