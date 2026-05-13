namespace ProductionSystem.Api.Dto;

public record LoginRequest(string Login, string Password);

public record RegisterRequest(string Login, string Password, string FullName);

public record AuthResponse(string Token, string Login, string Role, string? FullName);
