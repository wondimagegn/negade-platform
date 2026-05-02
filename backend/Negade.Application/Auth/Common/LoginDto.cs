using System.ComponentModel.DataAnnotations;

namespace Negade.Application.Auth.Common;

public class LoginDto
{
    [Required]
    [MaxLength(100)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
