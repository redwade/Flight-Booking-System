using Booking.Core.Entities;

namespace Booking.Core.Repositories;

public interface IBookingRepository
{
    Task<BookingEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookingEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<BookingEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookingEntity>> GetByFlightIdAsync(Guid flightId, CancellationToken cancellationToken = default);
    Task<BookingEntity?> GetByBookingReferenceAsync(string bookingReference, CancellationToken cancellationToken = default);
    Task<BookingEntity> CreateAsync(BookingEntity booking, CancellationToken cancellationToken = default);
    Task<BookingEntity> UpdateAsync(BookingEntity booking, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
