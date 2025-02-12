using HotChocolate;
using HotChocolate.Types;
using ProductProvider.Interfaces;
using ProductProvider.Models.Data.Entities;

namespace ProductProvider.Controllers.GraphQL;

[ExtendObjectType("Query")]
public class ProductQuery
{
    private readonly IProductRepository _repository;

    public ProductQuery(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProductEntity>> GetProductsAsync()
    {
        return await _repository.GetAllProductsAsync();
    }

    public async Task<IEnumerable<ProductEntity>> GetFilteredProductsAsync(string? search, string? businessType)
    {
        return await _repository.GetFilteredProductsAsync(search, businessType);
    }
}