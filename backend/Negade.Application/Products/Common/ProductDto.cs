namespace Negade.Application.Products.Common;

public record ProductDto(
    Guid Id,
    Guid? SupplierId,
    string? SupplierName,
    string Name,
    string? Description,
    string Category,
    decimal Price,
    string Unit,
    int StockQuantity,
    decimal AvailableQuantity,
    string Region,
    string City,
    bool IsAvailable,
    DateTime CreatedAt
);
