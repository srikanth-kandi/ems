# Database Management Script for EMS API
# This script provides comprehensive database management including seeding and reseeding

param(
    [Parameter(Mandatory=$false)]
    [string]$Action = "help",
    
    [Parameter(Mandatory=$false)]
    [string]$ConnectionString = "",
    
    [Parameter(Mandatory=$false)]
    [switch]$Force
)

# Colors for output
$Red = "`e[31m"
$Green = "`e[32m"
$Yellow = "`e[33m"
$Blue = "`e[34m"
$Reset = "`e[0m"

function Write-ColorOutput {
    param([string]$Message, [string]$Color = $Reset)
    Write-Host "$Color$Message$Reset"
}

function Show-Help {
    Write-ColorOutput "EMS Database Management Script" $Blue
    Write-ColorOutput "==============================" $Blue
    Write-ColorOutput ""
    Write-ColorOutput "Usage: .\database-management.ps1 -Action <action> [options]" $Yellow
    Write-ColorOutput ""
    Write-ColorOutput "Actions:" $Yellow
    Write-ColorOutput "  migrate     - Apply database migrations" $Green
    Write-ColorOutput "  seed        - Seed database with initial data" $Green
    Write-ColorOutput "  reseed      - Clear and reseed database with fresh data" $Green
    Write-ColorOutput "  clear       - Clear all data from database" $Green
    Write-ColorOutput "  status      - Show current database status" $Green
    Write-ColorOutput "  reset       - Reset database (drop, recreate, migrate, seed)" $Green
    Write-ColorOutput "  help        - Show this help message" $Green
    Write-ColorOutput ""
    Write-ColorOutput "Options:" $Yellow
    Write-ColorOutput "  -ConnectionString <string> - Override connection string" $Green
    Write-ColorOutput "  -Force                    - Skip confirmation prompts" $Green
    Write-ColorOutput ""
    Write-ColorOutput "Examples:" $Yellow
    Write-ColorOutput "  .\database-management.ps1 -Action migrate" $Green
    Write-ColorOutput "  .\database-management.ps1 -Action reseed -Force" $Green
    Write-ColorOutput "  .\database-management.ps1 -Action status" $Green
}

function Invoke-Migration {
    Write-ColorOutput "Applying database migrations..." $Blue
    try {
        dotnet ef database update --project EMS.API
        if ($LASTEXITCODE -eq 0) {
            Write-ColorOutput "✓ Migrations applied successfully" $Green
        } else {
            Write-ColorOutput "✗ Migration failed" $Red
            exit 1
        }
    } catch {
        Write-ColorOutput "✗ Error applying migrations: $($_.Exception.Message)" $Red
        exit 1
    }
}

function Invoke-Seed {
    Write-ColorOutput "Seeding database with initial data..." $Blue
    try {
        $response = Invoke-RestMethod -Uri "http://localhost:5000/api/seed/seed" -Method POST -ContentType "application/json"
        Write-ColorOutput "✓ Database seeded successfully" $Green
        Write-ColorOutput "Response: $($response.message)" $Green
    } catch {
        Write-ColorOutput "✗ Error seeding database: $($_.Exception.Message)" $Red
        Write-ColorOutput "Make sure the API is running on http://localhost:5000" $Yellow
        exit 1
    }
}

function Invoke-Reseed {
    Write-ColorOutput "Reseeding database with fresh data..." $Blue
    try {
        $response = Invoke-RestMethod -Uri "http://localhost:5000/api/seed/reseed" -Method POST -ContentType "application/json"
        Write-ColorOutput "✓ Database reseeded successfully" $Green
        Write-ColorOutput "Response: $($response.message)" $Green
    } catch {
        Write-ColorOutput "✗ Error reseeding database: $($_.Exception.Message)" $Red
        Write-ColorOutput "Make sure the API is running on http://localhost:5000" $Yellow
        exit 1
    }
}

function Invoke-Clear {
    Write-ColorOutput "Clearing all data from database..." $Blue
    if (-not $Force) {
        $confirmation = Read-Host "Are you sure you want to clear all data? (y/N)"
        if ($confirmation -ne "y" -and $confirmation -ne "Y") {
            Write-ColorOutput "Operation cancelled" $Yellow
            return
        }
    }
    
    try {
        $response = Invoke-RestMethod -Uri "http://localhost:5000/api/seed/clear" -Method DELETE -ContentType "application/json"
        Write-ColorOutput "✓ Database cleared successfully" $Green
        Write-ColorOutput "Response: $($response.message)" $Green
    } catch {
        Write-ColorOutput "✗ Error clearing database: $($_.Exception.Message)" $Red
        Write-ColorOutput "Make sure the API is running on http://localhost:5000" $Yellow
        exit 1
    }
}

function Get-Status {
    Write-ColorOutput "Getting database status..." $Blue
    try {
        $response = Invoke-RestMethod -Uri "http://localhost:5000/api/seed/status" -Method GET
        Write-ColorOutput "Database Status:" $Green
        Write-ColorOutput "  Departments: $($response.departments)" $Green
        Write-ColorOutput "  Employees: $($response.employees)" $Green
        Write-ColorOutput "  Attendances: $($response.attendances)" $Green
        Write-ColorOutput "  Performance Metrics: $($response.performanceMetrics)" $Green
        Write-ColorOutput "  Users: $($response.users)" $Green
        Write-ColorOutput "  Timestamp: $($response.timestamp)" $Green
    } catch {
        Write-ColorOutput "✗ Error getting database status: $($_.Exception.Message)" $Red
        Write-ColorOutput "Make sure the API is running on http://localhost:5000" $Yellow
        exit 1
    }
}

function Invoke-Reset {
    Write-ColorOutput "Resetting database (drop, recreate, migrate, seed)..." $Blue
    if (-not $Force) {
        $confirmation = Read-Host "Are you sure you want to reset the database? This will delete ALL data! (y/N)"
        if ($confirmation -ne "y" -and $confirmation -ne "Y") {
            Write-ColorOutput "Operation cancelled" $Yellow
            return
        }
    }
    
    try {
        # Drop database
        Write-ColorOutput "Dropping database..." $Blue
        dotnet ef database drop --project EMS.API --force
        
        # Create and migrate
        Write-ColorOutput "Creating and migrating database..." $Blue
        dotnet ef database update --project EMS.API
        
        # Seed data
        Write-ColorOutput "Seeding database..." $Blue
        Start-Sleep -Seconds 2  # Wait for API to be ready
        $response = Invoke-RestMethod -Uri "http://localhost:5000/api/seed/seed" -Method POST -ContentType "application/json"
        
        Write-ColorOutput "✓ Database reset completed successfully" $Green
        Write-ColorOutput "Response: $($response.message)" $Green
    } catch {
        Write-ColorOutput "✗ Error resetting database: $($_.Exception.Message)" $Red
        exit 1
    }
}

# Main script logic
switch ($Action.ToLower()) {
    "migrate" {
        Invoke-Migration
    }
    "seed" {
        Invoke-Seed
    }
    "reseed" {
        Invoke-Reseed
    }
    "clear" {
        Invoke-Clear
    }
    "status" {
        Get-Status
    }
    "reset" {
        Invoke-Reset
    }
    "help" {
        Show-Help
    }
    default {
        Write-ColorOutput "Unknown action: $Action" $Red
        Write-ColorOutput "Use -Action help to see available actions" $Yellow
        exit 1
    }
}
