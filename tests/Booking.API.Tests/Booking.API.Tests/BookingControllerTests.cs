using Booking.Application.Commands;
using Booking.Application.Queries;
using Booking.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Booking.API.Tests;

public class BookingControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<BookingsController>> _loggerMock;
    private readonly BookingsController _controller;

    public BookingControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<BookingsController>>();
        _controller = new BookingsController(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateBooking_ShouldReturnCreatedResult_WhenBookingIsCreated()
    {
        // Arrange
        var command = new CreateBookingCommand
        {
            FlightId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            PassengerName = "John Doe",
            PassengerEmail = "john@example.com",
            PassengerPhone = "+1234567890",
            NumberOfSeats = 2,
            TotalAmount = 500.00m
        };

        var response = new CreateBookingCommandResponse
        {
            BookingId = Guid.NewGuid(),
            BookingReference = "BK20251011001",
            Status = "Pending"
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateBookingCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.CreateBooking(command, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<CreateBookingCommandResponse>(createdResult.Value);
        Assert.Equal(response.BookingId, returnValue.BookingId);
        Assert.Equal(response.BookingReference, returnValue.BookingReference);
    }

    [Fact]
    public async Task GetBookingById_ShouldReturnOk_WhenBookingExists()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var response = new GetBookingByIdQueryResponse
        {
            Id = bookingId,
            PassengerName = "John Doe",
            Status = "Confirmed"
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetBookingByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetBookingById(bookingId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<GetBookingByIdQueryResponse>(okResult.Value);
        Assert.Equal(bookingId, returnValue.Id);
    }

    [Fact]
    public async Task GetBookingById_ShouldReturnNotFound_WhenBookingDoesNotExist()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetBookingByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetBookingByIdQueryResponse?)null);

        // Act
        var result = await _controller.GetBookingById(bookingId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
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
