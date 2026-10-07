using System.Globalization;

namespace AutoService.AvaloniaApp.Input;

public static class InputValues
{
    public static bool TryInt(string? text, out int value)
        => int.TryParse(text?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

    public static bool TryDecimal(string? text, out decimal value)
    {
        var normalized = text?.Trim().Replace(',', '.') ?? "";
        return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
    }

    public static bool TryDate(string? text, out DateTime value)
        => DateTime.TryParseExact(
            text?.Trim(),
            ["dd.MM.yyyy HH:mm", "dd.MM.yyyy H:mm", "dd.MM.yyyy"],
            CultureInfo.GetCultureInfo("ru-RU"),
            DateTimeStyles.None,
            out value);

    public static string Format(DateTime value) => value.ToString("dd.MM.yyyy HH:mm");
}
