using Microsoft.EntityFrameworkCore;
using ProductProvider.Models.Data.Entities;

namespace ProductProvider.Models.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

    public DbSet<ProductEntity> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductEntity>()
            .HasIndex(p => p.OrganizationNumber)
            .IsUnique();
    }
}