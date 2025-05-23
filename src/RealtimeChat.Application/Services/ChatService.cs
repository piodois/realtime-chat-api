using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Infrastructure.Data;
using RealtimeChat.Shared.DTOs;

namespace RealtimeChat.Application.Services;

public interface IChatService
{
    Task<IEnumerable<ChatRoomDto>> GetUserChatRoomsAsync(Guid userId);
    Task<ChatRoomDto?> CreateChatRoomAsync(CreateChatRoomDto createChatRoomDto, Guid userId);
    Task<bool> JoinChatRoomAsync(Guid chatRoomId, Guid userId);
    Task<IEnumerable<MessageDto>> GetMessagesAsync(Guid chatRoomId, int page = 1, int pageSize = 50);
    Task<MessageDto?> SendMessageAsync(SendMessageDto sendMessageDto, Guid userId);
}

public class ChatService : IChatService
{
    private readonly ChatDbContext _context;
    private readonly IMapper _mapper;

    public ChatService(ChatDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ChatRoomDto>> GetUserChatRoomsAsync(Guid userId)
    {
        var chatRooms = await _context.ChatRoomUsers
            .Where(cru => cru.UserId == userId)
            .Include(cru => cru.ChatRoom)
            .ThenInclude(cr => cr.CreatedBy)
            .Include(cru => cru.ChatRoom.ChatRoomUsers)
            .Select(cru => cru.ChatRoom)
            .ToListAsync();

        return chatRooms.Select(cr => new ChatRoomDto(
            cr.Id,
            cr.Name,
            cr.Description,
            cr.IsPrivate,
            _mapper.Map<UserDto>(cr.CreatedBy),
            cr.CreatedAt,
            cr.ChatRoomUsers.Count
        ));
    }

    public async Task<ChatRoomDto?> CreateChatRoomAsync(CreateChatRoomDto createChatRoomDto, Guid userId)
    {
        var chatRoom = new ChatRoom
        {
            Id = Guid.NewGuid(),
            Name = createChatRoomDto.Name,
            Description = createChatRoomDto.Description,
            IsPrivate = createChatRoomDto.IsPrivate,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.ChatRooms.Add(chatRoom);

        // Add creator as owner
        var chatRoomUser = new ChatRoomUser
        {
            ChatRoomId = chatRoom.Id,
            UserId = userId,
            JoinedAt = DateTime.UtcNow,
            Role = "Owner"
        };

        _context.ChatRoomUsers.Add(chatRoomUser);
        await _context.SaveChangesAsync();

        // Load related data for response
        await _context.Entry(chatRoom)
            .Reference(cr => cr.CreatedBy)
            .LoadAsync();

        return new ChatRoomDto(
            chatRoom.Id,
            chatRoom.Name,
            chatRoom.Description,
            chatRoom.IsPrivate,
            _mapper.Map<UserDto>(chatRoom.CreatedBy),
            chatRoom.CreatedAt,
            1
        );
    }

    public async Task<bool> JoinChatRoomAsync(Guid chatRoomId, Guid userId)
    {
        // Check if user is already in the room
        if (await _context.ChatRoomUsers
            .AnyAsync(cru => cru.ChatRoomId == chatRoomId && cru.UserId == userId))
            return false;

        var chatRoomUser = new ChatRoomUser
        {
            ChatRoomId = chatRoomId,
            UserId = userId,
            JoinedAt = DateTime.UtcNow,
            Role = "Member"
        };

        _context.ChatRoomUsers.Add(chatRoomUser);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<MessageDto>> GetMessagesAsync(Guid chatRoomId, int page = 1, int pageSize = 50)
    {
        var messages = await _context.Messages
            .Where(m => m.ChatRoomId == chatRoomId)
            .Include(m => m.User)
            .OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return messages.Select(m => new MessageDto(
            m.Id,
            m.Content,
            _mapper.Map<UserDto>(m.User),
            m.SentAt,
            m.IsEdited,
            m.EditedAt
        )).Reverse();
    }

    public async Task<MessageDto?> SendMessageAsync(SendMessageDto sendMessageDto, Guid userId)
    {
        // Verify user is in chat room
        if (!await _context.ChatRoomUsers
            .AnyAsync(cru => cru.ChatRoomId == sendMessageDto.ChatRoomId && cru.UserId == userId))
            return null;

        var message = new Message
        {
            Id = Guid.NewGuid(),
            Content = sendMessageDto.Content,
            UserId = userId,
            ChatRoomId = sendMessageDto.ChatRoomId,
            SentAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Load user data
        await _context.Entry(message)
            .Reference(m => m.User)
            .LoadAsync();

        return new MessageDto(
            message.Id,
            message.Content,
            _mapper.Map<UserDto>(message.User),
            message.SentAt,
            message.IsEdited,
            message.EditedAt
        );
    }
}