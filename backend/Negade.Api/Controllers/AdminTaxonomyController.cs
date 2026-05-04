using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Negade.Application.Admin.Common;
using Negade.Application.Common.Interfaces;
using Negade.Domain.Entities;
using Negade.Domain.Security;

namespace Negade.Api.Controllers;

[ApiController]
[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin")]
public class AdminTaxonomyController(IApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await dbContext.Categories
            .AsNoTracking()
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Name)
            .Select(category => new CategoryDto(
                category.Id,
                category.ParentCategoryId,
                category.Name,
                category.Description,
                category.SortOrder,
                category.IsActive,
                category.CreatedAt))
            .ToListAsync(cancellationToken);

        return Ok(categories);
    }

    [HttpPost("categories")]
    public async Task<ActionResult<CategoryDto>> CreateCategory(
        [FromBody] UpsertCategoryDto request,
        CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            ParentCategoryId = request.ParentCategoryId,
            Name = request.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            SortOrder = request.SortOrder,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await dbContext.Categories.AddAsync(category, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, ToDto(category));
    }

    [HttpPut("categories/{categoryId:guid}")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(
        Guid categoryId,
        [FromBody] UpsertCategoryDto request,
        CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories.FirstOrDefaultAsync(item => item.Id == categoryId, cancellationToken);
        if (category is null)
        {
            return NotFound();
        }

        category.ParentCategoryId = request.ParentCategoryId == category.Id ? null : request.ParentCategoryId;
        category.Name = request.Name.Trim();
        category.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        category.SortOrder = request.SortOrder;
        category.IsActive = request.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ToDto(category));
    }

    [HttpDelete("categories/{categoryId:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories.FirstOrDefaultAsync(item => item.Id == categoryId, cancellationToken);
        if (category is null)
        {
            return NotFound();
        }

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("regions")]
    public async Task<ActionResult<IEnumerable<RegionDto>>> GetRegions(CancellationToken cancellationToken)
    {
        var regions = await dbContext.Regions
            .AsNoTracking()
            .Include(region => region.Cities)
            .OrderBy(region => region.Name)
            .Select(region => new RegionDto(
                region.Id,
                region.Name,
                region.Code,
                region.IsActive,
                region.CreatedAt,
                region.Cities
                    .OrderBy(city => city.Name)
                    .Select(city => new CityDto(city.Id, city.RegionId, city.Name, city.IsActive, city.CreatedAt))))
            .ToListAsync(cancellationToken);

        return Ok(regions);
    }

    [HttpPost("regions")]
    public async Task<ActionResult<RegionDto>> CreateRegion(
        [FromBody] UpsertRegionDto request,
        CancellationToken cancellationToken)
    {
        var region = new Region
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Code = string.IsNullOrWhiteSpace(request.Code) ? null : request.Code.Trim(),
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await dbContext.Regions.AddAsync(region, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetRegions), new { id = region.Id }, ToDto(region));
    }

    [HttpPut("regions/{regionId:guid}")]
    public async Task<ActionResult<RegionDto>> UpdateRegion(
        Guid regionId,
        [FromBody] UpsertRegionDto request,
        CancellationToken cancellationToken)
    {
        var region = await dbContext.Regions
            .Include(item => item.Cities)
            .FirstOrDefaultAsync(item => item.Id == regionId, cancellationToken);
        if (region is null)
        {
            return NotFound();
        }

        region.Name = request.Name.Trim();
        region.Code = string.IsNullOrWhiteSpace(request.Code) ? null : request.Code.Trim();
        region.IsActive = request.IsActive;
        region.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ToDto(region));
    }

    [HttpDelete("regions/{regionId:guid}")]
    public async Task<IActionResult> DeleteRegion(Guid regionId, CancellationToken cancellationToken)
    {
        var region = await dbContext.Regions.FirstOrDefaultAsync(item => item.Id == regionId, cancellationToken);
        if (region is null)
        {
            return NotFound();
        }

        dbContext.Regions.Remove(region);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("cities")]
    public async Task<ActionResult<CityDto>> CreateCity(
        [FromBody] UpsertCityDto request,
        CancellationToken cancellationToken)
    {
        var city = new City
        {
            Id = Guid.NewGuid(),
            RegionId = request.RegionId,
            Name = request.Name.Trim(),
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await dbContext.Cities.AddAsync(city, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetRegions), new { id = city.Id }, ToDto(city));
    }

    [HttpPut("cities/{cityId:guid}")]
    public async Task<ActionResult<CityDto>> UpdateCity(
        Guid cityId,
        [FromBody] UpsertCityDto request,
        CancellationToken cancellationToken)
    {
        var city = await dbContext.Cities.FirstOrDefaultAsync(item => item.Id == cityId, cancellationToken);
        if (city is null)
        {
            return NotFound();
        }

        city.RegionId = request.RegionId;
        city.Name = request.Name.Trim();
        city.IsActive = request.IsActive;
        city.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ToDto(city));
    }

    [HttpDelete("cities/{cityId:guid}")]
    public async Task<IActionResult> DeleteCity(Guid cityId, CancellationToken cancellationToken)
    {
        var city = await dbContext.Cities.FirstOrDefaultAsync(item => item.Id == cityId, cancellationToken);
        if (city is null)
        {
            return NotFound();
        }

        dbContext.Cities.Remove(city);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static CategoryDto ToDto(Category category) =>
        new(
            category.Id,
            category.ParentCategoryId,
            category.Name,
            category.Description,
            category.SortOrder,
            category.IsActive,
            category.CreatedAt);

    private static RegionDto ToDto(Region region) =>
        new(
            region.Id,
            region.Name,
            region.Code,
            region.IsActive,
            region.CreatedAt,
            region.Cities
                .OrderBy(city => city.Name)
                .Select(ToDto));

    private static CityDto ToDto(City city) =>
        new(city.Id, city.RegionId, city.Name, city.IsActive, city.CreatedAt);
}
