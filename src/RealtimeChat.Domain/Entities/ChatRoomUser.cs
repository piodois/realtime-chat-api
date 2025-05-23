namespace RealtimeChat.Domain.Entities;

public class ChatRoomUser
{
    public Guid ChatRoomId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinedAt { get; set; }
    public string Role { get; set; } = "Member"; // Member, Admin, Owner

    // Navigation properties
    public ChatRoom ChatRoom { get; set; } = null!;
    public User User { get; set; } = null!;
}