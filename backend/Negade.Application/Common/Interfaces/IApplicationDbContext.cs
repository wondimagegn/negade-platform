using Microsoft.EntityFrameworkCore;
using Negade.Domain.Entities;

namespace Negade.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<AppUser> AppUsers { get; }
    DbSet<Category> Categories { get; }
    DbSet<Region> Regions { get; }
    DbSet<City> Cities { get; }
    DbSet<BusinessProfile> BusinessProfiles { get; }
    DbSet<Product> Products { get; }
    DbSet<Rfq> Rfqs { get; }
    DbSet<Quote> Quotes { get; }
    DbSet<TradeRating> TradeRatings { get; }
    DbSet<TradeHistory> TradeHistory { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
