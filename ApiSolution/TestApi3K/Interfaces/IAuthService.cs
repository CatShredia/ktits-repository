using TestApi3K.Model;
using TestApi3K.Requests;

namespace TestApi3K.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RegisterAsync(CreateNewUserAndLogin request);
}
