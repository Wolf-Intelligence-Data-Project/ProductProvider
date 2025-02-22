using Microsoft.EntityFrameworkCore;
using ProductProvider.Models.Data.Entities;
using ProductProvider.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductProvider.Models;
using ProductProvider.Interfaces;

namespace ProductProvider.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _context;

        public ProductRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductEntity>> GetAvailableProductsAsync(ProductFilterRequest filters, int quantity)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(filters.CompanyName))
                query = query.Where(p => p.CompanyName.Contains(filters.CompanyName));

            if (!string.IsNullOrEmpty(filters.BusinessType))
                query = query.Where(p => p.BusinessType == filters.BusinessType);

            if (filters.MinRevenue.HasValue)
                query = query.Where(p => p.Revenue >= filters.MinRevenue.Value);

            if (!string.IsNullOrEmpty(filters.CEO))
                query = query.Where(p => p.CEO.Contains(filters.CEO));

            // Add other fields similarly

            query = query
                .Where(p => (p.SoldUntil == null || p.SoldUntil < DateTime.UtcNow) && (p.ReservedUntil == null || p.ReservedUntil < DateTime.UtcNow))
                .OrderBy(p => Guid.NewGuid()) // Randomly pick products
                .Take(quantity);

            return await query.ToListAsync();
        }


        public async Task ReserveProductsAsync(List<ProductEntity> products, Guid userId)
        {
            var reservationTime = DateTime.UtcNow.AddMinutes(15);
            foreach (var product in products)
            {
                product.ReservedUntil = reservationTime;
                product.ReservedBy = userId;
            }
            await _context.SaveChangesAsync();
        }

        public async Task ReleaseExpiredReservationsAsync()
        {
            var expiredProducts = await _context.Products
                .Where(p => p.ReservedUntil < DateTime.UtcNow)
                .ToListAsync();

            foreach (var product in expiredProducts)
            {
                product.ReservedUntil = null;
                product.ReservedBy = null;
            }

            await _context.SaveChangesAsync();
        }
        public async Task<int> GetAvailableProductsQuantityAsync()
        {
            return await _context.Products
                                 .Where(p => (p.SoldUntil == null || p.SoldUntil < DateTime.UtcNow) &&
                                             (p.ReservedUntil == null || p.ReservedUntil < DateTime.UtcNow))
                                 .CountAsync();
        }

        public async Task AddProductsAsync(List<ProductEntity> products)
        {
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
        }


    }
}
