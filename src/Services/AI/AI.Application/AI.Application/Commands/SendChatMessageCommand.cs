using MediatR;

namespace AI.Application.Commands;

public record SendChatMessageCommand(
    string UserId,
    string Message,
    string? SessionId = null
) : IRequest<SendChatMessageResponse>;

public record SendChatMessageResponse(
    string Response,
    string SessionId,
    DateTime Timestamp
);
