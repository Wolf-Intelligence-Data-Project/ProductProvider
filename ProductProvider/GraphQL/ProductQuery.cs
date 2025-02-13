using ProductProvider.Models.Data.Entities;
using ProductProvider.Services;

namespace ProductProvider.GraphQL;

public class ProductQuery
{
    private readonly ProductService _productService;

    // Inject ProductService for filtering and retrieving product data
    public ProductQuery(ProductService productService)
    {
        _productService = productService;
    }

    // Define the GraphQL query to get filtered products
    public async Task<IEnumerable<ProductEntity>> GetFilteredProductsAsync(
        [GraphQLNonNullType] string? search,
        string? businessType,
        string? address,
        string? postalCode,
        string? city,
        string? phoneNumber,
        string? email,
        string? revenue,
        string? numberOfEmployees,
        string? ceo
    )
    {
        return await _productService.GetFilteredProductsAsync(
            search, businessType, address, postalCode, city, phoneNumber, email,
            revenue, numberOfEmployees, ceo
        );
    }
}
