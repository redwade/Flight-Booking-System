using AI.Core.Entities;

namespace AI.Core.Repositories;

public interface IChatMessageRepository
{
    Task<ChatMessage> CreateAsync(ChatMessage message);
    Task<IEnumerable<ChatMessage>> GetBySessionIdAsync(string sessionId);
    Task<IEnumerable<ChatMessage>> GetByUserIdAsync(string userId);
}
