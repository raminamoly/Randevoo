using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Randevoo.WebApi.Hubs;

[Authorize]
public class EventChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrWhiteSpace(userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));

        await base.OnConnectedAsync();
    }

    public static string UserGroup(long userId) => UserGroup(userId.ToString());

    private static string UserGroup(string userId) => $"user:{userId}";
}
