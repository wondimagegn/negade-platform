namespace Negade.Application.Admin.Common;

public record CategoryDto(
    Guid Id,
    Guid? ParentCategoryId,
    string Name,
    string? Description,
    int SortOrder,
    bool IsActive,
    DateTime CreatedAt);

public record UpsertCategoryDto(
    Guid? ParentCategoryId,
    string Name,
    string? Description,
    int SortOrder,
    bool IsActive);
