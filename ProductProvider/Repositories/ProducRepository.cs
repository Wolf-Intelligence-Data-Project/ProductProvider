using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductProvider.Interfaces;
using ProductProvider.Models.Data.Entities;
using ProductProvider.Models.Data;

namespace ProductProvider.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductEntity>> GetAllProductsAsync()
    {
        return await _context.Products
            .Where(p => p.SoldUntil == null && p.ReservedUntil == null)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductEntity>> GetFilteredProductsAsync(string? search, string? businessType)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.CompanyName.Contains(search) || p.City.Contains(search));
        }

        if (!string.IsNullOrEmpty(businessType))
        {
            query = query.Where(p => p.BusinessType == businessType);
        }

        return await query.ToListAsync();
    }
}