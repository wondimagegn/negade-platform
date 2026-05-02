using System.ComponentModel.DataAnnotations;

namespace Negade.Application.TradeTrust.Common;

public class CreateTradeRatingDto
{
    [Range(1, 5)]
    public int Score { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }
}
