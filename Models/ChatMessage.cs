namespace EventMonitoring.Models
{
    public class ChatMessage
    {
        public string Message { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public bool IsOwnMessage { get; set; } = false;
    }
} 