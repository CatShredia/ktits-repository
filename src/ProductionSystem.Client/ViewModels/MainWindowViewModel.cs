using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ProductionSystem.Client;
using CommunityToolkit.Mvvm.Input;
using ProductionSystem.Client.Services;

namespace ProductionSystem.Client.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly BackendApi _api;
    private readonly Action _requestLogin;

    public ObservableCollection<NavItem> NavItems { get; } = new();

    [ObservableProperty] private bool _isAuthenticated;
    [ObservableProperty] private bool _showLoginPrompt = true;
    [ObservableProperty] private string _userInfo = "Не авторизован";
    [ObservableProperty] private NavItem? _selectedNav;
    [ObservableProperty] private ViewModelBase? _currentPage;

    public MainWindowViewModel(BackendApi api, Action requestLogin)
    {
        _api = api;
        _requestLogin = requestLogin;
    }

    public void ApplySession()
    {
        IsAuthenticated = true;
        ShowLoginPrompt = false;
        UserInfo = $"{_api.FullName ?? _api.Login} ({_api.Role})";
        NavItems.Clear();
        foreach (var n in BuildNav())
            NavItems.Add(n);
        SelectedNav = NavItems.FirstOrDefault();
    }

    public void ClearSession()
    {
        IsAuthenticated = false;
        ShowLoginPrompt = true;
        UserInfo = "Не авторизован";
        NavItems.Clear();
        SelectedNav = null;
        CurrentPage = null;
    }

    partial void OnSelectedNavChanged(NavItem? value)
    {
        if (value != null && IsAuthenticated)
            CurrentPage = value.Create();
    }

    [RelayCommand]
    private void Logout()
    {
        _api.ClearAuth();
        ClearSession();
        _requestLogin();
    }

    private IEnumerable<NavItem> BuildNav()
    {
        var role = _api.Role ?? "";
        yield return new NavItem(TitleFor(role), () => new RoleHomeViewModel(role));
        yield return new NavItem("Заказы", () => new OrdersViewModel(_api));

        if (role == UserRoles.Customer)
            yield break;

        var canEdit = role is UserRoles.Manager or UserRoles.Director;
        yield return new NavItem("Материалы", () => new MaterialsViewModel(_api, canEdit));
        yield return new NavItem("Комплектующие", () => new ComponentsViewModel(_api, canEdit));

        if (role == UserRoles.Foreman)
        {
            yield return new NavItem("Сбои оборудования", () => new EquipmentFailuresViewModel(_api));
            yield return new NavItem("Контроль качества", () => new QualityControlViewModel(_api));
        }

        if (role == UserRoles.Director)
        {
            yield return new NavItem("Планировка цехов", () => new WorkshopLayoutViewModel(_api));
            yield return new NavItem("Работники", () => new WorkersViewModel(_api));
        }
    }

    private static string TitleFor(string r) => r switch
    {
        UserRoles.Customer => "Экран заказчика",
        UserRoles.Manager => "Экран менеджера",
        UserRoles.Designer => "Экран конструктора",
        UserRoles.Foreman => "Экран мастера",
        UserRoles.Director => "Экран директора",
        _ => "Главный экран",
    };
}

public record NavItem(string Title, Func<ViewModelBase> Create);
