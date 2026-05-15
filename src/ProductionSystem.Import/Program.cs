using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProductionSystem.Data;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var conn = config.GetConnectionString("Default")
           ?? Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")
           ?? "Host=localhost;Database=production_system;Username=postgres;Password=postgres";

var force = args.Contains("--force", StringComparer.OrdinalIgnoreCase);
var dataRoot = args.FirstOrDefault(a => !a.StartsWith('-')) ?? FindDataRoot();

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine($"Data root: {dataRoot}");

var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(conn).Options;
await using var db = new AppDbContext(options);

await db.Database.MigrateAsync();

if (await db.Users.AnyAsync() && !force)
{
    Console.WriteLine("База уже содержит пользователей. Запустите с --force для полной перезагрузки данных.");
    return;
}

await using var trx = await db.Database.BeginTransactionAsync();

await db.Database.ExecuteSqlRawAsync("""
    TRUNCATE TABLE
        order_quality_checks,
        order_status_history,
        order_dimensions,
        workshop_layout_items,
        equipment_failures,
        product_material_specs,
        product_component_specs,
        product_operation_specs,
        product_assembly_specs,
        customer_orders,
        workshops,
        worker_operations,
        equipment,
        materials,
        components,
        workers,
        users,
        suppliers,
        warehouses,
        products,
        equipment_types,
        production_operations
    RESTART IDENTITY CASCADE;
    """);

var whA = new Warehouse { Name = "Склад А" };
var whB = new Warehouse { Name = "Склад Б" };
db.Warehouses.AddRange(whA, whB);
await db.SaveChangesAsync();

foreach (var name in new[] { "Сборка", "Сварка", "Покраска", "Контроль качества", "Упаковка" })
    db.ProductionOperations.Add(new ProductionOperation { Name = name });
await db.SaveChangesAsync();

var supplierCache = new Dictionary<string, Supplier>(StringComparer.OrdinalIgnoreCase);

Supplier GetOrCreateSupplier(string? name, int deliveryDays)
{
    var key = string.IsNullOrWhiteSpace(name) ? "(не указан)" : name.Trim();
    if (supplierCache.TryGetValue(key, out var s))
    {
        if (deliveryDays > s.DeliveryDays)
            s.DeliveryDays = deliveryDays;
        return s;
    }

    s = new Supplier { Name = key, DeliveryDays = Math.Max(0, deliveryDays) };
    supplierCache[key] = s;
    db.Suppliers.Add(s);
    return s;
}

var materialsPath = Path.Combine(dataRoot, "Материалы.xlsx");
var componentsPath = Path.Combine(dataRoot, "Комплектующие.xlsx");
var usersPath = Path.Combine(dataRoot, "Пользователи.csv");
var matImgDir = Path.Combine(dataRoot, "Изображения", "Материалы");
var compImgDir = Path.Combine(dataRoot, "Изображения", "Комплектующие");
var userImgDir = Path.Combine(dataRoot, "Изображения", "Фото пользователей");

ImportMaterials(materialsPath, matImgDir, whA, whB);
ImportComponents(componentsPath, compImgDir, whA, whB);
await db.SaveChangesAsync();

await ImportUsers(usersPath, userImgDir);
await db.SaveChangesAsync();

await trx.CommitAsync();
Console.WriteLine("Импорт завершён.");

void ImportMaterials(string path, string imgDir, Warehouse a, Warehouse b)
{
    using var wb = new XLWorkbook(path);
    var ws = wb.Worksheet(1);
    var headers = ReadHeaderCells(ws.Row(1));
    int colArticle = FindCol(headers, "артикул");
    int colName = FindCol(headers, "наимен");
    int colType = FindCol(headers, "тип");
    int colUnit = FindCol(headers, "едини");
    int colQty = FindCol(headers, "колич");
    int? colSupplier = TryFindCol(headers, "постав", "поставщик");
    int colPrice = FindCol(headers, "закуп", "стоим", "цен");
    int? colDelivery = TryFindCol(headers, "срок", "достав");
    int colGost = FindCol(headers, "гост");
    int? colLen = TryFindCol(headers, "длин");
    int? colChar = TryFindCol(headers, "характ");
    int? colMass = TryFindCol(headers, "масса");

    var last = ws.LastRowUsed()?.RowNumber() ?? 1;
    for (var r = 2; r <= last; r++)
    {
        var row = ws.Row(r);
        var article = row.Cell(colArticle).GetString().Trim();
        if (string.IsNullOrEmpty(article))
            continue;

        Supplier? supplier = null;
        if (colSupplier is int cs)
        {
            var supplierName = row.Cell(cs).GetString().Trim();
            if (!string.IsNullOrEmpty(supplierName))
            {
                var delivery = colDelivery is int cd ? (int)Math.Round(row.Cell(cd).GetDouble()) : 0;
                supplier = GetOrCreateSupplier(supplierName, delivery);
            }
        }

        var warehouse = r % 2 == 0 ? b : a;
        string? characteristics = NullIfEmpty(colChar is int cch ? row.Cell(cch).GetString() : null);
        if (colMass is int cm)
        {
            var mass = TryDecimal(row.Cell(cm));
            if (mass.HasValue)
            {
                var massStr = $"Масса 1 м: {mass.Value} кг";
                characteristics = string.IsNullOrEmpty(characteristics)
                    ? massStr
                    : $"{characteristics}; {massStr}";
            }
        }

        var mat = new Material
        {
            Article = article,
            Name = row.Cell(colName).GetString().Trim(),
            MaterialType = row.Cell(colType).GetString().Trim(),
            Unit = row.Cell(colUnit).GetString().Trim(),
            Quantity = ToDecimal(row.Cell(colQty)),
            PurchasePrice = ToDecimal(row.Cell(colPrice)),
            Gost = NullIfEmpty(row.Cell(colGost).GetString()),
            Length = colLen is int clen ? TryDecimal(row.Cell(clen)) : null,
            Characteristics = NullIfEmpty(characteristics),
            Supplier = supplier,
            Warehouse = warehouse,
            Image = TryLoadImage(imgDir, article),
        };
        db.Materials.Add(mat);
    }
}

void ImportComponents(string path, string imgDir, Warehouse a, Warehouse b)
{
    using var wb = new XLWorkbook(path);
    var ws = wb.Worksheet(1);
    var headers = ReadHeaderCells(ws.Row(1));
    int colArticle = FindCol(headers, "артикул");
    int colName = FindCol(headers, "наимен");
    int colType = FindCol(headers, "тип");
    int colUnit = FindCol(headers, "едини");
    int colQty = FindCol(headers, "колич");
    int? colSupplier = TryFindCol(headers, "постав", "поставщик");
    int colPrice = FindCol(headers, "закуп", "стоим", "цен");
    int? colDelivery = TryFindCol(headers, "срок", "достав");
    int colWeight = FindCol(headers, "вес");

    var last = ws.LastRowUsed()?.RowNumber() ?? 1;
    for (var r = 2; r <= last; r++)
    {
        var row = ws.Row(r);
        var article = row.Cell(colArticle).GetString().Trim();
        if (string.IsNullOrEmpty(article))
            continue;

        Supplier? supplier = null;
        if (colSupplier is int cs)
        {
            var supplierName = row.Cell(cs).GetString().Trim();
            if (!string.IsNullOrEmpty(supplierName))
            {
                var delivery = colDelivery is int cd ? (int)Math.Round(row.Cell(cd).GetDouble()) : 0;
                supplier = GetOrCreateSupplier(supplierName, delivery);
            }
        }

        var warehouse = r % 2 == 0 ? a : b;
        var comp = new StockComponent
        {
            Article = article,
            Name = row.Cell(colName).GetString().Trim(),
            ComponentType = row.Cell(colType).GetString().Trim(),
            Unit = row.Cell(colUnit).GetString().Trim(),
            Quantity = ToDecimal(row.Cell(colQty)),
            PurchasePrice = ToDecimal(row.Cell(colPrice)),
            Weight = ToDecimal(row.Cell(colWeight)),
            Supplier = supplier,
            Warehouse = warehouse,
            Image = TryLoadImage(imgDir, article),
        };
        db.Components.Add(comp);
    }
}

async Task ImportUsers(string path, string imgDir)
{
    using var reader = new StreamReader(path, Encoding.UTF8);
    using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = true,
        MissingFieldFound = null,
        BadDataFound = null,
        TrimOptions = TrimOptions.Trim,
    });

    var rows = csv.GetRecords<UserCsvRow>().ToList();
    foreach (var row in rows)
    {
        var fullName = $"{row.LastName.Trim()} {row.FirstMiddle.Trim()}".Trim();
        var user = new AppUser
        {
            Login = row.Login.Trim(),
            Password = row.Password.Trim(),
            Role = row.Role.Trim(),
            FullName = fullName,
            Photo = TryLoadUserImage(imgDir, row.Login.Trim()),
        };
        db.Users.Add(user);
    }

    await Task.CompletedTask;
}

static string FindDataRoot()
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir != null)
    {
        var candidate = Path.Combine(dir.FullName, "task", "Ресурсы - Сессия 1", "data", "data");
        if (Directory.Exists(candidate))
            return candidate;
        dir = dir.Parent;
    }

    throw new DirectoryNotFoundException(
        "Не найден каталог task/Ресурсы - Сессия 1/data/data. Укажите путь первым аргументом.");
}

static List<(int Col, string Header)> ReadHeaderCells(IXLRow headerRow)
{
    var list = new List<(int, string)>();
    foreach (var cell in headerRow.CellsUsed())
    {
        var t = cell.GetString().Trim();
        if (t.Length > 0)
            list.Add((cell.Address.ColumnNumber, t));
    }

    return list;
}

static int FindCol(List<(int Col, string Header)> headers, params string[] keys)
{
    var col = TryFindCol(headers, keys);
    if (col is int c)
        return c;
    throw new InvalidOperationException($"Не найдена колонка: {string.Join(", ", keys)}");
}

static int? TryFindCol(List<(int Col, string Header)> headers, params string[] keys)
{
    foreach (var key in keys)
    {
        foreach (var (col, h) in headers)
        {
            if (h.Contains(key, StringComparison.OrdinalIgnoreCase))
                return col;
        }
    }

    return null;
}

static decimal ToDecimal(IXLCell cell)
{
    if (cell.DataType == XLDataType.Number)
        return (decimal)cell.GetDouble();
    if (decimal.TryParse(cell.GetString().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
        return d;
    return 0;
}

static decimal? TryDecimal(IXLCell cell)
{
    if (string.IsNullOrWhiteSpace(cell.GetString()) && cell.DataType != XLDataType.Number)
        return null;
    return ToDecimal(cell);
}

static string? NullIfEmpty(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

static byte[]? TryLoadImage(string dir, string key)
{
    if (!Directory.Exists(dir))
        return null;

    foreach (var ext in new[] { ".jpg", ".jpeg", ".png", ".gif", ".JPG", ".PNG", ".GIF", ".JPEG" })
    {
        var path = Path.Combine(dir, key + ext);
        if (File.Exists(path))
            return File.ReadAllBytes(path);
    }

    foreach (var f in Directory.EnumerateFiles(dir))
    {
        var name = Path.GetFileNameWithoutExtension(f);
        if (name.Equals(key, StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes(f);
    }

    return null;
}

static byte[]? TryLoadUserImage(string dir, string login)
{
    if (!Directory.Exists(dir))
        return null;

    foreach (var ext in new[] { ".jpg", ".jpeg", ".png", ".gif" })
    {
        var path = Path.Combine(dir, login + ext);
        if (File.Exists(path))
            return File.ReadAllBytes(path);
    }

    foreach (var f in Directory.EnumerateFiles(dir))
    {
        var name = Path.GetFileNameWithoutExtension(f);
        if (name.StartsWith(login, StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes(f);
    }

    return null;
}

internal sealed class UserCsvRow
{
    [Name("Фамилия")]
    public string LastName { get; set; } = "";

    [Name("Имя, отчество")]
    public string FirstMiddle { get; set; } = "";

    [Name("Login")]
    public string Login { get; set; } = "";

    [Name("Password")]
    public string Password { get; set; } = "";

    [Name("Role")]
    public string Role { get; set; } = "";
}
