using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ProductionSystem.Client;
using CommunityToolkit.Mvvm.Input;
using ProductionSystem.Client.Services;

namespace ProductionSystem.Client.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly BackendApi _api;
    private readonly Action _logout;

    public ObservableCollection<NavItem> NavItems { get; } = new();

    [ObservableProperty] private NavItem? _selectedNav;
    [ObservableProperty] private ViewModelBase? _currentPage;

    public MainWindowViewModel(BackendApi api, Action logout)
    {
        _api = api;
        _logout = logout;
        UserInfo = $"{api.FullName ?? api.Login} ({api.Role})";
        foreach (var n in BuildNav())
            NavItems.Add(n);
        SelectedNav = NavItems.FirstOrDefault();
    }

    public string UserInfo { get; }

    partial void OnSelectedNavChanged(NavItem? value)
    {
        if (value != null)
            CurrentPage = value.Create();
    }

    [RelayCommand]
    private void Logout()
    {
        _api.ClearAuth();
        _logout();
    }

    private IEnumerable<NavItem> BuildNav()
    {
        var role = _api.Role ?? "";
        yield return new NavItem(TitleFor(role), () => new RoleHomeViewModel(role));

        if (role == UserRoles.Customer)
            yield break;

        var canEdit = role is UserRoles.Manager or UserRoles.Director;
        yield return new NavItem("Материалы", () => new MaterialsViewModel(_api, canEdit));
        yield return new NavItem("Комплектующие", () => new ComponentsViewModel(_api, canEdit));

        if (role == UserRoles.Director)
            yield return new NavItem("Работники", () => new WorkersViewModel(_api));
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
