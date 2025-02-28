using ProductProvider.Models.Data.Entities;

namespace ProductProvider.Interfaces;

public interface IBusinessTypeService
{
    /// <summary>
    /// Formats the incoming business type filter string.
    /// </summary>
    /// <param name="businessTypeFilter">The raw business type filter input.</param>
    /// <returns>A formatted business type filter string.</returns>
    string FormatBusinessTypeFilter(string businessTypeFilter);

    /// <summary>
    /// Filters products based on the formatted business type filter.
    /// </summary>
    /// <param name="products">Queryable list of products.</param>
    /// <param name="businessTypeFilter">The business type filter input.</param>
    /// <returns>Filtered IQueryable of ProductEntity.</returns>
    IQueryable<ProductEntity> FilterByBusinessType(IQueryable<ProductEntity> products, string businessTypeFilter);
}
