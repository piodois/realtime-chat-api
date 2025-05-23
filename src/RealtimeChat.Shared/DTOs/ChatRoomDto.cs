namespace RealtimeChat.Shared.DTOs;

public record ChatRoomDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsPrivate,
    UserDto CreatedBy,
    DateTime CreatedAt,
    int MemberCount
);

public record CreateChatRoomDto(
    string Name,
    string? Description,
    bool IsPrivate
);

public record MessageDto(
    Guid Id,
    string Content,
    UserDto User,
    DateTime SentAt,
    bool IsEdited,
    DateTime? EditedAt
);

public record SendMessageDto(
    string Content,
    Guid ChatRoomId
);