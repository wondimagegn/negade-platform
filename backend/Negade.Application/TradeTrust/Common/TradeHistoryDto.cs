namespace Negade.Application.TradeTrust.Common;

public record TradeHistoryDto(
    Guid Id,
    Guid BusinessProfileId,
    string Description,
    string? CounterpartyName,
    decimal? Amount,
    DateTime TradeDate,
    DateTime CreatedAt
);
