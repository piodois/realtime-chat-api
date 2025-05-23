namespace RealtimeChat.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public Guid ChatRoomId { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ChatRoom ChatRoom { get; set; } = null!;
}