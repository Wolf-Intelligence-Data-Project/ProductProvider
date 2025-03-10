using ProductProvider.Models.Data.Entities;
using ProductProvider.Models.SNI_codes;
using System.ComponentModel;
using ProductProvider.Interfaces.Services;
using ProductProvider.Interfaces.Repositories;

namespace ProductProvider.Interfaces;
public class BusinessTypeService : IBusinessTypeService
{
    private readonly IProductRepository _productRepository;

    public BusinessTypeService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

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

    // Method to format the BusinessType from the database to match the enum
    private string FormatBusinessType(string businessType)
    {
        return businessType.Length >= 3 ? businessType.Substring(0, 3) : businessType;
    }

    // Method to get available business types from the database and match with the enum
    public async Task<IEnumerable<object>> GetAvailableBusinessTypes()
    {
        try
        {
            var availableBusinessTypes = await _productRepository.GetAvailableBusinessTypes();
            Console.WriteLine("Fetched business types from DB: " + string.Join(", ", availableBusinessTypes));

            // Format business types
            var formattedBusinessTypes = availableBusinessTypes
                .Select(bt => FormatBusinessType(bt))
                .Distinct()
                .ToList();

            // Get enum values
            var enumValues = Enum.GetValues(typeof(BusinessType))
                .Cast<BusinessType>()
                .Where(bt => formattedBusinessTypes.Contains(bt.ToString()))
                .Select(bt => new
                {
                    Value = bt.ToString(),
                    Description = GetEnumDescription(bt)
                })
                .ToList();

            return enumValues;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching business types: {ex.Message}");
            throw;
        }
    }

    // Helper method to get description for enum values
    private static string GetEnumDescription(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
        return attribute?.Description ?? value.ToString();
    }
    public async Task<IEnumerable<string>> GetAvailableCities()
    {
        var availableCities = await _productRepository.GetAvailableCities();
        Console.WriteLine("Fetched cities from DB: " + string.Join(", ", availableCities));

        return availableCities.Distinct().ToList(); // ✅ Returns unique cities
    }
}
