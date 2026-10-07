using AutoService.Data.Workshop;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoService.AvaloniaApp.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _auth;

    public LoginViewModel(AuthService auth) => _auth = auth;

    [ObservableProperty] private string login = "";
    [ObservableProperty] private string password = "";
    [ObservableProperty] private string error = "";

    public bool HasError => !string.IsNullOrWhiteSpace(Error);
    public event Action<CurrentUser>? SignedIn;

    partial void OnErrorChanged(string value) => OnPropertyChanged(nameof(HasError));

    public void Reset()
    {
        Password = "";
        Error = "";
    }

    [RelayCommand]
    private async Task SignInAsync()
    {
        try
        {
            Error = "";
            var result = await _auth.SignInAsync(Login, Password);
            if (!result.Success || result.User is null)
            {
                Error = result.Error ?? "Неверный логин или пароль.";
                return;
            }

            Password = "";
            SignedIn?.Invoke(result.User);
        }
        catch (Exception ex)
        {
            Error = "Не удалось войти. " + ex.Message;
        }
    }
}
