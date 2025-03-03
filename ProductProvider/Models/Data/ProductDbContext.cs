using Microsoft.EntityFrameworkCore;
using ProductProvider.Models.Data.Entities;

namespace ProductProvider.Models.Data;

public class ProductDbContext : DbContext
{
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<ReservationEntity> Reservations { get; set; } 

    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ProductEntity Configuration
        modelBuilder.Entity<ProductEntity>()
            .HasKey(p => p.ProductId);

        modelBuilder.Entity<ProductEntity>()
            .Property(p => p.Revenue)
            .HasColumnType("decimal(18, 2)"); 

        // ReservationEntity Configuration
        modelBuilder.Entity<ReservationEntity>()
            .HasKey(r => r.ReservationId);

        modelBuilder.Entity<ReservationEntity>()
            .Property(r => r.BusinessTypes)
            .HasColumnType("nvarchar(max)");

        modelBuilder.Entity<ReservationEntity>()
            .Property(r => r.Regions)
            .HasColumnType("nvarchar(max)");

        modelBuilder.Entity<ReservationEntity>()
            .Property(r => r.Cities)
            .HasColumnType("nvarchar(max)");

        modelBuilder.Entity<ReservationEntity>()
            .Property(r => r.PostalCodes)
            .HasColumnType("nvarchar(max)");
    }
}
