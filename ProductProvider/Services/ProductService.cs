﻿using ProductProvider.Models;
using ProductProvider.Models.Data.Entities;
using OfficeOpenXml;
using Microsoft.Extensions.Options;
using ProductProvider.Interfaces.Services;
using ProductProvider.Interfaces.Repositories;

namespace ProductProvider.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<PriceSettings> _priceSettings;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository productRepository, ILogger<ProductService> logger,IOptions<PriceSettings> priceSettings)
    {
        _productRepository = productRepository;
        _priceSettings = priceSettings;
        _logger = logger;
    }

    public async Task<ProductFilterResponse> GetProductCountAsync(ProductFilterRequest filters)
    {
        _logger.LogInformation("Filters received: {BusinessTypes}, {Cities}, {PostalCodes}, {MinRevenue}, {MaxRevenue}, {MinNumberOfEmployees}, {MaxNumberOfEmployees}",
            string.Join(",", filters.BusinessTypes ?? new List<string>()) ?? "null",
            string.Join(",", filters.Cities ?? new List<string>()) ?? "null",
            string.Join(",", filters.PostalCodes ?? new List<string>()) ?? "null",
            filters.MinRevenue?.ToString() ?? "null",
            filters.MaxRevenue?.ToString() ?? "null",
            filters.MinNumberOfEmployees?.ToString() ?? "null",
            filters.MaxNumberOfEmployees?.ToString() ?? "null");

        // Get the count from the repository
        var count = await _productRepository.GetFilteredProductsCountAsync(filters);

        
        // Ensure QuantityOfFiltered is required
        if (filters.QuantityOfFiltered > 0)
        {
            count = Math.Min(count, filters.QuantityOfFiltered);
        }

        // Get price and VAT rate from configuration
        decimal pricePerProduct = _priceSettings.Value.PricePerProduct; // SEK per product
        decimal vatRate = _priceSettings.Value.VatRate; // VAT rate (e.g., 25 for 25%)
        // Ensure price settings are being loaded
        if (pricePerProduct <= 0 || vatRate <= 0)
        {
            _logger.LogError("Invalid price settings: PricePerProduct = {PricePerProduct}, VatRate = {VatRate}",
                pricePerProduct, vatRate);
            throw new InvalidOperationException("Invalid price settings.");
        }
        // Calculate the total price before VAT
        decimal baseTotalPrice = count * pricePerProduct;

        // Calculate the total price after VAT
        decimal totalPriceWithVat = baseTotalPrice * (1 + vatRate / 100); // Apply VAT to total price

        // Return the response model
        return new ProductFilterResponse
        {
            AvailableQuantity = count,
            TotalPriceBeforeVat = baseTotalPrice,
            TotalPrice = totalPriceWithVat
        };
    }

    public async Task ImportProductsFromExcelAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("The uploaded file is empty or null.");
        }

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Fix for EPPlus License Issue

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets[0];

        if (worksheet == null)
        {
            throw new InvalidOperationException("The uploaded file does not contain a valid worksheet.");
        }

        var rowCount = worksheet.Dimension?.Rows ?? 0;
        if (rowCount < 2)
        {
            throw new InvalidOperationException("The uploaded file does not contain enough data.");
        }

        var productsToAdd = new List<ProductEntity>();
        var errors = new List<string>();

        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                var companyName = worksheet.Cells[row, 1].Text?.Trim();
                var organizationNumber = worksheet.Cells[row, 2].Text?.Trim();
                var address = worksheet.Cells[row, 3].Text?.Trim();
                var postalCode = worksheet.Cells[row, 4].Text?.Trim();
                var city = worksheet.Cells[row, 5].Text?.Trim();
                var phoneNumber = worksheet.Cells[row, 6].Text?.Trim();
                var email = worksheet.Cells[row, 7].Text?.Trim();
                var businessType = worksheet.Cells[row, 8].Text?.Trim();
                var revenue = int.TryParse(worksheet.Cells[row, 9].Text, out var rev) ? rev : 0;
                var employees = int.TryParse(worksheet.Cells[row, 10].Text, out var emp) ? emp : 0;
                var ceo = worksheet.Cells[row, 11].Text?.Trim();

                // No need to parse CustomerId they will stay null
                var product = new ProductEntity
                {
                    ProductId = Guid.NewGuid(),
                    CompanyName = companyName!,
                    OrganizationNumber = organizationNumber!,
                    Address = address,
                    PostalCode = postalCode,
                    City = city,
                    PhoneNumber = phoneNumber,
                    Email = email,
                    BusinessType = businessType,
                    Revenue = revenue,
                    NumberOfEmployees = employees,
                    CEO = ceo,
                    CustomerId = null,  // Explicitly set to null
                    ReservedUntil = null, // Ensure null for ReservedUntil as well
                    SoldUntil = null     // Ensure null for SoldUntil as well
                };

                productsToAdd.Add(product);
            }
            catch (Exception ex)
            {
                errors.Add($"Error parsing row {row}: {ex.Message}");
            }
        }

        if (productsToAdd.Count > 0)
        {
            await _productRepository.AddProductsAsync(productsToAdd);
        }
        else
        {
            throw new InvalidOperationException("No valid products were found in the uploaded file.");
        }

        if (errors.Any())
        {
            // Log the errors for debugging
            Console.WriteLine("Errors occurred during import:");
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }
        }
    }
}
