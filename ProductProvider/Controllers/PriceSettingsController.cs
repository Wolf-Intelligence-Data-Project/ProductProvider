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

    [HttpGet]
    public ActionResult<PriceSettings> GetPriceSettings()
    {
        var settings = _priceSettingsService.GetPriceSettings();
        return Ok(settings);
    }

    [HttpPost]
    public IActionResult PostPriceSettings([FromBody] PriceSettings newPriceSettings)
    {
        if (newPriceSettings == null)
        {
            return BadRequest("Invalid price settings.");
        }

        var existingSettings = _priceSettingsService.GetPriceSettings();

        if (existingSettings == null)
        {

            _priceSettingsService.SavePriceSettings(newPriceSettings);
            return CreatedAtAction(nameof(GetPriceSettings), new { }, newPriceSettings); // Return 201 status code without Id
        }
        else
        {

            _priceSettingsService.UpdatePriceSettings(newPriceSettings);
            return Ok("Price settings updated successfully.");
        }
    }
}
