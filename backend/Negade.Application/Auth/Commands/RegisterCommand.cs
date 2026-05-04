using MediatR;
using Negade.Application.Auth.Common;
using Negade.Application.Common.Interfaces;

namespace Negade.Application.Auth.Commands;

public record RegisterCommand(RegisterDto User) : IRequest<AuthResult>;

public class RegisterCommandHandler(IIdentityAuthService identityAuthService)
    : IRequestHandler<RegisterCommand, AuthResult>
{
    public Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken) =>
        identityAuthService.RegisterTraderAsync(request.User, cancellationToken);
}
