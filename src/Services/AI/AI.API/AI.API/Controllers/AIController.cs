using AI.Application.Commands;
using AI.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AI.API.Controllers;

[ApiController]
[Route("api/ai")]
public class AIController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AIController> _logger;

    public AIController(IMediator mediator, ILogger<AIController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Send a chat message to the AI assistant
    /// </summary>
    [HttpPost("chat")]
    public async Task<ActionResult<SendChatMessageResponse>> SendChatMessage([FromBody] SendChatMessageRequest request)
    {
        try
        {
            var command = new SendChatMessageCommand(
                request.UserId,
                request.Message,
                request.SessionId
            );

            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat message");
            return StatusCode(500, "An error occurred while processing your message");
        }
    }

    /// <summary>
    /// Get flight recommendations based on user preferences
    /// </summary>
    [HttpPost("recommendations")]
    public async Task<ActionResult<FlightRecommendationsResponse>> GetFlightRecommendations([FromBody] FlightRecommendationRequest request)
    {
        try
        {
            var query = new GetFlightRecommendationsQuery(
                request.UserId,
                request.Origin,
                request.Destination,
                request.DepartureDate,
                request.PreferredClass,
                request.MaxBudget
            );

            var response = await _mediator.Send(query);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating flight recommendations");
            return StatusCode(500, "An error occurred while generating recommendations");
        }
    }

    /// <summary>
    /// Get chat history for a user or session
    /// </summary>
    [HttpGet("chat/history")]
    public async Task<ActionResult<ChatHistoryResponse>> GetChatHistory([FromQuery] string? userId = null, [FromQuery] string? sessionId = null)
    {
        try
        {
            var query = new GetChatHistoryQuery(userId, sessionId);
            var response = await _mediator.Send(query);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving chat history");
            return StatusCode(500, "An error occurred while retrieving chat history");
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new { status = "healthy", service = "AI Service", timestamp = DateTime.UtcNow });
    }
}

// Request DTOs
public record SendChatMessageRequest(
    string UserId,
    string Message,
    string? SessionId = null
);

public record FlightRecommendationRequest(
    string UserId,
    string Origin,
    string Destination,
    DateTime? DepartureDate = null,
    string? PreferredClass = null,
    decimal? MaxBudget = null
);
