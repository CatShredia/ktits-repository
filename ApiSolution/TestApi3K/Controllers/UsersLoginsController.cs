using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestApi3K.Services.Interfaces;
using TestApi3K.Database.Requests;

namespace TestApi3K.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersLoginsController
    {
        private readonly IUserRepository _userLoginService;

        public UsersLoginsController(IUserRepository userLoginService)
        {
            _userLoginService = userLoginService;
        }

        [HttpGet]
        [Route("getAllUsers")]
        public async Task<IActionResult> GetAllUsers([FromHeader(Name = "X-User-Id")] int? userId)
        {
            return await _userLoginService.GetAllUsersAsync(userId ?? 0);
        }

        [HttpPost]
        [Route("createNewUserAndLogin")]
        public async Task<IActionResult> CreateNewUserAndLogin(CreateNewUserAndLogin newUser)
        {
            return await _userLoginService.CreateNewUserAndLoginAsync(newUser);
        }

        [HttpPut]
        [Route("putUserAndLogin")]
        public async Task<IActionResult> UpdateExitingUserAndLogin(EditUserAndLogin newUser)
        {
            return await _userLoginService.UpdateExitingUserAndLoginAsync(newUser);
        }

        [HttpDelete]
        [Route("deleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            return await _userLoginService.DeleteUserAsync(userId);
        }
    }
}
