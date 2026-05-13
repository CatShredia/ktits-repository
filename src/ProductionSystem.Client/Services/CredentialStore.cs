using System.IO;
using System.Text.Json;

namespace ProductionSystem.Client.Services;

public class CredentialStore
{
    private static string FilePath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ProductionSystem", "credentials.json");

    public string? Login { get; set; }
    public string? Password { get; set; }
    public bool Remember { get; set; }

    public static CredentialStore? Load()
    {
        try
        {
            if (!File.Exists(FilePath))
                return null;
            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<CredentialStore>(json);
        }
        catch
        {
            return null;
        }
    }

    public void Save()
    {
        var dir = Path.GetDirectoryName(FilePath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(this));
    }

    public static void Clear()
    {
        try
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
        catch
        {
            // ignore
        }
    }
}
