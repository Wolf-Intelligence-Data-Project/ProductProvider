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
        Task<ProductFilterResponse> GetProductCountAsync(ProductFilterRequest filters);
        Task<int> ReserveProductsAsync(ProductFilterRequest filters, Guid userId);
        Task ReleaseExpiredReservationsAsync();
        Task ImportProductsFromExcelAsync(IFormFile file);
        Task<int> GetAvailableProductsQuantityAsync();
    }
}
