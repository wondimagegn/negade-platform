namespace Negade.Domain.Entities;

public class TradeHistory
{
    public Guid Id { get; set; }
    public Guid BusinessProfileId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? CounterpartyName { get; set; }
    public decimal? Amount { get; set; }
    public DateTime TradeDate { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public BusinessProfile BusinessProfile { get; set; } = null!;
}
