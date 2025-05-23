using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RealtimeChat.Application.Services;
using RealtimeChat.Shared.DTOs;
using System.Security.Claims;

namespace RealtimeChat.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    public async Task JoinRoom(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
    }

    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
    }

    public async Task SendMessage(SendMessageDto sendMessageDto)
    {
        var userId = GetCurrentUserId();
        var message = await _chatService.SendMessageAsync(sendMessageDto, userId);

        if (message != null)
        {
            await Clients.Group(sendMessageDto.ChatRoomId.ToString())
                .SendAsync("ReceiveMessage", message);
        }
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();
        await Clients.All.SendAsync("UserConnected", userId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetCurrentUserId();
        await Clients.All.SendAsync("UserDisconnected", userId);
        await base.OnDisconnectedAsync(exception);
    }
}