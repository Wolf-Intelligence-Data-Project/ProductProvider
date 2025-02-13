using ProductProvider.Interfaces;
using ProductProvider.Models.Data.Entities;
using ProductProvider.Repositories;

namespace ProductProvider.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductEntity>> GetFilteredProductsAsync(
        string? search = null,
        string? businessType = null,
        string? address = null,
        string? postalCode = null,
        string? city = null,
        string? phoneNumber = null,
        string? email = null,
        string? revenue = null,
        string? numberOfEmployees = null,
        string? ceo = null)
    {
        // Here, we can apply more business-specific logic if needed.
        // For example, we can check user roles, apply global rules, log access, etc.

        return await _productRepository.GetFilteredProductsAsync(
            search, businessType, address, postalCode, city, phoneNumber, email, revenue,
            numberOfEmployees, ceo);
    }
}