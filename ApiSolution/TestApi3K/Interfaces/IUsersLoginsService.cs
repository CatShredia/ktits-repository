using Microsoft.AspNetCore.Mvc;
using TestApi3K.Requests;

namespace TestApi3K.Interfaces
{
    public interface IUsersLoginsService
    {
        Task<IActionResult> GetAllUsersAsync();
        Task<IActionResult> CreateNewUserAndLoginAsync(CreateNewUserAndLogin newUser);
    }
}
