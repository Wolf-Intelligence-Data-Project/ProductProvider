using ProductProvider.Models.Data.Entities;
using ProductProvider.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ProductProvider.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductEntity>> GetFilteredProductsAsync(ProductFilterRequest filters, int quantity);
        Task ReserveProductsAsync(ProductFilterRequest filters, int quantity, Guid userId);
        Task ReleaseExpiredReservationsAsync();
        Task ImportProductsFromExcelAsync(IFormFile file);
        Task<int> GetAvailableProductsQuantityAsync();
    }
}
