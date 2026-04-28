using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Negade.Application.DTOs;
using Negade.Domain.Entities;
using Negade.Infrastructure.Data;

namespace Negade.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await dbContext.Products
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.CreatedAt))
            .ToListAsync(cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            CreatedAt = DateTime.UtcNow
        };

        await dbContext.Products.AddAsync(product, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var dto = new ProductDto(product.Id, product.Name, product.Description, product.Price, product.StockQuantity, product.CreatedAt);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductDto request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product is null)
        {
            return NotFound();
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new ProductDto(product.Id, product.Name, product.Description, product.Price, product.StockQuantity, product.CreatedAt));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product is null)
        {
            return NotFound();
        }

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
