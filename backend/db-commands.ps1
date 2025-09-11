param(
    [Parameter(Position=0)]
    [string]$Command
)

Write-Host "EMS Database Management Commands" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green

if ([string]::IsNullOrEmpty($Command)) {
    Write-Host ""
    Write-Host "Usage: .\db-commands.ps1 [command]" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Available commands:" -ForegroundColor Cyan
    Write-Host "  migrate  - Apply database migrations" -ForegroundColor White
    Write-Host "  seed     - Seed the database with sample data" -ForegroundColor White
    Write-Host "  reseed   - Clear and reseed the database with fresh data" -ForegroundColor White
    Write-Host "  reset    - Drop, recreate, and seed the database" -ForegroundColor White
    Write-Host "  drop     - Drop the database" -ForegroundColor White
    Write-Host "  setup    - Complete database setup (migrate + seed)" -ForegroundColor White
    Write-Host ""
    Read-Host "Press Enter to continue"
    exit 1
}

Write-Host "Running command: $Command" -ForegroundColor Yellow
Write-Host ""

try {
    dotnet run --project EMS.API -- db $Command
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-Host "Command failed with error code $LASTEXITCODE" -ForegroundColor Red
        Read-Host "Press Enter to continue"
        exit $LASTEXITCODE
    }
    
    Write-Host ""
    Write-Host "Command completed successfully!" -ForegroundColor Green
    Read-Host "Press Enter to continue"
}
catch {
    Write-Host ""
    Write-Host "Error occurred: $($_.Exception.Message)" -ForegroundColor Red
    Read-Host "Press Enter to continue"
    exit 1
}
