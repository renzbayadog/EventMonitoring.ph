using System.Text.Json.Serialization;

namespace EventMonitoring.Models
{
    public class ChatMessage
    {
        [JsonPropertyName("Message")]
        public string Message { get; set; } = string.Empty;
        
        [JsonPropertyName("SenderName")]
        public string SenderName { get; set; } = string.Empty;
        
        [JsonPropertyName("Timestamp")]
        public string Timestamp { get; set; } = string.Empty;
        
        [JsonPropertyName("RoomName")]
        public string RoomName { get; set; } = string.Empty;
        
        [JsonIgnore]
        public bool IsOwnMessage { get; set; } = false;
    }
} 