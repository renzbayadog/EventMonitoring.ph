using Microsoft.AspNetCore.SignalR;

namespace EventMonitoring.Hubs
{
    public class CommunicationHub : Hub
    {
        public async Task Notification(string ClientId)
        {
            await Clients.All.SendAsync("Notify", ClientId);
        }
    }
}
