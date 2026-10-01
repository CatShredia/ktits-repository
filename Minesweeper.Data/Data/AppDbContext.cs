using Microsoft.EntityFrameworkCore;
using Minesweeper.Data.Models;

namespace Minesweeper.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Game> Games => Set<Game>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Login).IsUnique();
            entity.HasMany(u => u.Games).WithOne(g => g.User).HasForeignKey(g => g.UserId);
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(g => g.Id);
            // Indexes for fast Leaderboard queries
            entity.HasIndex(g => new { g.Size, g.Status, g.TimeInSeconds }); 
        });
    }
}