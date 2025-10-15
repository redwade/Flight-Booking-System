using AI.Core.Entities;
using AI.Core.Repositories;
using AI.Core.Services;
using MediatR;

namespace AI.Application.Commands;

public class SendChatMessageCommandHandler : IRequestHandler<SendChatMessageCommand, SendChatMessageResponse>
{
    private readonly IAIService _aiService;
    private readonly IChatMessageRepository _chatMessageRepository;

    public SendChatMessageCommandHandler(IAIService aiService, IChatMessageRepository chatMessageRepository)
    {
        _aiService = aiService;
        _chatMessageRepository = chatMessageRepository;
    }

    public async Task<SendChatMessageResponse> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
    {
        var sessionId = request.SessionId ?? Guid.NewGuid().ToString();

        // Save user message
        await _chatMessageRepository.CreateAsync(new ChatMessage
        {
            UserId = request.UserId,
            Role = "user",
            Content = request.Message,
            SessionId = sessionId
        });

        // Generate AI response
        var aiResponse = await _aiService.GenerateChatResponseAsync(request.Message, sessionId, cancellationToken);

        // Save assistant message
        await _chatMessageRepository.CreateAsync(new ChatMessage
        {
            UserId = request.UserId,
            Role = "assistant",
            Content = aiResponse,
            SessionId = sessionId
        });

        return new SendChatMessageResponse(aiResponse, sessionId, DateTime.UtcNow);
    }
}
