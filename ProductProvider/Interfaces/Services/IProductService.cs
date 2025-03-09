using ProductProvider.Models;

namespace ProductProvider.Services.Services;

public interface IProductService
{
    Task<ProductFilterResponse> GetProductCountAsync(ProductFilterRequest filters);
    Task ImportProductsFromExcelAsync(IFormFile file);
}
