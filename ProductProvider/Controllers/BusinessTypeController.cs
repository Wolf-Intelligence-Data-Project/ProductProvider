using Microsoft.AspNetCore.Mvc;
using ProductProvider.Interfaces.Services;

namespace ProductProvider.Controllers;

// Controller for delivering right businesstypes according to SNI codes (custom made enum)

[Route("api/[controller]")]
[ApiController]
public class BusinessTypeController : ControllerBase
{
    private readonly IBusinessTypeService _businessTypeService;
    private readonly ILogger<BusinessTypeController> _logger;
    public BusinessTypeController(IBusinessTypeService businessTypeService, ILogger<BusinessTypeController> logger)
    {
        _businessTypeService = businessTypeService;
        _logger = logger;
    }

    [HttpGet("get-business-types")]
    public async Task<IActionResult> GetBusinessTypes()
    {
        _logger.LogInformation("Received request to get business types.");

        try
        {
            var types = await _businessTypeService.GetAvailableBusinessTypes();

            if (types == null || !types.Any())
            {
                _logger.LogWarning("No business types found in the database.");
                return NotFound("No business types found.");
            }

            _logger.LogInformation("Successfully fetched business types.");
            return Ok(types);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching business types: {ex.Message}");
            return StatusCode(500, "Internal server error while fetching business types.");
        }
    }

    [HttpGet("available-cities")]
    public async Task<IActionResult> GetCities()
    {
        var types = await _businessTypeService.GetAvailableCities();
        return Ok(types);
    }
}
