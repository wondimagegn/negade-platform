using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.Common.Interfaces;
using Negade.Application.Products.Common;

namespace Negade.Application.Products.Queries;

public record GetProductsQuery : IRequest<IEnumerable<ProductDto>>;

public class GetProductsQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
{
    public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await dbContext.Products
            .AsNoTracking()
            .Include(p => p.Supplier)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

        return mapper.Map<IEnumerable<ProductDto>>(products);
    }
}
