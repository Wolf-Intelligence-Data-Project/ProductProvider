using ProductProvider.Models.Data.Entities;
using ProductProvider.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductProvider.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<int> GetFilteredProductsCountAsync(ProductFilterRequest filters);
        Task<List<Guid>> GetProductIdsForReservationAsync(ProductReserveRequest request);
        Task AddProductsAsync(List<ProductEntity> products);
    }
}
