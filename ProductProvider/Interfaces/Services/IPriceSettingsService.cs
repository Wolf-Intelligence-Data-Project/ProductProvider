using ProductProvider.Models;

namespace ProductProvider.Interfaces.Services;

public interface IPriceSettingsService
{
    PriceSettings GetPriceSettings();
    void SavePriceSettings(PriceSettings newPriceSettings);
    void UpdatePriceSettings(PriceSettings newPriceSettings);
    void UpdateRuntimeSettings(PriceSettings newPriceSettings);
}
