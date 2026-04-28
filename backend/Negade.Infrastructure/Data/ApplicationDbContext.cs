using Microsoft.EntityFrameworkCore;
using Negade.Application.Abstractions;
using Negade.Domain.Entities;

namespace Negade.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(p => p.Description)
                .HasMaxLength(2000);
            entity.Property(p => p.Price)
                .HasPrecision(18, 2);
            entity.Property(p => p.StockQuantity)
                .IsRequired();
            entity.Property(p => p.CreatedAt)
                .IsRequired();
        });
    }
}
