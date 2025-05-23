using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Services;
using RealtimeChat.Shared.DTOs;

namespace RealtimeChat.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);

        if (result == null)
            return Unauthorized("Invalid credentials");

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] CreateUserDto createUserDto)
    {
        var result = await _authService.RegisterAsync(createUserDto);

        if (result == null)
            return BadRequest("User already exists or invalid data");

        return Ok(result);
    }
}