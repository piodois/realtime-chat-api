namespace RealtimeChat.Shared.DTOs;

public record UserDto(
    Guid Id,
    string Username,
    string Email,
    string? Avatar,
    bool IsOnline,
    DateTime LastSeen
);

public record CreateUserDto(
    string Username,
    string Email,
    string Password
);

public record LoginDto(
    string Email,
    string Password
);

public record AuthResponseDto(
    string Token,
    UserDto User
);