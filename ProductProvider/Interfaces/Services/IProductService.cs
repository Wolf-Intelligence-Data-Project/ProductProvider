using ProductProvider.Models;

namespace ProductProvider.Interfaces.Services;

public interface IProductService
{
    Task<ProductFilterResponse> GetProductCountAsync(ProductFilterRequest filters);
    Task ImportProductsFromExcelAsync(IFormFile file);
}
