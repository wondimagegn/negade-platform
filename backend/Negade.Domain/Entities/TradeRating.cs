namespace Negade.Domain.Entities;

public class TradeRating
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public Guid? RaterUserId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public BusinessProfile Supplier { get; set; } = null!;
    public AppUser? RaterUser { get; set; }
}
