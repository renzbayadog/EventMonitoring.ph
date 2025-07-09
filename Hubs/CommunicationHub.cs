using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace EventMonitoring.Hubs
{
    public class CommunicationHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _userConnections = new();
        private static readonly ConcurrentDictionary<string, List<string>> _chatRooms = new();

        public async Task Notification(string ClientId)
        {
            await Clients.All.SendAsync("Notify", ClientId);
        }

        // Chat functionality
        public async Task SendMessage(string message, string senderName, string roomName = "general")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message) || string.IsNullOrWhiteSpace(senderName))
                {
                    return;
                }

                var chatMessage = new
                {
                    Message = message,
                    SenderName = senderName,
                    Timestamp = DateTime.Now.ToString("HH:mm"),
                    RoomName = roomName ?? "general"
                };

                await Clients.Group(roomName ?? "general").SendAsync("ReceiveMessage", chatMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        public async Task JoinChatRoom(string roomName, string userName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            
            if (!_chatRooms.ContainsKey(roomName))
            {
                _chatRooms[roomName] = new List<string>();
            }
            
            if (!_chatRooms[roomName].Contains(userName))
            {
                _chatRooms[roomName].Add(userName);
            }

            await Clients.Group(roomName).SendAsync("UserJoined", userName, roomName);
            await Clients.Group(roomName).SendAsync("UpdateUserList", _chatRooms[roomName]);
        }

        public async Task LeaveChatRoom(string roomName, string userName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            
            if (_chatRooms.ContainsKey(roomName))
            {
                _chatRooms[roomName].Remove(userName);
                await Clients.Group(roomName).SendAsync("UserLeft", userName, roomName);
                await Clients.Group(roomName).SendAsync("UpdateUserList", _chatRooms[roomName]);
            }
        }

        public async Task GetUserList(string roomName)
        {
            if (_chatRooms.ContainsKey(roomName))
            {
                await Clients.Caller.SendAsync("UpdateUserList", _chatRooms[roomName]);
            }
        }

        public override async Task OnConnectedAsync()
        {
            _userConnections[Context.ConnectionId] = Context.ConnectionId;
            await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _userConnections.TryRemove(Context.ConnectionId, out _);
            await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
