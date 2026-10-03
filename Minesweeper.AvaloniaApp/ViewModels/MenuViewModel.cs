using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Minesweeper.Core.Models;

namespace Minesweeper.AvaloniaApp.ViewModels;

public partial class MenuViewModel : ObservableObject
{
    private readonly MainViewModel _main;
    public int UserId { get; }
    [ObservableProperty] private string _login;

    public MenuViewModel(MainViewModel main, int userId, string login)
    {
        _main = main; UserId = userId; _login = login;
    }

    [RelayCommand] private void NewGame(GameSize size) => _main.CurrentPage = new GameViewModel(_main, UserId, Login, size);
    [RelayCommand] private void History() => _main.CurrentPage = new HistoryViewModel(_main, UserId, Login);
    [RelayCommand] private void Leaderboard() => _main.CurrentPage = new LeaderboardViewModel(_main, UserId, Login);
    [RelayCommand] private void Logout() => _main.NavigateToLogin();
}