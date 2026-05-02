using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.Common.Interfaces;
using Negade.Application.Products.Common;

namespace Negade.Application.Marketplace.Queries;

public record SearchMarketplaceProductsQuery(string? Search, string? Category, string? Region)
    : IRequest<IEnumerable<ProductDto>>;

public class SearchMarketplaceProductsQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<SearchMarketplaceProductsQuery, IEnumerable<ProductDto>>
{
    public async Task<IEnumerable<ProductDto>> Handle(
        SearchMarketplaceProductsQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Products
            .AsNoTracking()
            .Include(product => product.Supplier)
            .Where(product => product.IsAvailable);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(product =>
                product.Name.ToLower().Contains(search) ||
                (product.Description != null && product.Description.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            query = query.Where(product => product.Category.ToLower().Contains(request.Category.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(request.Region))
        {
            query = query.Where(product => product.Region.ToLower().Contains(request.Region.ToLower()));
        }

        var products = await query
            .OrderBy(product => product.Name)
            .ThenBy(product => product.Price)
            .ToListAsync(cancellationToken);

        return mapper.Map<IEnumerable<ProductDto>>(products);
    }
}
