namespace Flight.Core.Entities;

public class FlightEntity
{
    public Guid Id { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public string Airline { get; set; } = string.Empty;
    public string DepartureAirport { get; set; } = string.Empty;
    public string ArrivalAirport { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public decimal PricePerSeat { get; set; }
    public FlightStatus Status { get; set; }
    public string? AircraftType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum FlightStatus
{
    Scheduled = 0,
    Boarding = 1,
    Departed = 2,
    InFlight = 3,
    Landed = 4,
    Cancelled = 5,
    Delayed = 6
}
