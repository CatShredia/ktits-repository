using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ProductionSystem.Client.Models;
using ProductionSystem.Client.ViewModels;

namespace ProductionSystem.Client.Views;

public partial class WorkshopLayoutView : UserControl
{
    private const double IconSize = 36;
    private const double IconHalf = IconSize / 2;

    private WorkshopLayoutViewModel? _vm;
    private WorkshopDto? _boundWorkshop;
    private PropertyChangedEventHandler? _vmPropertyHandler;
    private readonly List<PlanIconEntry> _entries = new();

    private string? _dragIconType;
    private PlanIconEntry? _movingEntry;
    private PlanIconEntry? _selectedEntry;
    private Point _moveStart;
    private double _iconStartX;
    private double _iconStartY;

    public WorkshopLayoutView()
    {
        InitializeComponent();
        LoadToolbarIcons();
        AddHandler(PointerReleasedEvent, Global_PointerReleased, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        DataContextChanged += (_, _) => HookViewModel();
    }

    private void HookViewModel()
    {
        if (_vm is not null)
        {
            _vm.PlacedIcons.CollectionChanged -= OnPlacedIconsChanged;
            if (_vmPropertyHandler is not null)
                _vm.PropertyChanged -= _vmPropertyHandler;
        }

        _vm = DataContext as WorkshopLayoutViewModel;
        if (_vm is null)
            return;

        _vm.PlacedIcons.CollectionChanged += OnPlacedIconsChanged;
        _vmPropertyHandler = OnViewModelPropertyChanged;
        _vm.PropertyChanged += _vmPropertyHandler;
        Redraw();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WorkshopLayoutViewModel.SelectedWorkshop))
            Redraw();
    }

    private void OnPlacedIconsChanged(object? sender, NotifyCollectionChangedEventArgs e) => Redraw();

    private void LoadToolbarIcons()
    {
        ToolEquipmentImage.Source = LoadIcon("Equipment");
        ToolExitImage.Source = LoadIcon("Exit");
        ToolFireImage.Source = LoadIcon("FireExtinguisher");
        ToolFirstAidImage.Source = LoadIcon("FirstAid");
    }

    private void Toolbar_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Border border)
            return;

        _dragIconType = border.Tag?.ToString();
        HighlightToolbar(border);
        e.Pointer.Capture(border);
    }

    private void HighlightToolbar(Border selectedBorder)
    {
        foreach (var border in new[] { ToolEquipment, ToolExit, ToolFireExtinguisher, ToolFirstAid })
            border.Classes.Remove("selected");
        selectedBorder.Classes.Add("selected");
    }

    private void PlanCanvas_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_movingEntry is not null)
        {
            var pos = e.GetCurrentPoint(PlanCanvas).Position;
            var dx = pos.X - _moveStart.X;
            var dy = pos.Y - _moveStart.Y;
            Canvas.SetLeft(_movingEntry.Image, _iconStartX + dx);
            Canvas.SetTop(_movingEntry.Image, _iconStartY + dy);
        }
    }

    private void Global_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_dragIconType is null)
            return;

        var pos = e.GetCurrentPoint(PlanCanvas).Position;
        if (pos.X >= 0 && pos.Y >= 0 && pos.X <= PlanCanvas.Width && pos.Y <= PlanCanvas.Height)
            PlaceIconAtPixels(pos.X, pos.Y, _dragIconType);

        _dragIconType = null;
        e.Pointer.Capture(null);
    }

    private void PlanCanvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_movingEntry is null)
            return;

        CommitIconPosition(_movingEntry);
        _movingEntry = null;
        e.Pointer.Capture(null);
    }

    private void Icon_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Image image)
            return;

        var entry = _entries.FirstOrDefault(x => x.Image == image);
        if (entry is null)
            return;

        _selectedEntry = entry;
        _movingEntry = entry;
        _moveStart = e.GetCurrentPoint(PlanCanvas).Position;
        _iconStartX = Canvas.GetLeft(image);
        _iconStartY = Canvas.GetTop(image);
        e.Pointer.Capture(image);
        e.Handled = true;
    }

    private void Icon_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_movingEntry is null)
            return;

        var pos = e.GetCurrentPoint(PlanCanvas).Position;
        var dx = pos.X - _moveStart.X;
        var dy = pos.Y - _moveStart.Y;
        Canvas.SetLeft(_movingEntry.Image, _iconStartX + dx);
        Canvas.SetTop(_movingEntry.Image, _iconStartY + dy);
    }

    private void Icon_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_movingEntry is null)
            return;

        CommitIconPosition(_movingEntry);
        _movingEntry = null;
        e.Pointer.Capture(null);
    }

    private void DeleteSelected_Click(object? sender, RoutedEventArgs e)
    {
        if (_vm is null)
            return;

        if (_selectedEntry is null)
        {
            _vm.StatusMessage = "Выберите значок на схеме.";
            return;
        }

        _vm.RemoveIconCommand.Execute(_selectedEntry.Item);
        _selectedEntry = null;
        _vm.StatusMessage = "Значок удалён.";
    }

    private void PlaceIconAtPixels(double x, double y, string iconType)
    {
        if (_vm is null)
            return;

        var normalized = PixelToNormalized(x, y);
        _vm.PlaceIcon(normalized.X, normalized.Y, iconType);
    }

    private void CommitIconPosition(PlanIconEntry entry)
    {
        var left = Canvas.GetLeft(entry.Image);
        var top = Canvas.GetTop(entry.Image);
        var normalized = PixelToNormalized(left + IconHalf, top + IconHalf);
        _vm?.MoveIcon(entry.Item, normalized.X, normalized.Y);
    }

    private Point PixelToNormalized(double pixelX, double pixelY)
    {
        var w = PlanCanvas.Width;
        var h = PlanCanvas.Height;
        if (w <= 0 || h <= 0)
            return new Point(0, 0);

        return new Point(
            Math.Clamp(pixelX / w, 0, 1),
            Math.Clamp(pixelY / h, 0, 1));
    }

    private void Redraw()
    {
        PlanCanvas.Children.Clear();
        _entries.Clear();
        _movingEntry = null;
        _dragIconType = null;
        _selectedEntry = null;

        _vm = DataContext as WorkshopLayoutViewModel;
        if (_vm?.SelectedWorkshop is null)
            return;

        var workshop = _vm.SelectedWorkshop;
        if (!ReferenceEquals(workshop, _boundWorkshop))
        {
            _boundWorkshop = workshop;
            SetBackground(workshop);
        }

        foreach (var item in _vm.PlacedIcons)
            AddPlanIcon(item);
    }

    private void AddPlanIcon(WorkshopLayoutItemDto item)
    {
        var img = LoadIcon(item.IconType);
        if (img is null)
            return;

        var image = new Image
        {
            Source = img,
            Width = IconSize,
            Height = IconSize,
            Tag = item,
        };

        var left = item.X * PlanCanvas.Width - IconHalf;
        var top = item.Y * PlanCanvas.Height - IconHalf;
        Canvas.SetLeft(image, left);
        Canvas.SetTop(image, top);

        image.PointerPressed += Icon_PointerPressed;
        image.PointerMoved += Icon_PointerMoved;
        image.PointerReleased += Icon_PointerReleased;

        PlanCanvas.Children.Add(image);
        _entries.Add(new PlanIconEntry { Item = item, Image = image });
    }

    private void SetBackground(WorkshopDto workshop)
    {
        if (string.IsNullOrEmpty(workshop.FloorPlanBase64))
        {
            PlanCanvas.Width = 800;
            PlanCanvas.Height = 500;
            PlanCanvas.Background = new SolidColorBrush(Color.Parse("#F8F8F8"));
            return;
        }

        try
        {
            var bytes = Convert.FromBase64String(workshop.FloorPlanBase64);
            using var ms = new MemoryStream(bytes);
            var bitmap = new Bitmap(ms);
            PlanCanvas.Width = bitmap.PixelSize.Width;
            PlanCanvas.Height = bitmap.PixelSize.Height;
            PlanCanvas.Background = new ImageBrush(bitmap) { Stretch = Stretch.UniformToFill };
        }
        catch
        {
            PlanCanvas.Width = 800;
            PlanCanvas.Height = 500;
        }
    }

    private static Bitmap? LoadIcon(string iconType)
    {
        var file = iconType switch
        {
            "FireExtinguisher" => "FireExtinguisher.png",
            "FirstAid" => "FirstAid.png",
            "Exit" => "Exit.jpg",
            _ => "Equipment.png",
        };

        var uri = new Uri($"avares://ProductionSystem.Client/Assets/WorkshopIcons/{file}");
        try
        {
            return new Bitmap(AssetLoader.Open(uri));
        }
        catch
        {
            return null;
        }
    }

    private sealed class PlanIconEntry
    {
        public required WorkshopLayoutItemDto Item { get; init; }
        public required Image Image { get; init; }
    }
}
