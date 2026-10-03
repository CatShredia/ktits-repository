using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Minesweeper.Core.Models;
using Minesweeper.Data.Models;

namespace Minesweeper.AvaloniaApp.ViewModels;

public partial class MineMapViewModel : ObservableObject
{
    private readonly MainViewModel _main;
    private readonly int _userId;
    private readonly string _login;
    private readonly GameSize? _leaderboardSize;

    public string Title { get; }
    public int Width { get; }
    public ObservableCollection<MapCell> Cells { get; } = new();

    [ObservableProperty] private bool _isMissing;

    public MineMapViewModel(
        MainViewModel main,
        int userId,
        string login,
        GameSize? leaderboardSize,
        string title,
        string mineMap)
    {
        _main = main;
        _userId = userId;
        _login = login;
        _leaderboardSize = leaderboardSize;
        Title = title;

        var map = MineMapCodec.Decode(mineMap);
        if (map == null)
        {
            IsMissing = true;
            return;
        }

        Width = map.GetLength(0);
        int height = map.GetLength(1);
        for (int y = 0; y < height; y++)
            for (int x = 0; x < Width; x++)
                Cells.Add(new MapCell(map[x, y] ? "*" : ".", map[x, y]));
    }

    [RelayCommand]
    private void Back()
    {
        _main.CurrentPage = _leaderboardSize is GameSize size
            ? new LeaderboardViewModel(_main, _userId, _login, size)
            : new HistoryViewModel(_main, _userId, _login);
    }
}

public sealed record MapCell(string Text, bool IsMine);
