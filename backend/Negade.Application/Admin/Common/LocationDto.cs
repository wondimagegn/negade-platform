namespace Negade.Application.Admin.Common;

public record RegionDto(
    Guid Id,
    string Name,
    string? Code,
    bool IsActive,
    DateTime CreatedAt,
    IEnumerable<CityDto> Cities);

public record CityDto(
    Guid Id,
    Guid RegionId,
    string Name,
    bool IsActive,
    DateTime CreatedAt);

public record UpsertRegionDto(string Name, string? Code, bool IsActive);

public record UpsertCityDto(Guid RegionId, string Name, bool IsActive);
