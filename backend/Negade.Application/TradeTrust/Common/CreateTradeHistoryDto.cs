using System.ComponentModel.DataAnnotations;

namespace Negade.Application.TradeTrust.Common;

public class CreateTradeHistoryDto
{
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? CounterpartyName { get; set; }

    [Range(0, 9999999999.99)]
    public decimal? Amount { get; set; }

    public DateTime TradeDate { get; set; } = DateTime.UtcNow;
}
