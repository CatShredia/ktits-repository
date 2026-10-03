using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Minesweeper.Core.Interfaces;
using Minesweeper.Data.Repositories;
using Minesweeper.AvaloniaApp;

namespace Minesweeper.AvaloniaApp.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly MainViewModel _main;
    [ObservableProperty] private string _login = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private string _message = "";

    public LoginViewModel(MainViewModel main) => _main = main;

    [RelayCommand]
    private async Task Submit()
    {
        await using var scope = App.Services.CreateAsyncScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var user = await repo.GetByLoginAsync(Login);

        if (user != null && hasher.Verify(Password, user.PasswordHash))
        {
            _main.NavigateToMenu(user.Id, user.Login);
        }
        else if (user == null)
        {
            var newUser = await repo.CreateAsync(Login, hasher.Generate(Password));
            _main.NavigateToMenu(newUser.Id, newUser.Login);
        }
        else
        {
            Message = "Wrong password";
        }
    }
}