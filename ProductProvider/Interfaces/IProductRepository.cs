using ProductProvider.Models.Data.Entities;
using ProductProvider.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductProvider.Interfaces
{
    public interface IProductRepository
    {
        Task<List<ProductEntity>> GetAvailableProductsAsync(ProductFilterRequest filters, int quantity);
        Task ReserveProductsAsync(List<ProductEntity> products, Guid userId);
        Task ReleaseExpiredReservationsAsync();
        Task<int> GetAvailableProductsQuantityAsync();
        Task AddProductsAsync(List<ProductEntity> products);
    }
}
