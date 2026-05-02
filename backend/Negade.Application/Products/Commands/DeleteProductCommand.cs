using MediatR;
using Microsoft.EntityFrameworkCore;
using Negade.Application.Common.Interfaces;

namespace Negade.Application.Products.Commands;

public record DeleteProductCommand(Guid Id) : IRequest<bool>;

public class DeleteProductCommandHandler(IApplicationDbContext dbContext) : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var existing = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        dbContext.Products.Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
