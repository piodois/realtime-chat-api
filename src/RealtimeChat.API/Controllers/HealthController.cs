using Microsoft.AspNetCore.Mvc;

namespace RealtimeChat.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public ActionResult<object> Get()
    {
        return Ok(new
        {
            message = "¡API funcionando correctamente!",
            timestamp = DateTime.UtcNow,
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
        });
    }

    [HttpGet("ping")]
    public ActionResult<string> Ping()
    {
        return Ok("pong");
    }
}