namespace Negade.Application.BusinessProfiles.Common;

public record BusinessProfileDto(
    Guid Id,
    string BusinessName,
    string OwnerName,
    string TinNumber,
    string PhoneNumber,
    string Region,
    string City,
    string? Address,
    string BusinessType,
    string VerificationStatus,
    decimal RatingAverage,
    int RatingCount,
    int TradeCount,
    DateTime CreatedAt
);
