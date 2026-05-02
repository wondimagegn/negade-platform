namespace Negade.Domain.Entities;

public class Quote
{
    public Guid Id { get; set; }
    public Guid RfqId { get; set; }
    public Guid SupplierId { get; set; }
    public Guid? SupplierUserId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal QuantityAvailable { get; set; }
    public int DeliveryTimeInDays { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Rfq Rfq { get; set; } = null!;
    public BusinessProfile Supplier { get; set; } = null!;
    public AppUser? SupplierUser { get; set; }
}
