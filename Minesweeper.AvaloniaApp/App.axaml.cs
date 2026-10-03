using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Minesweeper.AvaloniaApp.ViewModels;
using Minesweeper.AvaloniaApp.Views;
using Minesweeper.AvaloniaApp.Logging;
using Minesweeper.Core.Interfaces;
using Minesweeper.Data;
using Minesweeper.Data.Repositories;

namespace Minesweeper.AvaloniaApp;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            var sc = new ServiceCollection();
            sc.AddDataLayer();
            sc.AddSingleton<MainViewModel>();
            Services = sc.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
            Task.Run(() => DatabaseInitializer.InitializeAsync(Services)).GetAwaiter().GetResult();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new MainWindow { DataContext = Services.GetRequiredService<MainViewModel>() };

            base.OnFrameworkInitializationCompleted();
        }
        catch (Exception ex)
        {
            ExceptionLog.Write("Framework initialization", ex);
            throw;
        }
    }
}