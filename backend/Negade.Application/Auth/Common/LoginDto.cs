using System.ComponentModel.DataAnnotations;

namespace Negade.Application.Auth.Common;

public class LoginDto : IValidatableObject
{
    [MaxLength(256)]
    public string? Identifier { get; set; }

    [MaxLength(100)]
    public string? PhoneNumber { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public string LoginIdentifier => (Identifier ?? PhoneNumber ?? string.Empty).Trim();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(LoginIdentifier))
        {
            yield return new ValidationResult(
                "Email, username, or phone is required.",
                [nameof(Identifier)]);
        }
    }
}
