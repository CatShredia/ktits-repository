using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductionSystem.Client.Models;
using ProductionSystem.Client.Services;

namespace ProductionSystem.Client.ViewModels;

public partial class InventoryReportViewModel : ViewModelBase
{
    private readonly BackendApi _api;

    public ObservableCollection<string> KindOptions { get; } = new() { "Материалы", "Комплектующие" };
    public ObservableCollection<string> TypeOptions { get; } = new() { "Все" };
    public ObservableCollection<InventoryWarehouseGroupDto> Warehouses { get; } = new();

    [ObservableProperty] private string _selectedKind = "Материалы";
    [ObservableProperty] private string _selectedType = "Все";
    [ObservableProperty] private string _summaryText = "";

    public InventoryReportViewModel(BackendApi api)
    {
        _api = api;
        _ = RefreshTypesAsync();
        _ = RefreshAsync();
    }

    partial void OnSelectedKindChanged(string value) => _ = RefreshTypesAsync();

    [RelayCommand]
    private async Task RefreshTypesAsync()
    {
        var kind = SelectedKind == "Материалы" ? "materials" : "components";
        var types = await _api.GetInventoryReportTypesAsync(kind);
        TypeOptions.Clear();
        TypeOptions.Add("Все");
        if (types != null)
        {
            foreach (var t in types.Where(t => !string.IsNullOrWhiteSpace(t)))
                TypeOptions.Add(t);
        }

        if (!TypeOptions.Contains(SelectedType))
            SelectedType = "Все";
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        var kind = SelectedKind == "Материалы" ? "materials" : "components";
        var type = SelectedType == "Все" ? null : SelectedType;
        var report = await _api.GetInventoryReportAsync(kind, type);
        Warehouses.Clear();
        if (report is null)
        {
            SummaryText = "Не удалось загрузить отчёт.";
            return;
        }

        foreach (var w in report.Warehouses)
            Warehouses.Add(w);

        SummaryText =
            $"{report.Kind}, тип: {report.TypeFilter}  |  Складов: {report.Warehouses.Count}  |  Итого количество: {report.GrandTotalQuantity}";
    }

    [RelayCommand]
    private async Task PrintAsync()
    {
        var owner = DialogService.TryGetMainWindow();
        if (owner is null)
            return;

        var sb = new StringBuilder();
        sb.AppendLine($"Отчёт по остаткам — {SelectedKind}");
        sb.AppendLine(SummaryText);
        sb.AppendLine();
        foreach (var wh in Warehouses)
        {
            sb.AppendLine($"--- Склад: {wh.WarehouseName} (итого: {wh.WarehouseTotalQuantity}) ---");
            foreach (var l in wh.Lines)
                sb.AppendLine($"{l.Article}\t{l.Name}\t{l.Type}\t{l.Quantity} {l.Unit}\tцена {l.PurchasePrice}");
            sb.AppendLine();
        }

        await PrintPreviewService.ShowAsync(owner, "Отчёт по остаткам", sb.ToString());
    }
}
