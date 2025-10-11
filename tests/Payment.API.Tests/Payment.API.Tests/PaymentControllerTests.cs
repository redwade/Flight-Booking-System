using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Payment.API.Controllers;
using Payment.Application.Commands;
using Payment.Application.Queries;
using Xunit;

namespace Payment.API.Tests;

public class PaymentControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<PaymentsController>> _loggerMock;
    private readonly PaymentsController _controller;

    public PaymentControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<PaymentsController>>();
        _controller = new PaymentsController(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ProcessPayment_ShouldReturnCreatedResult_WhenPaymentIsProcessed()
    {
        // Arrange
        var command = new ProcessPaymentCommand
        {
            BookingId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Amount = 500.00m,
            Currency = "USD",
            PaymentMethod = "CreditCard"
        };

        var response = new ProcessPaymentCommandResponse
        {
            PaymentId = Guid.NewGuid(),
            Status = "Completed",
            TransactionId = "TXN123456"
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<ProcessPaymentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.ProcessPayment(command, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<ProcessPaymentCommandResponse>(createdResult.Value);
        Assert.Equal(response.PaymentId, returnValue.PaymentId);
        Assert.Equal("Completed", returnValue.Status);
    }

    [Fact]
    public async Task GetPaymentsByBookingId_ShouldReturnOk_WithPaymentList()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var payments = new List<GetPaymentByBookingIdQueryResponse>
        {
            new GetPaymentByBookingIdQueryResponse
            {
                Id = Guid.NewGuid(),
                BookingId = bookingId,
                Amount = 500.00m,
                Status = "Completed"
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetPaymentByBookingIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(payments);

        // Act
        var result = await _controller.GetPaymentsByBookingId(bookingId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<GetPaymentByBookingIdQueryResponse>>(okResult.Value);
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
