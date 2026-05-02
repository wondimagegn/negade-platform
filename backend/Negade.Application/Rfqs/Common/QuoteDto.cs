namespace Negade.Application.Rfqs.Common;

public record QuoteDto(
    Guid Id,
    Guid RfqId,
    Guid SupplierId,
    string SupplierName,
    decimal UnitPrice,
    decimal QuantityAvailable,
    int DeliveryTimeInDays,
    string? Notes,
    DateTime CreatedAt
);
