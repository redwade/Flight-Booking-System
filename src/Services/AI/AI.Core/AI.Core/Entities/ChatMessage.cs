namespace AI.Core.Entities;

public class ChatMessage
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // "user" or "assistant"
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? SessionId { get; set; }
}
