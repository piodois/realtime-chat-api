using Microsoft.EntityFrameworkCore;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Data;

public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ChatRoomUser> ChatRoomUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
        });

        // ChatRoom configuration
        modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(e => e.CreatedBy)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedById)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Message configuration
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).HasMaxLength(2000).IsRequired();

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Messages)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ChatRoom)
                  .WithMany(cr => cr.Messages)
                  .HasForeignKey(e => e.ChatRoomId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ChatRoomUser configuration (Many-to-Many)
        modelBuilder.Entity<ChatRoomUser>(entity =>
        {
            entity.HasKey(e => new { e.ChatRoomId, e.UserId });

            entity.HasOne(e => e.ChatRoom)
                  .WithMany(cr => cr.ChatRoomUsers)
                  .HasForeignKey(e => e.ChatRoomId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                  .WithMany(u => u.ChatRoomUsers)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}