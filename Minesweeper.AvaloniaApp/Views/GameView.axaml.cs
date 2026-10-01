using Avalonia.Controls;
using Avalonia.Input;
using Minesweeper.AvaloniaApp.ViewModels;

namespace Minesweeper.AvaloniaApp.Views;

public partial class GameView : UserControl
{
    public GameView() => InitializeComponent();

    private void CellPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is CellViewModel cell
                                 && DataContext is GameViewModel vm)
        {
            var pt = e.GetCurrentPoint(btn);
            if (pt.Properties.IsRightButtonPressed) vm.ToggleFlagCommand.Execute(cell);
            else if (pt.Properties.IsLeftButtonPressed) vm.OpenCellCommand.Execute(cell);
        }
    }
}