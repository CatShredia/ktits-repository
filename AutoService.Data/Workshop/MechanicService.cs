using Npgsql;

namespace AutoService.Data.Workshop;

public sealed class MechanicWorkService
{
    private readonly IDbContextFactory<AutoServiceDbContext> _factory;

    public MechanicWorkService(IDbContextFactory<AutoServiceDbContext> factory) => _factory = factory;

    public async Task<IReadOnlyList<OpenAppointmentRow>> GetOpenAppointmentsAsync(int userId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var mechanic = await RequireMechanicAsync(db, userId);
        var rows = await db.Appointments.AsNoTracking()
            .Include(x => x.Client)
            .Include(x => x.Car)
            .Include(x => x.Service)
            .Where(x => x.MechanicId == mechanic.Id && x.WorkOrder == null && x.Status != AppointmentStatus.Отменена)
            .OrderBy(x => x.ScheduledAt)
            .ToListAsync();
        return rows.Select(x => new OpenAppointmentRow
        {
            Id = x.Id,
            When = x.ScheduledAt.ToString("dd.MM.yyyy HH:mm"),
            ClientName = x.Client.FullName,
            Car = x.Car.LicensePlate + " " + x.Car.Model,
            Service = x.Service.Name
        }).ToList();
    }

    public async Task<IReadOnlyList<MechanicOrderRow>> GetMyOrdersAsync(int userId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var mechanic = await RequireMechanicAsync(db, userId);
        var rows = await db.WorkOrders.AsNoTracking()
            .Include(x => x.Appointment).ThenInclude(x => x.Client)
            .Include(x => x.Appointment).ThenInclude(x => x.Car)
            .Include(x => x.Appointment).ThenInclude(x => x.Service)
            .Where(x => x.MechanicId == mechanic.Id)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
        return rows.Select(MapOrder).ToList();
    }

    public async Task<int> CreateWorkOrderAsync(int userId, int appointmentId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var mechanic = await RequireMechanicAsync(db, userId);
        var appointment = await db.Appointments.FirstOrDefaultAsync(x => x.Id == appointmentId && x.MechanicId == mechanic.Id)
            ?? throw new WorkshopException("Эта запись назначена другому механику или не найдена.");
        if (appointment.Status == AppointmentStatus.Отменена)
            throw new WorkshopException("По отменённой записи заказ-наряд не создаётся.");
        if (await db.WorkOrders.AnyAsync(x => x.AppointmentId == appointmentId))
            throw new WorkshopException("По этой записи уже есть заказ-наряд.");

        var order = new WorkOrder
        {
            AppointmentId = appointmentId,
            MechanicId = mechanic.Id,
            CreatedAt = Stamp(DateTime.Now),
            Status = WorkOrderStatus.Создан
        };
        db.WorkOrders.Add(order);
        await SaveAsync(db);
        return order.Id;
    }

    public async Task<MechanicOrderDetails> GetDetailsAsync(int userId, int workOrderId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var mechanic = await RequireMechanicAsync(db, userId);
        var order = await db.WorkOrders.AsNoTracking()
            .Include(x => x.Appointment).ThenInclude(x => x.Client)
            .Include(x => x.Appointment).ThenInclude(x => x.Car).ThenInclude(x => x.Brand)
            .Include(x => x.Inspection)
            .Include(x => x.Services).ThenInclude(x => x.Service)
            .Include(x => x.Parts).ThenInclude(x => x.Part)
            .FirstOrDefaultAsync(x => x.Id == workOrderId && x.MechanicId == mechanic.Id)
            ?? throw new WorkshopException("Заказ-наряд не найден среди назначенных вам.");

        var car = order.Appointment.Car;
        var inspection = order.Inspection;
        return new MechanicOrderDetails
        {
            CarText = $"{car.Brand.Name} {car.Model}, {car.ManufactureYear}\nVIN {car.Vin}\nГосномер {car.LicensePlate}, пробег {car.Mileage} км\nКлиент {order.Appointment.Client.FullName}",
            Fault = order.FaultDescription ?? "",
            Engine = inspection?.EngineState ?? "",
            Brakes = inspection?.BrakeState ?? "",
            Suspension = inspection?.SuspensionState ?? "",
            Electrical = inspection?.ElectricalState ?? "",
            Recommendations = inspection?.Recommendations ?? "",
            CanEditLines = order.Status is not WorkOrderStatus.Завершён and not WorkOrderStatus.Отменён,
            Services = order.Services
                .OrderBy(x => x.Service.Name)
                .Select(x => new OrderLineRow { Id = x.ServiceId, Title = $"{x.Service.Name} · {x.Price:0.00} ₽" })
                .ToList(),
            Parts = order.Parts
                .OrderBy(x => x.Part.Name)
                .Select(x => new OrderLineRow { Id = x.PartId, Title = $"{x.Part.Name} · {x.Quantity} шт. · {x.UnitPrice:0.00} ₽" })
                .ToList()
        };
    }

    public async Task<IReadOnlyList<LookupItem>> GetMyServicesAsync(int userId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var mechanic = await RequireMechanicAsync(db, userId);
        var links = await db.MechanicServices.AsNoTracking()
            .Include(x => x.Service)
            .Where(x => x.MechanicId == mechanic.Id)
            .OrderBy(x => x.Service.Name)
            .ToListAsync();
        return links.Select(x => new LookupItem { Id = x.ServiceId, Title = $"{x.Service.Name} · {x.Service.BasePrice:0.00} ₽" }).ToList();
    }

    public async Task<IReadOnlyList<LookupItem>> GetPartsAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        var parts = await db.Parts.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
        return parts.Select(x => new LookupItem { Id = x.Id, Title = $"{x.Name} · ост. {x.Quantity}" }).ToList();
    }

    public async Task SaveInspectionAsync(int userId, int workOrderId, string? fault, string? engine, string? brakes, string? suspension, string? electrical, string? recommendations)
    {
        var engineText = Limit(engine, "Двигатель", FieldLimits.Inspection);
        var brakeText = Limit(brakes, "Тормозная система", FieldLimits.Inspection);
        var suspensionText = Limit(suspension, "Подвеска", FieldLimits.Inspection);
        var electricalText = Limit(electrical, "Электрооборудование", FieldLimits.Inspection);
        var recommendationText = Limit(recommendations, "Рекомендации", FieldLimits.Inspection);
        var faultText = OptionalLimit(fault, "Неисправности", FieldLimits.Fault);

        await using var db = await _factory.CreateDbContextAsync();
        var mechanic = await RequireMechanicAsync(db, userId);
        var order = await RequireOrderAsync(db, mechanic.Id, workOrderId);
        if (order.Status == WorkOrderStatus.Отменён)
            throw new WorkshopException("Осмотр отменённого заказ-наряда не сохраняется.");

        order.FaultDescription = faultText;
        var inspection = await db.InspectionResults.FirstOrDefaultAsync(x => x.WorkOrderId == workOrderId);
        if (inspection is null)
        {
            db.InspectionResults.Add(new InspectionResult
            {
                WorkOrderId = workOrderId,
                EngineState = engineText,
                BrakeState = brakeText,
                SuspensionState = suspensionText,
                ElectricalState = electricalText,
                Recommendations = recommendationText
            });
        }
        else
        {
            inspection.EngineState = engineText;
            inspection.BrakeState = brakeText;
            inspection.SuspensionState = suspensionText;
            inspection.ElectricalState = electricalText;
            inspection.Recommendations = recommendationText;
        }

        await SaveAsync(db);
    }

    public async Task AddServiceAsync(int userId, int workOrderId, int serviceId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var mechanic = await RequireMechanicAsync(db, userId);
        var order = await RequireOrderAsync(db, mechanic.Id, workOrderId);
        EnsureLinesEditable(order);
        var link = await db.MechanicServices.Include(x => x.Service)
            .FirstOrDefaultAsync(x => x.MechanicId == mechanic.Id && x.ServiceId == serviceId)
            ?? throw new WorkshopException("Эту услугу вы не выполняете.");
        if (await db.WorkOrderServices.AnyAsync(x => x.WorkOrderId == workOrderId && x.ServiceId == serviceId))
            throw new WorkshopException("Услуга уже есть в заказ-наряде.");

        db.WorkOrderServices.Add(new WorkOrderService
        {
            WorkOrderId = workOrderId,
            ServiceId = serviceId,
            Price = link.Service.BasePrice
        });
        await SaveAsync(db);
    }

    public async Task RemoveServiceAsync(int userId, int workOrderId, int serviceId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var mechanic = await RequireMechanicAsync(db, userId);
        var order = await RequireOrderAsync(db, mechanic.Id, workOrderId);
        EnsureLinesEditable(order);
        var line = await db.WorkOrderServices.FirstOrDefaultAsync(x => x.WorkOrderId == workOrderId && x.ServiceId == serviceId)
            ?? throw new WorkshopException("Услуга не найдена в заказ-наряде.");
        db.Remove(line);
        await SaveAsync(db);
    }

    public async Task AddPartAsync(int userId, int workOrderId, int partId, int quantity)
    {
        if (quantity <= 0)
            throw new WorkshopException("Количество должно быть больше нуля.");

        await using var db = await _factory.CreateDbContextAsync();
        await using var tx = await db.Database.BeginTransactionAsync();
        var mechanic = await RequireMechanicAsync(db, userId);
        var order = await RequireOrderAsync(db, mechanic.Id, workOrderId);
        EnsureLinesEditable(order);
        var part = await db.Parts.FirstOrDefaultAsync(x => x.Id == partId)
            ?? throw new WorkshopException("Выберите запчасть из списка.");
        if (quantity > part.Quantity)
            throw new WorkshopException($"На складе {part.Quantity} шт., запрошено {quantity}. Запчасть не добавлена.");

        var line = await db.WorkOrderParts.FirstOrDefaultAsync(x => x.WorkOrderId == workOrderId && x.PartId == partId);
        if (line is null)
        {
            db.WorkOrderParts.Add(new WorkOrderPart
            {
                WorkOrderId = workOrderId,
                PartId = partId,
                Quantity = quantity,
                UnitPrice = part.SalePrice
            });
        }
        else
        {
            line.Quantity += quantity;
        }

        part.Quantity -= quantity;
        await SaveAsync(db);
        await tx.CommitAsync();
    }

    public async Task RemovePartAsync(int userId, int workOrderId, int partId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        await using var tx = await db.Database.BeginTransactionAsync();
        var mechanic = await RequireMechanicAsync(db, userId);
        var order = await RequireOrderAsync(db, mechanic.Id, workOrderId);
        EnsureLinesEditable(order);
        var line = await db.WorkOrderParts.FirstOrDefaultAsync(x => x.WorkOrderId == workOrderId && x.PartId == partId)
            ?? throw new WorkshopException("Запчасть не найдена в заказ-наряде.");
        var part = await db.Parts.FirstAsync(x => x.Id == partId);
        part.Quantity += line.Quantity;
        db.Remove(line);
        await SaveAsync(db);
        await tx.CommitAsync();
    }

    public async Task ChangeStatusAsync(int userId, int workOrderId, WorkOrderStatus status)
    {
        await using var db = await _factory.CreateDbContextAsync();
        await using var tx = await db.Database.BeginTransactionAsync();
        var mechanic = await RequireMechanicAsync(db, userId);
        var order = await RequireOrderAsync(db, mechanic.Id, workOrderId);
        if (order.Status == WorkOrderStatus.Завершён && status != order.Status)
            throw new WorkshopException("Завершённый заказ-наряд нельзя перевести в другой статус.");
        if (order.Status == WorkOrderStatus.Отменён && status != order.Status)
            throw new WorkshopException("Отменённый заказ-наряд нельзя открыть снова.");

        if (status == WorkOrderStatus.Отменён && order.Status != WorkOrderStatus.Отменён)
        {
            var lines = await db.WorkOrderParts.Where(x => x.WorkOrderId == workOrderId).ToListAsync();
            foreach (var line in lines)
            {
                var part = await db.Parts.FirstAsync(x => x.Id == line.PartId);
                part.Quantity += line.Quantity;
                db.Remove(line);
            }
        }

        order.Status = status;
        if (status == WorkOrderStatus.Завершён)
        {
            var appointment = await db.Appointments.FirstAsync(x => x.Id == order.AppointmentId);
            appointment.Status = AppointmentStatus.Выполнена;
        }

        await SaveAsync(db);
        await tx.CommitAsync();
    }

    private static MechanicOrderRow MapOrder(WorkOrder order) => new()
    {
        Id = order.Id,
        CreatedAt = order.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
        Status = StatusText.WorkOrder(order.Status),
        StatusValue = order.Status,
        ClientName = order.Appointment.Client.FullName,
        Car = order.Appointment.Car.LicensePlate + " " + order.Appointment.Car.Model,
        Service = order.Appointment.Service.Name
    };

    private static async Task<Mechanic> RequireMechanicAsync(AutoServiceDbContext db, int userId)
        => await db.Mechanics.FirstOrDefaultAsync(x => x.UserId == userId)
            ?? throw new WorkshopException("Учётная запись не связана с механиком.");

    private static async Task<WorkOrder> RequireOrderAsync(AutoServiceDbContext db, int mechanicId, int workOrderId)
        => await db.WorkOrders.FirstOrDefaultAsync(x => x.Id == workOrderId && x.MechanicId == mechanicId)
            ?? throw new WorkshopException("Заказ-наряд не найден среди назначенных вам.");

    private static void EnsureLinesEditable(WorkOrder order)
    {
        if (order.Status is WorkOrderStatus.Завершён or WorkOrderStatus.Отменён)
            throw new WorkshopException("В этом статусе состав заказ-наряда не меняется.");
    }

    private static string Limit(string? value, string label, int max)
    {
        var text = value?.Trim() ?? "";
        if (text.Length == 0)
            throw new WorkshopException($"Заполните поле «{label}».");
        if (text.Length > max)
            throw new WorkshopException($"Поле «{label}» длиннее {max} символов.");
        return text;
    }

    private static string? OptionalLimit(string? value, string label, int max)
    {
        var text = value?.Trim() ?? "";
        if (text.Length == 0)
            return null;
        if (text.Length > max)
            throw new WorkshopException($"Поле «{label}» длиннее {max} символов.");
        return text;
    }

    private static DateTime Stamp(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Unspecified);

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
