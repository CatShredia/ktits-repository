using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductionSystem.Client.Services;
using ProductionSystem.Client.ViewModels;
using ProductionSystem.Client.Views;

namespace ProductionSystem.Client;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Dispatcher.UIThread.UnhandledException += (_, e) =>
            {
                CrashLog.Write("[Dispatcher.UIThread.UnhandledException] " + e.Exception);
                e.Handled = true;
            };

            BindingPlugins.DataValidators.RemoveAt(0);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<BackendApi>();
            Services = services.BuildServiceProvider();

            var api = Services.GetRequiredService<BackendApi>();
            var mainWindow = new MainWindow();
            LoginWindow? loginWindow = null;
            MainWindowViewModel? mainVm = null;

            void OnAuthenticated()
            {
                loginWindow?.Close();
                loginWindow = null;
                mainVm!.ApplySession();
            }

            async Task OpenRegisterFlowAsync(Window owner)
            {
                var registered = false;
                var reg = new RegisterWindow();
                var registerFlowEnded = new TaskCompletionSource();

                reg.DataContext = new RegisterViewModel(
                    api,
                    onSuccess: () =>
                    {
                        registered = true;
                        reg.Close();
                    },
                    onCancel: () => reg.Close());

                void OnRegisterClosed(object? _, EventArgs __) =>
                    registerFlowEnded.TrySetResult();

                reg.Closed += OnRegisterClosed;
                reg.Show(owner);
                await registerFlowEnded.Task.ConfigureAwait(true);
                reg.Closed -= OnRegisterClosed;

                if (registered)
                    OnAuthenticated();
            }

            void ShowLogin()
            {
                api.ClearAuth();
                mainVm!.ClearSession();

                if (loginWindow is { IsVisible: true })
                {
                    loginWindow.Activate();
                    return;
                }

                var win = new LoginWindow();
                loginWindow = win;
                win.DataContext = new LoginViewModel(
                    api,
                    OnAuthenticated,
                    () => OpenRegisterFlowAsync(win));

                win.Closed += OnLoginClosed;
                win.Show(mainWindow);
                win.Activate();
            }

            void OnLoginClosed(object? _, EventArgs __)
            {
                if (loginWindow is null)
                    return;

                loginWindow.Closed -= OnLoginClosed;
                loginWindow = null;

                if (!mainVm!.IsAuthenticated)
                    ShowLogin();
            }

            mainVm = new MainWindowViewModel(api, ShowLogin);
            mainWindow.DataContext = mainVm;
            desktop.MainWindow = mainWindow;

            mainWindow.Show();
            ShowLogin();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
