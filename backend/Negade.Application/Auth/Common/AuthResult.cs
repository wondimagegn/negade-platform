namespace Negade.Application.Auth.Common;

public record AuthResult(
    AuthResponseDto? Response,
    IReadOnlyCollection<string> Errors
)
{
    public static AuthResult Success(AuthResponseDto response) => new(response, []);

    public static AuthResult Failure(params string[] errors) => new(null, errors);
}
