namespace Negade.Application.Rfqs.Common;

public record RfqDto(
    Guid Id,
    Guid? BuyerUserId,
    string BuyerName,
    string BuyerPhoneNumber,
    string? BuyerBusinessName,
    string ProductName,
    string Category,
    decimal Quantity,
    string Unit,
    string DeliveryRegion,
    string DeliveryCity,
    string? Notes,
    string Status,
    int QuoteCount,
    DateTime CreatedAt
);
