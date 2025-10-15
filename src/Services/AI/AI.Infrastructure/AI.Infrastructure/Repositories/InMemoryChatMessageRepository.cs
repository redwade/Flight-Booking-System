using AI.Core.Entities;
using AI.Core.Repositories;
using System.Collections.Concurrent;

namespace AI.Infrastructure.Repositories;

public class InMemoryChatMessageRepository : IChatMessageRepository
{
    private readonly ConcurrentDictionary<Guid, ChatMessage> _messages = new();

    public Task<ChatMessage> CreateAsync(ChatMessage message)
    {
        message.Id = Guid.NewGuid();
        message.Timestamp = DateTime.UtcNow;
        _messages.TryAdd(message.Id, message);
        return Task.FromResult(message);
    }

    public Task<IEnumerable<ChatMessage>> GetBySessionIdAsync(string sessionId)
    {
        var messages = _messages.Values
            .Where(m => m.SessionId == sessionId)
            .OrderBy(m => m.Timestamp)
            .AsEnumerable();
        return Task.FromResult(messages);
    }

    public Task<IEnumerable<ChatMessage>> GetByUserIdAsync(string userId)
    {
        var messages = _messages.Values
            .Where(m => m.UserId == userId)
            .OrderBy(m => m.Timestamp)
            .AsEnumerable();
        return Task.FromResult(messages);
    }
}
