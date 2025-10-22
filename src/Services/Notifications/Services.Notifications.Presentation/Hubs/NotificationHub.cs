using Common.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Services.Notifications.Presentation.Hubs;

#pragma warning disable CA1515 // Consider making public types internal
[Authorize]
public class NotificationHub : Hub
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected to NotificationHub. ConnectionId: {ConnectionId}", Context.ConnectionId);

        // auto join System Admin group if the connected user is a System Admin
        if (Context.User?.IsInRole(CommonAppCts.Roles.SystemAdmin) == true)
        {
            string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await JoinSystemAdminGroup(userId);
            }
        }
        
        await base.OnConnectedAsync();
    }

    public async Task JoinSystemAdminGroup(string systemAdminId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"SystemAdmin_{systemAdminId}");
        _logger.LogInformation("System admin {SystemAdminId} joined group. ConnectionId: {ConnectionId}", systemAdminId, Context.ConnectionId);
    }

    public async Task LeaveSystemAdminGroup(string systemAdminId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"SystemAdmin_{systemAdminId}");
        _logger.LogInformation("System admin {SystemAdminId} left group. ConnectionId: {ConnectionId}", systemAdminId, Context.ConnectionId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
        {
            _logger.LogWarning(exception, "Client disconnected with error. ConnectionId: {ConnectionId}", Context.ConnectionId);
        }
        else
        {
            _logger.LogInformation("Client disconnected from NotificationHub. ConnectionId: {ConnectionId}", Context.ConnectionId);
        }
        await base.OnDisconnectedAsync(exception);
    }
}
