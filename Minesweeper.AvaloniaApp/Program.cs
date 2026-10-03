using Avalonia;
using System;
using Minesweeper.AvaloniaApp.Logging;

namespace Minesweeper.AvaloniaApp;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        ExceptionLog.RegisterHandlers();

        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            ExceptionLog.Write("Fatal startup exception", ex);
            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogExceptionsToFile();
}