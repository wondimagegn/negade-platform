using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negade.Application.BusinessProfiles.Commands;
using Negade.Application.BusinessProfiles.Common;
using Negade.Application.BusinessProfiles.Queries;
using Negade.Domain.Security;

namespace Negade.Api.Controllers;

[ApiController]
[Route("api/business-profiles")]
public class BusinessProfilesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BusinessProfileDto>>> GetAll(
        [FromQuery] string? region,
        [FromQuery] string? businessType,
        [FromQuery] bool verifiedOnly,
        CancellationToken cancellationToken)
    {
        var profiles = await mediator.Send(
            new GetBusinessProfilesQuery(region, businessType, verifiedOnly),
            cancellationToken);

        return Ok(profiles);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<IEnumerable<BusinessProfileDto>>> GetMine(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var profiles = await mediator.Send(new GetMyBusinessProfilesQuery(userId.Value), cancellationToken);
        return Ok(profiles);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BusinessProfileDto>> Create(
        [FromBody] CreateBusinessProfileDto request,
        CancellationToken cancellationToken)
    {
        var created = await mediator.Send(new CreateBusinessProfileCommand(request, GetUserId()), cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPatch("{businessProfileId:guid}/verification")]
    public async Task<ActionResult<BusinessProfileDto>> Verify(
        Guid businessProfileId,
        [FromBody] VerifyBusinessProfileDto request,
        CancellationToken cancellationToken)
    {
        var updated = await mediator.Send(
            new VerifyBusinessProfileCommand(businessProfileId, request),
            cancellationToken);

        return updated is null ? NotFound() : Ok(updated);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{businessProfileId:guid}")]
    public async Task<ActionResult<BusinessProfileDto>> Update(
        Guid businessProfileId,
        [FromBody] UpdateBusinessProfileDto request,
        CancellationToken cancellationToken)
    {
        var updated = await mediator.Send(
            new UpdateBusinessProfileCommand(businessProfileId, request),
            cancellationToken);

        return updated is null ? NotFound() : Ok(updated);
    }

    private Guid? GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out var userId) ? userId : null;
    }
}
