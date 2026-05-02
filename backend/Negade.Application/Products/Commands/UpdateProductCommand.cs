using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.Common.Interfaces;
using Negade.Application.Products.Common;

namespace Negade.Application.Products.Commands;

public record UpdateProductCommand(Guid Id, UpdateProductDto Product) : IRequest<ProductDto?>;

public class UpdateProductCommandHandler(IApplicationDbContext dbContext, IMapper mapper)
    : IRequestHandler<UpdateProductCommand, ProductDto?>
{
    public async Task<ProductDto?> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var existing = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        mapper.Map(request.Product, existing);

        await dbContext.SaveChangesAsync(cancellationToken);

        return mapper.Map<ProductDto>(existing);
    }
}
