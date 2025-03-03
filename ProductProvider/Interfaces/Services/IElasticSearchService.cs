using System.Threading.Tasks;
using System.Collections.Generic;
using ProductProvider.Models.Data.Entities;
using ProductProvider.Models;

namespace ProductProvider.Interfaces.Services
{
    public interface IElasticSearchService
    {
        Task<int> SearchProductsAsync(ProductFilterRequest filters);
        Task IndexProductAsync(ProductEntity product);
        Task DeleteProductAsync(string productId);
    }
}
