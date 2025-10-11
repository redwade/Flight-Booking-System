using BuildingBlocks.MassTransit.Messages;
using MassTransit;
using MediatR;
using Payment.Core.Entities;
using Payment.Core.Repositories;

namespace Payment.Application.Commands;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, ProcessPaymentCommandResponse>
{
    private readonly IPaymentRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ProcessPaymentCommandHandler(IPaymentRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<ProcessPaymentCommandResponse> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = new PaymentEntity
        {
            BookingId = request.BookingId,
            UserId = request.UserId,
            Amount = request.Amount,
            Currency = request.Currency,
            PaymentMethod = Enum.Parse<PaymentMethod>(request.PaymentMethod, true),
            Status = PaymentStatus.Processing,
            PaymentDate = DateTime.UtcNow,
            TransactionId = GenerateTransactionId()
        };

        var createdPayment = await _repository.CreateAsync(payment, cancellationToken);

        // Simulate payment processing
        await Task.Delay(1000, cancellationToken);
        
        // Update payment status
        createdPayment.Status = PaymentStatus.Completed;
        await _repository.UpdateAsync(createdPayment, cancellationToken);

        // Publish payment processed event
        await _publishEndpoint.Publish(new PaymentProcessedEvent
        {
            PaymentId = createdPayment.Id,
            BookingId = createdPayment.BookingId,
            UserId = createdPayment.UserId,
            Amount = createdPayment.Amount,
            PaymentStatus = createdPayment.Status.ToString(),
            TransactionId = createdPayment.TransactionId,
            PaymentDate = createdPayment.PaymentDate
        }, cancellationToken);

        return new ProcessPaymentCommandResponse
        {
            PaymentId = createdPayment.Id,
            Status = createdPayment.Status.ToString(),
            TransactionId = createdPayment.TransactionId
        };
    }

    private static string GenerateTransactionId()
    {
        return $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(10000, 99999)}";
    }
}
