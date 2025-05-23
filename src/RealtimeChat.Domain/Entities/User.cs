using System.ComponentModel.DataAnnotations;

namespace RealtimeChat.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public bool IsOnline { get; set; }
    public DateTime LastSeen { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<ChatRoomUser> ChatRoomUsers { get; set; } = new List<ChatRoomUser>();
}