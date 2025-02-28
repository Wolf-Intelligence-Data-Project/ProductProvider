using Microsoft.EntityFrameworkCore;
using ProductProvider.Models.Data.Entities;
using ProductProvider.Models.Data;
using Dapper;
using ProductProvider.Models;
using ProductProvider.Interfaces;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace ProductProvider.Repositories
{
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



        //public async Task<List<ProductEntity>> GetFilteredProductsRandomlyAsync(ProductFilterRequest filters, int quantity)
        //{
        //    // Split strings before using them in the query
        //    var businessTypes = string.IsNullOrEmpty(filters.BusinessType) ? null : filters.BusinessType.Split(',').ToList();
        //    var cities = string.IsNullOrEmpty(filters.City) ? null : filters.City.Split(',').ToList();
        //    var postalCodes = string.IsNullOrEmpty(filters.PostalCode) ? null : filters.PostalCode.Split(',').ToList();

        //    var query = _context.Products.AsQueryable();

        //    if (businessTypes != null && businessTypes.Any())
        //        query = query.Where(p => businessTypes.Contains(p.BusinessType));

        //    if (cities != null && cities.Any())
        //        query = query.Where(p => cities.Contains(p.City));

        //    if (postalCodes != null && postalCodes.Any())
        //        query = query.Where(p => postalCodes.Contains(p.PostalCode));

        //    if (filters.MinRevenue.HasValue)
        //        query = query.Where(p => p.Revenue >= filters.MinRevenue.Value);

        //    if (filters.MinNumberOfEmployees.HasValue)
        //        query = query.Where(p => p.NumberOfEmployees >= filters.MinNumberOfEmployees.Value);

        //    query = query.Where(p => p.SoldUntil == null && p.ReservedUntil == null);

        //    // Randomize the product selection using OrderBy with a Guid.NewGuid() to shuffle the results
        //    var randomProducts = await query.OrderBy(p => Guid.NewGuid())
        //                                     .Take(quantity) // Limit the number of random products
        //                                     .ToListAsync(); // Execute the query asynchronously

        //    return randomProducts;
        //}


        public async Task<List<Guid>> GetProductIdsForReservationAsync(ProductFilterRequest filters)
        {
            // Handle each filter and split values if needed
            var businessTypes = filters.BusinessTypes?.Where(bt => !string.IsNullOrEmpty(bt)).ToList();
            var cities = filters.Cities?.Where(c => !string.IsNullOrEmpty(c)).ToList();
            var postalCodes = filters.PostalCodes?.Where(pc => !string.IsNullOrEmpty(pc)).ToList();

            var query = _context.Products.AsQueryable();

            // Apply businessTypes filter if provided
            if (businessTypes != null && businessTypes.Any())
                query = query.Where(p => businessTypes.Contains(p.BusinessType));

            // Apply cities filter if provided
            if (cities != null && cities.Any())
                query = query.Where(p => cities.Contains(p.City));

            // Apply postalCodes filter if provided
            if (postalCodes != null && postalCodes.Any())
                query = query.Where(p => postalCodes.Contains(p.PostalCode));

            if (filters.MinRevenue.HasValue)
                query = query.Where(p => p.Revenue >= filters.MinRevenue.Value);

            if (filters.MinNumberOfEmployees.HasValue)
                query = query.Where(p => p.NumberOfEmployees >= filters.MinNumberOfEmployees.Value);

            // Ensure products are available (not sold or reserved)
            query = query.Where(p => p.SoldUntil == null && p.ReservedUntil == null);

            // Return the list of products matching the filters
            return await query.Select(p => p.ProductId).ToListAsync();
        }


        public async Task ReserveProductsByIdsAsync(List<Guid> productIds, Guid companyId)
        {
            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToListAsync();

            foreach (var product in products)
            {
                product.ReservedBy = companyId;
                product.ReservedUntil = DateTime.UtcNow.AddMinutes(15);
            }

            _context.Products.UpdateRange(products);
            await _context.SaveChangesAsync();
        }

        public async Task ReleaseExpiredReservationsAsync()
        {
            var expiredProducts = await _context.Products
                .Where(p => p.ReservedUntil < DateTime.UtcNow)
                .ToListAsync();

            foreach (var product in expiredProducts)
            {
                product.ReservedUntil = null;
                product.ReservedBy = null;
            }

            await _context.SaveChangesAsync();
        }
        public async Task<int> GetAvailableProductsQuantityAsync()
        {
            return await _context.Products
                                 .Where(p => (p.SoldUntil == null || p.SoldUntil < DateTime.UtcNow) &&
                                             (p.ReservedUntil == null || p.ReservedUntil < DateTime.UtcNow))
                                 .CountAsync();
        }

        public async Task AddProductsAsync(List<ProductEntity> products)
        {
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
        }
    }
}
