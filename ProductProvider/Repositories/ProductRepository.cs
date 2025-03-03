using Microsoft.EntityFrameworkCore;
using ProductProvider.Models.Data.Entities;
using ProductProvider.Models.Data;
using Dapper;
using ProductProvider.Models;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using ProductProvider.Interfaces.Repositories;
using ProductProvider.Interfaces.Services;
using System.Data.Common;

namespace ProductProvider.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;
    private readonly IBusinessTypeService _businessTypeService;
    private readonly string _connectionString;
    

    public ProductRepository(ProductDbContext context, IBusinessTypeService businessTypeService, IConfiguration configuration)
    {
        _context = context;
        _businessTypeService = businessTypeService;
        _connectionString = configuration.GetConnectionString("ProductDatabase");
    }

    //using dapper for better performance 
    public async Task<int> GetFilteredProductsCountAsync(ProductFilterRequest filters)
    {
        var query = new StringBuilder("SELECT COUNT(*) FROM Products WHERE SoldUntil IS NULL AND ReservedUntil IS NULL");

        var parameters = new DynamicParameters();

        // Apply businessTypes filter if provided
        if (filters.BusinessTypes?.Any() == true)
        {
            query.Append(" AND (");
            for (int i = 0; i < filters.BusinessTypes.Count; i++)
            {
                var paramName = $"@BusinessType{i}";

                // Reformat the SNI code (Insert space after the first letter and before the numbers)
                string formattedSni = Regex.Replace(filters.BusinessTypes[i], @"^([A-Z])(\d{2})", "$1 $2") + "%";

                query.Append($" BusinessType LIKE {paramName}");
                if (i < filters.BusinessTypes.Count - 1) query.Append(" OR");

                parameters.Add(paramName, formattedSni);
            }
            query.Append(")");
        }


        // Apply cities filter
        if (filters.Cities?.Any() == true)
        {
            query.Append(" AND City IN @Cities");
            parameters.Add("@Cities", filters.Cities);
        }

        // Apply postalCodes filter
        if (filters.PostalCodes?.Any() == true)
        {
            query.Append(" AND PostalCode IN @PostalCodes");
            parameters.Add("@PostalCodes", filters.PostalCodes);
        }

        // Apply revenue filter
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

        // Apply employee filter
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
        // Validate quantity
        if (filters.QuantityOfFiltered <= 0)
        {
            return new List<Guid>(); 
        }

        // Build the base SQL query with filters dynamically
        var sql = @"
                SELECT TOP (@Quantity) p.ProductId
                FROM Products p
                WHERE (p.SoldUntil IS NULL AND p.ReservedUntil IS NULL)";

        var parameters = new DynamicParameters();
        parameters.Add("Quantity", filters.QuantityOfFiltered);

        // Add filters if provided
        if (filters.BusinessTypes?.Any() == true)
        {
            sql += " AND p.BusinessType IN @BusinessTypes";
            parameters.Add("BusinessTypes", filters.BusinessTypes);
        }

        if (filters.Cities?.Any() == true)
        {
            sql += " AND p.City IN @Cities";
            parameters.Add("Cities", filters.Cities);
        }

        if (filters.PostalCodes?.Any() == true)
        {
            sql += " AND p.PostalCode IN @PostalCodes";
            parameters.Add("PostalCodes", filters.PostalCodes);
        }

        if (filters.MinRevenue.HasValue)
        {
            sql += " AND p.Revenue >= @MinRevenue";
            parameters.Add("MinRevenue", filters.MinRevenue);
        }

        if (filters.MinNumberOfEmployees.HasValue)
        {
            sql += " AND p.NumberOfEmployees >= @MinNumberOfEmployees";
            parameters.Add("MinNumberOfEmployees", filters.MinNumberOfEmployees);
        }

        // Randomize the order before selecting TOP (@Quantity)
        sql += " ORDER BY NEWID()";

        using var connection = new SqlConnection(_connectionString);
        return (await connection.QueryAsync<Guid>(sql, parameters)).ToList();
    }

    public async Task ReserveProductsByIdsAsync(List<Guid> productIds, Guid companyId)
    {
        // Prepare the SQL update statement
        var sql = @"
                UPDATE Products
                SET ReservedBy = @CompanyId, ReservedUntil = @ReservedUntil
                WHERE ProductId IN @ProductIds";

        var parameters = new DynamicParameters();
        parameters.Add("CompanyId", companyId);
        parameters.Add("ReservedUntil", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time")).AddMinutes(15));
        parameters.Add("ProductIds", productIds);

        // Execute the query with Dapper
        using var connection = new SqlConnection(_connectionString); // Assuming _connectionString is your DB connection string
        await connection.ExecuteAsync(sql, parameters);
    }

    public async Task<int> GetAvailableProductsQuantityAsync()
    {
        return await _context.Products
                             .Where(p => (p.SoldUntil == null || p.SoldUntil < TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"))) &&
                                         (p.ReservedUntil == null || p.ReservedUntil < TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"))))
                             .CountAsync();
    }

    public async Task AddProductsAsync(List<ProductEntity> products)
    {
        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();
    }
}
