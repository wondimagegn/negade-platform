namespace Negade.Domain.Entities;

public class Rfq
{
    public Guid Id { get; set; }
    public Guid? BuyerUserId { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public string BuyerPhoneNumber { get; set; } = string.Empty;
    public string? BuyerBusinessName { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string DeliveryRegion { get; set; } = string.Empty;
    public string DeliveryCity { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string Status { get; set; } = "Open";
    public int QuoteCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public AppUser? BuyerUser { get; set; }
    public List<Quote> Quotes { get; set; } = [];
}
