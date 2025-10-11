using Flight.Application.Commands;
using Flight.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flight.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FlightsController> _logger;

    public FlightsController(IMediator mediator, ILogger<FlightsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<CreateFlightCommandResponse>> CreateFlight(
        [FromBody] CreateFlightCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(SearchFlights), new { id = result.FlightId }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating flight");
            return StatusCode(500, "An error occurred while creating the flight");
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<SearchFlightsQueryResponse>>> SearchFlights(
        [FromQuery] string departureAirport,
        [FromQuery] string arrivalAirport,
        [FromQuery] DateTime departureDate,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new SearchFlightsQuery(departureAirport, arrivalAirport, departureDate);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching flights");
            return StatusCode(500, "An error occurred while searching flights");
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "Flight API", timestamp = DateTime.UtcNow });
    }
}
