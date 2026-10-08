using System.Collections.ObjectModel;
using AutoService.AvaloniaApp.Input;
using AppSession = AutoService.AvaloniaApp.AppSession;
using AutoService.Core.Enums;
using AutoService.Data.Workshop;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoService.AvaloniaApp.ViewModels;

// Заказ-наряды текущего механика.
// Создаётся в AppComposition и показывается в MechanicOrdersView.
public partial class MechanicOrdersViewModel : AppPageViewModel
{
    private readonly AppSession _session;
    private readonly MechanicWorkService _mechanic;
    private bool _refreshing;
    private int? _keepOrderId;

    public MechanicOrdersViewModel(AppSession session, MechanicWorkService mechanic)
    {
        _session = session;
        _mechanic = mechanic;
        Statuses = Enum.GetValues<WorkOrderStatus>()
            .Select(x => new StatusChoice<WorkOrderStatus> { Title = StatusText.WorkOrder(x), Value = x })
            .ToArray();
    }

    public ObservableCollection<OpenAppointmentRow> Appointments { get; } = [];
    public ObservableCollection<MechanicOrderRow> Orders { get; } = [];
    public ObservableCollection<LookupItem> Services { get; } = [];
    public ObservableCollection<LookupItem> Parts { get; } = [];
    public ObservableCollection<OrderLineRow> ServiceLines { get; } = [];
    public ObservableCollection<OrderLineRow> PartLines { get; } = [];
    public IReadOnlyList<StatusChoice<WorkOrderStatus>> Statuses { get; }

    [ObservableProperty] private OpenAppointmentRow? selectedAppointment;
    [ObservableProperty] private MechanicOrderRow? selectedOrder;
    [ObservableProperty] private string carText = "";
    [ObservableProperty] private string fault = "";
    [ObservableProperty] private string engine = "";
    [ObservableProperty] private string brakes = "";
    [ObservableProperty] private string suspension = "";
    [ObservableProperty] private string electrical = "";
    [ObservableProperty] private string recommendations = "";
    [ObservableProperty] private bool canEditLines;
    [ObservableProperty] private StatusChoice<WorkOrderStatus>? selectedStatus;
    [ObservableProperty] private LookupItem? selectedService;
    [ObservableProperty] private LookupItem? selectedPart;
    [ObservableProperty] private string quantity = "1";
    [ObservableProperty] private OrderLineRow? selectedServiceLine;
    [ObservableProperty] private OrderLineRow? selectedPartLine;

    public bool HasOrder => SelectedOrder is not null;

    partial void OnSelectedOrderChanged(MechanicOrderRow? value)
    {
        OnPropertyChanged(nameof(HasOrder));
        if (_refreshing)
            return;
        _ = LoadDetailsAsync(value);
    }

    public override Task LoadAsync() => ReloadAsync();

    [RelayCommand]
    private Task CreateOrderAsync() => RunAsync(async () =>
    {
        if (SelectedAppointment is null)
            throw new WorkshopException("Выберите запись без заказ-наряда.");
        _keepOrderId = await _mechanic.CreateWorkOrderAsync(UserId, SelectedAppointment.Id);
        await ReloadAsync();
    });

    [RelayCommand]
    private Task SaveInspectionAsync() => RunAsync(async () =>
    {
        if (SelectedOrder is null)
            throw new WorkshopException("Выберите заказ-наряд.");
        await _mechanic.SaveInspectionAsync(UserId, SelectedOrder.Id, Fault, Engine, Brakes, Suspension, Electrical, Recommendations);
        await ReloadAsync();
    });

    [RelayCommand]
    private Task SaveStatusAsync() => RunAsync(async () =>
    {
        if (SelectedOrder is null || SelectedStatus?.Value is not WorkOrderStatus status)
            throw new WorkshopException("Выберите заказ-наряд и статус.");
        await _mechanic.ChangeStatusAsync(UserId, SelectedOrder.Id, status);
        await ReloadAsync();
    });

    [RelayCommand]
    private Task AddServiceAsync() => RunAsync(async () =>
    {
        if (SelectedOrder is null || SelectedService is null)
            throw new WorkshopException("Выберите заказ-наряд и услугу.");
        await _mechanic.AddServiceAsync(UserId, SelectedOrder.Id, SelectedService.Id);
        await ReloadAsync();
    });

    [RelayCommand]
    private Task RemoveServiceAsync() => RunAsync(async () =>
    {
        if (SelectedOrder is null || SelectedServiceLine is null)
            throw new WorkshopException("Выберите услугу в заказ-наряде.");
        await _mechanic.RemoveServiceAsync(UserId, SelectedOrder.Id, SelectedServiceLine.Id);
        await ReloadAsync();
    });

    [RelayCommand]
    private Task AddPartAsync() => RunAsync(async () =>
    {
        if (SelectedOrder is null || SelectedPart is null)
            throw new WorkshopException("Выберите заказ-наряд и запчасть.");
        if (!InputValues.TryInt(Quantity, out var count))
            throw new WorkshopException("Количество должно быть целым числом.");
        await _mechanic.AddPartAsync(UserId, SelectedOrder.Id, SelectedPart.Id, count);
        await ReloadAsync();
    });

    [RelayCommand]
    private Task RemovePartAsync() => RunAsync(async () =>
    {
        if (SelectedOrder is null || SelectedPartLine is null)
            throw new WorkshopException("Выберите запчасть в заказ-наряде.");
        await _mechanic.RemovePartAsync(UserId, SelectedOrder.Id, SelectedPartLine.Id);
        await ReloadAsync();
    });

    private async Task ReloadAsync()
    {
        await RunAsync(async () =>
        {
            var keep = _keepOrderId ?? SelectedOrder?.Id;
            _keepOrderId = null;
            _refreshing = true;
            Appointments.Clear();
            foreach (var item in await _mechanic.GetOpenAppointmentsAsync(UserId))
                Appointments.Add(item);
            Orders.Clear();
            foreach (var item in await _mechanic.GetMyOrdersAsync(UserId))
                Orders.Add(item);
            Services.Clear();
            foreach (var item in await _mechanic.GetMyServicesAsync(UserId))
                Services.Add(item);
            Parts.Clear();
            foreach (var item in await _mechanic.GetPartsAsync())
                Parts.Add(item);
            SelectedOrder = Orders.FirstOrDefault(x => x.Id == keep);
            _refreshing = false;
            if (SelectedOrder is null)
                ClearDetails();
            else
                await LoadDetailsAsync(SelectedOrder);
        });
    }

    private async Task LoadDetailsAsync(MechanicOrderRow? order)
    {
        if (order is null)
        {
            ClearDetails();
            return;
        }

        await RunAsync(async () =>
        {
            var details = await _mechanic.GetDetailsAsync(UserId, order.Id);
            CarText = details.CarText;
            Fault = details.Fault;
            Engine = details.Engine;
            Brakes = details.Brakes;
            Suspension = details.Suspension;
            Electrical = details.Electrical;
            Recommendations = details.Recommendations;
            CanEditLines = details.CanEditLines;
            SelectedStatus = Statuses.FirstOrDefault(x => x.Value == order.StatusValue);
            ServiceLines.Clear();
            foreach (var line in details.Services)
                ServiceLines.Add(line);
            PartLines.Clear();
            foreach (var line in details.Parts)
                PartLines.Add(line);
        });
    }

    private void ClearDetails()
    {
        CarText = Fault = Engine = Brakes = Suspension = Electrical = Recommendations = "";
        CanEditLines = false;
        SelectedStatus = null;
        ServiceLines.Clear();
        PartLines.Clear();
    }

    private int UserId => _session.User?.Id ?? throw new WorkshopException("Сессия истекла. Войдите снова.");
}
