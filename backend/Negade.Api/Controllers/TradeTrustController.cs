using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negade.Application.TradeTrust.Commands;
using Negade.Application.TradeTrust.Common;
using Negade.Application.TradeTrust.Queries;

namespace Negade.Api.Controllers;

[ApiController]
[Route("api/business-profiles/{businessProfileId:guid}")]
public class TradeTrustController(IMediator mediator) : ControllerBase
{
    [HttpGet("ratings")]
    public async Task<ActionResult<IEnumerable<TradeRatingDto>>> GetRatings(
        Guid businessProfileId,
        CancellationToken cancellationToken)
    {
        var ratings = await mediator.Send(new GetTradeRatingsBySupplierQuery(businessProfileId), cancellationToken);
        return Ok(ratings);
    }

    [Authorize]
    [HttpPost("ratings")]
    public async Task<ActionResult<TradeRatingDto>> CreateRating(
        Guid businessProfileId,
        [FromBody] CreateTradeRatingDto request,
        CancellationToken cancellationToken)
    {
        var created = await mediator.Send(
            new CreateTradeRatingCommand(businessProfileId, request, GetUserId()),
            cancellationToken);

        return created is null ? NotFound() : CreatedAtAction(nameof(GetRatings), new { businessProfileId }, created);
    }

    [HttpGet("trade-history")]
    public async Task<ActionResult<IEnumerable<TradeHistoryDto>>> GetTradeHistory(
        Guid businessProfileId,
        CancellationToken cancellationToken)
    {
        var history = await mediator.Send(
            new GetTradeHistoryByBusinessProfileQuery(businessProfileId),
            cancellationToken);

        return Ok(history);
    }

    [Authorize]
    [HttpPost("trade-history")]
    public async Task<ActionResult<TradeHistoryDto>> CreateTradeHistory(
        Guid businessProfileId,
        [FromBody] CreateTradeHistoryDto request,
        CancellationToken cancellationToken)
    {
        var created = await mediator.Send(
            new CreateTradeHistoryCommand(businessProfileId, request, GetUserId()),
            cancellationToken);

        return created is null ? NotFound() : CreatedAtAction(nameof(GetTradeHistory), new { businessProfileId }, created);
    }

    private Guid? GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out var userId) ? userId : null;
    }
}
