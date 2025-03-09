using ProductProvider.Models.Data.Entities;
using ProductProvider.Models;

namespace ProductProvider.Services.Services;

public interface IElasticSearchService
{
    Task<int> SearchProductsAsync(ProductFilterRequest filters);
    Task IndexProductAsync(ProductEntity product);
    Task DeleteProductAsync(string productId);
}
