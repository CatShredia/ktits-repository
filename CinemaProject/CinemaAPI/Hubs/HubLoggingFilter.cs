using Microsoft.AspNetCore.SignalR;

namespace CinemaAPI.Hubs;

public class HubLoggingFilter : IHubFilter
{
    private readonly ILogger<HubLoggingFilter> _logger;

    public HubLoggingFilter(ILogger<HubLoggingFilter> logger)
    {
        _logger = logger;
    }

    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        _logger.LogWarning("[HUB FILTER] Calling method: {MethodName} with args: {Args}",
            invocationContext.HubMethodName,
            string.Join(", ", invocationContext.HubMethodArguments));

        try
        {
            var result = await next(invocationContext);
            _logger.LogWarning("[HUB FILTER] Method {MethodName} completed", invocationContext.HubMethodName);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[HUB FILTER] Method {MethodName} threw exception", invocationContext.HubMethodName);
            throw;
        }
    }

    public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        _logger.LogWarning("[HUB FILTER] Client connected: {ConnectionId}, User: {UserId}",
            context.Context.ConnectionId,
            context.Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "anonymous");
        await next(context);
    }

    public async Task OnDisconnectedAsync(HubLifetimeContext context, Exception? exception, Func<HubLifetimeContext, Exception?, Task> next)
    {
        _logger.LogWarning("[HUB FILTER] Client disconnected: {ConnectionId}", context.Context.ConnectionId);
        await next(context, exception);
    }
}
