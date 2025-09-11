@echo off
echo EMS Database Management Commands
echo ================================

if "%1"=="" (
    echo Usage: db-commands.bat [command]
    echo.
    echo Available commands:
    echo   migrate  - Apply database migrations
    echo   seed     - Seed the database with sample data
    echo   reseed   - Clear and reseed the database with fresh data
    echo   reset    - Drop, recreate, and seed the database
    echo   drop     - Drop the database
    echo   setup    - Complete database setup (migrate + seed)
    echo.
    pause
    exit /b 1
)

echo Running command: %1
echo.

dotnet run --project EMS.API -- db %*

if %ERRORLEVEL% neq 0 (
    echo.
    echo Command failed with error code %ERRORLEVEL%
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo Command completed successfully!
pause
