using Microsoft.EntityFrameworkCore;

namespace ArkanoidAPI.Models;

/// <summary>
/// Контекст базы данных для Arkanoid API
/// </summary>
public class ArkanoidDbContext : DbContext
{
    public ArkanoidDbContext(DbContextOptions<ArkanoidDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Таблица пользователей
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Таблица скинов
    /// </summary>
    public DbSet<Skin> Skins { get; set; }

    /// <summary>
    /// Таблица инвентаря пользователей (связь User-Skin)
    /// </summary>
    public DbSet<UserSkin> UserSkins { get; set; }

    /// <summary>
    /// Таблица истории покупок
    /// </summary>
    public DbSet<Purchase> Purchases { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Конфигурация User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Coins).HasDefaultValue(100);
        });

        // Конфигурация Skin
        modelBuilder.Entity<Skin>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Price).HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        // Конфигурация UserSkin
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

        // Конфигурация Purchase
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
