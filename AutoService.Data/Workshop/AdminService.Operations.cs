namespace AutoService.Data.Workshop;

public sealed partial class AdminService
{
    public async Task<PageResult<AppointmentRow>> GetAppointmentsAsync(string? search, AppointmentStatus? status, int? mechanicId, DateTime? day, int page, int pageSize)
    {
        await using var db = await _factory.CreateDbContextAsync();
        IQueryable<Appointment> query = db.Appointments.AsNoTracking()
            .Include(x => x.Client).Include(x => x.Car).Include(x => x.Service).Include(x => x.Mechanic).ThenInclude(x => x.User).Include(x => x.RepairBay);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = Like(search);
            query = query.Where(x => EF.Functions.ILike(x.Client.FullName, pattern, "\\"));
        }

        if (status is AppointmentStatus appointmentStatus)
            query = query.Where(x => x.Status == appointmentStatus);
        if (mechanicId is int mechanic)
            query = query.Where(x => x.MechanicId == mechanic);
        if (day is DateTime date)
        {
            var start = date.Date;
            query = query.Where(x => x.ScheduledAt >= start && x.ScheduledAt < start.AddDays(1));
        }

        return await PageAsync(query.OrderBy(x => x.ScheduledAt).ThenBy(x => x.Id), page, pageSize, x => new AppointmentRow
        {
            Id = x.Id,
            ClientId = x.ClientId,
            CarId = x.CarId,
            ServiceId = x.ServiceId,
            MechanicId = x.MechanicId,
            BayId = x.RepairBayId,
            ScheduledAt = x.ScheduledAt,
            When = x.ScheduledAt.ToString("dd.MM.yyyy HH:mm"),
            ClientName = x.Client.FullName,
            Car = x.Car.LicensePlate + " " + x.Car.Model,
            Service = x.Service.Name,
            Mechanic = x.Mechanic.User.FullName,
            Bay = x.RepairBay.Name,
            Status = x.Status.ToString(),
            StatusValue = x.Status
        });
    }

    public async Task SaveAppointmentAsync(int? id, int clientId, int carId, int serviceId, int mechanicId, int bayId, DateTime scheduledAt, AppointmentStatus status)
    {
        await using var db = await _factory.CreateDbContextAsync();
        if (!await db.Cars.AnyAsync(x => x.Id == carId && x.ClientId == clientId))
            throw new WorkshopException("Автомобиль не принадлежит выбранному клиенту.");
        if (!await db.MechanicServices.AnyAsync(x => x.MechanicId == mechanicId && x.ServiceId == serviceId))
            throw new WorkshopException("Механик не выполняет выбранную услугу.");
        if (!await db.RepairBays.AnyAsync(x => x.Id == bayId))
            throw new WorkshopException("Выберите ремонтное место из списка.");

        Appointment appointment;
        if (id is null)
        {
            appointment = new Appointment();
            db.Appointments.Add(appointment);
        }
        else
        {
            appointment = await db.Appointments.FirstOrDefaultAsync(x => x.Id == id) ?? throw new WorkshopException("Запись не найдена.");
        }

        appointment.ClientId = clientId;
        appointment.CarId = carId;
        appointment.ServiceId = serviceId;
        appointment.MechanicId = mechanicId;
        appointment.RepairBayId = bayId;
        appointment.ScheduledAt = scheduledAt;
        appointment.Status = status;
        await SaveAsync(db);
    }

    public async Task<IReadOnlyList<ScheduleRow>> GetSchedulesAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        var rows = await db.MechanicSchedules.AsNoTracking()
            .Include(x => x.Mechanic).ThenInclude(x => x.User)
            .Include(x => x.RepairBay)
            .OrderBy(x => x.StartsAt)
            .ToListAsync();
        return rows.Select(x => new ScheduleRow
        {
            Id = x.Id,
            MechanicId = x.MechanicId,
            BayId = x.RepairBayId,
            Mechanic = x.Mechanic.User.FullName,
            Bay = x.RepairBay.Name,
            StartsAt = x.StartsAt,
            EndsAt = x.EndsAt,
            Starts = x.StartsAt.ToString("dd.MM.yyyy HH:mm"),
            Ends = x.EndsAt.ToString("dd.MM.yyyy HH:mm")
        }).ToList();
    }

    public async Task SaveScheduleAsync(int? id, int mechanicId, int bayId, DateTime startsAt, DateTime endsAt)
    {
        if (endsAt <= startsAt)
            throw new WorkshopException("Окончание смены должно быть позже начала.");
        await using var db = await _factory.CreateDbContextAsync();
        if (!await db.Mechanics.AnyAsync(x => x.Id == mechanicId) || !await db.RepairBays.AnyAsync(x => x.Id == bayId))
            throw new WorkshopException("Выберите механика и ремонтное место из списков.");

        MechanicSchedule schedule;
        if (id is null)
        {
            schedule = new MechanicSchedule();
            db.MechanicSchedules.Add(schedule);
        }
        else
        {
            schedule = await db.MechanicSchedules.FirstOrDefaultAsync(x => x.Id == id) ?? throw new WorkshopException("Запись не найдена.");
        }

        schedule.MechanicId = mechanicId;
        schedule.RepairBayId = bayId;
        schedule.StartsAt = startsAt;
        schedule.EndsAt = endsAt;
        await SaveAsync(db);
    }

    public async Task DeleteScheduleAsync(int id) => await DeleteAsync<MechanicSchedule>(id);

    public async Task<PageResult<WorkOrderRow>> GetWorkOrdersAsync(string? search, WorkOrderStatus? status, int? mechanicId, DateTime? day, int page, int pageSize)
    {
        await using var db = await _factory.CreateDbContextAsync();
        IQueryable<WorkOrder> query = db.WorkOrders.AsNoTracking()
            .Include(x => x.Mechanic).ThenInclude(x => x.User)
            .Include(x => x.Appointment).ThenInclude(x => x.Client)
            .Include(x => x.Appointment).ThenInclude(x => x.Car)
            .Include(x => x.Appointment).ThenInclude(x => x.Service);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = Like(search);
            query = query.Where(x => EF.Functions.ILike(x.Appointment.Client.FullName, pattern, "\\")
                || EF.Functions.ILike(x.Appointment.Car.LicensePlate, pattern, "\\"));
        }

        if (status is WorkOrderStatus orderStatus)
            query = query.Where(x => x.Status == orderStatus);
        if (mechanicId is int mechanic)
            query = query.Where(x => x.MechanicId == mechanic);
        if (day is DateTime date)
        {
            var start = date.Date;
            query = query.Where(x => x.CreatedAt >= start && x.CreatedAt < start.AddDays(1));
        }

        return await PageAsync(query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id), page, pageSize, x => new WorkOrderRow
        {
            Id = x.Id,
            CreatedAt = x.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
            Status = StatusText.WorkOrder(x.Status),
            Mechanic = x.Mechanic.User.FullName,
            ClientName = x.Appointment.Client.FullName,
            Car = x.Appointment.Car.LicensePlate,
            Service = x.Appointment.Service.Name
        });
    }

    public async Task<IReadOnlyList<LookupItem>> GetCompletedWorkOrdersAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.WorkOrders.AsNoTracking()
            .Where(x => x.Status == WorkOrderStatus.Завершён)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new LookupItem
            {
                Id = x.Id,
                Title = "№" + x.Id + " · " + x.Appointment.Client.FullName + " · " + x.Appointment.Car.LicensePlate
            }).ToListAsync();
    }

    public async Task<decimal> QuoteWorkOrderAsync(int workOrderId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var order = await db.WorkOrders.AsNoTracking()
            .Include(x => x.Services)
            .Include(x => x.Parts)
            .FirstOrDefaultAsync(x => x.Id == workOrderId) ?? throw new WorkshopException("Запись не найдена.");
        if (order.Status != WorkOrderStatus.Завершён)
            throw new WorkshopException("Счёт можно сформировать только по завершённому заказ-наряду.");
        return order.Services.Sum(x => x.Price) + order.Parts.Sum(x => x.UnitPrice * x.Quantity);
    }

    public async Task<IReadOnlyList<InvoiceRow>> GetInvoicesAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        var invoices = await db.Invoices.AsNoTracking()
            .Include(x => x.Payments)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
        return invoices.Select(MapInvoice).ToList();
    }

    public async Task CreateInvoiceAsync(int workOrderId, DateTime createdAt, decimal amountBeforeDiscount, decimal discount, decimal taxRate)
    {
        var (tax, total) = InvoiceMath.Calculate(amountBeforeDiscount, discount, taxRate);
        await using var db = await _factory.CreateDbContextAsync();
        var order = await db.WorkOrders.FirstOrDefaultAsync(x => x.Id == workOrderId) ?? throw new WorkshopException("Запись не найдена.");
        if (order.Status != WorkOrderStatus.Завершён)
            throw new WorkshopException("Счёт можно сформировать только по завершённому заказ-наряду.");

        db.Invoices.Add(new Invoice
        {
            WorkOrderId = workOrderId,
            CreatedAt = createdAt,
            AmountBeforeDiscount = amountBeforeDiscount,
            Discount = discount,
            TaxRate = taxRate,
            Tax = tax,
            Total = total,
            Status = InvoiceStatus.Выставлен
        });
        await SaveAsync(db);
    }

    public async Task RegisterPaymentAsync(int invoiceId, DateTime paidAt, decimal amount, PaymentMethod method, PaymentStatus status, string? transactionNumber)
    {
        var number = Required(transactionNumber, "Номер транзакции");
        if (amount <= 0)
            throw new WorkshopException("Сумма платежа должна быть больше нуля.");

        await using var db = await _factory.CreateDbContextAsync();
        var invoice = await db.Invoices.Include(x => x.Payments).FirstOrDefaultAsync(x => x.Id == invoiceId) ?? throw new WorkshopException("Запись не найдена.");
        var paid = invoice.Payments.Where(x => x.Status == PaymentStatus.Проведён).Sum(x => x.Amount);
        if (status == PaymentStatus.Проведён && amount > invoice.Total - paid)
            throw new WorkshopException("Сумма больше остатка по счёту.");

        db.Payments.Add(new Payment
        {
            InvoiceId = invoiceId,
            PaidAt = paidAt,
            Amount = amount,
            Method = method,
            Status = status,
            TransactionNumber = number
        });
        var conducted = paid + (status == PaymentStatus.Проведён ? amount : 0);
        invoice.Status = conducted <= 0
            ? InvoiceStatus.Выставлен
            : conducted < invoice.Total
                ? InvoiceStatus.ЧастичноОплачен
                : InvoiceStatus.Оплачен;
        await SaveAsync(db);
    }

    private static InvoiceRow MapInvoice(Invoice invoice)
    {
        var paid = invoice.Payments.Where(x => x.Status == PaymentStatus.Проведён).Sum(x => x.Amount);
        return new InvoiceRow
        {
            Id = invoice.Id,
            WorkOrderId = invoice.WorkOrderId,
            CreatedAt = invoice.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
            Status = StatusText.Invoice(invoice.Status),
            AmountBeforeDiscount = invoice.AmountBeforeDiscount,
            Discount = invoice.Discount,
            TaxRate = invoice.TaxRate,
            Tax = invoice.Tax,
            Total = invoice.Total,
            Paid = paid,
            Remainder = invoice.Total - paid
        };
    }
}
