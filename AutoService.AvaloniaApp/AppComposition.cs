using AutoService.AvaloniaApp.ViewModels;
using AutoService.Data;
using AutoService.Data.Workshop;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AutoService.AvaloniaApp;

public static class AppComposition
{
    public static IServiceProvider Build()
    {
        var services = new ServiceCollection();
        var connectionString = AutoServiceDbContextFactory.ResolveConnectionString();
        services.AddDbContextFactory<AutoServiceDbContext>(options => options.UseNpgsql(connectionString));
        services.AddSingleton<AuthService>();
        services.AddSingleton<AdminService>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<LoginViewModel>();
        services.AddSingleton<ShellViewModel>();
        services.AddTransient<ClientsViewModel>();
        services.AddTransient<CarsViewModel>();
        services.AddTransient<CatalogViewModel>();
        services.AddTransient<AppointmentsViewModel>();
        services.AddTransient<SchedulesViewModel>();
        services.AddTransient<WorkOrdersViewModel>();
        services.AddTransient<InvoicesViewModel>();
        return services.BuildServiceProvider();
    }
}
