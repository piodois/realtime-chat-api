namespace RealtimeChat.Domain.Entities;

public class ChatRoom
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPrivate { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public User CreatedBy { get; set; } = null!;
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<ChatRoomUser> ChatRoomUsers { get; set; } = new List<ChatRoomUser>();
}