using Microsoft.EntityFrameworkCore;
using ArkanoidAPI.Models;

namespace ArkanoidAPI.Database;

public class ArkanoidDbContext : DbContext
{
    public ArkanoidDbContext(DbContextOptions<ArkanoidDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Skin> Skins { get; set; }

    public DbSet<UserSkin> UserSkins { get; set; }

    public DbSet<Purchase> Purchases { get; set; }

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
            entity.Property(e => e.Coins).HasDefaultValue(100);
        });

        modelBuilder.Entity<Skin>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Price).HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<UserSkin>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.SkinId }).IsUnique();

            entity.HasOne(e => e.User)
                  .WithMany(u => u.UserSkins)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Skin)
                  .WithMany(s => s.UserSkins)
                  .HasForeignKey(e => e.SkinId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Purchases)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Skin)
                  .WithMany(s => s.Purchases)
                  .HasForeignKey(e => e.SkinId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
