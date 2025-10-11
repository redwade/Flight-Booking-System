using Flight.Core.Entities;
using Flight.Core.Repositories;
using MediatR;

namespace Flight.Application.Commands;

public class CreateFlightCommandHandler : IRequestHandler<CreateFlightCommand, CreateFlightCommandResponse>
{
    private readonly IFlightRepository _repository;

    public CreateFlightCommandHandler(IFlightRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateFlightCommandResponse> Handle(CreateFlightCommand request, CancellationToken cancellationToken)
    {
        var flight = new FlightEntity
        {
            FlightNumber = request.FlightNumber,
            Airline = request.Airline,
            DepartureAirport = request.DepartureAirport,
            ArrivalAirport = request.ArrivalAirport,
            DepartureTime = request.DepartureTime,
            ArrivalTime = request.ArrivalTime,
            TotalSeats = request.TotalSeats,
            AvailableSeats = request.TotalSeats,
            PricePerSeat = request.PricePerSeat,
            AircraftType = request.AircraftType,
            Status = FlightStatus.Scheduled
        };

        var createdFlight = await _repository.CreateAsync(flight, cancellationToken);

        return new CreateFlightCommandResponse
        {
            FlightId = createdFlight.Id,
            FlightNumber = createdFlight.FlightNumber,
            Status = createdFlight.Status.ToString()
        };
    }
}
