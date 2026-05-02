using System.ComponentModel.DataAnnotations;

namespace Negade.Application.BusinessProfiles.Common;

public class CreateBusinessProfileDto
{
    [Required]
    [MaxLength(200)]
    public string BusinessName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string OwnerName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string TinNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Region { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    [Required]
    [MaxLength(100)]
    public string BusinessType { get; set; } = string.Empty;
}
