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
        //Task<List<ProductEntity>> GetFilteredProductsRandomlyAsync(ProductFilterRequest filters, int quantity);
        Task ReserveProductsByIdsAsync(List<Guid> productIds, Guid companyId);
        Task<List<Guid>> GetProductIdsForReservationAsync(ProductReserveRequest request);
        Task<int> GetAvailableProductsQuantityAsync();
        Task AddProductsAsync(List<ProductEntity> products);
    }
}
