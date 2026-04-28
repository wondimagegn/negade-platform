using Microsoft.EntityFrameworkCore;
using Negade.Domain.Entities;

namespace Negade.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
