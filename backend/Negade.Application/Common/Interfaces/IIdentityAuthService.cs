using Negade.Application.Auth.Common;

namespace Negade.Application.Common.Interfaces;

public interface IIdentityAuthService
{
    Task<AuthResult> RegisterTraderAsync(RegisterDto request, CancellationToken cancellationToken);
    Task<AuthResult> LoginAsync(LoginDto request, CancellationToken cancellationToken);
}
