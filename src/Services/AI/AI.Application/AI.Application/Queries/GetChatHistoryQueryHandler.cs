using AI.Core.Repositories;
using MediatR;

namespace AI.Application.Queries;

public class GetChatHistoryQueryHandler : IRequestHandler<GetChatHistoryQuery, ChatHistoryResponse>
{
    private readonly IChatMessageRepository _chatMessageRepository;

    public GetChatHistoryQueryHandler(IChatMessageRepository chatMessageRepository)
    {
        _chatMessageRepository = chatMessageRepository;
    }

    public async Task<ChatHistoryResponse> Handle(GetChatHistoryQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<AI.Core.Entities.ChatMessage> messages;

        if (!string.IsNullOrEmpty(request.SessionId))
        {
            messages = await _chatMessageRepository.GetBySessionIdAsync(request.SessionId);
        }
        else if (!string.IsNullOrEmpty(request.UserId))
        {
            messages = await _chatMessageRepository.GetByUserIdAsync(request.UserId);
        }
        else
        {
            return new ChatHistoryResponse(new List<ChatMessageDto>());
        }

        var messageDtos = messages.Select(m => new ChatMessageDto(
            m.Id,
            m.UserId,
            m.Role,
            m.Content,
            m.Timestamp,
            m.SessionId
        )).ToList();

        return new ChatHistoryResponse(messageDtos);
    }
}
