namespace Negade.Domain.Entities;

public class BusinessProfile
{
    public Guid Id { get; set; }
    public Guid? OwnerUserId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string TinNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string BusinessType { get; set; } = string.Empty;
    public string VerificationStatus { get; set; } = "Pending";
    public decimal RatingAverage { get; set; }
    public int RatingCount { get; set; }
    public int TradeCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public AppUser? OwnerUser { get; set; }
    public List<Product> Products { get; set; } = [];
    public List<Quote> Quotes { get; set; } = [];
    public List<TradeRating> Ratings { get; set; } = [];
    public List<TradeHistory> TradeHistory { get; set; } = [];
}
