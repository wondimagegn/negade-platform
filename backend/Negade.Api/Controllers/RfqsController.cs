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
        var created = await mediator.Send(new CreateQuoteCommand(rfqId, request, GetUserId()), cancellationToken);
        return created is null ? NotFound() : CreatedAtAction(nameof(GetQuotes), new { rfqId }, created);
    }

    private Guid? GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out var userId) ? userId : null;
    }
}
