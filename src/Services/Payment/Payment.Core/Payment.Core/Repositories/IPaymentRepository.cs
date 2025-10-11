using Payment.Core.Entities;

namespace Payment.Core.Repositories;

public interface IPaymentRepository
{
    Task<PaymentEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentEntity>> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PaymentEntity?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
    Task<PaymentEntity> CreateAsync(PaymentEntity payment, CancellationToken cancellationToken = default);
    Task<PaymentEntity> UpdateAsync(PaymentEntity payment, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
