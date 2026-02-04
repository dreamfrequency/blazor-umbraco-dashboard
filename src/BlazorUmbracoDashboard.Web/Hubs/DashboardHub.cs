using Microsoft.AspNetCore.SignalR;

namespace BlazorUmbracoDashboard.Web.Hubs;

public class DashboardHub : Hub
{
    public async Task SendContentUpdate(string action, string contentName)
    {
        await Clients.Others.SendAsync("ReceiveContentUpdate", action, contentName);
    }

    public async Task SendNotification(string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", message);
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("ReceiveNotification", $"A user connected to the dashboard.");
        await base.OnConnectedAsync();
    }
}
