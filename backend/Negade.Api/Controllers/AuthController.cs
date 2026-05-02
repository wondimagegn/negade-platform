using MediatR;
using Microsoft.AspNetCore.Mvc;
using Negade.Application.Auth.Commands;
using Negade.Application.Auth.Common;

namespace Negade.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(
        [FromBody] RegisterDto request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new RegisterCommand(request), cancellationToken);
        return response is null ? Conflict("Phone number is already registered.") : Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginDto request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new LoginCommand(request), cancellationToken);
        return response is null ? Unauthorized() : Ok(response);
    }
}
