using Negade.Domain.Entities;

namespace Negade.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string CreateToken(AppUser user);
}
