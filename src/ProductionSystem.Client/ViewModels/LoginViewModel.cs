using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductionSystem.Client.Services;

namespace ProductionSystem.Client.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly BackendApi _api;
    private readonly Action _onSuccess;
    private readonly Func<Task> _openRegisterFlow;

    [ObservableProperty] private string _login = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private bool _rememberMe;
    [ObservableProperty] private string? _errorMessage;

    public LoginViewModel(BackendApi api, Action onSuccess, Func<Task> openRegisterFlow)
    {
        _api = api;
        _onSuccess = onSuccess;
        _openRegisterFlow = openRegisterFlow;
    }

    public async Task TryAutoLoginAsync()
    {
        var store = CredentialStore.Load();
        if (store is not { Remember: true, Login: { } l, Password: { } p })
            return;

        Login = l;
        Password = p;
        RememberMe = true;

        var (ok, err, _) = await _api.LoginAsync(l, p);
        if (ok)
            await Dispatcher.UIThread.InvokeAsync(_onSuccess);
        else
            ErrorMessage = err ?? "Не удалось выполнить автоматический вход.";
    }

    [RelayCommand]
    public async Task SignInAsync()
    {
        ErrorMessage = null;
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Введите логин и пароль.";
            return;
        }

        var (ok, err, _) = await _api.LoginAsync(Login.Trim(), Password);
        if (!ok)
        {
            ErrorMessage = err ?? "Неверный логин или пароль.";
            return;
        }

        if (RememberMe)
        {
            new CredentialStore { Login = Login.Trim(), Password = Password, Remember = true }.Save();
        }
        else
        {
            CredentialStore.Clear();
        }

        await Dispatcher.UIThread.InvokeAsync(_onSuccess);
    }

    [RelayCommand]
    private async Task GoRegisterAsync() => await _openRegisterFlow();
}
