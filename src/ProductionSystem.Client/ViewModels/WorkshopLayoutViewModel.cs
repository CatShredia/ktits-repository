using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductionSystem.Client.Models;
using ProductionSystem.Client.Services;

namespace ProductionSystem.Client.ViewModels;

public partial class WorkshopLayoutViewModel : ViewModelBase
{
    private readonly BackendApi _api;
    private List<WorkshopLayoutItemDto> _snapshot = new();

    public ObservableCollection<WorkshopDto> Workshops { get; } = new();
    public ObservableCollection<WorkshopLayoutItemDto> PlacedIcons { get; } = new();
    public ObservableCollection<string> PaletteIcons { get; } = new()
    {
        "Equipment", "FireExtinguisher", "FirstAid", "Exit",
    };

    [ObservableProperty] private WorkshopDto? _selectedWorkshop;
    [ObservableProperty] private string? _selectedPaletteIcon;
    [ObservableProperty] private string? _statusMessage;
    [ObservableProperty] private double _zoom = 1.0;

    public WorkshopLayoutViewModel(BackendApi api)
    {
        _api = api;
        _ = LoadAsync();
    }

    partial void OnSelectedWorkshopChanged(WorkshopDto? value)
    {
        PlacedIcons.Clear();
        if (value?.LayoutItems != null)
        {
            foreach (var i in value.LayoutItems)
                PlacedIcons.Add(new WorkshopLayoutItemDto { IconType = i.IconType, X = i.X, Y = i.Y });
        }

        _snapshot = PlacedIcons.Select(Clone).ToList();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        var list = await _api.GetWorkshopsAsync();
        Workshops.Clear();
        if (list == null)
            return;
        foreach (var w in list)
            Workshops.Add(w);
        SelectedWorkshop = Workshops.FirstOrDefault();
    }

    public void PlaceIcon(double x, double y)
    {
        if (string.IsNullOrEmpty(SelectedPaletteIcon))
            return;

        PlacedIcons.Add(new WorkshopLayoutItemDto
        {
            IconType = SelectedPaletteIcon,
            X = Math.Clamp(x, 0, 1),
            Y = Math.Clamp(y, 0, 1),
        });
    }

    [RelayCommand]
    private void RemoveIcon(WorkshopLayoutItemDto item) => PlacedIcons.Remove(item);

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (SelectedWorkshop is null)
            return;

        var (ok, err) = await _api.SaveWorkshopLayoutAsync(SelectedWorkshop.Id, PlacedIcons.ToList());
        StatusMessage = ok ? "План сохранён." : err;
        if (ok)
            _snapshot = PlacedIcons.Select(Clone).ToList();
    }

    [RelayCommand]
    private void CancelChanges()
    {
        PlacedIcons.Clear();
        foreach (var i in _snapshot)
            PlacedIcons.Add(Clone(i));
        StatusMessage = "Изменения отменены.";
    }

    [RelayCommand]
    private void ZoomIn() => Zoom = Math.Min(3, Zoom + 0.25);

    [RelayCommand]
    private void ZoomOut() => Zoom = Math.Max(0.5, Zoom - 0.25);

    private static WorkshopLayoutItemDto Clone(WorkshopLayoutItemDto i) =>
        new() { IconType = i.IconType, X = i.X, Y = i.Y };
}
