using AutoService.Core.Security;

namespace AutoService.Data.Workshop;

public sealed class AuthService
{
    private readonly IDbContextFactory<AutoServiceDbContext> _factory;

    public AuthService(IDbContextFactory<AutoServiceDbContext> factory) => _factory = factory;

    public async Task<AuthResult> SignInAsync(string? login, string? password)
    {
        var normalizedLogin = login?.Trim() ?? "";
        if (normalizedLogin.Length == 0 || string.IsNullOrEmpty(password))
            return Fail("Введите логин и пароль.");

        await using var db = await _factory.CreateDbContextAsync();
        var user = await db.Users.AsNoTracking()
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Login == normalizedLogin);
        if (user is null || !PasswordHasher.Verify(password, user.PasswordHash))
            return Fail("Неверный логин или пароль.");

        return new AuthResult
        {
            Success = true,
            User = new CurrentUser
            {
                Id = user.Id,
                FullName = user.FullName,
                Login = user.Login,
                Role = user.Role.Name
            }
        };
    }

    private static AuthResult Fail(string error) => new() { Success = false, Error = error };
}
