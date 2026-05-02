using System.ComponentModel.DataAnnotations;

namespace Negade.Application.Products.Common;

public class UpdateProductDto
{
    public Guid? SupplierId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Range(0, 9999999999.99)]
    public decimal Price { get; set; }

    [Required]
    [MaxLength(50)]
    public string Unit { get; set; } = "pcs";

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [Range(0, 9999999999.99)]
    public decimal AvailableQuantity { get; set; }

    [MaxLength(100)]
    public string Region { get; set; } = string.Empty;

    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;
}
