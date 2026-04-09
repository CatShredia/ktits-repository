using CinemaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Data;

public class DatabaseContext : DbContext
{
      public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
      {
      }

      public DbSet<Genre> Genres { get; set; } = null!;
      public DbSet<Film> Films { get; set; } = null!;
      public DbSet<Rating> Ratings { get; set; } = null!;
      public DbSet<User> Users { get; set; } = null!;
      public DbSet<Login> Logins { get; set; } = null!;
      public DbSet<Role> Roles { get; set; } = null!;
      public DbSet<Conversation> Conversations { get; set; } = null!;
      public DbSet<ConversationType> ConversationTypes { get; set; } = null!;
      public DbSet<ConversationParticipant> ConversationParticipants { get; set; } = null!;
      public DbSet<Message> Messages { get; set; } = null!;

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genre>(entity =>
            {
                  entity.HasKey(e => e.Id);
                  entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Film>(entity =>
            {
                  entity.HasKey(e => e.Id);
                  entity.Property(e => e.Name).IsRequired();
                  entity.HasOne(e => e.Genre)
                    .WithMany()
                    .HasForeignKey(e => e.GenreId)
                    .OnDelete(DeleteBehavior.SetNull);
                  entity.HasOne(e => e.Author)
                    .WithMany(u => u.Films)
                    .HasForeignKey(e => e.AuthorId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                  entity.HasKey(e => e.Id);
                  entity.Property(e => e.Value).IsRequired();
                  entity.HasOne(e => e.Film)
                    .WithMany(f => f.Ratings)
                    .HasForeignKey(e => e.FilmId)
                    .OnDelete(DeleteBehavior.Cascade);
                  entity.HasOne(e => e.Author)
                    .WithMany(u => u.Ratings)
                    .HasForeignKey(e => e.AuthorId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<User>(entity =>
            {
                  entity.HasKey(e => e.Id);
                  entity.Property(e => e.Email).IsRequired();
                  entity.HasOne(e => e.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.SetNull);
                  entity.HasOne(e => e.Login)
                    .WithOne(l => l.User)
                    .HasForeignKey<Login>(l => l.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Login>(entity =>
            {
                  entity.HasKey(e => e.Id);
                  entity.Property(e => e.LoginValue).IsRequired();
                  entity.HasIndex(e => e.LoginValue).IsUnique();
            });

            modelBuilder.Entity<Conversation>(entity =>
            {
                  entity.HasKey(e => e.Id);
                  entity.HasOne(e => e.ConversationType)
                    .WithMany(ct => ct.Conversations)
                    .HasForeignKey(e => e.ConversationTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
                  entity.HasMany(e => e.Participants)
                    .WithOne(p => p.Conversation)
                    .HasForeignKey(p => p.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);
                  entity.HasMany(e => e.Messages)
                    .WithOne(m => m.Conversation)
                    .HasForeignKey(m => m.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ConversationType>(entity =>
            {
                  entity.HasKey(e => e.Id);
                  entity.Property(e => e.Name).IsRequired();
                  entity.HasIndex(e => e.Name).IsUnique();
                  entity.HasData(
                      new ConversationType { Id = 1, Name = "Direct" },
                      new ConversationType { Id = 2, Name = "Group" },
                      new ConversationType { Id = 3, Name = "Channel" },
                      new ConversationType { Id = 4, Name = "Comments" }
                  );
            });

            modelBuilder.Entity<ConversationParticipant>(entity =>
            {
                  entity.HasKey(e => new { e.ConversationId, e.UserId });
                  entity.HasOne(e => e.Conversation)
                    .WithMany(c => c.Participants)
                    .HasForeignKey(e => e.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);
                  entity.HasOne(e => e.User)
                    .WithMany(u => u.ConversationParticipants)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                  entity.HasKey(e => e.Id);
                  entity.Property(e => e.Content).IsRequired();
                  entity.HasOne(e => e.Conversation)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(e => e.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);
                  entity.HasOne(e => e.Sender)
                    .WithMany(u => u.Messages)
                    .HasForeignKey(e => e.SenderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
      }
}
