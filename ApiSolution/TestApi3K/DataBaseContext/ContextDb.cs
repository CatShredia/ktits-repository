using Microsoft.EntityFrameworkCore;
using TestApi3K.Model;

namespace TestApi3K.DataBaseContext
{
    public class ContextDb : DbContext
    {
        public ContextDb(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Logins> Logins { get; set; }
    }
}
