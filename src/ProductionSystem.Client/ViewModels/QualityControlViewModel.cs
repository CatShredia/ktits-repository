using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductionSystem.Client.Models;
using ProductionSystem.Client.Services;

namespace ProductionSystem.Client.ViewModels;

public partial class QualityControlViewModel : ViewModelBase
{
    private readonly BackendApi _api;

    public ObservableCollection<OrderListItemDto> Orders { get; } = new();
    public ObservableCollection<QualityCheckDto> Checks { get; } = new();

    [ObservableProperty] private OrderListItemDto? _selectedOrder;
    [ObservableProperty] private string _parameterName = "";
    public ObservableCollection<string> Grades { get; } = new() { "+", "-" };

    [ObservableProperty] private string _grade = "+";
    [ObservableProperty] private string _comment = "";
    [ObservableProperty] private string? _statusMessage;

    public QualityControlViewModel(BackendApi api)
    {
        _api = api;
        _ = LoadOrdersAsync();
    }

    partial void OnSelectedOrderChanged(OrderListItemDto? value) => _ = LoadChecksAsync();

    private async Task LoadOrdersAsync()
    {
        var list = await _api.GetOrdersAsync("current");
        Orders.Clear();
        if (list != null)
        {
            foreach (var o in list.Where(x => x.Status is "Контроль" or "Производство"))
                Orders.Add(o);
        }
    }

    private async Task LoadChecksAsync()
    {
        Checks.Clear();
        if (SelectedOrder is null)
            return;

        var list = await _api.GetQualityChecksAsync(SelectedOrder.Number);
        if (list != null)
        {
            foreach (var c in list)
                Checks.Add(c);
        }
    }

    [RelayCommand]
    public async Task SaveCheckAsync()
    {
        if (SelectedOrder is null || string.IsNullOrWhiteSpace(ParameterName))
        {
            StatusMessage = "Выберите заказ и укажите параметр.";
            return;
        }

        var (ok, err) = await _api.UpsertQualityCheckAsync(
            SelectedOrder.Number,
            ParameterName.Trim(),
            Grade.Trim(),
            string.IsNullOrWhiteSpace(Comment) ? null : Comment.Trim());

        StatusMessage = ok ? "Сохранено." : err;
        if (ok)
        {
            ParameterName = "";
            Comment = "";
            await LoadChecksAsync();
        }
    }
}
