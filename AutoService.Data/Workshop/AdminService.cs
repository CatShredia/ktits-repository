using Npgsql;

namespace AutoService.Data.Workshop;

// Клиенты и автомобили: поиск, сохранение и удаление.
// Вызывается из ClientsViewModel и CarsViewModel.
public sealed partial class AdminService
{
    private readonly IDbContextFactory<AutoServiceDbContext> _factory;

    public AdminService(IDbContextFactory<AutoServiceDbContext> factory) => _factory = factory;

    public async Task<PageResult<ClientRow>> GetClientsAsync(string? search, string field, int page, int pageSize)
    {
        await using var db = await _factory.CreateDbContextAsync();
        IQueryable<Client> query = db.Clients.AsNoTracking();
        query = ApplyText(query, search, field, static (items, pattern) => items.Where(x =>
            EF.Functions.ILike(x.FullName, pattern, "\\")
            || EF.Functions.ILike(x.Phone, pattern, "\\")
            || (x.Email != null && EF.Functions.ILike(x.Email, pattern, "\\"))),
            static (items, pattern) => items.Where(x => EF.Functions.ILike(x.FullName, pattern, "\\")),
            new Dictionary<string, Func<IQueryable<Client>, string, IQueryable<Client>>>
            {
                ["phone"] = (items, pattern) => items.Where(x => EF.Functions.ILike(x.Phone, pattern, "\\")),
                ["email"] = (items, pattern) => items.Where(x => x.Email != null && EF.Functions.ILike(x.Email, pattern, "\\"))
            });
        return await PageAsync(query.OrderBy(x => x.FullName).ThenBy(x => x.Id), page, pageSize, x => new ClientRow
        {
            Id = x.Id,
            FullName = x.FullName,
            Phone = x.Phone,
            Email = x.Email ?? ""
        });
    }

    public async Task SaveClientAsync(int? id, string? fullName, string? phone, string? email)
    {
        var name = Required(fullName, "ФИО");
        var phoneText = Required(phone, "Телефон");
        var emailText = Optional(email);
        if (emailText is not null && !emailText.Contains('@'))
            throw new WorkshopException("В адресе почты должен быть символ @.");

        await using var db = await _factory.CreateDbContextAsync();
        if (id is null)
        {
            db.Clients.Add(new Client { FullName = name, Phone = phoneText, Email = emailText });
        }
        else
        {
            var client = await db.Clients.FirstOrDefaultAsync(x => x.Id == id) ?? throw new WorkshopException("Запись не найдена.");
            client.FullName = name;
            client.Phone = phoneText;
            client.Email = emailText;
        }

        await SaveAsync(db);
    }

    public async Task DeleteClientAsync(int id) => await DeleteAsync<Client>(id);

    public async Task<PageResult<CarRow>> GetCarsAsync(string? search, string field, int page, int pageSize)
    {
        await using var db = await _factory.CreateDbContextAsync();
        IQueryable<Car> query = db.Cars.AsNoTracking().Include(x => x.Client).Include(x => x.Brand).Include(x => x.Category);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = Like(search);
            query = field switch
            {
                "vin" => query.Where(x => EF.Functions.ILike(x.Vin, pattern, "\\")),
                "brand" => query.Where(x => EF.Functions.ILike(x.Brand.Name, pattern, "\\")),
                "model" => query.Where(x => EF.Functions.ILike(x.Model, pattern, "\\")),
                "plate" => query.Where(x => EF.Functions.ILike(x.LicensePlate, pattern, "\\")),
                _ => query.Where(x => EF.Functions.ILike(x.Vin, pattern, "\\")
                    || EF.Functions.ILike(x.Brand.Name, pattern, "\\")
                    || EF.Functions.ILike(x.Model, pattern, "\\")
                    || EF.Functions.ILike(x.LicensePlate, pattern, "\\"))
            };
        }

        return await PageAsync(query.OrderBy(x => x.Mileage).ThenBy(x => x.Id), page, pageSize, x => new CarRow
        {
            Id = x.Id,
            ClientId = x.ClientId,
            BrandId = x.CarBrandId,
            CategoryId = x.CarCategoryId,
            ClientName = x.Client.FullName,
            Brand = x.Brand.Name,
            Category = x.Category.Name,
            Model = x.Model,
            Year = x.ManufactureYear,
            Vin = x.Vin,
            LicensePlate = x.LicensePlate,
            Mileage = x.Mileage,
            Color = x.Color ?? "",
            Notes = x.Notes ?? ""
        });
    }

    public async Task SaveCarAsync(int? id, int clientId, int brandId, int categoryId, string? model, int year, string? vin, string? plate, int mileage, string? color, string? notes)
    {
        var modelText = Required(model, "Модель");
        var vinText = Required(vin, "VIN").ToUpperInvariant();
        var plateText = Required(plate, "Госномер");
        if (vinText.Length != 17)
            throw new WorkshopException("VIN должен содержать 17 символов.");
        if (year < 1970 || year > 2100)
            throw new WorkshopException("Год выпуска должен быть от 1970 до 2100.");
        if (mileage < 0)
            throw new WorkshopException("Пробег не может быть отрицательным.");

        await using var db = await _factory.CreateDbContextAsync();
        if (!await db.Clients.AnyAsync(x => x.Id == clientId))
            throw new WorkshopException("Выберите клиента из списка.");
        Car car;
        if (id is null)
        {
            car = new Car { Model = modelText, Vin = vinText, LicensePlate = plateText };
            db.Cars.Add(car);
        }
        else
        {
            car = await db.Cars.FirstOrDefaultAsync(x => x.Id == id) ?? throw new WorkshopException("Запись не найдена.");
        }

        car.ClientId = clientId;
        car.CarBrandId = brandId;
        car.CarCategoryId = categoryId;
        car.Model = modelText;
        car.ManufactureYear = year;
        car.Vin = vinText;
        car.LicensePlate = plateText;
        car.Mileage = mileage;
        car.Color = Optional(color);
        car.Notes = Optional(notes);
        await SaveAsync(db);
    }

    public async Task DeleteCarAsync(int id) => await DeleteAsync<Car>(id);

    public async Task<IReadOnlyList<LookupItem>> GetCarsByClientAsync(int clientId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Cars.AsNoTracking()
            .Where(x => x.ClientId == clientId)
            .OrderBy(x => x.LicensePlate)
            .Select(x => new LookupItem { Id = x.Id, Title = x.LicensePlate + " " + x.Model })
            .ToListAsync();
    }

    public Task<IReadOnlyList<LookupItem>> GetClientsLookupAsync() => LookupAsync(db =>
        db.Clients.AsNoTracking().OrderBy(x => x.FullName).Select(x => new LookupItem { Id = x.Id, Title = x.FullName }));

    public Task<IReadOnlyList<LookupItem>> GetBrandsAsync() => LookupAsync(db =>
        db.CarBrands.AsNoTracking().OrderBy(x => x.Name).Select(x => new LookupItem { Id = x.Id, Title = x.Name }));

    public Task<IReadOnlyList<LookupItem>> GetCategoriesAsync() => LookupAsync(db =>
        db.CarCategories.AsNoTracking().OrderBy(x => x.Name).Select(x => new LookupItem { Id = x.Id, Title = x.Name }));

    public Task<IReadOnlyList<LookupItem>> GetServicesLookupAsync() => LookupAsync(db =>
        db.Services.AsNoTracking().OrderBy(x => x.Name).Select(x => new LookupItem { Id = x.Id, Title = x.Name }));

    public Task<IReadOnlyList<LookupItem>> GetBaysAsync() => LookupAsync(db =>
        db.RepairBays.AsNoTracking().OrderBy(x => x.Name).Select(x => new LookupItem { Id = x.Id, Title = x.Name }));

    public Task<IReadOnlyList<LookupItem>> GetSuppliersAsync() => LookupAsync(db =>
        db.Suppliers.AsNoTracking().OrderBy(x => x.Name).Select(x => new LookupItem { Id = x.Id, Title = x.Name }));

    public Task<IReadOnlyList<LookupItem>> GetDepartmentsAsync() => LookupAsync(db =>
        db.Departments.AsNoTracking().OrderBy(x => x.Name).Select(x => new LookupItem { Id = x.Id, Title = x.Name }));

    public Task<IReadOnlyList<LookupItem>> GetSpecializationsAsync() => LookupAsync(db =>
        db.Specializations.AsNoTracking().OrderBy(x => x.Name).Select(x => new LookupItem { Id = x.Id, Title = x.Name }));

    public async Task<IReadOnlyList<LookupItem>> GetMechanicsAsync(int? serviceId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        IQueryable<Mechanic> query = db.Mechanics.AsNoTracking().Include(x => x.User);
        if (serviceId is int id)
            query = query.Where(x => x.Services.Any(s => s.ServiceId == id));
        return await query.OrderBy(x => x.User.FullName)
            .Select(x => new LookupItem { Id = x.Id, Title = x.User.FullName })
            .ToListAsync();
    }

    private async Task<IReadOnlyList<LookupItem>> LookupAsync(Func<AutoServiceDbContext, IQueryable<LookupItem>> query)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await query(db).ToListAsync();
    }

    private static IQueryable<T> ApplyText<T>(
        IQueryable<T> query,
        string? search,
        string field,
        Func<IQueryable<T>, string, IQueryable<T>> all,
        Func<IQueryable<T>, string, IQueryable<T>> name,
        IReadOnlyDictionary<string, Func<IQueryable<T>, string, IQueryable<T>>> fields)
    {
        if (string.IsNullOrWhiteSpace(search))
            return query;
        var pattern = Like(search);
        if (fields.TryGetValue(field, out var specific))
            return specific(query, pattern);
        return field == "name" ? name(query, pattern) : all(query, pattern);
    }

    private static async Task<PageResult<TResult>> PageAsync<T, TResult>(IQueryable<T> query, int page, int pageSize, Func<T, TResult> selector)
    {
        pageSize = pageSize == 20 ? 20 : 10;
        page = Math.Max(1, page);
        var total = await query.CountAsync();
        var entities = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PageResult<TResult>(entities.Select(selector).ToList(), total, page, pageSize);
    }

    private static string Like(string text)
    {
        var escaped = text.Trim().Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
        return "%" + escaped + "%";
    }

    private static string Required(string? value, string label)
    {
        var text = value?.Trim() ?? "";
        if (text.Length == 0)
            throw new WorkshopException($"Заполните поле «{label}».");
        return text;
    }

    private static string? Optional(string? value)
    {
        var text = value?.Trim() ?? "";
        return text.Length == 0 ? null : text;
    }

    private async Task DeleteAsync<T>(int id) where T : class
    {
        await using var db = await _factory.CreateDbContextAsync();
        var entity = await db.Set<T>().FindAsync(id) ?? throw new WorkshopException("Запись не найдена.");
        db.Remove(entity);
        await SaveAsync(db);
    }

    private static async Task SaveAsync(AutoServiceDbContext db)
    {
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            var postgres = FindPostgres(ex);
            throw postgres?.SqlState switch
            {
                "23505" => new WorkshopException("Такое значение уже есть в базе."),
                "23503" => new WorkshopException("Запись связана с другими данными и не может быть изменена или удалена."),
                "23514" => new WorkshopException("Данные не проходят проверку базы."),
                _ => new WorkshopException("Не удалось сохранить данные.")
            };
        }
    }

    private static PostgresException? FindPostgres(Exception exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (current is PostgresException postgres)
                return postgres;
        }

        return null;
    }
}
