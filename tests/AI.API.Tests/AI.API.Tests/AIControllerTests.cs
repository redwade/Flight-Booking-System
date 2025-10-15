using AI.API.Controllers;
using AI.Application.Commands;
using AI.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AI.API.Tests;

public class AIControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<AIController>> _loggerMock;
    private readonly AIController _controller;

    public AIControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<AIController>>();
        _controller = new AIController(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task SendChatMessage_ReturnsOkResult_WithResponse()
    {
        // Arrange
        var request = new SendChatMessageRequest("user123", "Hello", "session-001");
        var expectedResponse = new SendChatMessageResponse("Hi there!", "session-001", DateTime.UtcNow);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<SendChatMessageCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.SendChatMessage(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<SendChatMessageResponse>(okResult.Value);
        Assert.Equal(expectedResponse.Response, response.Response);
        Assert.Equal(expectedResponse.SessionId, response.SessionId);
    }

    [Fact]
    public async Task GetFlightRecommendations_ReturnsOkResult_WithRecommendations()
    {
        // Arrange
        var request = new FlightRecommendationRequest(
            "user123",
            "New York",
            "London",
            DateTime.UtcNow.AddDays(30),
            "Business",
            2000
        );

        var expectedResponse = new FlightRecommendationsResponse(
            "Here are some recommendations...",
            DateTime.UtcNow
        );

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetFlightRecommendationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetFlightRecommendations(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<FlightRecommendationsResponse>(okResult.Value);
        Assert.Equal(expectedResponse.Recommendations, response.Recommendations);
    }

    [Fact]
    public async Task GetChatHistory_ReturnsOkResult_WithHistory()
    {
        // Arrange
        var userId = "user123";
        var expectedResponse = new ChatHistoryResponse(new List<ChatMessageDto>
        {
            new ChatMessageDto(Guid.NewGuid(), userId, "user", "Hello", DateTime.UtcNow, "session-001"),
            new ChatMessageDto(Guid.NewGuid(), userId, "assistant", "Hi there!", DateTime.UtcNow, "session-001")
        });

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetChatHistoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetChatHistory(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ChatHistoryResponse>(okResult.Value);
        Assert.Equal(2, response.Messages.Count);
    }

    [Fact]
    public void HealthCheck_ReturnsOkResult()
    {
        // Act
        var result = _controller.HealthCheck();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }
}
