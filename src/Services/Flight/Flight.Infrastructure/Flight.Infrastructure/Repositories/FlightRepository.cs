using Flight.Core.Entities;
using Flight.Core.Repositories;
using MongoDB.Driver;

namespace Flight.Infrastructure.Repositories;

public class FlightRepository : IFlightRepository
{
    private readonly IMongoCollection<FlightEntity> _flights;

    public FlightRepository(IMongoDatabase database)
    {
        _flights = database.GetCollection<FlightEntity>("flights");
    }

    public async Task<FlightEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cursor = await _flights.FindAsync(f => f.Id == id, cancellationToken: cancellationToken);
        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<FlightEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cursor = await _flights.FindAsync(_ => true, cancellationToken: cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FlightEntity>> GetByFlightNumberAsync(string flightNumber, CancellationToken cancellationToken = default)
    {
        var cursor = await _flights.FindAsync(f => f.FlightNumber == flightNumber, cancellationToken: cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FlightEntity>> SearchFlightsAsync(string departureAirport, string arrivalAirport, DateTime departureDate, CancellationToken cancellationToken = default)
    {
        var startDate = departureDate.Date;
        var endDate = startDate.AddDays(1);

        var filter = Builders<FlightEntity>.Filter.And(
            Builders<FlightEntity>.Filter.Eq(f => f.DepartureAirport, departureAirport),
            Builders<FlightEntity>.Filter.Eq(f => f.ArrivalAirport, arrivalAirport),
            Builders<FlightEntity>.Filter.Gte(f => f.DepartureTime, startDate),
            Builders<FlightEntity>.Filter.Lt(f => f.DepartureTime, endDate)
        );

        var cursor = await _flights.FindAsync(filter, cancellationToken: cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public async Task<FlightEntity> CreateAsync(FlightEntity flight, CancellationToken cancellationToken = default)
    {
        flight.Id = Guid.NewGuid();
        flight.CreatedAt = DateTime.UtcNow;
        await _flights.InsertOneAsync(flight, cancellationToken: cancellationToken);
        return flight;
    }

    public async Task<FlightEntity> UpdateAsync(FlightEntity flight, CancellationToken cancellationToken = default)
    {
        flight.UpdatedAt = DateTime.UtcNow;
        await _flights.ReplaceOneAsync(f => f.Id == flight.Id, flight, cancellationToken: cancellationToken);
        return flight;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _flights.DeleteOneAsync(f => f.Id == id, cancellationToken);
        return result.DeletedCount > 0;
    }

    public async Task<bool> UpdateAvailableSeatsAsync(Guid id, int seats, CancellationToken cancellationToken = default)
    {
        var update = Builders<FlightEntity>.Update
            .Set(f => f.AvailableSeats, seats)
            .Set(f => f.UpdatedAt, DateTime.UtcNow);

        var result = await _flights.UpdateOneAsync(f => f.Id == id, update, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }
}
