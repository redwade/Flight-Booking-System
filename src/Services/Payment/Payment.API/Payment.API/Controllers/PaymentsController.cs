using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Commands;
using Payment.Application.Queries;

namespace Payment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IMediator mediator, ILogger<PaymentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ProcessPaymentCommandResponse>> ProcessPayment(
        [FromBody] ProcessPaymentCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetPaymentsByBookingId), new { bookingId = result.PaymentId }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment");
            return StatusCode(500, "An error occurred while processing the payment");
        }
    }

    [HttpGet("booking/{bookingId}")]
    public async Task<ActionResult<IEnumerable<GetPaymentByBookingIdQueryResponse>>> GetPaymentsByBookingId(
        Guid bookingId,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new GetPaymentByBookingIdQuery(bookingId), cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments for booking {BookingId}", bookingId);
            return StatusCode(500, "An error occurred while retrieving payments");
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "Payment API", timestamp = DateTime.UtcNow });
    }
}
