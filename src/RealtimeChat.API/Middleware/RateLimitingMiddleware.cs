using System.Collections.Concurrent;

namespace RealtimeChat.API.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ConcurrentDictionary<string, UserRequestInfo> _requests = new();
    private readonly int _maxRequests = 100; // Max requests per minute
    private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1);

    public RateLimitingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientId(context);

        if (!IsRequestAllowed(clientId))
        {
            context.Response.StatusCode = 429; // Too Many Requests
            await context.Response.WriteAsync("Rate limit exceeded");
            return;
        }

        await _next(context);
    }

    private string GetClientId(HttpContext context)
    {
        // Use IP address or user ID from JWT token
        var userIdClaim = context.User?.FindFirst("sub")?.Value;
        return userIdClaim ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private bool IsRequestAllowed(string clientId)
    {
        var now = DateTime.UtcNow;

        _requests.AddOrUpdate(clientId,
            new UserRequestInfo { LastRequest = now, RequestCount = 1 },
            (key, existing) =>
            {
                // Reset counter if time window has passed
                if (now - existing.LastRequest > _timeWindow)
                {
                    existing.RequestCount = 1;
                    existing.LastRequest = now;
                }
                else
                {
                    existing.RequestCount++;
                }
                return existing;
            });

        return _requests[clientId].RequestCount <= _maxRequests;
    }

    private class UserRequestInfo
    {
        public DateTime LastRequest { get; set; }
        public int RequestCount { get; set; }
    }
}