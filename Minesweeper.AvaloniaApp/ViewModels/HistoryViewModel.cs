using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Minesweeper.AvaloniaApp.Logging;
using Minesweeper.Data.Repositories;

namespace Minesweeper.AvaloniaApp.ViewModels;

public partial class HistoryViewModel : ObservableObject
{
    private readonly MainViewModel _main;
    private readonly int _userId;
    private readonly string _login;

    public ObservableCollection<HistoryRow> Games { get; } = new();

    [ObservableProperty] private bool _isEmpty;

    public HistoryViewModel(MainViewModel main, int userId, string login)
    {
        _main = main;
        _userId = userId;
        _login = login;
        _ = LoadAsync();
    }

    [RelayCommand]
    private void ShowMap(HistoryRow row) =>
        _main.CurrentPage = new MineMapViewModel(
            _main, _userId, _login, null, $"{row.Date} | {row.Size} | {row.Status}", row.MineMap);

    [RelayCommand]
    private void Back() => _main.NavigateToMenu(_userId, _login);

    private async Task LoadAsync()
    {
        try
        {
            await using var scope = App.Services.CreateAsyncScope();
            var games = await scope.ServiceProvider.GetRequiredService<IGameRepository>()
                .GetUserHistoryAsync(_userId);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Games.Clear();
                foreach (var game in games)
                {
                    Games.Add(new HistoryRow(
                        game.Date.ToString("g"),
                        game.Size.ToString(),
                        game.Status.ToString(),
                        $"{game.TimeInSeconds}s",
                        game.MineMap));
                }

                IsEmpty = Games.Count == 0;
            });
        }
        catch (Exception ex)
        {
            ExceptionLog.Write("History", ex);
        }
    }
}

public sealed record HistoryRow(string Date, string Size, string Status, string Time, string MineMap);
