using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductionSystem.Client.Models;
using ProductionSystem.Client.Services;

namespace ProductionSystem.Client.ViewModels;

public partial class OrderPlanningViewModel : ViewModelBase
{
    private readonly BackendApi _api;
    private readonly string _orderNumber;

    public ObservableCollection<ProcurementLineDto> ProcurementLines { get; } = new();
    public ObservableCollection<GanttBarDto> GanttBars { get; } = new();

    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private string _summaryText = "";
    [ObservableProperty] private string _productName = "";
    [ObservableProperty] private int _totalMinutes;
    [ObservableProperty] private decimal _totalProcurementCost;

    public string Title => $"Планирование заказа {_orderNumber}";

    public OrderPlanningViewModel(BackendApi api, string orderNumber)
    {
        _api = api;
        _orderNumber = orderNumber;
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        var data = await _api.GetOrderPlanningAsync(_orderNumber);
        if (data is null)
        {
            ErrorMessage = "Не удалось загрузить оценку (проверьте статус заказа и спецификацию изделия).";
            return;
        }

        ProductName = data.ProductName;
        TotalMinutes = data.TotalMinutes;
        TotalProcurementCost = data.TotalProcurementCost;

        ProcurementLines.Clear();
        foreach (var l in data.ProcurementLines)
            ProcurementLines.Add(l);

        GanttBars.Clear();
        foreach (var b in data.GanttBars)
            GanttBars.Add(b);

        var prodHours = data.ProductionMinutes / 60.0;
        var totalHours = data.TotalMinutes / 60.0;
        SummaryText =
            $"Изделие: {data.ProductName}  |  Себестоимость закупки недостающего: {data.TotalProcurementCost:F2} руб.  |  " +
            $"Доставка (недостающее): {data.MinDeliveryDaysForShortage} дн.  |  " +
            $"Производство: {prodHours:F1} ч  |  Итого с доставкой: {totalHours:F1} ч ({data.TotalMinutes} мин)";
    }

    [RelayCommand]
    private async Task PrintAsync()
    {
        var owner = DialogService.TryGetMainWindow();
        if (owner is null)
            return;

        await PrintPreviewService.ShowAsync(owner, Title, BuildPrintText());
    }

    private string BuildPrintText()
    {
        var sb = new StringBuilder();
        sb.AppendLine(Title);
        sb.AppendLine(SummaryText);
        sb.AppendLine();
        sb.AppendLine("=== Закупка ===");
        foreach (var l in ProcurementLines)
        {
            sb.AppendLine($"{l.Kind}\t{l.Article}\t{l.Name}\tнужно {l.RequiredQuantity}\tесть {l.AvailableQuantity}\t" +
                          $"недостача {l.ShortageQuantity}\tцена {l.PurchasePrice}\tсумма {l.LineCost}\tдоставка {l.DeliveryDays} дн.");
        }

        sb.AppendLine();
        sb.AppendLine("=== Диаграмма Ганта ===");
        foreach (var b in GanttBars.OrderBy(x => x.EquipmentMarking ?? "фон").ThenBy(x => x.StartMinutes))
        {
            sb.AppendLine($"{b.EquipmentMarking ?? "—"}\t{b.ProductName}\t{b.OperationName}\t" +
                          $"{b.StartMinutes}-{b.EndMinutes} мин{(b.IsBackground ? " (фон)" : "")}");
        }

        return sb.ToString();
    }
}
