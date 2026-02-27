using Microsoft.AspNetCore.Mvc;
using TestApi3K.Model;
using TestApi3K.Requests;

namespace TestApi3K.Interfaces
{
    public interface IUsersLoginsService
    {
        Task<IActionResult> GetAllUsersAsync(int userId);
        Task<IActionResult> CreateNewUserAndLoginAsync(CreateNewUserAndLogin newUser);
        Task<Users?> GetUserByLoginAsync(string login);
        Task<bool> CreateUserAsync(CreateNewUserAndLogin newUser);
        Task<Users?> GetUserWithLoginDetailsAsync(string login, string password);
        Task<bool> IsUserAdminAsync(int userId);
    }
}
