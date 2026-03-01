using Microsoft.AspNetCore.Mvc;
using TestApi3K.Database.Models;
using TestApi3K.Database.Requests;

namespace TestApi3K.Services.Interfaces
{
    public interface IUsersLoginsService
    {
        Task<IActionResult> GetAllUsersAsync(int userId);
        Task<IActionResult> CreateNewUserAndLoginAsync(CreateNewUserAndLogin newUser);
        Task<IActionResult> UpdateExitingUserAndLoginAsync(EditUserAndLogin newUser);
        Task<IActionResult> DeleteUserAsync(int userId);
        Task<Users?> GetUserByLoginAsync(string login);
        Task<bool> CreateUserAsync(CreateNewUserAndLogin newUser);
        Task<Users?> GetUserWithLoginDetailsAsync(string login, string password);
        Task<bool> IsUserAdminAsync(int userId);
    }
}
