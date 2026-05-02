using System.ComponentModel.DataAnnotations;

namespace Negade.Application.Rfqs.Common;

public class CreateRfqDto
{
    [Required]
    [MaxLength(200)]
    public string BuyerName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string BuyerPhoneNumber { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? BuyerBusinessName { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Range(0.01, 9999999999.99)]
    public decimal Quantity { get; set; }

    [Required]
    [MaxLength(50)]
    public string Unit { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string DeliveryRegion { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string DeliveryCity { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Notes { get; set; }
}
