namespace PriceSettingsLibrary;

public interface IPriceSettingsService
{
    decimal GetPricePerProduct();
    Task SetPricePerProduct(decimal price);
}