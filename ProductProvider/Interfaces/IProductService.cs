using ProductProvider.Models.Data.Entities;

namespace ProductProvider.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductEntity>> GetFilteredProductsAsync(
        string? search = null,
        string? businessType = null,
        string? address = null,
        string? postalCode = null,
        string? city = null,
        string? phoneNumber = null,
        string? email = null,
        string? revenue = null,
        string? numberOfEmployees = null,
        string? ceo = null);
}