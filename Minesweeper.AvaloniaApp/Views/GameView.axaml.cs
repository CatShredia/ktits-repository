using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Minesweeper.AvaloniaApp.ViewModels;

namespace Minesweeper.AvaloniaApp.Views;

public partial class GameView : UserControl
{
    public GameView()
    {
        InitializeComponent();
        AddHandler(PointerPressedEvent, CellPointerPressed, handledEventsToo: true);
    }

    private void CellPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not GameViewModel vm) return;

        var btn = (e.Source as Visual)?.FindAncestorOfType<Button>(includeSelf: true);
        if (btn?.DataContext is not CellViewModel cell) return;

        var pt = e.GetCurrentPoint(btn);
        if (pt.Properties.IsRightButtonPressed) vm.ToggleFlagCommand.Execute(cell);
        else if (pt.Properties.IsLeftButtonPressed) vm.OpenCellCommand.Execute(cell);
    }
}
