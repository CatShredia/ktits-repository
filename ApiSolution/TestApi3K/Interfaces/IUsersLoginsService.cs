using Microsoft.AspNetCore.Mvc;
using TestApi3K.Model;
using TestApi3K.Requests;

namespace TestApi3K.Interfaces
{
    public interface IUsersLoginsService
    {
        Task<IActionResult> GetAllUsersAsync();
        Task<IActionResult> CreateNewUserAndLoginAsync(CreateNewUserAndLogin newUser);
        Task<Users?> GetUserByLoginAsync(string login);
    }
}
