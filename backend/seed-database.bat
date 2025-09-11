@echo off
REM Simple Database Seeding Script for EMS API

if "%1"=="" goto :help
if "%1"=="help" goto :help
if "%1"=="migrate" goto :migrate
if "%1"=="seed" goto :seed
if "%1"=="reseed" goto :reseed
if "%1"=="status" goto :status

echo Unknown action: %1
echo Use "seed-database.bat help" to see available actions
exit /b 1

:help
echo EMS Database Management Script
echo ==============================
echo.
echo Usage: seed-database.bat ^<action^>
echo.
echo Actions:
echo   migrate     - Apply database migrations
echo   seed        - Seed database with initial data
echo   reseed      - Clear and reseed database with fresh data
echo   status      - Show current database status
echo.
echo Examples:
echo   seed-database.bat migrate
echo   seed-database.bat reseed
echo   seed-database.bat status
goto :eof

:migrate
echo Applying database migrations...
cd EMS.API
dotnet ef database update
if %errorlevel% neq 0 (
    echo Migration failed
    exit /b 1
)
echo Migrations applied successfully
goto :eof

:seed
echo Seeding database with initial data...
echo Getting authentication token...
for /f "tokens=*" %%i in ('curl -s -X POST http://localhost:5000/api/auth/login -H "Content-Type: application/json" -d "{\"username\":\"admin\",\"password\":\"admin123\"}"') do set LOGIN_RESPONSE=%%i
echo %LOGIN_RESPONSE%
echo Seeding database...
curl -X POST http://localhost:5000/api/seed/seed -H "Content-Type: application/json" -H "Authorization: Bearer %TOKEN%"
if %errorlevel% neq 0 (
    echo Error seeding database. Make sure the API is running on http://localhost:5000
    exit /b 1
)
echo Database seeded successfully
goto :eof

:reseed
echo Reseeding database with fresh data...
curl -X POST http://localhost:5000/api/seed/reseed -H "Content-Type: application/json"
if %errorlevel% neq 0 (
    echo Error reseeding database. Make sure the API is running on http://localhost:5000
    exit /b 1
)
echo Database reseeded successfully
goto :eof

:status
echo Getting database status...
curl -X GET http://localhost:5000/api/seed/status -H "Content-Type: application/json"
if %errorlevel% neq 0 (
    echo Error getting database status. Make sure the API is running on http://localhost:5000
    exit /b 1
)
goto :eof
