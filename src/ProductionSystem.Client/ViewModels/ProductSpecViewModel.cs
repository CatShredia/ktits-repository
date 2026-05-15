using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductionSystem.Client.Models;
using ProductionSystem.Client.Services;

namespace ProductionSystem.Client.ViewModels;

public partial class ProductSpecViewModel : ViewModelBase
{
    private readonly BackendApi _api;
    private readonly string _productName;
    private readonly bool _canEdit;

    public ObservableCollection<ProductDrawingDto> Drawings { get; } = new();
    public ObservableCollection<ProductMeasurementDto> Measurements { get; } = new();
    public ObservableCollection<ProductMaterialLineDto> Materials { get; } = new();
    public ObservableCollection<ProductComponentLineDto> Components { get; } = new();
    public ObservableCollection<ProductAssemblyLineDto> Assemblies { get; } = new();
    public ObservableCollection<ProductOperationLineDto> Operations { get; } = new();
    public ObservableCollection<string> ProductNames { get; } = new();
    public ObservableCollection<string> EquipmentTypes { get; } = new();
    public ObservableCollection<OperationCatalogItem> OperationCatalog { get; } = new();

    [ObservableProperty] private string _dimensions = "";
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private string _drawingTitle = "";
    [ObservableProperty] private string _drawingSource = "Конструктор";
    [ObservableProperty] private ProductDrawingDto? _selectedDrawing;

    public string Title => $"Спецификация: {_productName}";
    public bool CanEdit => _canEdit;
    public string ProductName => _productName;

    public ProductSpecViewModel(BackendApi api, string productName, bool canEdit)
    {
        _api = api;
        _productName = productName;
        _canEdit = canEdit;
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        var products = await _api.GetProductsAsync();
        ProductNames.Clear();
        if (products != null)
        {
            foreach (var p in products)
                ProductNames.Add(p.Name);
        }

        var catalog = await _api.GetOperationsCatalogAsync();
        EquipmentTypes.Clear();
        OperationCatalog.Clear();
        if (catalog != null)
        {
            foreach (var t in catalog.EquipmentTypes)
                EquipmentTypes.Add(t);
            foreach (var o in catalog.Operations)
                OperationCatalog.Add(o);
        }

        var detail = await _api.GetProductAsync(_productName);
        if (detail is null)
        {
            ErrorMessage = "Не удалось загрузить спецификацию.";
            return;
        }

        Dimensions = detail.Dimensions;
        Drawings.Clear();
        foreach (var d in detail.Drawings) Drawings.Add(d);
        Measurements.Clear();
        foreach (var m in detail.Measurements) Measurements.Add(m);
        Materials.Clear();
        foreach (var m in detail.Materials) Materials.Add(m);
        Components.Clear();
        foreach (var c in detail.Components) Components.Add(c);
        Assemblies.Clear();
        foreach (var a in detail.Assemblies) Assemblies.Add(a);
        Operations.Clear();
        foreach (var o in detail.Operations) Operations.Add(o);
    }

    [RelayCommand]
    private void AddMeasurement() =>
        Measurements.Add(new ProductMeasurementDto { Description = "Новый замер", Unit = "шт", Value = 1 });

    [RelayCommand]
    private void RemoveMeasurement(ProductMeasurementDto? m)
    {
        if (m != null) Measurements.Remove(m);
    }

    [RelayCommand]
    private async Task AddMaterialAsync()
    {
        var catalog = await _api.GetMaterialsCatalogAsync();
        if (catalog is { Count: > 0 })
            Materials.Add(new ProductMaterialLineDto
            {
                MaterialId = catalog[0].MaterialId,
                Article = catalog[0].Article,
                Name = catalog[0].Name,
                Unit = catalog[0].Unit,
                Quantity = 1,
            });
    }

    [RelayCommand]
    private void RemoveMaterial(ProductMaterialLineDto? line)
    {
        if (line != null) Materials.Remove(line);
    }

    [RelayCommand]
    private async Task AddComponentAsync()
    {
        var catalog = await _api.GetComponentsCatalogAsync();
        if (catalog is { Count: > 0 })
            Components.Add(new ProductComponentLineDto
            {
                ComponentId = catalog[0].ComponentId,
                Article = catalog[0].Article,
                Name = catalog[0].Name,
                Unit = catalog[0].Unit,
                Quantity = 1,
            });
    }

    [RelayCommand]
    private void RemoveComponent(ProductComponentLineDto? line)
    {
        if (line != null) Components.Remove(line);
    }

    [RelayCommand]
    private void AddAssembly() =>
        Assemblies.Add(new ProductAssemblyLineDto { ChildProductName = ProductNames.FirstOrDefault() ?? "", Quantity = 1 });

    [RelayCommand]
    private void RemoveAssembly(ProductAssemblyLineDto? line)
    {
        if (line != null) Assemblies.Remove(line);
    }

    [RelayCommand]
    private void AddOperation()
    {
        var op = OperationCatalog.FirstOrDefault();
        Operations.Add(new ProductOperationLineDto
        {
            OperationId = op?.Id ?? 1,
            OperationName = op?.Name ?? "",
            SequenceNumber = Operations.Count + 1,
            EquipmentTypeName = EquipmentTypes.FirstOrDefault(),
            DurationMinutes = 60,
            RequiresEquipment = true,
        });
    }

    [RelayCommand]
    private void RemoveOperation(ProductOperationLineDto? line)
    {
        if (line != null) Operations.Remove(line);
    }

    [RelayCommand]
    private async Task AddDrawingAsync()
    {
        if (string.IsNullOrWhiteSpace(DrawingTitle))
        {
            ErrorMessage = "Укажите название чертежа.";
            return;
        }

        var (ok, err) = await _api.AddProductDrawingAsync(_productName, DrawingTitle, DrawingSource, null);
        if (!ok)
        {
            ErrorMessage = err;
            return;
        }

        DrawingTitle = "";
        await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteDrawingAsync()
    {
        if (SelectedDrawing is null)
            return;
        var (ok, err) = await _api.DeleteProductDrawingAsync(_productName, SelectedDrawing.Id);
        if (!ok) ErrorMessage = err;
        else await LoadAsync();
    }

    [RelayCommand]
    public async Task<bool> SaveAsync()
    {
        if (!_canEdit)
            return true;

        var body = new ProductUpdateRequest
        {
            Dimensions = Dimensions,
            Measurements = Measurements.ToList(),
            Materials = Materials.ToList(),
            Components = Components.ToList(),
            Assemblies = Assemblies.ToList(),
            Operations = Operations.ToList(),
        };

        var (ok, err, _) = await _api.UpdateProductAsync(_productName, body);
        if (!ok)
        {
            ErrorMessage = err;
            return false;
        }

        return true;
    }

    [RelayCommand]
    private async Task PrintAsync()
    {
        var owner = DialogService.TryGetMainWindow();
        if (owner is null)
            return;

        var text = BuildPrintText();
        await PrintPreviewService.ShowAsync(owner, Title, text);
    }

    private string BuildPrintText()
    {
        var lines = new List<string>
        {
            Title,
            $"Габариты: {Dimensions}",
            "",
            "=== Замеры ===",
        };
        foreach (var m in Measurements)
            lines.Add($"- {m.Description}: {m.Value} {m.Unit}");
        lines.Add("");
        lines.Add("=== Материалы ===");
        foreach (var m in Materials)
            lines.Add($"- {m.Article} {m.Name}: {m.Quantity} {m.Unit}");
        lines.Add("");
        lines.Add("=== Комплектующие ===");
        foreach (var c in Components)
            lines.Add($"- {c.Article} {c.Name}: {c.Quantity} {c.Unit}");
        lines.Add("");
        lines.Add("=== Сборочные единицы ===");
        foreach (var a in Assemblies)
            lines.Add($"- {a.ChildProductName} x {a.Quantity}");
        lines.Add("");
        lines.Add("=== Операции ===");
        foreach (var o in Operations.OrderBy(x => x.SequenceNumber))
            lines.Add($"- {o.SequenceNumber}. {o.OperationName} ({o.EquipmentTypeName}, {o.DurationMinutes} мин) — {o.Description}");
        return string.Join(Environment.NewLine, lines);
    }
}
