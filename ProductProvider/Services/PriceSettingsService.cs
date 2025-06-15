using Newtonsoft.Json;
using ProductProvider.Interfaces.Services;
using ProductProvider.Models;

namespace ProductProvider.Services;

public class PriceSettingsService : IPriceSettingsService
{
    private readonly PriceSettings _priceSettings;
    private readonly IConfiguration _configuration;
    private readonly string _priceSettingsFilePath = "appsettings.json";

    public PriceSettingsService(IConfiguration configuration)
    {
        _configuration = configuration;
        _priceSettings = new PriceSettings();
        configuration.GetSection("PriceSettings").Bind(_priceSettings);
    }

    public PriceSettings GetPriceSettings()
    {
        return _priceSettings;
    }

    public void SavePriceSettings(PriceSettings newPriceSettings)
    {
        var json = File.ReadAllText(_priceSettingsFilePath);
        var jsonObj = JsonConvert.DeserializeObject<dynamic>(json);

        if (jsonObj["PriceSettings"] == null)
        {
            jsonObj["PriceSettings"] = newPriceSettings;
        }

        File.WriteAllText(_priceSettingsFilePath, JsonConvert.SerializeObject(jsonObj, Formatting.Indented));
    }

    public void UpdatePriceSettings(PriceSettings newPriceSettings)
    {
        var json = File.ReadAllText(_priceSettingsFilePath);
        var jsonObj = JsonConvert.DeserializeObject<dynamic>(json);

        jsonObj["PriceSettings"] = newPriceSettings;

        File.WriteAllText(_priceSettingsFilePath, JsonConvert.SerializeObject(jsonObj, Formatting.Indented));
    }

    public void UpdateRuntimeSettings(PriceSettings newPriceSettings)
    {
        _priceSettings.PricePerProduct = newPriceSettings.PricePerProduct;
        _priceSettings.VatRate = newPriceSettings.VatRate;
    }
}