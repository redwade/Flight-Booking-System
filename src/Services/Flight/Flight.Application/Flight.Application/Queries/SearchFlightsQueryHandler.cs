using Flight.Core.Repositories;
using MediatR;

namespace Flight.Application.Queries;

public class SearchFlightsQueryHandler : IRequestHandler<SearchFlightsQuery, IEnumerable<SearchFlightsQueryResponse>>
{
    private readonly IFlightRepository _repository;

    public SearchFlightsQueryHandler(IFlightRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SearchFlightsQueryResponse>> Handle(SearchFlightsQuery request, CancellationToken cancellationToken)
    {
        var flights = await _repository.SearchFlightsAsync(
            request.DepartureAirport,
            request.ArrivalAirport,
            request.DepartureDate,
            cancellationToken);

        return flights.Select(f => new SearchFlightsQueryResponse
        {
            Id = f.Id,
            FlightNumber = f.FlightNumber,
            Airline = f.Airline,
            DepartureAirport = f.DepartureAirport,
            ArrivalAirport = f.ArrivalAirport,
            DepartureTime = f.DepartureTime,
            ArrivalTime = f.ArrivalTime,
            AvailableSeats = f.AvailableSeats,
            PricePerSeat = f.PricePerSeat,
            Status = f.Status.ToString()
        });
    }
}
