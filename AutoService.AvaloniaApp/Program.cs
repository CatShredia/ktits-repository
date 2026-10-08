using Avalonia;

namespace AutoService.AvaloniaApp;

// Точка входа приложения Avalonia.
// Вызывается средой при запуске процесса.
internal static class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
