using System;
using System.Linq;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Minesweeper.Core.Models;
using Minesweeper.Core.Services;
using Minesweeper.Data.Repositories;
using Minesweeper.AvaloniaApp;

namespace Minesweeper.AvaloniaApp.ViewModels;

public partial class GameViewModel : ObservableObject, IGameTimer
{
    private readonly MainViewModel _main;
    private readonly int _userId;
    private readonly GameSize _size;
    private GameSession _session = null!;
    private DispatcherTimer? _dispatcherTimer;

    public int BoardWidth { get; }
    public ObservableCollection<CellViewModel> Cells { get; } = new();

    [ObservableProperty] private string _timerText = "0s";
    [ObservableProperty] private string _statusText = "";

    public int ElapsedSeconds { get; private set; }
    public event Action<int>? OnTick;

    public GameViewModel(MainViewModel main, int userId, GameSize size)
    {
        _main = main; _userId = userId; _size = size;

        var (w, h, m) = size switch
        {
            GameSize.Amateur => (12, 12, 20),
            GameSize.Professional => (20, 20, 40),
            _ => (9, 9, 10)
        };

        BoardWidth = w;
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                Cells.Add(new CellViewModel(x, y));

        _session = new GameSession(w, h, m, this);

        _session.OnCellsRevealed += cells => Dispatcher.UIThread.Post(() =>
        {
            foreach (var c in cells) UpdateCellVm(c);
        });

        _session.OnCellFlagged += c => Dispatcher.UIThread.Post(() => UpdateCellVm(c));

        _session.OnGameEnded += async s =>
        {
            StatusText = s == GameStatus.Win ? "🎉 You Win!" : "💥 Boom!";
            if (s == GameStatus.Lose)
                for (int x = 0; x < w; x++)
                    for (int y = 0; y < h; y++)
                        if (_session.Board.Grid[x, y].IsMine) UpdateCellVm(_session.Board.Grid[x, y]);

            await App.Services.GetRequiredService<IGameRepository>()
                .SaveGameAsync(userId, size, s, ElapsedSeconds, _session.Board.GetMineMap());
        };

        OnTick += s => Dispatcher.UIThread.Post(() => TimerText = $"{s}s");
    }

    private void UpdateCellVm(Cell c)
    {
        var vm = Cells.First(v => v.X == c.X && v.Y == c.Y);

        // Обновляем логические флаги
        vm.IsHidden = !c.IsRevealed && !c.IsFlagged;
        vm.IsFlagged = c.IsFlagged;
        vm.IsMine = c.IsRevealed && c.IsMine;
        vm.IsRevealed = c.IsRevealed && !c.IsMine;
        vm.AdjacentMines = c.AdjacentMines;

        // Обновляем текст
        if (c.IsFlagged)
        {
            vm.Text = "🚩";
        }
        else if (!c.IsRevealed)
        {
            vm.Text = "";
        }
        else if (c.IsMine)
        {
            vm.Text = "💣";
        }
        else
        {
            vm.Text = c.AdjacentMines == 0 ? "" : c.AdjacentMines.ToString();
        }
    }

    [RelayCommand] private void OpenCell(CellViewModel c) => _session.MakeMove(c.X, c.Y, false);
    [RelayCommand] private void ToggleFlag(CellViewModel c) => _session.MakeMove(c.X, c.Y, true);
    [RelayCommand] private void Back() => _main.CurrentPage = new MenuViewModel(_main, _userId, "");

    public void Start()
    {
        _dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _dispatcherTimer.Tick += (_, _) => { ElapsedSeconds++; OnTick?.Invoke(ElapsedSeconds); };
        _dispatcherTimer.Start();
    }

    public void Stop() => _dispatcherTimer?.Stop();
    public void Reset() { Stop(); ElapsedSeconds = 0; }
}