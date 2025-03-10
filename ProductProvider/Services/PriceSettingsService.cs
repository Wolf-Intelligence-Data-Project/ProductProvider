using Newtonsoft.Json;
using ProductProvider.Interfaces.Services;
using ProductProvider.Models;

namespace ProductProvider.Services;

public class PriceSettingsService : IPriceSettingsService
{
    private readonly PriceSettings _priceSettings;
    private readonly IConfiguration _configuration;
    private readonly string _priceSettingsFilePath = "appsettings.json"; // Path to your appsettings.json file

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
        // Save to persistent storage or update appsettings.json (just an example here)
        var json = File.ReadAllText(_priceSettingsFilePath);
        var jsonObj = JsonConvert.DeserializeObject<dynamic>(json);

        // Check if PriceSettings already exists
        if (jsonObj["PriceSettings"] == null)
        {
            // If the settings don't exist, create a new section for them
            jsonObj["PriceSettings"] = newPriceSettings;
        }

        // Write the updated JSON back to the file
        File.WriteAllText(_priceSettingsFilePath, JsonConvert.SerializeObject(jsonObj, Formatting.Indented));
    }

    public void UpdatePriceSettings(PriceSettings newPriceSettings)
    {
        // Update the existing settings in the file or database
        var json = File.ReadAllText(_priceSettingsFilePath);
        var jsonObj = JsonConvert.DeserializeObject<dynamic>(json);

        // Update the PriceSettings section with new values
        jsonObj["PriceSettings"] = newPriceSettings;

        // Write the updated JSON back to the file
        File.WriteAllText(_priceSettingsFilePath, JsonConvert.SerializeObject(jsonObj, Formatting.Indented));
    }

    public void UpdateRuntimeSettings(PriceSettings newPriceSettings)
    {
        // Update the in-memory settings (for the current runtime session)
        _priceSettings.PricePerProduct = newPriceSettings.PricePerProduct;
        _priceSettings.VatRate = newPriceSettings.VatRate;
    }
}