using Microsoft.Extensions.Configuration;

namespace PriceSettingsLibrary;

public class PriceSettingsService : IPriceSettingsService
{
    private readonly IConfiguration _configuration;
    private decimal _pricePerProduct;

    public PriceSettingsService(IConfiguration configuration)
    {
        _configuration = configuration;
        _pricePerProduct = decimal.Parse(_configuration["PriceSettings:PricePerProduct"]);
    }

    public decimal GetPricePerProduct()
    {
        return _pricePerProduct;
    }

    public async Task SetPricePerProduct(decimal price)
    {
        _pricePerProduct = price;
        // Optionally save the updated price in a persistent storage (database, file, etc.)
        await Task.CompletedTask;
    }
}