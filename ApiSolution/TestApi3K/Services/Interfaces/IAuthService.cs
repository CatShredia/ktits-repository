using Microsoft.AspNetCore.Mvc;
using TestApi3K.Database.Models;
using TestApi3K.Database.Requests;

namespace TestApi3K.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthResponse?> RegisterAsync(CreateNewUserAndLogin request);
    }
}
