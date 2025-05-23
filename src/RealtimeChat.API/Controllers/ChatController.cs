using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Services;
using RealtimeChat.Shared.DTOs;
using System.Security.Claims;

namespace RealtimeChat.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    [HttpGet("rooms")]
    public async Task<ActionResult<IEnumerable<ChatRoomDto>>> GetChatRooms()
    {
        var userId = GetCurrentUserId();
        var chatRooms = await _chatService.GetUserChatRoomsAsync(userId);
        return Ok(chatRooms);
    }

    [HttpPost("rooms")]
    public async Task<ActionResult<ChatRoomDto>> CreateChatRoom([FromBody] CreateChatRoomDto createChatRoomDto)
    {
        var userId = GetCurrentUserId();
        var chatRoom = await _chatService.CreateChatRoomAsync(createChatRoomDto, userId);

        if (chatRoom == null)
            return BadRequest("Failed to create chat room");

        return Ok(chatRoom);
    }

    [HttpPost("rooms/{chatRoomId:guid}/join")]
    public async Task<ActionResult> JoinChatRoom(Guid chatRoomId)
    {
        var userId = GetCurrentUserId();
        var success = await _chatService.JoinChatRoomAsync(chatRoomId, userId);

        if (!success)
            return BadRequest("Failed to join chat room");

        return Ok();
    }

    [HttpGet("rooms/{chatRoomId:guid}/messages")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(
        Guid chatRoomId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var messages = await _chatService.GetMessagesAsync(chatRoomId, page, pageSize);
        return Ok(messages);
    }
}