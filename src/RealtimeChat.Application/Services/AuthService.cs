using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Infrastructure.Data;
using RealtimeChat.Infrastructure.Services;
using RealtimeChat.Shared.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace RealtimeChat.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto?> RegisterAsync(CreateUserDto createUserDto);
    Task<UserDto?> GetUserByIdAsync(Guid userId);
}

public class AuthService : IAuthService
{
    private readonly ChatDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public AuthService(ChatDbContext context, IJwtService jwtService, IMapper mapper)
    {
        _context = context;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            return null;

        // Update user status
        user.IsOnline = true;
        user.LastSeen = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);
        var userDto = _mapper.Map<UserDto>(user);

        return new AuthResponseDto(token, userDto);
    }

    public async Task<AuthResponseDto?> RegisterAsync(CreateUserDto createUserDto)
    {
        // Check if user already exists
        if (await _context.Users.AnyAsync(u => u.Email == createUserDto.Email))
            return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            PasswordHash = HashPassword(createUserDto.Password),
            IsOnline = true,
            LastSeen = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);
        var userDto = _mapper.Map<UserDto>(user);

        return new AuthResponseDto(token, userDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        var passwordHash = HashPassword(password);
        return passwordHash == hash;
    }
}