using Minesweeper.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Minesweeper.Data.Repositories;

public interface IUserRepository
{
    Task<User?> GetByLoginAsync(string login);
    Task<User> CreateAsync(string login, string passwordHash);
}

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) => _context = context;

    public async Task<User?> GetByLoginAsync(string login) 
        => await _context.Users.FirstOrDefaultAsync(u => u.Login == login);

    public async Task<User> CreateAsync(string login, string passwordHash)
    {
        var user = new User { Login = login, PasswordHash = passwordHash };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}