using Flight.Core.Entities;

namespace Flight.Core.Repositories;

public interface IFlightRepository
{
    Task<FlightEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightEntity>> GetByFlightNumberAsync(string flightNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightEntity>> SearchFlightsAsync(string departureAirport, string arrivalAirport, DateTime departureDate, CancellationToken cancellationToken = default);
    Task<FlightEntity> CreateAsync(FlightEntity flight, CancellationToken cancellationToken = default);
    Task<FlightEntity> UpdateAsync(FlightEntity flight, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> UpdateAvailableSeatsAsync(Guid id, int seats, CancellationToken cancellationToken = default);
}
