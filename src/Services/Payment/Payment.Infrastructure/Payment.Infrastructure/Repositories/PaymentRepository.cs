using Microsoft.EntityFrameworkCore;
using Payment.Core.Entities;
using Payment.Core.Repositories;
using Payment.Infrastructure.Data;

namespace Payment.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _context;

    public PaymentRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<PaymentEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Payments.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PaymentEntity>> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Where(p => p.BookingId == bookingId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PaymentEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Where(p => p.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaymentEntity?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.TransactionId == transactionId, cancellationToken);
    }

    public async Task<PaymentEntity> CreateAsync(PaymentEntity payment, CancellationToken cancellationToken = default)
    {
        payment.Id = Guid.NewGuid();
        payment.CreatedAt = DateTime.UtcNow;
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(cancellationToken);
        return payment;
    }

    public async Task<PaymentEntity> UpdateAsync(PaymentEntity payment, CancellationToken cancellationToken = default)
    {
        payment.UpdatedAt = DateTime.UtcNow;
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync(cancellationToken);
        return payment;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var payment = await GetByIdAsync(id, cancellationToken);
        if (payment == null) return false;

        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
