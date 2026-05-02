using Microsoft.EntityFrameworkCore;
using Negade.Domain.Entities;

namespace Negade.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<BusinessProfile> BusinessProfiles { get; }
    DbSet<Product> Products { get; }
    DbSet<Rfq> Rfqs { get; }
    DbSet<Quote> Quotes { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
