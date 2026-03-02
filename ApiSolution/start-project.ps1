# Сценарий запуска проекта ApiSolution
# Запускает API и Blazor, затем открывает вкладки в Chrome

Write-Host "=== Запуск ApiSolution ===" -ForegroundColor Green

# Останавливаем существующие процессы (если есть)
Write-Host "Остановка существующих процессов..." -ForegroundColor Yellow
Get-Process -Name "TestApi3K" -ErrorAction SilentlyContinue | Stop-Process -Force
Get-Process -Name "TestBlazorAssembly" -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 1

# Собираем проекты (по отдельности, т.к. TestBlazor3K отсутствует в решении)
Write-Host "Сборка проекта..." -ForegroundColor Yellow
dotnet build "$PSScriptRoot\TestApi3K\TestApi3K.csproj"
if ($LASTEXITCODE -ne 0) {
    Write-Host "Ошибка сборки API!" -ForegroundColor Red
    exit 1
}

dotnet build "$PSScriptRoot\TestBlazorAssembly\TestBlazorAssembly.csproj"
if ($LASTEXITCODE -ne 0) {
    Write-Host "Ошибка сборки Blazor!" -ForegroundColor Red
    exit 1
}

# Запускаем API (порт 5039)
Write-Host "Запуск API (порт 5039)..." -ForegroundColor Cyan
Start-Process dotnet -ArgumentList "run", "--project", "TestApi3K/TestApi3K.csproj", "--no-build", "--launch-profile", "http" -WindowStyle Normal
$apiProcess = $true

# Ждём пока API запустится
Write-Host "Ожидание запуска API..." -ForegroundColor Gray
Start-Sleep -Seconds 5

# Проверяем доступность API
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5039/swagger" -TimeoutSec 3 -UseBasicParsing
    Write-Host "API доступен!" -ForegroundColor Green
} catch {
    Write-Host "API ещё не готов, продолжаем..." -ForegroundColor Gray
}

# Запускаем Blazor (порт 5156)
Write-Host "Запуск Blazor (порт 5156)..." -ForegroundColor Cyan
Start-Process dotnet -ArgumentList "run", "--project", "TestBlazorAssembly/TestBlazorAssembly.csproj", "--no-build", "--launch-profile", "http" -WindowStyle Normal
$blazorProcess = $true

# Ждём пока Blazor запустится
Write-Host "Ожидание запуска Blazor..." -ForegroundColor Gray
Start-Sleep -Seconds 5

# Открываем вкладки в Chrome
Write-Host "Открытие вкладок в Chrome..." -ForegroundColor Green
Start-Process "chrome" -ArgumentList "http://localhost:5039/swagger"
Start-Sleep -Milliseconds 500
Start-Process "chrome" -ArgumentList "http://localhost:5156"

Write-Host "`n=== Готово! ===" -ForegroundColor Green
Write-Host "API Swagger: http://localhost:5039/swagger" -ForegroundColor Cyan
Write-Host "Blazor App:   http://localhost:5156" -ForegroundColor Cyan
Write-Host "`nДля остановки нажмите Ctrl+C и закройте процессы dotnet" -ForegroundColor Yellow
