using BuildingBlocks.MassTransit.Messages;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MassTransit.Tests;

public class MassTransitIntegrationTests
{
    [Fact]
    public async Task BookingCreatedEvent_ShouldBePublished()
    {
        // Arrange
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddHandler<BookingCreatedEvent>(async context =>
                {
                    await Task.CompletedTask;
                });
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        try
        {
            // Act
            await harness.Bus.Publish(new BookingCreatedEvent
            {
                BookingId = Guid.NewGuid(),
                FlightId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                PassengerName = "Test User",
                PassengerEmail = "test@example.com",
                NumberOfSeats = 2,
                TotalAmount = 500.00m,
                BookingDate = DateTime.UtcNow
            });

            // Assert
            Assert.True(await harness.Published.Any<BookingCreatedEvent>());
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Fact]
    public async Task PaymentProcessedEvent_ShouldBePublished()
    {
        // Arrange
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddHandler<PaymentProcessedEvent>(async context =>
                {
                    await Task.CompletedTask;
                });
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        try
        {
            // Act
            await harness.Bus.Publish(new PaymentProcessedEvent
            {
                PaymentId = Guid.NewGuid(),
                BookingId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Amount = 500.00m,
                PaymentStatus = "Completed",
                TransactionId = "TXN123",
                PaymentDate = DateTime.UtcNow
            });

            // Assert
            Assert.True(await harness.Published.Any<PaymentProcessedEvent>());
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Fact]
    public async Task SendNotificationCommand_ShouldBePublished()
    {
        // Arrange
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddHandler<SendNotificationCommand>(async context =>
                {
                    await Task.CompletedTask;
                });
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        try
        {
            // Act
            await harness.Bus.Publish(new SendNotificationCommand
            {
                UserId = Guid.NewGuid(),
                BookingId = Guid.NewGuid(),
                NotificationType = "Email",
                Subject = "Test Notification",
                Message = "This is a test message",
                Recipient = "test@example.com"
            });

            // Assert
            Assert.True(await harness.Published.Any<SendNotificationCommand>());
        }
        finally
        {
            await harness.Stop();
        }
    }
}
