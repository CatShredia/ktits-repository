using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductionSystem.Client.Models;
using ProductionSystem.Client.Services;

namespace ProductionSystem.Client.ViewModels;

public partial class ComponentsViewModel : ViewModelBase
{
    private readonly BackendApi _api;
    private readonly bool _canEdit;

    public ObservableCollection<WarehouseOption> WarehouseFilters { get; } = new();
    public ObservableCollection<ComponentDto> Items { get; } = new();

    [ObservableProperty] private WarehouseOption? _selectedWarehouseFilter;
    [ObservableProperty] private ComponentDto? _selectedItem;
    [ObservableProperty] private ComponentUpdateRequest _editor = new();
    [ObservableProperty] private string _editQuantityText = "0";
    [ObservableProperty] private string _editPurchasePriceText = "0";
    [ObservableProperty] private string _editWeightText = "0";
    [ObservableProperty] private string _editWarehouseIdText = "1";
    [ObservableProperty] private string _summaryText = "";

    public bool CanEdit => _canEdit;

    public ComponentsViewModel(BackendApi api, bool canEdit)
    {
        _api = api;
        _canEdit = canEdit;
        WarehouseFilters.Add(new WarehouseOption(null, "Все склады"));
        _ = InitAsync();
    }

    private async Task InitAsync()
    {
        var list = await _api.GetWarehousesAsync();
        if (list != null)
        {
            foreach (var w in list)
                WarehouseFilters.Add(new WarehouseOption(w.Id, w.Name));
        }

        SelectedWarehouseFilter = WarehouseFilters.FirstOrDefault();
        await RefreshAsync();
    }

    partial void OnSelectedWarehouseFilterChanged(WarehouseOption? value) => _ = RefreshAsync();

    partial void OnSelectedItemChanged(ComponentDto? value)
    {
        if (value == null)
            return;

        Editor = new ComponentUpdateRequest
        {
            Name = value.Name,
            Unit = value.Unit,
            Quantity = value.Quantity,
            ComponentType = value.ComponentType,
            PurchasePrice = value.PurchasePrice,
            Weight = value.Weight,
            WarehouseId = value.WarehouseId,
        };

        EditQuantityText = value.Quantity.ToString(CultureInfo.InvariantCulture);
        EditPurchasePriceText = value.PurchasePrice.ToString(CultureInfo.InvariantCulture);
        EditWeightText = value.Weight.ToString(CultureInfo.InvariantCulture);
        EditWarehouseIdText = value.WarehouseId.ToString(CultureInfo.InvariantCulture);
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        var wid = SelectedWarehouseFilter?.Id;
        var data = await _api.GetComponentsAsync(wid);
        if (data == null)
            return;

        Items.Clear();
        foreach (var m in data.Items)
            Items.Add(m);

        SummaryText =
            $"Позиций в выборке: {data.FilteredPositionCount}  |  Всего в БД: {data.TotalPositionsInDatabase}  |  Суммарное количество (выборка): {data.FilteredTotalQuantity}  |  Закупочная стоимость (выборка): {data.FilteredTotalPurchaseCost:F2}";
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (SelectedItem == null || !_canEdit)
            return;

        if (!decimal.TryParse(EditQuantityText.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var qty))
        {
            await ShowErrorAsync("Некорректное количество.");
            return;
        }

        if (!decimal.TryParse(EditPurchasePriceText.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
        {
            await ShowErrorAsync("Некорректная цена.");
            return;
        }

        if (!decimal.TryParse(EditWeightText.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var weight))
        {
            await ShowErrorAsync("Некорректный вес.");
            return;
        }

        if (!int.TryParse(EditWarehouseIdText.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var wid))
        {
            await ShowErrorAsync("Некорректный идентификатор склада.");
            return;
        }

        Editor.Quantity = qty;
        Editor.PurchasePrice = price;
        Editor.Weight = weight;
        Editor.WarehouseId = wid;

        var (ok, err) = await _api.UpdateComponentAsync(SelectedItem.Article, Editor);
        if (!ok)
        {
            await ShowErrorAsync(err);
            return;
        }

        await RefreshAsync();
    }

    [RelayCommand]
    public async Task DeleteAsync()
    {
        if (SelectedItem == null || !_canEdit)
            return;

        if (SelectedItem.Quantity != 0)
        {
            await ShowErrorAsync("Удаление возможно только при нулевом количестве на складе.");
            return;
        }

        var owner = DialogService.TryGetMainWindow();
        if (owner == null)
            return;

        if (!await DialogService.ConfirmAsync(owner, "Удалить выбранную позицию?"))
            return;

        var (ok, err) = await _api.DeleteComponentAsync(SelectedItem.Article);
        if (!ok)
        {
            await ShowErrorAsync(err);
            return;
        }

        await RefreshAsync();
    }

    private static async Task ShowErrorAsync(string? message)
    {
        var owner = DialogService.TryGetMainWindow();
        if (owner != null)
            await DialogService.ShowInfoAsync(owner, message ?? "Ошибка");
    }
}
