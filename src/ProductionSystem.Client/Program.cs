using System;
using System.Threading.Tasks;
using Avalonia;

namespace ProductionSystem.Client;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        CrashLog.Write("Main: start");
        try
        {
            Console.WriteLine($"Лог при ошибках: {CrashLog.LogPath}");
        }
        catch
        {
            /* ignore */
        }

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            CrashLog.Write("[AppDomain.UnhandledException] " + e.ExceptionObject);
        };

        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            CrashLog.Write("[UnobservedTask] " + e.Exception);
            e.SetObserved();
        };

        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            CrashLog.Write("[Main] " + ex);
            Environment.ExitCode = 1;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
