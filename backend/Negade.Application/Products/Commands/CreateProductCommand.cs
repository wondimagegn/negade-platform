using MapsterMapper;
using MediatR;
using Negade.Application.Common.Interfaces;
using Negade.Application.Products.Common;
using Negade.Domain.Entities;

namespace Negade.Application.Products.Commands;

public record CreateProductCommand(CreateProductDto Product) : IRequest<ProductDto>;

public class CreateProductCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = mapper.Map<Product>(request.Product);
        product.Id = Guid.NewGuid();
        product.CreatedAt = DateTime.UtcNow;
        product.AvailableQuantity = product.AvailableQuantity == 0 ? product.StockQuantity : product.AvailableQuantity;

        await dbContext.Products.AddAsync(product, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return mapper.Map<ProductDto>(product);
    }
}
