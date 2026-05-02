namespace Negade.Application.TradeTrust.Common;

public record TradeRatingDto(
    Guid Id,
    Guid SupplierId,
    Guid? RaterUserId,
    int Score,
    string? Comment,
    DateTime CreatedAt
);
