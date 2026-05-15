using System.Security.Claims;

namespace ProductionSystem.Api.Services;

public static class AuthUserAccessor
{
    public static string? GetLogin(ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Name)
        ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? user.FindFirstValue("sub");

    public static string? GetRole(ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Role);
}
