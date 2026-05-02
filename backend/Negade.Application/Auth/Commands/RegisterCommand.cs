using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.Auth.Common;
using Negade.Application.Common.Interfaces;
using Negade.Domain.Entities;

namespace Negade.Application.Auth.Commands;

public record RegisterCommand(RegisterDto User) : IRequest<AuthResponseDto?>;

public class RegisterCommandHandler(IApplicationDbContext dbContext, IJwtTokenService jwtTokenService)
    : IRequestHandler<RegisterCommand, AuthResponseDto?>
{
    public async Task<AuthResponseDto?> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var phoneNumber = request.User.PhoneNumber.Trim();
        var userExists = await dbContext.AppUsers.AnyAsync(
            user => user.PhoneNumber == phoneNumber,
            cancellationToken);

        if (userExists)
        {
            return null;
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            FullName = request.User.FullName.Trim(),
            PhoneNumber = phoneNumber,
            Email = string.IsNullOrWhiteSpace(request.User.Email) ? null : request.User.Email.Trim(),
            PasswordHash = PasswordHasher.Hash(request.User.Password),
            Role = "Trader",
            CreatedAt = DateTime.UtcNow
        };

        await dbContext.AppUsers.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToAuthResponse(user, jwtTokenService.CreateToken(user));
    }

    private static AuthResponseDto ToAuthResponse(AppUser user, string token) =>
        new(token, user.Id, user.FullName, user.PhoneNumber, user.Role);
}
