using ProductProvider.Models.Data.Entities;

namespace ProductProvider.Interfaces.Services;

public interface IBusinessTypeService
{
    string FormatBusinessTypeFilter(string businessTypeFilter);

    IQueryable<ProductEntity> FilterByBusinessType(IQueryable<ProductEntity> products, string businessTypeFilter);

    Task<IEnumerable<object>> GetAvailableBusinessTypes();

    Task<IEnumerable<string>> GetAvailableCities();
}
