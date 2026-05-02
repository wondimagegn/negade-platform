namespace Negade.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public Guid? SupplierId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Unit { get; set; } = "pcs";
    public int StockQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public BusinessProfile? Supplier { get; set; }
}
