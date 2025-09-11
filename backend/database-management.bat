@echo off
REM Database Management Script for EMS API
REM This script provides comprehensive database management including seeding and reseeding

setlocal enabledelayedexpansion

if "%1"=="" goto :help
if "%1"=="help" goto :help
if "%1"=="migrate" goto :migrate
if "%1"=="seed" goto :seed
if "%1"=="reseed" goto :reseed
if "%1"=="clear" goto :clear
if "%1"=="status" goto :status
if "%1"=="reset" goto :reset

echo Unknown action: %1
echo Use "database-management.bat help" to see available actions
exit /b 1

:help
echo EMS Database Management Script
echo ==============================
echo.
echo Usage: database-management.bat ^<action^> [options]
echo.
echo Actions:
echo   migrate     - Apply database migrations
echo   seed        - Seed database with initial data
echo   reseed      - Clear and reseed database with fresh data
echo   clear       - Clear all data from database
echo   status      - Show current database status
echo   reset       - Reset database (drop, recreate, migrate, seed)
echo   help        - Show this help message
echo.
echo Examples:
echo   database-management.bat migrate
echo   database-management.bat reseed
echo   database-management.bat status
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
curl -X POST http://localhost:5000/api/seed/seed -H "Content-Type: application/json"
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

:clear
echo Clearing all data from database...
set /p confirmation="Are you sure you want to clear all data? (y/N): "
if /i not "%confirmation%"=="y" (
    echo Operation cancelled
    goto :eof
)
curl -X DELETE http://localhost:5000/api/seed/clear -H "Content-Type: application/json"
if %errorlevel% neq 0 (
    echo Error clearing database. Make sure the API is running on http://localhost:5000
    exit /b 1
)
echo Database cleared successfully
goto :eof

:status
echo Getting database status...
curl -X GET http://localhost:5000/api/seed/status -H "Content-Type: application/json"
if %errorlevel% neq 0 (
    echo Error getting database status. Make sure the API is running on http://localhost:5000
    exit /b 1
)
goto :eof

:reset
echo Resetting database (drop, recreate, migrate, seed)...
set /p confirmation="Are you sure you want to reset the database? This will delete ALL data! (y/N): "
if /i not "%confirmation%"=="y" (
    echo Operation cancelled
    goto :eof
)

echo Dropping database...
cd EMS.API
dotnet ef database drop --force
if %errorlevel% neq 0 (
    echo Error dropping database
    exit /b 1
)

echo Creating and migrating database...
dotnet ef database update
if %errorlevel% neq 0 (
    echo Error creating/migrating database
    exit /b 1
)

echo Seeding database...
timeout /t 2 /nobreak >nul
curl -X POST http://localhost:5000/api/seed/seed -H "Content-Type: application/json"
if %errorlevel% neq 0 (
    echo Error seeding database. Make sure the API is running on http://localhost:5000
    exit /b 1
)

echo Database reset completed successfully
goto :eof
