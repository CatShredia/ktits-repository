using System.Text;

namespace ProductionSystem.Client;

/// <summary>Запись в файл при падении (WinExe не всегда показывает консоль у dotnet run).</summary>
public static class CrashLog
{
    public static string LogPath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ProductionSystem.Client",
        "crash.log");

    public static void Write(string message)
    {
        try
        {
            var dir = Path.GetDirectoryName(LogPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            File.AppendAllText(LogPath, $"{DateTime.UtcNow:O} {message}\n\n", Encoding.UTF8);
        }
        catch
        {
            /* ignore */
        }

        try
        {
            Console.Error.WriteLine(message);
        }
        catch
        {
            /* ignore */
        }
    }
}
