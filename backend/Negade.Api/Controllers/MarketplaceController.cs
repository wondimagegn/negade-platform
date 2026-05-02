using MediatR;
using Microsoft.AspNetCore.Mvc;
using Negade.Application.Marketplace.Queries;
using Negade.Application.Products.Common;

namespace Negade.Api.Controllers;

[ApiController]
[Route("api/marketplace")]
public class MarketplaceController(IMediator mediator) : ControllerBase
{
    [HttpGet("products")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] string? region,
        CancellationToken cancellationToken)
    {
        var products = await mediator.Send(
            new SearchMarketplaceProductsQuery(search, category, region),
            cancellationToken);

        return Ok(products);
    }
}
