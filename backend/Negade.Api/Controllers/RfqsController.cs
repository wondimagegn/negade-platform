using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negade.Application.Rfqs.Commands;
using Negade.Application.Rfqs.Common;
using Negade.Application.Rfqs.Queries;

namespace Negade.Api.Controllers;

[ApiController]
[Route("api/rfqs")]
public class RfqsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RfqDto>>> GetAll(
        [FromQuery] string? category,
        [FromQuery] string? region,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var rfqs = await mediator.Send(new GetRfqsQuery(category, region, status), cancellationToken);
        return Ok(rfqs);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RfqDto>> Create(
        [FromBody] CreateRfqDto request,
        CancellationToken cancellationToken)
    {
        var created = await mediator.Send(new CreateRfqCommand(request, GetUserId()), cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [Authorize]
    [HttpPatch("{rfqId:guid}/status")]
    public async Task<ActionResult<RfqDto>> UpdateStatus(
        Guid rfqId,
        [FromBody] UpdateRfqStatusDto request,
        CancellationToken cancellationToken)
    {
        var updated = await mediator.Send(new UpdateRfqStatusCommand(rfqId, request), cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [Authorize]
    [HttpGet("my-quotes")]
    public async Task<ActionResult<IEnumerable<SupplierQuoteDto>>> GetMyQuotes(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var quotes = await mediator.Send(new GetMyQuotesQuery(userId.Value), cancellationToken);
        return Ok(quotes);
    }

    [HttpGet("{rfqId:guid}/quotes")]
    public async Task<ActionResult<IEnumerable<QuoteDto>>> GetQuotes(
        Guid rfqId,
        CancellationToken cancellationToken)
    {
        var quotes = await mediator.Send(new GetQuotesByRfqQuery(rfqId), cancellationToken);
        return Ok(quotes);
    }

    [Authorize]
    [HttpPost("{rfqId:guid}/quotes")]
    public async Task<ActionResult<QuoteDto>> CreateQuote(
        Guid rfqId,
        [FromBody] CreateQuoteDto request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateQuoteCommand(rfqId, request, GetUserId()), cancellationToken);
        return result.FailureReason switch
        {
            CreateQuoteFailureReason.None => CreatedAtAction(nameof(GetQuotes), new { rfqId }, result.Quote),
            CreateQuoteFailureReason.SupplierNotOwned => Forbid(),
            CreateQuoteFailureReason.SupplierNotFound => BadRequest("Supplier profile was not found."),
            _ => NotFound()
        };
    }

    private Guid? GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out var userId) ? userId : null;
    }
}
