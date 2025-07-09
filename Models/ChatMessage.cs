using System.Text.Json.Serialization;

namespace EventMonitoring.Models
{
    public class ChatMessage
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        
        [JsonPropertyName("senderName")]
        public string SenderName { get; set; } = string.Empty;
        
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = string.Empty;
        
        [JsonPropertyName("roomName")]
        public string RoomName { get; set; } = string.Empty;

        [JsonIgnore]
        public bool IsOwnMessage { get; set; } = false;
    }
} 