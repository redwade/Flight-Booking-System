using Flight.API.Controllers;
using Flight.Application.Commands;
using Flight.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Flight.API.Tests;

public class FlightControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<FlightsController>> _loggerMock;
    private readonly FlightsController _controller;

    public FlightControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<FlightsController>>();
        _controller = new FlightsController(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateFlight_ShouldReturnCreatedResult_WhenFlightIsCreated()
    {
        // Arrange
        var command = new CreateFlightCommand
        {
            FlightNumber = "FL123",
            Airline = "Test Airlines",
            DepartureAirport = "JFK",
            ArrivalAirport = "LAX",
            DepartureTime = DateTime.UtcNow.AddDays(1),
            ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(5),
            TotalSeats = 150,
            PricePerSeat = 250.00m
        };

        var response = new CreateFlightCommandResponse
        {
            FlightId = Guid.NewGuid(),
            FlightNumber = "FL123",
            Status = "Scheduled"
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateFlightCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.CreateFlight(command, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<CreateFlightCommandResponse>(createdResult.Value);
        Assert.Equal(response.FlightId, returnValue.FlightId);
    }

    [Fact]
    public async Task SearchFlights_ShouldReturnOk_WithFlightList()
    {
        // Arrange
        var flights = new List<SearchFlightsQueryResponse>
        {
            new SearchFlightsQueryResponse
            {
                Id = Guid.NewGuid(),
                FlightNumber = "FL123",
                Airline = "Test Airlines",
                DepartureAirport = "JFK",
                ArrivalAirport = "LAX",
                AvailableSeats = 100,
                PricePerSeat = 250.00m
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<SearchFlightsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(flights);

        // Act
        var result = await _controller.SearchFlights("JFK", "LAX", DateTime.UtcNow, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<SearchFlightsQueryResponse>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public void Health_ShouldReturnHealthyStatus()
    {
        // Act
        var result = _controller.Health();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }
}
