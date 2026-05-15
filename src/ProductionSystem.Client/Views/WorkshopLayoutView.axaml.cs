using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ProductionSystem.Client.Models;
using ProductionSystem.Client.ViewModels;

namespace ProductionSystem.Client.Views;

public partial class WorkshopLayoutView : UserControl
{
    private WorkshopLayoutViewModel? _vm;
    private WorkshopDto? _boundWorkshop;

    public WorkshopLayoutView()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => Redraw();
    }

    private void PlanCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_vm is null || _vm.SelectedPaletteIcon is null)
            return;

        var pos = e.GetPosition(PlanCanvas);
        var x = pos.X / PlanCanvas.Bounds.Width;
        var y = pos.Y / PlanCanvas.Bounds.Height;
        _vm.PlaceIcon(x, y);
        Redraw();
    }

    private void Redraw()
    {
        PlanCanvas.Children.Clear();
        _vm = DataContext as WorkshopLayoutViewModel;
        if (_vm?.SelectedWorkshop is null)
            return;

        var workshop = _vm.SelectedWorkshop;
        if (!ReferenceEquals(workshop, _boundWorkshop))
        {
            _boundWorkshop = workshop;
            SetBackground(workshop);
        }

        var scale = _vm.Zoom;
        foreach (var item in _vm.PlacedIcons)
        {
            var img = TryLoadIcon(item.IconType);
            if (img is null)
                continue;

            var image = new Image
            {
                Source = img,
                Width = 32 * scale,
                Height = 32 * scale,
                Tag = item,
            };
            Canvas.SetLeft(image, item.X * PlanCanvas.Width - 16);
            Canvas.SetTop(image, item.Y * PlanCanvas.Height - 16);
            image.PointerPressed += Icon_PointerPressed;
            PlanCanvas.Children.Add(image);
        }
    }

    private void Icon_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(PlanCanvas).Properties.IsRightButtonPressed &&
            sender is Image { Tag: WorkshopLayoutItemDto item } &&
            _vm is not null)
        {
            _vm.RemoveIconCommand.Execute(item);
            Redraw();
        }
    }

    private static Bitmap? TryLoadIcon(string iconType)
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

    private void SetBackground(WorkshopDto workshop)
    {
        if (string.IsNullOrEmpty(workshop.FloorPlanBase64))
            return;

        try
        {
            var bytes = Convert.FromBase64String(workshop.FloorPlanBase64);
            using var ms = new MemoryStream(bytes);
            PlanCanvas.Background = new ImageBrush(new Bitmap(ms))
            {
                Stretch = Stretch.UniformToFill,
            };
        }
        catch
        {
            /* ignore */
        }
    }
}
