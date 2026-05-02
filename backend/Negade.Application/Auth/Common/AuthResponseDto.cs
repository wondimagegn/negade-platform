namespace Negade.Application.Auth.Common;

public record AuthResponseDto(
    string Token,
    Guid UserId,
    string FullName,
    string PhoneNumber,
    string Role
);
