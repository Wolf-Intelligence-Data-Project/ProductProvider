using ProductProvider.Models.Data.Entities;
using ProductProvider.Interfaces.Services;

namespace ProductProvider.Interfaces;
public class BusinessTypeService : IBusinessTypeService
{
    public string FormatBusinessTypeFilter(string businessTypeFilter)
    {
        if (string.IsNullOrEmpty(businessTypeFilter))
        {
            return string.Empty;
        }

        string formattedFilter = businessTypeFilter.Length >= 3 ? businessTypeFilter.Substring(0, 3) : businessTypeFilter;
        formattedFilter = formattedFilter.Split('.')[0];

        return formattedFilter;
    }

    public IQueryable<ProductEntity> FilterByBusinessType(IQueryable<ProductEntity> products, string businessTypeFilter)
    {
        if (string.IsNullOrEmpty(businessTypeFilter))
        {
            return products;
        }

        var formattedFilter = FormatBusinessTypeFilter(businessTypeFilter);
        return products.Where(p => p.BusinessType.StartsWith(formattedFilter));
    }
}
