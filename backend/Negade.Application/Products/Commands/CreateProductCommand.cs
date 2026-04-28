using MapsterMapper;
using MediatR;
using Negade.Application.Abstractions;
using Negade.Application.DTOs;
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

        await dbContext.Products.AddAsync(product, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return mapper.Map<ProductDto>(product);
    }
}
