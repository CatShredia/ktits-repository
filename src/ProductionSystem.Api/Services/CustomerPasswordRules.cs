using System.Text.RegularExpressions;

namespace ProductionSystem.Api.Services;

public static class CustomerPasswordRules
{
    private static readonly Regex Forbidden = new(@"[\*&\{\}\|\+]", RegexOptions.Compiled);

    public static bool TryValidate(string password, out string? error)
    {
        error = null;
        if (password.Length is < 4 or > 16)
        {
            error = "Пароль должен быть от 4 до 16 символов.";
            return false;
        }

        if (Forbidden.IsMatch(password))
        {
            error = "Пароль не должен содержать символы: * & { } | +";
            return false;
        }

        if (!password.Any(char.IsUpper))
        {
            error = "Пароль должен содержать хотя бы одну заглавную букву.";
            return false;
        }

        if (!password.Any(char.IsDigit))
        {
            error = "Пароль должен содержать хотя бы одну цифру.";
            return false;
        }

        return true;
    }
}
