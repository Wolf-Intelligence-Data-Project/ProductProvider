using Microsoft.EntityFrameworkCore;
using ProductProvider.Models.Data.Entities;

namespace ProductProvider.Models.Data;

public class ProductDbContext : DbContext
{
    // DbSet for ProductEntity table
    public DbSet<ProductEntity> Products { get; set; }

    // DbSet for ReservationEntity table
    public DbSet<ReservationEntity> Reservations { get; set; }

    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure ProductEntity
        modelBuilder.Entity<ProductEntity>(entity =>
        {
            entity.ToTable("Products"); // Specify the table name in the database
            entity.HasKey(e => e.ProductId); // Define ProductId as primary key

            // Define other properties' column mappings, if necessary
            entity.Property(e => e.CompanyName).IsRequired();
            entity.Property(e => e.OrganizationNumber).IsRequired();
            entity.Property(e => e.Address).IsRequired();
            entity.Property(e => e.PostalCode).IsRequired();
            entity.Property(e => e.City).IsRequired();
            entity.Property(e => e.PhoneNumber).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.BusinessType).IsRequired();
            entity.Property(e => e.Revenue).IsRequired();
            entity.Property(e => e.NumberOfEmployees).IsRequired();
            entity.Property(e => e.CEO).IsRequired();
            entity.Property(e => e.CustomerId).IsRequired(false);
            entity.Property(e => e.SoldUntil).IsRequired(false);
            entity.Property(e => e.ReservedUntil).IsRequired(false);
        });

        // Configure ReservationEntity
        modelBuilder.Entity<ReservationEntity>(entity =>
        {
            entity.ToTable("Reservations"); // Specify the table name in the database
            entity.HasKey(e => e.ReservationId); // Define ReservationId as primary key

            // Define other properties' column mappings
            entity.Property(e => e.CustomerId).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.ReservedFrom).IsRequired();
            entity.Property(e => e.SoldFrom).IsRequired(false);

            // Additional mappings for the new properties
            entity.Property(e => e.BusinessTypes).IsRequired(false); // Nullable field
            entity.Property(e => e.Regions).IsRequired(false); // Nullable field
            entity.Property(e => e.CitiesByRegion).IsRequired(false); // Nullable field
            entity.Property(e => e.Cities).IsRequired(false); // Nullable field
            entity.Property(e => e.PostalCodes).IsRequired(false); // Nullable field
            entity.Property(e => e.MinRevenue).IsRequired(false); // Nullable field
            entity.Property(e => e.MaxRevenue).IsRequired(false); // Nullable field
            entity.Property(e => e.MinNumberOfEmployees).IsRequired(false); // Nullable field
            entity.Property(e => e.MaxNumberOfEmployees).IsRequired(false); // Nullable field
        });
    }
}



//using Microsoft.EntityFrameworkCore;
//using ProductProvider.Models.Data.Entities;

//namespace ProductProvider.Models.Data;

//public class ProductDbContext : DbContext
//{
//    // DbSet only for ProductEntity table
//    public DbSet<ProductEntity> Products { get; set; }

//    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        base.OnModelCreating(modelBuilder);

//        // Configure ProductEntity
//        modelBuilder.Entity<ProductEntity>(entity =>
//        {
//            entity.ToTable("Products"); // Specify the table name in the database
//            entity.HasKey(e => e.ProductId); // Define ProductId as primary key

//            // Define other properties' column mappings, if necessary
//            entity.Property(e => e.CompanyName).IsRequired();
//            entity.Property(e => e.OrganizationNumber).IsRequired();
//            entity.Property(e => e.Address).IsRequired();
//            entity.Property(e => e.PostalCode).IsRequired();
//            entity.Property(e => e.City).IsRequired();
//            entity.Property(e => e.PhoneNumber).IsRequired();
//            entity.Property(e => e.Email).IsRequired();
//            entity.Property(e => e.BusinessType).IsRequired();
//            entity.Property(e => e.Revenue).IsRequired();
//            entity.Property(e => e.NumberOfEmployees).IsRequired();
//            entity.Property(e => e.CEO).IsRequired();
//            entity.Property(e => e.CustomerId).IsRequired(false);
//            entity.Property(e => e.SoldUntil).IsRequired(false);
//            entity.Property(e => e.ReservedUntil).IsRequired(false);
//        });

//        // Exclude Reservations table from migrations
//        modelBuilder.Ignore<ReservationEntity>();
//    }
//}
