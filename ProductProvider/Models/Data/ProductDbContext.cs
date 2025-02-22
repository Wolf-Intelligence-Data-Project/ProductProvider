using Microsoft.EntityFrameworkCore;
using ProductProvider.Models.Data.Entities;

namespace ProductProvider.Models.Data
{
    public class ProductDbContext : DbContext
    {
        public DbSet<ProductEntity> Products { get; set; }

        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicitly define the primary key for ProductEntity
            modelBuilder.Entity<ProductEntity>()
                .HasKey(p => p.ProductId);

            modelBuilder.Entity<ProductEntity>()
                .Property(p => p.Revenue)
                .HasColumnType("decimal(18, 2)"); // Specify precision and scale for Revenue
        }
    }
}
