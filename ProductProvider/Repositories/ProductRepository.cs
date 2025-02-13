using ProductProvider.Models.Data.Entities;
using ProductProvider.Models.Data;
using Microsoft.EntityFrameworkCore;
using ProductProvider.Interfaces;

namespace ProductProvider.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _dbContext;

    public ProductRepository(ProductDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ProductEntity>> GetFilteredProductsAsync(
        string? search = null,
        string? businessType = null,
        string? address = null,
        string? postalCode = null,
        string? city = null,
        string? phoneNumber = null,
        string? email = null,
        string? revenue = null,
        string? numberOfEmployees = null,
        string? ceo = null)
    {
        var query = _dbContext.Products.AsQueryable();

        // Apply filters directly to the query (data access concerns)
        if (!string.IsNullOrEmpty(search)) query = query.Where(p => p.CompanyName.Contains(search));
        if (!string.IsNullOrEmpty(businessType)) query = query.Where(p => p.BusinessType.Contains(businessType));
        if (!string.IsNullOrEmpty(address)) query = query.Where(p => p.Address.Contains(address));
        if (!string.IsNullOrEmpty(postalCode)) query = query.Where(p => p.PostalCode.Contains(postalCode));
        if (!string.IsNullOrEmpty(city)) query = query.Where(p => p.City.Contains(city));
        if (!string.IsNullOrEmpty(phoneNumber)) query = query.Where(p => p.PhoneNumber.Contains(phoneNumber));
        if (!string.IsNullOrEmpty(email)) query = query.Where(p => p.Email.Contains(email));
        if (!string.IsNullOrEmpty(revenue)) query = query.Where(p => p.Revenue.Contains(revenue));
        if (!string.IsNullOrEmpty(numberOfEmployees)) query = query.Where(p => p.NumberOfEmployees.Contains(numberOfEmployees));
        if (!string.IsNullOrEmpty(ceo)) query = query.Where(p => p.CEO.Contains(ceo));

        // Only return products that are not sold or reserved
        query = query.Where(p => p.SoldUntil == null || p.ReservedUntil == null);

        return await query.ToListAsync();
    }
}