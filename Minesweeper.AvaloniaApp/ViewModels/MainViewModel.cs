using CommunityToolkit.Mvvm.ComponentModel;

namespace Minesweeper.AvaloniaApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private object? _currentPage;

    public MainViewModel()
    {
        NavigateToLogin();
    }

    public void NavigateToLogin() => CurrentPage = new LoginViewModel(this);
    public void NavigateToMenu(int userId, string login) => CurrentPage = new MenuViewModel(this, userId, login);
}