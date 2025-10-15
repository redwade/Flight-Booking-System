using MediatR;

namespace AI.Application.Queries;

public record GetChatHistoryQuery(
    string? UserId = null,
    string? SessionId = null
) : IRequest<ChatHistoryResponse>;

public record ChatHistoryResponse(
    List<ChatMessageDto> Messages
);

public record ChatMessageDto(
    Guid Id,
    string UserId,
    string Role,
    string Content,
    DateTime Timestamp,
    string? SessionId
);
