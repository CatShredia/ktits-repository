using Minesweeper.Core.Interfaces;

namespace Minesweeper.Data.Services;

public class BcryptPasswordHasher : IPasswordHasher
{
    public string Generate(string password) => BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    public bool Verify(string password, string hashedPassword) => BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
}