using MediatR;
using Negade.Application.Auth.Common;
using Negade.Application.Common.Interfaces;

namespace Negade.Application.Auth.Commands;

public record LoginCommand(LoginDto Login) : IRequest<AuthResult>;

public class LoginCommandHandler(IIdentityAuthService identityAuthService)
    : IRequestHandler<LoginCommand, AuthResult>
{
    public Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken) =>
        identityAuthService.LoginAsync(request.Login, cancellationToken);
}
