using Microsoft.EntityFrameworkCore;
using Negade.Application.Common.Interfaces;
using Negade.Domain.Entities;

namespace Negade.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<BusinessProfile> BusinessProfiles => Set<BusinessProfile>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Rfq> Rfqs => Set<Rfq>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<TradeRating> TradeRatings => Set<TradeRating>();
    public DbSet<TradeHistory> TradeHistory => Set<TradeHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(u => u.PhoneNumber)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(u => u.Email)
                .HasMaxLength(200);
            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(50);
            entity.HasIndex(u => u.PhoneNumber)
                .IsUnique();
        });

        modelBuilder.Entity<BusinessProfile>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasOne(p => p.OwnerUser)
                .WithMany(u => u.BusinessProfiles)
                .HasForeignKey(p => p.OwnerUserId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.Property(p => p.BusinessName)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(p => p.OwnerName)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(p => p.TinNumber)
                .HasMaxLength(50);
            entity.Property(p => p.PhoneNumber)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(p => p.Region)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(p => p.City)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(p => p.Address)
                .HasMaxLength(500);
            entity.Property(p => p.BusinessType)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(p => p.VerificationStatus)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(p => p.RatingAverage)
                .HasPrecision(3, 2);
            entity.HasIndex(p => p.TinNumber);
            entity.HasIndex(p => new { p.Region, p.City });
            entity.HasIndex(p => p.OwnerUserId);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(p => p.Description)
                .HasMaxLength(2000);
            entity.Property(p => p.Category)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(p => p.Price)
                .HasPrecision(18, 2);
            entity.Property(p => p.Unit)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(p => p.StockQuantity)
                .IsRequired();
            entity.Property(p => p.AvailableQuantity)
                .HasPrecision(18, 2);
            entity.Property(p => p.Region)
                .HasMaxLength(100);
            entity.Property(p => p.City)
                .HasMaxLength(100);
            entity.Property(p => p.CreatedAt)
                .IsRequired();
            entity.HasIndex(p => p.Name);
            entity.HasIndex(p => p.Category);
            entity.HasIndex(p => new { p.Region, p.City });
            entity.HasIndex(p => p.IsAvailable);
        });

        modelBuilder.Entity<Rfq>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasOne(r => r.BuyerUser)
                .WithMany()
                .HasForeignKey(r => r.BuyerUserId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.Property(r => r.BuyerName)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(r => r.BuyerPhoneNumber)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(r => r.BuyerBusinessName)
                .HasMaxLength(200);
            entity.Property(r => r.ProductName)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(r => r.Category)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(r => r.Quantity)
                .HasPrecision(18, 2);
            entity.Property(r => r.Unit)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(r => r.DeliveryRegion)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(r => r.DeliveryCity)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(r => r.Notes)
                .HasMaxLength(2000);
            entity.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(50);
            entity.HasIndex(r => r.Status);
            entity.HasIndex(r => new { r.Category, r.DeliveryRegion });
            entity.HasIndex(r => r.BuyerUserId);
        });

        modelBuilder.Entity<Quote>(entity =>
        {
            entity.HasKey(q => q.Id);
            entity.HasOne(q => q.Rfq)
                .WithMany(r => r.Quotes)
                .HasForeignKey(q => q.RfqId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(q => q.Supplier)
                .WithMany(s => s.Quotes)
                .HasForeignKey(q => q.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(q => q.SupplierUser)
                .WithMany()
                .HasForeignKey(q => q.SupplierUserId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.Property(q => q.UnitPrice)
                .HasPrecision(18, 2);
            entity.Property(q => q.QuantityAvailable)
                .HasPrecision(18, 2);
            entity.Property(q => q.Notes)
                .HasMaxLength(2000);
            entity.HasIndex(q => q.SupplierUserId);
        });

        modelBuilder.Entity<TradeRating>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasOne(r => r.Supplier)
                .WithMany(s => s.Ratings)
                .HasForeignKey(r => r.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(r => r.RaterUser)
                .WithMany()
                .HasForeignKey(r => r.RaterUserId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.Property(r => r.Comment)
                .HasMaxLength(1000);
            entity.HasIndex(r => r.SupplierId);
            entity.HasIndex(r => r.RaterUserId);
        });

        modelBuilder.Entity<TradeHistory>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.HasOne(h => h.BusinessProfile)
                .WithMany(p => p.TradeHistory)
                .HasForeignKey(h => h.BusinessProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(h => h.Description)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(h => h.CounterpartyName)
                .HasMaxLength(200);
            entity.Property(h => h.Amount)
                .HasPrecision(18, 2);
            entity.HasIndex(h => h.BusinessProfileId);
        });
    }
}
