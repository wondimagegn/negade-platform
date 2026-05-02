using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.Auth.Common;
using Negade.Application.Common.Interfaces;
using Negade.Domain.Entities;

namespace Negade.Application.Auth.Commands;

public record LoginCommand(LoginDto Login) : IRequest<AuthResponseDto?>;

public class LoginCommandHandler(IApplicationDbContext dbContext, IJwtTokenService jwtTokenService)
    : IRequestHandler<LoginCommand, AuthResponseDto?>
{
    public async Task<AuthResponseDto?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var phoneNumber = request.Login.PhoneNumber.Trim();
        var user = await dbContext.AppUsers.FirstOrDefaultAsync(
            candidate => candidate.PhoneNumber == phoneNumber,
            cancellationToken);

        if (user is null || !PasswordHasher.Verify(request.Login.Password, user.PasswordHash))
        {
            return null;
        }

        return ToAuthResponse(user, jwtTokenService.CreateToken(user));
    }

    private static AuthResponseDto ToAuthResponse(AppUser user, string token) =>
        new(token, user.Id, user.FullName, user.PhoneNumber, user.Role);
}
