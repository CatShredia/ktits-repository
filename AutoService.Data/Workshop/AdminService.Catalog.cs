using AutoService.Core.Security;

namespace AutoService.Data.Workshop;

public sealed partial class AdminService
{
    public async Task<IReadOnlyList<CatalogRow>> GetCatalogAsync(CatalogKind kind)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return kind switch
        {
            CatalogKind.Brands => await db.CarBrands.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new CatalogRow { Id = x.Id, Name = x.Name, Title = x.Name }).ToListAsync(),
            CatalogKind.Categories => await db.CarCategories.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new CatalogRow { Id = x.Id, Name = x.Name, Title = x.Name }).ToListAsync(),
            CatalogKind.Departments => await db.Departments.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new CatalogRow { Id = x.Id, Name = x.Name, Title = x.Name }).ToListAsync(),
            CatalogKind.Specializations => await db.Specializations.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new CatalogRow { Id = x.Id, Name = x.Name, Title = x.Name }).ToListAsync(),
            CatalogKind.Services => (await db.Services.AsNoTracking().OrderBy(x => x.BasePrice).ToListAsync())
                .Select(x => new CatalogRow
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.BasePrice,
                    Duration = x.DurationMinutes,
                    Title = $"{x.Name} · {x.BasePrice:0.00} ₽"
                }).ToList(),
            CatalogKind.Suppliers => await db.Suppliers.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new CatalogRow
                {
                    Id = x.Id,
                    Name = x.Name,
                    Phone = x.Phone ?? "",
                    Email = x.Email ?? "",
                    Title = x.Name
                }).ToListAsync(),
            CatalogKind.Parts => (await db.Parts.AsNoTracking().OrderBy(x => x.Name).ToListAsync())
                .Select(x => new CatalogRow
                {
                    Id = x.Id,
                    Name = x.Name,
                    Sku = x.Sku,
                    SupplierId = x.SupplierId,
                    PurchasePrice = x.PurchasePrice,
                    Price = x.SalePrice,
                    Quantity = x.Quantity,
                    MinQuantity = x.MinQuantity,
                    Title = $"{x.Name} · ост. {x.Quantity}"
                }).ToList(),
            CatalogKind.Bays => await db.RepairBays.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new CatalogRow { Id = x.Id, Name = x.Name, Kind = x.Kind, Title = x.Name }).ToListAsync(),
            CatalogKind.Mechanics => await MechanicsAsync(db),
            _ => throw new WorkshopException("Неизвестный справочник.")
        };
    }

    public async Task SaveNamedAsync(CatalogKind kind, int? id, string? name)
    {
        var text = Required(name, "Название");
        await using var db = await _factory.CreateDbContextAsync();
        switch (kind)
        {
            case CatalogKind.Brands:
                await SaveNamed(db, db.CarBrands, id, () => new CarBrand { Name = text }, x => x.Name = text);
                break;
            case CatalogKind.Categories:
                await SaveNamed(db, db.CarCategories, id, () => new CarCategory { Name = text }, x => x.Name = text);
                break;
            case CatalogKind.Departments:
                await SaveNamed(db, db.Departments, id, () => new Department { Name = text }, x => x.Name = text);
                break;
            case CatalogKind.Specializations:
                await SaveNamed(db, db.Specializations, id, () => new Specialization { Name = text }, x => x.Name = text);
                break;
            default:
                throw new WorkshopException("Этот справочник сохраняется отдельной формой.");
        }
    }

    public async Task DeleteNamedAsync(CatalogKind kind, int id)
    {
        switch (kind)
        {
            case CatalogKind.Brands: await DeleteAsync<CarBrand>(id); break;
            case CatalogKind.Categories: await DeleteAsync<CarCategory>(id); break;
            case CatalogKind.Departments: await DeleteAsync<Department>(id); break;
            case CatalogKind.Specializations: await DeleteAsync<Specialization>(id); break;
            case CatalogKind.Services: await DeleteAsync<Service>(id); break;
            case CatalogKind.Suppliers: await DeleteAsync<Supplier>(id); break;
            case CatalogKind.Parts: await DeleteAsync<Part>(id); break;
            case CatalogKind.Bays: await DeleteAsync<RepairBay>(id); break;
            case CatalogKind.Mechanics: await DeleteAsync<Mechanic>(id); break;
            default: throw new WorkshopException("Неизвестный справочник.");
        }
    }

    public async Task SaveServiceAsync(int? id, string? name, string? description, decimal price, int duration)
    {
        var serviceName = Required(name, "Название");
        var text = Required(description, "Описание");
        if (price < 0)
            throw new WorkshopException("Стоимость не может быть отрицательной.");
        if (duration <= 0)
            throw new WorkshopException("Продолжительность должна быть больше нуля.");

        await using var db = await _factory.CreateDbContextAsync();
        Service service;
        if (id is null)
        {
            service = new Service { Name = serviceName, Description = text };
            db.Services.Add(service);
        }
        else
        {
            service = await db.Services.FirstOrDefaultAsync(x => x.Id == id) ?? throw new WorkshopException("Запись не найдена.");
        }

        service.Name = serviceName;
        service.Description = text;
        service.BasePrice = price;
        service.DurationMinutes = duration;
        await SaveAsync(db);
    }

    public async Task<PageResult<ServiceRow>> GetServicesAsync(string? search, decimal? maxPrice, int? maxDuration, int page, int pageSize)
    {
        await using var db = await _factory.CreateDbContextAsync();
        IQueryable<Service> query = db.Services.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = Like(search);
            query = query.Where(x => EF.Functions.ILike(x.Name, pattern, "\\") || EF.Functions.ILike(x.Description, pattern, "\\"));
        }

        if (maxPrice is decimal price)
            query = query.Where(x => x.BasePrice <= price);
        if (maxDuration is int duration)
            query = query.Where(x => x.DurationMinutes <= duration);

        return await PageAsync(query.OrderBy(x => x.BasePrice).ThenBy(x => x.Id), page, pageSize, x => new ServiceRow
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            BasePrice = x.BasePrice,
            DurationMinutes = x.DurationMinutes
        });
    }

    public async Task SaveSupplierAsync(int? id, string? name, string? phone, string? email)
    {
        var supplierName = Required(name, "Название");
        await using var db = await _factory.CreateDbContextAsync();
        Supplier supplier;
        if (id is null)
        {
            supplier = new Supplier { Name = supplierName };
            db.Suppliers.Add(supplier);
        }
        else
        {
            supplier = await db.Suppliers.FirstOrDefaultAsync(x => x.Id == id) ?? throw new WorkshopException("Запись не найдена.");
        }

        supplier.Name = supplierName;
        supplier.Phone = Optional(phone);
        supplier.Email = Optional(email);
        await SaveAsync(db);
    }

    public async Task SavePartAsync(int? id, int supplierId, string? name, string? sku, decimal purchase, decimal sale, int quantity, int minQuantity)
    {
        var partName = Required(name, "Название");
        var article = Required(sku, "Артикул");
        if (purchase < 0 || sale < 0)
            throw new WorkshopException("Цены не могут быть отрицательными.");
        if (quantity < 0 || minQuantity < 0)
            throw new WorkshopException("Количество не может быть отрицательным.");

        await using var db = await _factory.CreateDbContextAsync();
        if (!await db.Suppliers.AnyAsync(x => x.Id == supplierId))
            throw new WorkshopException("Выберите поставщика из списка.");
        Part part;
        if (id is null)
        {
            part = new Part { Name = partName, Sku = article };
            db.Parts.Add(part);
        }
        else
        {
            part = await db.Parts.FirstOrDefaultAsync(x => x.Id == id) ?? throw new WorkshopException("Запись не найдена.");
        }

        part.SupplierId = supplierId;
        part.Name = partName;
        part.Sku = article;
        part.PurchasePrice = purchase;
        part.SalePrice = sale;
        part.Quantity = quantity;
        part.MinQuantity = minQuantity;
        await SaveAsync(db);
    }

    public async Task<PageResult<PartRow>> GetPartsAsync(string? search, int? supplierId, bool belowMinimum, int page, int pageSize)
    {
        await using var db = await _factory.CreateDbContextAsync();
        IQueryable<Part> query = db.Parts.AsNoTracking().Include(x => x.Supplier);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = Like(search);
            query = query.Where(x => EF.Functions.ILike(x.Name, pattern, "\\") || EF.Functions.ILike(x.Sku, pattern, "\\"));
        }

        if (supplierId is int supplier)
            query = query.Where(x => x.SupplierId == supplier);
        if (belowMinimum)
            query = query.Where(x => x.Quantity < x.MinQuantity);

        return await PageAsync(query.OrderBy(x => x.Name), page, pageSize, x => new PartRow
        {
            Id = x.Id,
            SupplierId = x.SupplierId,
            Name = x.Name,
            Sku = x.Sku,
            Supplier = x.Supplier.Name,
            PurchasePrice = x.PurchasePrice,
            SalePrice = x.SalePrice,
            Quantity = x.Quantity,
            MinQuantity = x.MinQuantity
        });
    }

    public async Task SaveBayAsync(int? id, string? name, RepairBayKind kind)
    {
        var bayName = Required(name, "Название");
        await using var db = await _factory.CreateDbContextAsync();
        RepairBay bay;
        if (id is null)
        {
            bay = new RepairBay { Name = bayName };
            db.RepairBays.Add(bay);
        }
        else
        {
            bay = await db.RepairBays.FirstOrDefaultAsync(x => x.Id == id) ?? throw new WorkshopException("Запись не найдена.");
        }

        bay.Name = bayName;
        bay.Kind = kind;
        await SaveAsync(db);
    }

    public async Task SaveMechanicAsync(int? id, string? fullName, string? login, string? password, int departmentId, int specializationId, IReadOnlyCollection<int> serviceIds)
    {
        var name = Required(fullName, "ФИО");
        var userLogin = Required(login, "Логин");
        var services = serviceIds.Distinct().ToArray();
        if (services.Length == 0)
            throw new WorkshopException("Отметьте хотя бы одну услугу, которую выполняет механик.");

        await using var db = await _factory.CreateDbContextAsync();
        if (!await db.Departments.AnyAsync(x => x.Id == departmentId) || !await db.Specializations.AnyAsync(x => x.Id == specializationId))
            throw new WorkshopException("Выберите отдел и специализацию из списков.");
        if (await db.Services.CountAsync(x => services.Contains(x.Id)) != services.Length)
            throw new WorkshopException("Выберите услуги из списка.");

        Mechanic mechanic;
        if (id is null)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new WorkshopException("Для нового механика задайте пароль.");
            var roleId = await db.Roles.Where(x => x.Name == RoleNames.Mechanic).Select(x => x.Id).SingleAsync();
            var user = new User
            {
                RoleId = roleId,
                FullName = name,
                Login = userLogin,
                PasswordHash = PasswordHasher.HashPassword(password)
            };
            mechanic = new Mechanic { User = user, DepartmentId = departmentId, SpecializationId = specializationId };
            db.Mechanics.Add(mechanic);
        }
        else
        {
            mechanic = await db.Mechanics.Include(x => x.User).Include(x => x.Services).FirstOrDefaultAsync(x => x.Id == id) ?? throw new WorkshopException("Запись не найдена.");
            mechanic.User.FullName = name;
            mechanic.User.Login = userLogin;
            if (!string.IsNullOrWhiteSpace(password))
                mechanic.User.PasswordHash = PasswordHasher.HashPassword(password);
            mechanic.DepartmentId = departmentId;
            mechanic.SpecializationId = specializationId;
        }

        await SaveAsync(db);
        await ReplaceServicesAsync(db, mechanic.Id, services);
    }

    private static async Task<IReadOnlyList<CatalogRow>> MechanicsAsync(AutoServiceDbContext db)
    {
        var items = await db.Mechanics.AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Department)
            .Include(x => x.Services)
            .OrderBy(x => x.User.FullName)
            .ToListAsync();
        return items.Select(x => new CatalogRow
        {
            Id = x.Id,
            Name = x.User.FullName,
            Login = x.User.Login,
            DepartmentId = x.DepartmentId,
            SpecializationId = x.SpecializationId,
            ServiceIds = x.Services.Select(s => s.ServiceId).ToArray(),
            Title = x.User.FullName + " · " + x.Department.Name
        }).ToList();
    }

    private static async Task SaveNamed<T>(AutoServiceDbContext db, DbSet<T> set, int? id, Func<T> create, Action<T> assign) where T : class
    {
        T entity;
        if (id is null)
        {
            entity = create();
            set.Add(entity);
        }
        else
        {
            entity = await set.FindAsync(id) ?? throw new WorkshopException("Запись не найдена.");
            assign(entity);
        }

        await SaveAsync(db);
    }

    private static async Task ReplaceServicesAsync(AutoServiceDbContext db, int mechanicId, IReadOnlyCollection<int> serviceIds)
    {
        var existing = await db.MechanicServices.Where(x => x.MechanicId == mechanicId).ToListAsync();
        var desired = serviceIds.ToHashSet();
        foreach (var row in existing.Where(x => !desired.Contains(x.ServiceId)))
            db.Remove(row);
        var current = existing.Select(x => x.ServiceId).ToHashSet();
        foreach (var serviceId in desired.Where(id => !current.Contains(id)))
            db.MechanicServices.Add(new MechanicService { MechanicId = mechanicId, ServiceId = serviceId });
        await SaveAsync(db);
    }
}
