namespace Negade.Application.Rfqs.Common;

public record SupplierQuoteDto(
    Guid Id,
    Guid RfqId,
    Guid SupplierId,
    Guid? SupplierUserId,
    string SupplierName,
    decimal UnitPrice,
    decimal QuantityAvailable,
    int DeliveryTimeInDays,
    string? Notes,
    DateTime CreatedAt,
    string RfqProductName,
    string RfqCategory,
    decimal RfqQuantity,
    string RfqUnit,
    string RfqDeliveryRegion,
    string RfqDeliveryCity,
    string RfqStatus,
    string BuyerName,
    string? BuyerBusinessName
);
