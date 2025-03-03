using ProductProvider.Models.Data.Entities;
using ProductProvider.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ProductProvider.Interfaces.Services
{
    public interface IProductService
    {
        Task<ProductFilterResponse> GetProductCountAsync(ProductFilterRequest filters);
        Task ImportProductsFromExcelAsync(IFormFile file);
    }
}
