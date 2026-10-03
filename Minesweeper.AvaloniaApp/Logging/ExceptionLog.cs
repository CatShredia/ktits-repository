using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Minesweeper.AvaloniaApp.Logging;

public static class ExceptionLog
{
    private static readonly object Gate = new();

    public static string FilePath { get; } =
        Path.Combine(AppContext.BaseDirectory, "logs", "exceptions.log");

    public static void RegisterHandlers()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception exception)
                Write("Unhandled exception", exception);
        };

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            Write("Unobserved task exception", args.Exception);
            args.SetObserved();
        };
    }

    public static void Write(string title, Exception exception)
    {
        var message = $"{title}{Environment.NewLine}{exception}";
        Trace.WriteLine(message);
        Append(message);
    }

    public static void Append(string message)
    {
        try
        {
            var directory = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            var line = $"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}{Environment.NewLine}";
            lock (Gate)
                File.AppendAllText(FilePath, line);
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Failed to write exception log: {ex}");
        }
    }
}
