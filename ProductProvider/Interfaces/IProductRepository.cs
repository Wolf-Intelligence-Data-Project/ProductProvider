using ProductProvider.Models.Data.Entities;

namespace ProductProvider.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<ProductEntity>> GetAllProductsAsync();
    Task<IEnumerable<ProductEntity>> GetFilteredProductsAsync(string? search, string? businessType);
}