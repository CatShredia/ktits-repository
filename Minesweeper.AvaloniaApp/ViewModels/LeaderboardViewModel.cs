using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Minesweeper.AvaloniaApp.Logging;
using Minesweeper.Core.Models;
using Minesweeper.Data.Repositories;

namespace Minesweeper.AvaloniaApp.ViewModels;

public partial class LeaderboardViewModel : ObservableObject
{
    private readonly MainViewModel _main;
    private readonly int _userId;
    private readonly string _login;
    private int _loadVersion;

    public ObservableCollection<LeaderboardRow> Entries { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Title))]
    private GameSize _size = GameSize.Beginner;

    [ObservableProperty] private bool _isEmpty;

    public string Title => $"Top 10 ({Size})";

    public LeaderboardViewModel(MainViewModel main, int userId, string login)
    {
        _main = main;
        _userId = userId;
        _login = login;
        _ = LoadAsync();
    }

    [RelayCommand]
    private void SelectSize(GameSize size) => Size = size;

    [RelayCommand]
    private void Back() => _main.NavigateToMenu(_userId, _login);

    partial void OnSizeChanged(GameSize value) => _ = LoadAsync();

    private async Task LoadAsync()
    {
        var version = ++_loadVersion;
        var size = Size;

        try
        {
            await using var scope = App.Services.CreateAsyncScope();
            var top = await scope.ServiceProvider.GetRequiredService<IGameRepository>()
                .GetLeaderboardAsync(size);

            if (version != _loadVersion)
                return;

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (version != _loadVersion)
                    return;

                Entries.Clear();
                for (var i = 0; i < top.Count; i++)
                {
                    Entries.Add(new LeaderboardRow(
                        $"{i + 1}.",
                        $"User#{top[i].UserId}",
                        $"{top[i].TimeInSeconds}s",
                        top[i].Date.ToString("g")));
                }

                IsEmpty = Entries.Count == 0;
            });
        }
        catch (Exception ex)
        {
            ExceptionLog.Write("Leaderboard", ex);
        }
    }
}

public sealed record LeaderboardRow(string Place, string User, string Time, string Date);
