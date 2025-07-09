using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;

namespace EventMonitoring.Services
{
    public interface IChatService
    {
        Task InitializeAsync();
        Task SendMessageAsync(string message, string senderName, string roomName = "general");
        Task JoinRoomAsync(string roomName, string userName);
        Task LeaveRoomAsync(string roomName, string userName);
        Task GetUserListAsync(string roomName);
        void OnMessageReceived(Action<object> callback);
        void OnUserJoined(Action<string, string> callback);
        void OnUserLeft(Action<string, string> callback);
        void OnUserListUpdated(Action<List<string>> callback);
        Task DisconnectAsync();
        bool IsConnected { get; }
    }

    public class ChatService : IChatService, IAsyncDisposable
    {
        private readonly NavigationManager _navigationManager;
        private HubConnection? _hubConnection;
        private bool _isInitialized = false;

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public ChatService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            var hubUrl = _navigationManager.ToAbsoluteUri("/communicationhub");
            Console.WriteLine($"Connecting to SignalR hub at: {hubUrl}");

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            try
            {
                await _hubConnection.StartAsync();
                _isInitialized = true;
                Console.WriteLine($"SignalR connection established. State: {_hubConnection.State}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to SignalR hub: {ex.Message}");
                throw;
            }
        }

        public async Task SendMessageAsync(string message, string senderName, string roomName = "general")
        {
            try
            {
                Console.WriteLine($"ChatService.SendMessageAsync called with: message='{message}', sender='{senderName}', room='{roomName}'");
                Console.WriteLine($"Hub connection state: {_hubConnection?.State}");
                
                if (_hubConnection?.State == HubConnectionState.Connected)
                {
                    Console.WriteLine("Invoking SendMessage on hub...");
                    await _hubConnection.InvokeAsync("SendMessage", message, senderName, roomName);
                    Console.WriteLine("SendMessage invoked successfully");
                }
                else
                {
                    Console.WriteLine("Hub connection is not connected. Current state: " + _hubConnection?.State);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                Console.WriteLine($"Exception type: {ex.GetType().Name}");
            }
        }

        public async Task JoinRoomAsync(string roomName, string userName)
        {
            try
            {
                if (_hubConnection?.State == HubConnectionState.Connected)
                {
                    await _hubConnection.InvokeAsync("JoinChatRoom", roomName, userName);
                }
                else
                {
                    Console.WriteLine("Hub connection is not connected. Current state: " + _hubConnection?.State);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error joining room: {ex.Message}");
            }
        }

        public async Task LeaveRoomAsync(string roomName, string userName)
        {
            try
            {
                if (_hubConnection?.State == HubConnectionState.Connected)
                {
                    await _hubConnection.InvokeAsync("LeaveChatRoom", roomName, userName);
                }
                else
                {
                    Console.WriteLine("Hub connection is not connected. Current state: " + _hubConnection?.State);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error leaving room: {ex.Message}");
            }
        }

        public async Task GetUserListAsync(string roomName)
        {
            try
            {
                if (_hubConnection?.State == HubConnectionState.Connected)
                {
                    await _hubConnection.InvokeAsync("GetUserList", roomName);
                }
                else
                {
                    Console.WriteLine("Hub connection is not connected. Current state: " + _hubConnection?.State);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user list: {ex.Message}");
            }
        }

        public void OnMessageReceived(Action<object> callback)
        {
            _hubConnection?.On<object>("ReceiveMessage", callback);
        }

        public void OnUserJoined(Action<string, string> callback)
        {
            _hubConnection?.On<string, string>("UserJoined", callback);
        }

        public void OnUserLeft(Action<string, string> callback)
        {
            _hubConnection?.On<string, string>("UserLeft", callback);
        }

        public void OnUserListUpdated(Action<List<string>> callback)
        {
            _hubConnection?.On<List<string>>("UpdateUserList", callback);
        }

        public async Task DisconnectAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
                _isInitialized = false;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync();
        }
    }
} 