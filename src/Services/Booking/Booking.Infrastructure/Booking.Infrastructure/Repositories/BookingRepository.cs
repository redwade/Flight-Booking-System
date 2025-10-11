using Booking.Core.Entities;
using Booking.Core.Repositories;
using StackExchange.Redis;
using System.Text.Json;

namespace Booking.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly IDatabase _database;
    private const string KeyPrefix = "booking:";

    public BookingRepository(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task<BookingEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var value = await _database.StringGetAsync($"{KeyPrefix}{id}");
        return value.HasValue ? JsonSerializer.Deserialize<BookingEntity>(value!) : null;
    }

    public async Task<IEnumerable<BookingEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{KeyPrefix}*").ToList();
        
        var bookings = new List<BookingEntity>();
        foreach (var key in keys)
        {
            var value = await _database.StringGetAsync(key);
            if (value.HasValue)
            {
                var booking = JsonSerializer.Deserialize<BookingEntity>(value!);
                if (booking != null) bookings.Add(booking);
            }
        }
        return bookings;
    }

    public async Task<IEnumerable<BookingEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var allBookings = await GetAllAsync(cancellationToken);
        return allBookings.Where(b => b.UserId == userId);
    }

    public async Task<IEnumerable<BookingEntity>> GetByFlightIdAsync(Guid flightId, CancellationToken cancellationToken = default)
    {
        var allBookings = await GetAllAsync(cancellationToken);
        return allBookings.Where(b => b.FlightId == flightId);
    }

    public async Task<BookingEntity?> GetByBookingReferenceAsync(string bookingReference, CancellationToken cancellationToken = default)
    {
        var allBookings = await GetAllAsync(cancellationToken);
        return allBookings.FirstOrDefault(b => b.BookingReference == bookingReference);
    }

    public async Task<BookingEntity> CreateAsync(BookingEntity booking, CancellationToken cancellationToken = default)
    {
        booking.Id = Guid.NewGuid();
        booking.CreatedAt = DateTime.UtcNow;
        var json = JsonSerializer.Serialize(booking);
        await _database.StringSetAsync($"{KeyPrefix}{booking.Id}", json);
        return booking;
    }

    public async Task<BookingEntity> UpdateAsync(BookingEntity booking, CancellationToken cancellationToken = default)
    {
        booking.UpdatedAt = DateTime.UtcNow;
        var json = JsonSerializer.Serialize(booking);
        await _database.StringSetAsync($"{KeyPrefix}{booking.Id}", json);
        return booking;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _database.KeyDeleteAsync($"{KeyPrefix}{id}");
    }
}
