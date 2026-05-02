using MediatR;
using Microsoft.AspNetCore.Mvc;
using Negade.Application.BusinessProfiles.Commands;
using Negade.Application.BusinessProfiles.Common;
using Negade.Application.BusinessProfiles.Queries;

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

    [HttpPost]
    public async Task<ActionResult<BusinessProfileDto>> Create(
        [FromBody] CreateBusinessProfileDto request,
        CancellationToken cancellationToken)
    {
        var created = await mediator.Send(new CreateBusinessProfileCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }
}
