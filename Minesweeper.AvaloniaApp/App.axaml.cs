using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Minesweeper.AvaloniaApp.ViewModels;
using Minesweeper.AvaloniaApp.Views;
using Minesweeper.Core.Interfaces;
using Minesweeper.Data;
using Minesweeper.Data.Repositories;

namespace Minesweeper.AvaloniaApp;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override async void OnFrameworkInitializationCompleted()
    {
        var sc = new ServiceCollection();
        sc.AddDataLayer("minesweeper.db");
        sc.AddSingleton<MainViewModel>();
        Services = sc.BuildServiceProvider();
        await DatabaseInitializer.InitializeAsync(Services);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow { DataContext = Services.GetRequiredService<MainViewModel>() };

        base.OnFrameworkInitializationCompleted();
    }
}