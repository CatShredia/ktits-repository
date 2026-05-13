using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
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

            void OpenMain()
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(api, OpenLogin),
                };
            }

            void OpenRegister(LoginWindow loginWindow)
            {
                var reg = new RegisterWindow();
                reg.DataContext = new RegisterViewModel(
                    api,
                    () =>
                    {
                        reg.Close();
                        loginWindow.Close();
                        OpenMain();
                    },
                    () => reg.Close());

                reg.ShowDialog(loginWindow);
            }

            void OpenLogin()
            {
                api.ClearAuth();

                var loginWindow = new LoginWindow();
                loginWindow.DataContext = new LoginViewModel(api, OpenMain, () => OpenRegister(loginWindow));
                desktop.MainWindow = loginWindow;
            }

            OpenLogin();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
