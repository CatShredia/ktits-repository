using Microsoft.EntityFrameworkCore;
using RustyAPI.Database.Models;

namespace RustyAPI.Database;

public class RustyDbContext : DbContext
{
    public RustyDbContext(DbContextOptions<RustyDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserLevelProgress> UserLevelProgresses => Set<UserLevelProgress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Coins).HasDefaultValue(0);
            entity.Property(e => e.LastCompletedLevelIndex).HasDefaultValue(0);
        });

        modelBuilder.Entity<UserLevelProgress>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.LevelKey }).IsUnique();
            entity.Property(e => e.LevelKey).HasMaxLength(100).IsRequired();
            entity.Property(e => e.StarsCollected).HasDefaultValue(0);
            entity.Property(e => e.Completed).HasDefaultValue(false);

            entity.HasOne(e => e.User)
                  .WithMany(u => u.LevelProgresses)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
