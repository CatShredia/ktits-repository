using System.Collections.ObjectModel;
using AutoService.Data.Workshop;
using AppSession = AutoService.AvaloniaApp.AppSession;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace AutoService.AvaloniaApp.ViewModels;

public sealed class NavItem
{
    public required string Title { get; init; }
    public required Func<AppPageViewModel> Create { get; init; }
    public override string ToString() => Title;
}

public partial class ShellViewModel : ObservableObject
{
    private readonly IServiceProvider _services;
    private bool _suppress;

    public ShellViewModel(IServiceProvider services) => _services = services;

    public event Action? SignedOut;

    [ObservableProperty] private string userTitle = "";
    [ObservableProperty] private NavItem? selectedMenu;
    [ObservableProperty] private object? content;

    public ObservableCollection<NavItem> Menu { get; } = [];

    public void Enter(CurrentUser user)
    {
        UserTitle = user.FullName + " · " + user.Role;
        _services.GetRequiredService<AppSession>().User = user;
        _suppress = true;
        Menu.Clear();
        if (user.Role == RoleNames.Admin)
        {
            Add("Клиенты", () => _services.GetRequiredService<ClientsViewModel>());
            Add("Автомобили", () => _services.GetRequiredService<CarsViewModel>());
            Add("Справочники", () => _services.GetRequiredService<CatalogViewModel>());
            Add("Записи", () => _services.GetRequiredService<AppointmentsViewModel>());
            Add("Расписание", () => _services.GetRequiredService<SchedulesViewModel>());
            Add("Заказ-наряды", () => _services.GetRequiredService<WorkOrdersViewModel>());
            Add("Счета", () => _services.GetRequiredService<InvoicesViewModel>());
            Add("Отзывы", () => _services.GetRequiredService<ReviewsViewModel>());
        }
        else if (user.Role == RoleNames.Mechanic)
        {
            Add("Мои заказы", () => _services.GetRequiredService<MechanicOrdersViewModel>());
        }
        else
        {
            Add("Аналитика", () => _services.GetRequiredService<AnalyticsViewModel>());
            Add("Отзывы", () => _services.GetRequiredService<ReviewsViewModel>());
        }

        _suppress = false;
        SelectedMenu = Menu[0];
    }

    partial void OnSelectedMenuChanged(NavItem? value)
    {
        if (_suppress || value is null)
            return;
        _ = OpenAsync(value);
    }

    [RelayCommand]
    private void Logout()
    {
        _suppress = true;
        SelectedMenu = null;
        Menu.Clear();
        Content = null;
        UserTitle = "";
        _suppress = false;
        SignedOut?.Invoke();
    }

    private void Add(string title, Func<AppPageViewModel> create) => Menu.Add(new NavItem { Title = title, Create = create });

    private async Task OpenAsync(NavItem item)
    {
        var page = item.Create();
        Content = page;
        await page.LoadAsync();
    }
}

public partial class MainViewModel : ObservableObject
{
    public MainViewModel(LoginViewModel login, ShellViewModel shell)
    {
        login.SignedIn += user =>
        {
            shell.Enter(user);
            Current = shell;
        };
        shell.SignedOut += () =>
        {
            login.Reset();
            Current = login;
        };
        Current = login;
    }

    [ObservableProperty] private object? current;
}

public partial class RoleHomeViewModel : AppPageViewModel
{
    public RoleHomeViewModel(string title, string description)
    {
        Title = title;
        Description = description;
    }

    public string Title { get; }
    public string Description { get; }

    public override Task LoadAsync() => Task.CompletedTask;
}
