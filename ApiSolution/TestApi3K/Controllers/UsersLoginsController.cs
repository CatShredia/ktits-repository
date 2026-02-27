using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TestApi3K.Interfaces;
using TestApi3K.Requests;

namespace TestApi3K.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersLoginsController
    {
        private readonly IUsersLoginsService _userLoginService;

        public UsersLoginsController(IUsersLoginsService userLoginService)
        {
            _userLoginService = userLoginService;
        }

        [HttpGet]
        [Route("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            return await _userLoginService.GetAllUsersAsync();
        }

        [HttpPost]
        [Route("createNewUserAndLogin")]
        public async Task<IActionResult> CreateNewUserAndLogin(CreateNewUserAndLogin newUser)
        {
            return await _userLoginService.CreateNewUserAndLoginAsync(newUser);
        }
    }
}
