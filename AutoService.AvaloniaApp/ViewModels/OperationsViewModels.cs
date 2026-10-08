using System.Collections.ObjectModel;
using AutoService.AvaloniaApp.Input;
using AutoService.Core.Enums;
using AutoService.Data.Workshop;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoService.AvaloniaApp.ViewModels;

// Пункт фильтра по статусу.
// Используется в моделях записей, заказ-нарядов и заказов механика.
public sealed class StatusChoice<T> where T : struct
{
    public string Title { get; init; } = "";
    public T? Value { get; init; }
    public override string ToString() => Title;
}

// Записи на обслуживание.
// Создаётся в AppComposition и показывается в AppointmentsView.
public partial class AppointmentsViewModel : PagedViewModel
{
    private readonly AdminService _admin;
    private bool _filling;

    public AppointmentsViewModel(AdminService admin)
    {
        _admin = admin;
        Statuses = [new StatusChoice<AppointmentStatus> { Title = "Все статусы" }, ..Enum.GetValues<AppointmentStatus>().Select(x => new StatusChoice<AppointmentStatus> { Title = x.ToString(), Value = x })];
        Status = Statuses[0];
        EditStatus = AppointmentStatus.Запланирована;
    }

    public ObservableCollection<AppointmentRow> Items { get; } = [];
    public ObservableCollection<LookupItem> Clients { get; } = [];
    public ObservableCollection<LookupItem> Cars { get; } = [];
    public ObservableCollection<LookupItem> Services { get; } = [];
    public ObservableCollection<LookupItem> Mechanics { get; } = [];
    public ObservableCollection<LookupItem> FilterMechanics { get; } = [];
    public ObservableCollection<LookupItem> Bays { get; } = [];
    public IReadOnlyList<StatusChoice<AppointmentStatus>> Statuses { get; }
    public AppointmentStatus[] EditStatuses { get; } = Enum.GetValues<AppointmentStatus>();

    [ObservableProperty] private string search = "";
    [ObservableProperty] private string day = "";
    [ObservableProperty] private StatusChoice<AppointmentStatus>? status;
    [ObservableProperty] private LookupItem? filterMechanic;
    [ObservableProperty] private AppointmentRow? selected;
    [ObservableProperty] private int? editingId;
    [ObservableProperty] private LookupItem? client;
    [ObservableProperty] private LookupItem? car;
    [ObservableProperty] private LookupItem? service;
    [ObservableProperty] private LookupItem? mechanic;
    [ObservableProperty] private LookupItem? bay;
    [ObservableProperty] private string when = "";
    [ObservableProperty] private AppointmentStatus editStatus;

    partial void OnClientChanged(LookupItem? value)
    {
        if (!_filling)
            _ = LoadCarsAsync(value?.Id);
    }

    partial void OnServiceChanged(LookupItem? value)
    {
        if (!_filling)
            _ = LoadMechanicsAsync(value?.Id);
    }

    partial void OnSelectedChanged(AppointmentRow? value)
    {
        if (value is null)
            return;
        _filling = true;
        EditingId = value.Id;
        Client = Clients.FirstOrDefault(x => x.Id == value.ClientId);
        Service = Services.FirstOrDefault(x => x.Id == value.ServiceId);
        Bay = Bays.FirstOrDefault(x => x.Id == value.BayId);
        When = InputValues.Format(value.ScheduledAt);
        EditStatus = value.StatusValue;
        _filling = false;
        _ = SelectCarAndMechanicAsync(value.CarId, value.MechanicId);
    }

    public override Task LoadAsync() => RunAsync(async () =>
    {
        await Fill(Clients, () => _admin.GetClientsLookupAsync());
        await Fill(Services, () => _admin.GetServicesLookupAsync());
        await Fill(Bays, () => _admin.GetBaysAsync());
        await Fill(FilterMechanics, () => _admin.GetMechanicsAsync(null));
        DateTime? date = null;
        if (!string.IsNullOrWhiteSpace(Day))
        {
            if (!InputValues.TryDate(Day, out var parsed))
                throw new WorkshopException("Дата фильтра: дд.мм.гггг или дд.мм.гггг чч:мм.");
            date = Day.Trim().Length <= 10 ? parsed.Date : parsed;
        }

        var result = await _admin.GetAppointmentsAsync(Search, Status?.Value, FilterMechanic?.Id, date, Page, PageSize);
        ApplyPage(result, items =>
        {
            Items.Clear();
            foreach (var item in items)
                Items.Add(item);
        });
        MarkReady();
    });

    [RelayCommand]
    private async Task SearchAsync()
    {
        Page = 1;
        await LoadAsync();
    }

    [RelayCommand]
    private void NewItem()
    {
        Selected = null;
        EditingId = null;
        Client = Car = Service = Mechanic = Bay = null;
        When = "";
        EditStatus = AppointmentStatus.Запланирована;
        Cars.Clear();
        Mechanics.Clear();
        Error = "";
    }

    [RelayCommand]
    private Task SaveAsync() => RunAsync(async () =>
    {
        if (Client is null || Car is null || Service is null || Mechanic is null || Bay is null)
            throw new WorkshopException("Заполните клиента, автомобиль, услугу, механика и место.");
        if (!InputValues.TryDate(When, out var scheduled) || When.Trim().Length <= 10)
            throw new WorkshopException("Дата и время: дд.мм.гггг чч:мм.");
        await _admin.SaveAppointmentAsync(EditingId, Client.Id, Car.Id, Service.Id, Mechanic.Id, Bay.Id, scheduled, EditStatus);
        await LoadAsync();
    });

    private async Task LoadCarsAsync(int? clientId)
    {
        Cars.Clear();
        if (clientId is not int id)
            return;
        foreach (var item in await _admin.GetCarsByClientAsync(id))
            Cars.Add(item);
    }

    private async Task LoadMechanicsAsync(int? serviceId)
    {
        Mechanics.Clear();
        if (serviceId is not int id)
            return;
        foreach (var item in await _admin.GetMechanicsAsync(id))
            Mechanics.Add(item);
    }

    private async Task SelectCarAndMechanicAsync(int carId, int mechanicId)
    {
        await LoadCarsAsync(Client?.Id);
        await LoadMechanicsAsync(Service?.Id);
        Car = Cars.FirstOrDefault(x => x.Id == carId);
        Mechanic = Mechanics.FirstOrDefault(x => x.Id == mechanicId);
    }

    private static async Task Fill(ObservableCollection<LookupItem> target, Func<Task<IReadOnlyList<LookupItem>>> load)
    {
        var items = await load();
        target.Clear();
        foreach (var item in items)
            target.Add(item);
    }
}

// Смены механиков на ремонтных местах.
// Создаётся в AppComposition и показывается в SchedulesView.
public partial class SchedulesViewModel : AppPageViewModel
{
    private readonly AdminService _admin;

    public SchedulesViewModel(AdminService admin) => _admin = admin;

    public ObservableCollection<ScheduleRow> Items { get; } = [];
    public ObservableCollection<LookupItem> Mechanics { get; } = [];
    public ObservableCollection<LookupItem> Bays { get; } = [];

    [ObservableProperty] private ScheduleRow? selected;
    [ObservableProperty] private int? editingId;
    [ObservableProperty] private LookupItem? mechanic;
    [ObservableProperty] private LookupItem? bay;
    [ObservableProperty] private string starts = "";
    [ObservableProperty] private string ends = "";

    partial void OnSelectedChanged(ScheduleRow? value)
    {
        if (value is null)
            return;
        EditingId = value.Id;
        Mechanic = Mechanics.FirstOrDefault(x => x.Id == value.MechanicId);
        Bay = Bays.FirstOrDefault(x => x.Id == value.BayId);
        Starts = value.Starts;
        Ends = value.Ends;
    }

    public override Task LoadAsync() => RunAsync(async () =>
    {
        Mechanics.Clear();
        foreach (var item in await _admin.GetMechanicsAsync(null))
            Mechanics.Add(item);
        Bays.Clear();
        foreach (var item in await _admin.GetBaysAsync())
            Bays.Add(item);
        Items.Clear();
        foreach (var item in await _admin.GetSchedulesAsync())
            Items.Add(item);
    });

    [RelayCommand]
    private void NewItem()
    {
        Selected = null;
        EditingId = null;
        Mechanic = Bay = null;
        Starts = Ends = "";
        Error = "";
    }

    [RelayCommand]
    private Task SaveAsync() => RunAsync(async () =>
    {
        if (Mechanic is null || Bay is null)
            throw new WorkshopException("Выберите механика и ремонтное место.");
        if (!InputValues.TryDate(Starts, out var startsAt) || !InputValues.TryDate(Ends, out var endsAt))
            throw new WorkshopException("Время смены: дд.мм.гггг чч:мм.");
        await _admin.SaveScheduleAsync(EditingId, Mechanic.Id, Bay.Id, startsAt, endsAt);
        await LoadAsync();
    });

    [RelayCommand]
    private Task DeleteAsync() => RunAsync(async () =>
    {
        if (EditingId is not int id)
            throw new WorkshopException("Выберите смену.");
        await _admin.DeleteScheduleAsync(id);
        NewItem();
        await LoadAsync();
    });
}

// Просмотр заказ-нарядов администратором.
// Создаётся в AppComposition и показывается в WorkOrdersView.
public partial class WorkOrdersViewModel : PagedViewModel
{
    private readonly AdminService _admin;

    public WorkOrdersViewModel(AdminService admin)
    {
        _admin = admin;
        Statuses = [new StatusChoice<WorkOrderStatus> { Title = "Все статусы" }, ..Enum.GetValues<WorkOrderStatus>().Select(x => new StatusChoice<WorkOrderStatus> { Title = StatusText.WorkOrder(x), Value = x })];
        Status = Statuses[0];
    }

    public ObservableCollection<WorkOrderRow> Items { get; } = [];
    public ObservableCollection<LookupItem> Mechanics { get; } = [];
    public IReadOnlyList<StatusChoice<WorkOrderStatus>> Statuses { get; }

    [ObservableProperty] private string search = "";
    [ObservableProperty] private string day = "";
    [ObservableProperty] private StatusChoice<WorkOrderStatus>? status;
    [ObservableProperty] private LookupItem? mechanic;

    public override Task LoadAsync() => RunAsync(async () =>
    {
        Mechanics.Clear();
        foreach (var item in await _admin.GetMechanicsAsync(null))
            Mechanics.Add(item);
        DateTime? date = null;
        if (!string.IsNullOrWhiteSpace(Day))
        {
            if (!InputValues.TryDate(Day, out var parsed))
                throw new WorkshopException("Дата фильтра: дд.мм.гггг.");
            date = parsed.Date;
        }

        var result = await _admin.GetWorkOrdersAsync(Search, Status?.Value, Mechanic?.Id, date, Page, PageSize);
        ApplyPage(result, items =>
        {
            Items.Clear();
            foreach (var item in items)
                Items.Add(item);
        });
        MarkReady();
    });

    [RelayCommand]
    private async Task SearchAsync()
    {
        Page = 1;
        await LoadAsync();
    }
}

// Счета и регистрация платежей.
// Создаётся в AppComposition и показывается в InvoicesView.
public partial class InvoicesViewModel : AppPageViewModel
{
    private readonly AdminService _admin;

    public InvoicesViewModel(AdminService admin)
    {
        _admin = admin;
        Method = PaymentMethod.Карта;
        PaymentStatus = PaymentStatus.Проведён;
    }

    public ObservableCollection<InvoiceRow> Items { get; } = [];
    public ObservableCollection<LookupItem> Orders { get; } = [];
    public PaymentMethod[] Methods { get; } = Enum.GetValues<PaymentMethod>();
    public PaymentStatus[] PaymentStatuses { get; } = Enum.GetValues<PaymentStatus>();

    [ObservableProperty] private LookupItem? order;
    [ObservableProperty] private string invoiceDate = "";
    [ObservableProperty] private string amount = "";
    [ObservableProperty] private string discount = "0";
    [ObservableProperty] private string taxRate = "20";
    [ObservableProperty] private string taxText = "";
    [ObservableProperty] private string totalText = "";
    [ObservableProperty] private InvoiceRow? invoice;
    [ObservableProperty] private string paymentDate = "";
    [ObservableProperty] private string paymentAmount = "";
    [ObservableProperty] private string transaction = "";
    [ObservableProperty] private PaymentMethod method;
    [ObservableProperty] private PaymentStatus paymentStatus;

    partial void OnOrderChanged(LookupItem? value) => _ = QuoteAsync(value?.Id);

    public override Task LoadAsync() => RunAsync(async () =>
    {
        Orders.Clear();
        foreach (var item in await _admin.GetCompletedWorkOrdersAsync())
            Orders.Add(item);
        Items.Clear();
        foreach (var item in await _admin.GetInvoicesAsync())
            Items.Add(item);
        if (string.IsNullOrWhiteSpace(InvoiceDate))
            InvoiceDate = InputValues.Format(DateTime.Now);
        if (string.IsNullOrWhiteSpace(PaymentDate))
            PaymentDate = InputValues.Format(DateTime.Now);
    });

    [RelayCommand]
    private void Recalculate()
    {
        if (!InputValues.TryDecimal(Amount, out var amountValue) || !InputValues.TryDecimal(Discount, out var discountValue) || !InputValues.TryDecimal(TaxRate, out var rate))
        {
            Error = "Сумма, скидка и ставка налога должны быть числами.";
            return;
        }

        try
        {
            var (tax, total) = InvoiceMath.Calculate(amountValue, discountValue, rate);
            TaxText = tax.ToString("0.00");
            TotalText = total.ToString("0.00");
            Error = "";
        }
        catch (WorkshopException ex)
        {
            Error = ex.Message;
        }
    }

    [RelayCommand]
    private Task CreateInvoiceAsync() => RunAsync(async () =>
    {
        if (Order is null)
            throw new WorkshopException("Выберите завершённый заказ-наряд.");
        if (!InputValues.TryDate(InvoiceDate, out var created) || !InputValues.TryDecimal(Amount, out var amountValue)
            || !InputValues.TryDecimal(Discount, out var discountValue) || !InputValues.TryDecimal(TaxRate, out var rate))
            throw new WorkshopException("Проверьте дату, сумму, скидку и ставку налога.");
        await _admin.CreateInvoiceAsync(Order.Id, created, amountValue, discountValue, rate);
        await LoadAsync();
    });

    [RelayCommand]
    private Task PayAsync() => RunAsync(async () =>
    {
        if (Invoice is null)
            throw new WorkshopException("Выберите счёт.");
        if (!InputValues.TryDate(PaymentDate, out var paidAt) || !InputValues.TryDecimal(PaymentAmount, out var amountValue))
            throw new WorkshopException("Проверьте дату и сумму платежа.");
        await _admin.RegisterPaymentAsync(Invoice.Id, paidAt, amountValue, Method, PaymentStatus, Transaction);
        await LoadAsync();
    });

    private async Task QuoteAsync(int? workOrderId)
    {
        if (workOrderId is not int id)
            return;
        try
        {
            var quote = await _admin.QuoteWorkOrderAsync(id);
            Amount = quote.ToString("0.00");
            Recalculate();
        }
        catch (WorkshopException ex)
        {
            Error = ex.Message;
        }
    }
}
