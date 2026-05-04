using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Negade.Application.Auth.Common;
using Negade.Application.Common.Interfaces;
using Negade.Domain.Entities;
using Negade.Domain.Security;

namespace Negade.Infrastructure.Auth;

public class IdentityAuthService(
    UserManager<AppUser> userManager,
    IJwtTokenService jwtTokenService) : IIdentityAuthService
{
    public async Task<AuthResult> RegisterTraderAsync(RegisterDto request, CancellationToken cancellationToken)
    {
        var phoneNumber = request.PhoneNumber.Trim();
        var userName = string.IsNullOrWhiteSpace(request.UserName) ? phoneNumber : request.UserName.Trim();
        var email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        var normalizedEmail = email?.ToUpperInvariant();
        var existingUser = await userManager.Users.AnyAsync(
            user =>
                user.PhoneNumber == phoneNumber ||
                user.UserName == userName ||
                (normalizedEmail != null && user.NormalizedEmail == normalizedEmail),
            cancellationToken);

        if (existingUser)
        {
            return AuthResult.Failure("Phone, username, or email is already registered.");
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            UserName = userName,
            PhoneNumber = phoneNumber,
            Email = email,
            PhoneNumberConfirmed = true,
            LockoutEnabled = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return AuthResult.Failure(createResult.Errors.Select(error => error.Description).ToArray());
        }

        var roleResult = await userManager.AddToRoleAsync(user, AppRoles.Trader);
        if (!roleResult.Succeeded)
        {
            return AuthResult.Failure(roleResult.Errors.Select(error => error.Description).ToArray());
        }

        return await ToAuthResultAsync(user);
    }

    public async Task<AuthResult> LoginAsync(LoginDto request, CancellationToken cancellationToken)
    {
        var identifier = request.LoginIdentifier;
        var user = await FindByIdentifierAsync(identifier, cancellationToken);

        if (user is not null && userManager.SupportsUserLockout && await userManager.IsLockedOutAsync(user))
        {
            return AuthResult.Failure("This account is temporarily locked. Please try again later.");
        }

        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            if (user is not null && userManager.SupportsUserLockout)
            {
                await userManager.AccessFailedAsync(user);
            }

            return AuthResult.Failure("Invalid email, username, phone, or password.");
        }

        if (userManager.SupportsUserLockout)
        {
            await userManager.ResetAccessFailedCountAsync(user);
        }

        return await ToAuthResultAsync(user);
    }

    private async Task<AppUser?> FindByIdentifierAsync(string identifier, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(identifier);
        if (user is not null)
        {
            return user;
        }

        user = await userManager.FindByEmailAsync(identifier);
        if (user is not null)
        {
            return user;
        }

        return await userManager.Users.FirstOrDefaultAsync(
            candidate => candidate.PhoneNumber == identifier,
            cancellationToken);
    }

    private async Task<AuthResult> ToAuthResultAsync(AppUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var primaryRole = roles.Contains(AppRoles.Admin) ? AppRoles.Admin : roles.FirstOrDefault() ?? AppRoles.Trader;
        var token = jwtTokenService.CreateToken(user, roles);

        return AuthResult.Success(new AuthResponseDto(
            token,
            user.Id,
            user.FullName,
            user.PhoneNumber ?? string.Empty,
            primaryRole));
    }
}
