using CommunityToolkit.Mvvm.ComponentModel;

namespace Minesweeper.AvaloniaApp.ViewModels;

public partial class CellViewModel : ObservableObject
{
    public int X { get; }
    public int Y { get; }
    [ObservableProperty] private string _text = "";
    [ObservableProperty] private string _classes = "cell-hidden";

    public CellViewModel(int x, int y) { X = x; Y = y; }
}