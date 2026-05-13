using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductionSystem.Client.Services;

namespace ProductionSystem.Client.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly BackendApi _api;
    private readonly Action _onSuccess;
    private readonly Action _close;

    [ObservableProperty] private string _login = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private string _fullName = "";
    [ObservableProperty] private string? _errorMessage;

    public RegisterViewModel(BackendApi api, Action onSuccess, Action close)
    {
        _api = api;
        _onSuccess = onSuccess;
        _close = close;
    }

    [RelayCommand]
    public async Task RegisterAsync()
    {
        ErrorMessage = null;
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Укажите логин и пароль.";
            return;
        }

        if (!ValidatePassword(Password, out var pwdError))
        {
            ErrorMessage = pwdError;
            return;
        }

        var (ok, err) = await _api.RegisterAsync(Login.Trim(), Password, FullName.Trim());
        if (!ok)
        {
            ErrorMessage = err ?? "Ошибка регистрации.";
            return;
        }

        CredentialStore.Clear();
        _close();
        _onSuccess();
    }

    [RelayCommand]
    private void Cancel() => _close();

    private static bool ValidatePassword(string password, out string? error)
    {
        error = null;
        if (password.Length is < 4 or > 16)
        {
            error = "Пароль должен быть от 4 до 16 символов.";
            return false;
        }

        if (password.IndexOfAny("*&{}|+".ToCharArray()) >= 0)
        {
            error = "Пароль не должен содержать символы: * & { } | +";
            return false;
        }

        if (!password.Any(char.IsUpper))
        {
            error = "Пароль должен содержать хотя бы одну заглавную букву.";
            return false;
        }

        if (!password.Any(char.IsDigit))
        {
            error = "Пароль должен содержать хотя бы одну цифру.";
            return false;
        }

        return true;
    }
}
