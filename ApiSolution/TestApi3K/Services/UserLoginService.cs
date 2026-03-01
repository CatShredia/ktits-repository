using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestApi3K.Database;
using TestApi3K.Services.Interfaces;
using TestApi3K.Database.Models;
using TestApi3K.Database.Requests;

namespace TestApi3K.Services
{
    public class UserLoginService : IUsersLoginsService
    {
        private readonly ContextDb _context;

        public UserLoginService(ContextDb context)
        {
            _context = context;
        }

        public async Task<Users?> GetUserByLoginAsync(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return null;
            }

            var selectedLogin = await _context.Logins
                .AsNoTracking()
                .Include(logins => logins.Users)
                .FirstOrDefaultAsync(logins => logins.Login == login);

            if (selectedLogin == null)
            {
                return null;
            }

            return selectedLogin.Users;
        }

        public async Task<Users?> GetUserWithLoginDetailsAsync(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var selectedLogin = await _context.Logins
                .AsNoTracking()
                .Include(logins => logins.Users)
                .FirstOrDefaultAsync(logins => logins.Login == login);

            if (selectedLogin == null)
            {
                return null;
            }

            return selectedLogin.Users;
        }

        public async Task<bool> IsUserAdminAsync(int userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.id_User == userId);

            if (user == null || user.Roles == null)
            {
                return false;
            }

            return user.Roles.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<IActionResult> GetAllUsersAsync(int userId)
        {
            var isAdmin = await IsUserAdminAsync(userId);

            if (!isAdmin)
            {
                return new StatusCodeResult(403);
            }

            var users = await _context.Users
                .Include(u => u.Roles)
                .ToListAsync();

            return new OkObjectResult(new
            {
                data = new { users = users },
                status = true
            });
        }

        public async Task<IActionResult> CreateNewUserAndLoginAsync(CreateNewUserAndLogin newUser)
        {
            var user = new Users()
            {
                Name = newUser.Name,
                Description = newUser.Description,
                id_Role = newUser.id_Role
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var login = new Logins()
            {
                User_id = user.id_User,
                Login = newUser.Login,
                Password = newUser.Password,
            };

            await _context.Logins.AddAsync(login);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true
            });
        }

        public async Task<bool> CreateUserAsync(CreateNewUserAndLogin newUser)
        {
            try
            {
                var user = new Users()
                {
                    Name = newUser.Name,
                    Description = newUser.Description,
                    id_Role = newUser.id_Role
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                var login = new Logins()
                {
                    User_id = user.id_User,
                    Login = newUser.Login,
                    Password = newUser.Password,
                };

                await _context.Logins.AddAsync(login);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IActionResult> UpdateExitingUserAndLoginAsync(EditUserAndLogin newUser)
        {
            try
            {
                if (newUser.id_User <= 0)
                {
                    return new BadRequestObjectResult("User ID is required.");
                }

                var user = await _context.Users.FindAsync(newUser.id_User);
                if (user == null)
                {
                    return new NotFoundObjectResult("User not found.");
                }

                user.Name = newUser.Name;
                user.Description = newUser.Description;
                user.id_Role = newUser.id_Role > 0 ? newUser.id_Role : 1;

                var login = await _context.Logins.FirstOrDefaultAsync(l => l.User_id == newUser.id_User);
                if (login != null)
                {
                    login.Login = newUser.Login;
                    if (!string.IsNullOrWhiteSpace(newUser.Password))
                    {
                        login.Password = newUser.Password;
                    }
                }

                await _context.SaveChangesAsync();

                return new OkObjectResult(new { Success = true, Message = "User updated successfully." });
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> DeleteUserAsync(int userId)
        {
            if (userId <= 0)
            {
                return new BadRequestObjectResult("User ID is required.");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new NotFoundObjectResult("User not found.");
            }

            var login = await _context.Logins.FirstOrDefaultAsync(l => l.User_id == userId);
            if (login != null)
            {
                _context.Logins.Remove(login);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { Success = true, Message = "User deleted successfully." });
        }

    }
}
