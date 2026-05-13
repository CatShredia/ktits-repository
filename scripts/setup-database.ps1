#Requires -Version 5.1
<#
.SYNOPSIS
  Обновляет схему БД, импортирует данные из Excel/CSV, применяет seed-erd-test-data.sql и выводит счётчики таблиц.

.DESCRIPTION
  1. dotnet ef database update (Data + Api)
  2. dotnet run --project ProductionSystem.Import -- --force
  3. psql: scripts/seed-erd-test-data.sql
  4. Итоговые COUNT(*) по основным таблицам

  Строка подключения читается из src/ProductionSystem.Api/appsettings.json → ConnectionStrings:Default
  Нужен psql в PATH (клиент PostgreSQL).
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

function Get-RepoRoot {
    $scriptDir = $PSScriptRoot
    if (-not $scriptDir) { $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path }
    return (Resolve-Path (Join-Path $scriptDir '..')).Path
}

function Read-NpgsqlConnectionMap {
    param([string]$JsonPath)
    if (-not (Test-Path -LiteralPath $JsonPath)) {
        throw "Файл не найден: $JsonPath"
    }
    $j = Get-Content -LiteralPath $JsonPath -Raw -Encoding UTF8 | ConvertFrom-Json
    $cs = $j.ConnectionStrings.Default
    if ([string]::IsNullOrWhiteSpace($cs)) {
        throw "В $JsonPath не задан ConnectionStrings.Default"
    }
    $map = @{}
    foreach ($part in ($cs -split ';')) {
        $p = $part.Trim()
        if (-not $p) { continue }
        $eq = $p.IndexOf('=')
        if ($eq -lt 1) { continue }
        $key = $p.Substring(0, $eq).Trim().ToLowerInvariant()
        $val = $p.Substring($eq + 1).Trim()
        $map[$key] = $val
    }
    $hostName = $map['host']
    if (-not $hostName) { $hostName = $map['server'] }
    $db = $map['database']
    if (-not $db) { $db = $map['dbname'] }
    $user = $map['username']
    if (-not $user) { $user = $map['user id'] }
    if (-not $user) { $user = $map['userid'] }
    $pass = $map['password']
    $port = $map['port']
    if (-not $port) { $port = '5432' }
    if (-not $hostName -or -not $db -or -not $user) {
        throw "Не удалось разобрать строку подключения (нужны Host/Server, Database, Username/User Id): $JsonPath"
    }
    return @{
        Host     = $hostName
        Port     = $port
        Database = $db
        User     = $user
        Password = $pass
    }
}

function Invoke-PsqlUtf8Text {
    param(
        [hashtable]$Conn,
        [string]$SqlText
    )
    $writeEnc = New-Object System.Text.UTF8Encoding $false
    $tmp = [System.IO.Path]::Combine(
        [System.IO.Path]::GetTempPath(),
        'psql-' + [Guid]::NewGuid().ToString('n') + '.sql'
    )
    try {
        [System.IO.File]::WriteAllText($tmp, $SqlText, $writeEnc)
        $savedEnc = $env:PGCLIENTENCODING
        $env:PGCLIENTENCODING = 'UTF8'
        try {
            & psql -h $Conn.Host -p $Conn.Port -U $Conn.User -d $Conn.Database -v ON_ERROR_STOP=1 -f $tmp
            if ($LASTEXITCODE -ne 0) {
                throw "psql завершился с кодом $LASTEXITCODE"
            }
        }
        finally {
            if ($null -ne $savedEnc) { $env:PGCLIENTENCODING = $savedEnc }
            else { Remove-Item Env:PGCLIENTENCODING -ErrorAction SilentlyContinue }
        }
    }
    finally {
        Remove-Item -LiteralPath $tmp -ErrorAction SilentlyContinue
    }
}

function Invoke-PsqlUtf8File {
    param(
        [hashtable]$Conn,
        [string]$FilePath
    )
    $readEnc = New-Object System.Text.UTF8Encoding $true
    $text = [System.IO.File]::ReadAllText($FilePath, $readEnc)
    Invoke-PsqlUtf8Text -Conn $Conn -SqlText $text
}

$repoRoot = Get-RepoRoot
$dataProj = Join-Path $repoRoot 'src\ProductionSystem.Data\ProductionSystem.Data.csproj'
$apiProj = Join-Path $repoRoot 'src\ProductionSystem.Api\ProductionSystem.Api.csproj'
$importProj = Join-Path $repoRoot 'src\ProductionSystem.Import\ProductionSystem.Import.csproj'
$appsettings = Join-Path $repoRoot 'src\ProductionSystem.Api\appsettings.json'
$seedSql = Join-Path $repoRoot 'scripts\seed-erd-test-data.sql'

foreach ($p in @($dataProj, $apiProj, $importProj, $appsettings, $seedSql)) {
    if (-not (Test-Path -LiteralPath $p)) { throw "Не найден путь: $p" }
}

$psql = Get-Command psql -ErrorAction SilentlyContinue
if (-not $psql) {
    throw "Команда psql не найдена в PATH. Установите клиент PostgreSQL и добавьте каталог bin в PATH."
}

$conn = Read-NpgsqlConnectionMap -JsonPath $appsettings

Write-Host "Корень репозитория: $repoRoot" -ForegroundColor Cyan
Write-Host "Строка подключения: $appsettings" -ForegroundColor DarkGray

Push-Location $repoRoot
try {
    Write-Host "`n[1/4] dotnet ef database update ..." -ForegroundColor Yellow
    Write-Host "  База данных: $($conn.Database)" -ForegroundColor Gray
    Write-Host "  Пользователь: $($conn.User)" -ForegroundColor Gray
    Write-Host "  Хост: $($conn.Host), порт: $($conn.Port)" -ForegroundColor Gray
    dotnet ef database update --project $dataProj --startup-project $apiProj
    if ($LASTEXITCODE -ne 0) { throw "database update завершился с кодом $LASTEXITCODE" }

    Write-Host "`n[2/4] Импорт (ProductionSystem.Import -- --force) ..." -ForegroundColor Yellow
    dotnet run --project $importProj -- --force
    if ($LASTEXITCODE -ne 0) { throw "Import завершился с кодом $LASTEXITCODE" }

    $env:PGPASSWORD = $conn.Password

    Write-Host "`n[3/4] psql: seed-erd-test-data.sql (UTF-8, PGCLIENTENCODING=UTF8) ..." -ForegroundColor Yellow
    Invoke-PsqlUtf8File -Conn $conn -FilePath $seedSql

    $summarySql = @"
SELECT 'users'::text AS table_name, count(*)::bigint AS row_count FROM users
UNION ALL SELECT 'suppliers'::text, count(*)::bigint FROM suppliers
UNION ALL SELECT 'warehouses'::text, count(*)::bigint FROM warehouses
UNION ALL SELECT 'materials'::text, count(*)::bigint FROM materials
UNION ALL SELECT 'components'::text, count(*)::bigint FROM components
UNION ALL SELECT 'workers'::text, count(*)::bigint FROM workers
UNION ALL SELECT 'worker_operations'::text, count(*)::bigint FROM worker_operations
UNION ALL SELECT 'production_operations'::text, count(*)::bigint FROM production_operations
UNION ALL SELECT 'products'::text, count(*)::bigint FROM products
UNION ALL SELECT 'customer_orders'::text, count(*)::bigint FROM customer_orders
UNION ALL SELECT 'equipment_types'::text, count(*)::bigint FROM equipment_types
UNION ALL SELECT 'equipment'::text, count(*)::bigint FROM equipment
UNION ALL SELECT 'product_material_specs'::text, count(*)::bigint FROM product_material_specs
UNION ALL SELECT 'product_component_specs'::text, count(*)::bigint FROM product_component_specs
UNION ALL SELECT 'product_operation_specs'::text, count(*)::bigint FROM product_operation_specs
UNION ALL SELECT 'product_assembly_specs'::text, count(*)::bigint FROM product_assembly_specs
ORDER BY 1;
"@

    Write-Host "`n[4/4] Итоговые значения (COUNT), столбцы: table_name, row_count:" -ForegroundColor Yellow
    Invoke-PsqlUtf8Text -Conn $conn -SqlText $summarySql

    Write-Host "`nГотово." -ForegroundColor Green
}
finally {
    Remove-Item Env:PGPASSWORD -ErrorAction SilentlyContinue
    Pop-Location
}
