using Microsoft.AspNetCore.Mvc;
using ProductProvider.Interfaces.Services;
using ProductProvider.Models;

namespace ProductProvider.Controllers;

[Route("api/pricesettings")]
[ApiController]
public class PriceSettingsController : ControllerBase
{
    private readonly IPriceSettingsService _priceSettingsService;

    public PriceSettingsController(IPriceSettingsService priceSettingsService)
    {
        _priceSettingsService = priceSettingsService;
    }

    // Get Price Settings (already provided)
    [HttpGet]
    public ActionResult<PriceSettings> GetPriceSettings()
    {
        var settings = _priceSettingsService.GetPriceSettings();
        return Ok(settings);
    }

    // Post new Price Settings (Save in app settings or persistent storage)
    [HttpPost]
    public IActionResult PostPriceSettings([FromBody] PriceSettings newPriceSettings)
    {
        if (newPriceSettings == null)
        {
            return BadRequest("Invalid price settings.");
        }

        // Check if the price settings already exist
        var existingSettings = _priceSettingsService.GetPriceSettings();

        if (existingSettings == null)
        {
            // If the settings don't exist, create them
            _priceSettingsService.SavePriceSettings(newPriceSettings);
            return CreatedAtAction(nameof(GetPriceSettings), new { }, newPriceSettings); // Return 201 status code without Id
        }
        else
        {
            // If the settings exist, update them
            _priceSettingsService.UpdatePriceSettings(newPriceSettings);
            return Ok("Price settings updated successfully.");
        }
    }
}
