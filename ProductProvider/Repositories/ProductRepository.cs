using Microsoft.EntityFrameworkCore;
using ProductProvider.Models.Data.Entities;
using ProductProvider.Models.Data;
using Dapper;
using ProductProvider.Models;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using ProductProvider.Interfaces;
using ProductProvider.Interfaces.Repositories;
using System;

namespace ProductProvider.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _productDbContext;
    private readonly string _connectionString;
    private readonly ILogger<BusinessTypeService> _logger;
    private readonly DateTime _stockholmTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm"));


    public ProductRepository(ProductDbContext productDbContext, IConfiguration configuration, ILogger<BusinessTypeService> logger)
    {
        _productDbContext = productDbContext;
        _connectionString = configuration.GetConnectionString("ProductDatabase");
        _logger = logger;
    }

    //using dapper for better performance 
    public async Task<int> GetFilteredProductsCountAsync(ProductFilterRequest filters)
    {

        var parameters = new DynamicParameters();

        parameters.Add("@NowStockholm", _stockholmTime);

        var query = new StringBuilder("SELECT COUNT(*) FROM Products WHERE ReservedUntil IS NULL AND (SoldUntil IS NULL OR SoldUntil < @NowStockholm)");

        // Apply businessTypes filter
        if (filters.BusinessTypes?.Any() == true)
        {
            query.Append(" AND (");
            for (int i = 0; i < filters.BusinessTypes.Count; i++)
            {
                var paramName = $"@BusinessType{i}";
                string formattedSni = Regex.Replace(filters.BusinessTypes[i], @"^([A-Z])(\d{2})", "$1 $2") + "%";
                query.Append($" BusinessType LIKE {paramName}");
                if (i < filters.BusinessTypes.Count - 1) query.Append(" OR");
                parameters.Add(paramName, formattedSni);
            }
            query.Append(")");
        }

        if (filters.Cities?.Any() == true)
        {
            query.Append(" AND City IN @Cities");
            parameters.Add("@Cities", filters.Cities);
        }

        if (filters.PostalCodes?.Any() == true)
        {
            query.Append(" AND PostalCode IN @PostalCodes");
            parameters.Add("@PostalCodes", filters.PostalCodes);
        }

        if (filters.MinRevenue.HasValue)
        {
            query.Append(" AND Revenue >= @MinRevenue");
            parameters.Add("@MinRevenue", filters.MinRevenue);
        }

        if (filters.MaxRevenue.HasValue)
        {
            query.Append(" AND Revenue <= @MaxRevenue");
            parameters.Add("@MaxRevenue", filters.MaxRevenue);
        }

        if (filters.MinNumberOfEmployees.HasValue)
        {
            query.Append(" AND NumberOfEmployees >= @MinNumberOfEmployees");
            parameters.Add("@MinNumberOfEmployees", filters.MinNumberOfEmployees);
        }

        if (filters.MaxNumberOfEmployees.HasValue)
        {
            query.Append(" AND NumberOfEmployees <= @MaxNumberOfEmployees");
            parameters.Add("@MaxNumberOfEmployees", filters.MaxNumberOfEmployees);
        }

        using var connection = new SqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<int>(query.ToString(), parameters);
    }

    public async Task<List<Guid>> GetProductIdsForReservationAsync(ProductReserveRequest filters)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@NowStockholm", _stockholmTime);

        if (filters.QuantityOfFiltered <= 0)
        {
            return new List<Guid>();
        }

        parameters.Add("Quantity", filters.QuantityOfFiltered);

        var sql = new StringBuilder(@"
        SELECT TOP (@Quantity) p.ProductId
        FROM Products p
        WHERE (p.SoldUntil IS NULL OR p.SoldUntil < @NowStockholm)
          AND p.ReservedUntil IS NULL
    ");

        // Add filters dynamically
        if (filters.BusinessTypes?.Any() == true)
        {
            sql.Append(" AND p.BusinessType IN @BusinessTypes");
            parameters.Add("BusinessTypes", filters.BusinessTypes);
        }

        if (filters.Cities?.Any() == true)
        {
            sql.Append(" AND p.City IN @Cities");
            parameters.Add("Cities", filters.Cities);
        }

        if (filters.PostalCodes?.Any() == true)
        {
            sql.Append(" AND p.PostalCode IN @PostalCodes");
            parameters.Add("PostalCodes", filters.PostalCodes);
        }

        if (filters.MinRevenue.HasValue)
        {
            sql.Append(" AND p.Revenue >= @MinRevenue");
            parameters.Add("MinRevenue", filters.MinRevenue);
        }

        if (filters.MinNumberOfEmployees.HasValue)
        {
            sql.Append(" AND p.NumberOfEmployees >= @MinNumberOfEmployees");
            parameters.Add("MinNumberOfEmployees", filters.MinNumberOfEmployees);
        }

        // Randomize result
        sql.Append(" ORDER BY NEWID()");

        using var connection = new SqlConnection(_connectionString);
        return (await connection.QueryAsync<Guid>(sql.ToString(), parameters)).ToList();
    }

    public async Task<IEnumerable<string>> GetAvailableBusinessTypes()
    {
        string sql = @"
            SELECT DISTINCT LEFT(REPLACE(BusinessType, ' ', ''), CHARINDEX('.', BusinessType + '.') - 1) 
            FROM Products
            WHERE BusinessType IS NOT NULL AND CHARINDEX('.', BusinessType) > 0";

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryAsync<string>(sql);

            _logger.LogInformation("Fetched Business Types: {BusinessTypes}", string.Join(", ", result));

            return result.Where(bt => !string.IsNullOrEmpty(bt)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching business types.");
            throw;
        }
    }
    public async Task<IEnumerable<string>> GetAvailableCities()
    {
        string sql = @"
        SELECT DISTINCT City 
        FROM Products
        WHERE City IS NOT NULL AND City <> ''";

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryAsync<string>(sql);

            _logger.LogInformation("Fetched Cities: {Cities}", string.Join(", ", result));

            return result.Where(city => !string.IsNullOrEmpty(city)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching cities.");
            throw;
        }
    }
    public async Task AddProductsAsync(List<ProductEntity> products)
    {
        await _productDbContext.Products.AddRangeAsync(products);
        await _productDbContext.SaveChangesAsync();
    }
}
