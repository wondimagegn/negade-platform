using System.ComponentModel.DataAnnotations;

namespace Negade.Application.Auth.Common;

public class RegisterDto
{
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
