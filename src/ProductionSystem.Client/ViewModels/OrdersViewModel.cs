using System.Collections.ObjectModel;
using ProductionSystem.Client;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductionSystem.Client.Models;
using ProductionSystem.Client.Services;

namespace ProductionSystem.Client.ViewModels;

public partial class OrdersViewModel : ViewModelBase
{
    private readonly BackendApi _api;

    public ObservableCollection<OrderListItemDto> Items { get; } = new();
    public ObservableCollection<string> FilterOptions { get; } = new()
    {
        "Все", "Новые", "Текущие", "Выполненные", "Отклонённые",
    };

    [ObservableProperty] private string _selectedFilter = "Все";
    [ObservableProperty] private OrderListItemDto? _selectedOrder;
    [ObservableProperty] private string? _statusMessage;
    [ObservableProperty] private string _statusComment = "";
    [ObservableProperty] private string _costText = "";
    [ObservableProperty] private DateTime? _plannedDate = DateTime.Today.AddDays(30);
    [ObservableProperty] private bool _showHistory;
    [ObservableProperty] private string _historyText = "";

    public string Role => _api.Role ?? "";

    public OrdersViewModel(BackendApi api)
    {
        _api = api;
        _ = RefreshAsync();
    }

    partial void OnSelectedFilterChanged(string value) => _ = RefreshAsync();

    [RelayCommand]
    public async Task RefreshAsync()
    {
        var filter = SelectedFilter switch
        {
            "Новые" => "new",
            "Текущие" => "current",
            "Выполненные" => "completed",
            "Отклонённые" => "rejected",
            _ => null,
        };

        var list = await _api.GetOrdersAsync(filter);
        Items.Clear();
        if (list != null)
        {
            foreach (var o in list)
                Items.Add(o);
        }
    }

    [RelayCommand]
    private async Task ShowHistoryAsync()
    {
        if (SelectedOrder is null)
            return;

        var hist = await _api.GetOrderHistoryAsync(SelectedOrder.Number);
        HistoryText = hist is null || hist.Count == 0
            ? "История пуста."
            : string.Join(Environment.NewLine,
                hist.Select(h => $"{h.ChangedAt:dd.MM.yyyy HH:mm} — {h.Status}" +
                                 (string.IsNullOrWhiteSpace(h.Comment) ? "" : $" ({h.Comment})")));
        ShowHistory = true;
    }

    [RelayCommand]
    private async Task CreateOrderAsync()
    {
        var owner = DialogService.TryGetMainWindow();
        if (owner is null)
            return;

        var dlg = new Views.OrderEditWindow(_api, null);
        await dlg.ShowDialog(owner);
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task EditOrderAsync()
    {
        if (SelectedOrder is null || SelectedOrder.Status != "Новый")
            return;

        var owner = DialogService.TryGetMainWindow();
        if (owner is null)
            return;

        var dlg = new Views.OrderEditWindow(_api, SelectedOrder.Number);
        await dlg.ShowDialog(owner);
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task DeleteOrderAsync()
    {
        if (SelectedOrder is null)
            return;

        var owner = DialogService.TryGetMainWindow();
        if (owner is null)
            return;

        if (!await DialogService.ConfirmAsync(owner, "Удалить заказ?"))
            return;

        var (ok, err) = await _api.DeleteOrderAsync(SelectedOrder.Number);
        StatusMessage = ok ? "Заказ удалён." : err;
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task ChangeStatusAsync(string targetStatus)
    {
        if (SelectedOrder is null)
            return;

        decimal? cost = null;
        if (decimal.TryParse(CostText.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var c))
            cost = c;

        DateOnly? planned = PlannedDate.HasValue
            ? DateOnly.FromDateTime(PlannedDate.Value)
            : null;

        var (ok, err) = await _api.ChangeOrderStatusAsync(SelectedOrder.Number, new OrderStatusChangeRequest
        {
            Status = targetStatus,
            Comment = string.IsNullOrWhiteSpace(StatusComment) ? null : StatusComment.Trim(),
            EstimatedCost = cost,
            PlannedCompletionDate = planned,
        });

        StatusMessage = ok ? $"Статус изменён на «{targetStatus}»." : err;
        if (ok)
            await RefreshAsync();
    }

    [RelayCommand]
    private async Task CancelOrderAsync()
    {
        if (SelectedOrder is null)
            return;

        var (ok, err) = await _api.CancelOrderByCustomerAsync(
            SelectedOrder.Number,
            string.IsNullOrWhiteSpace(StatusComment) ? null : StatusComment.Trim());
        StatusMessage = ok ? "Заказ отменён." : err;
        if (ok)
            await RefreshAsync();
    }

    public bool CanCreateOrder => Role is UserRoles.Customer or UserRoles.Manager;
    public bool CanEditSelected => SelectedOrder?.Status == "Новый" && CanCreateOrder;
    public bool CanDeleteSelected => CanEditSelected;
}
