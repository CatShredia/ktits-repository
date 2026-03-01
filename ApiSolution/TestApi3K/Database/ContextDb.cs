using Microsoft.EntityFrameworkCore;
using TestApi3K.Database.Models;

namespace TestApi3K.Database
{
    public class ContextDb : DbContext
    {
        public ContextDb(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Logins> Logins { get; set; }
        public DbSet<Roles> Roles { get; set; }
    }
}
