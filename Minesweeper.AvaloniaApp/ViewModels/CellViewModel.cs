using CommunityToolkit.Mvvm.ComponentModel;

namespace Minesweeper.AvaloniaApp.ViewModels;

public partial class CellViewModel : ObservableObject
{
    public int X { get; }
    public int Y { get; }

    [ObservableProperty] private string _text = "";

    [ObservableProperty] private bool _isHidden = true;
    [ObservableProperty] private bool _isRevealed;
    [ObservableProperty] private bool _isMine;
    [ObservableProperty] private bool _isFlagged;
    [ObservableProperty] private int _adjacentMines;

    public CellViewModel(int x, int y) 
    { 
        X = x; 
        Y = y; 
    }
}