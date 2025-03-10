using Microsoft.AspNetCore.Mvc;
using ProductProvider.Interfaces.Services;

namespace ProductProvider.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BusinessTypeController : ControllerBase
{
    private readonly IBusinessTypeService _businessTypeService;

    public BusinessTypeController(IBusinessTypeService businessTypeService)
    {
        _businessTypeService = businessTypeService;
    }

    // Endpoint to get available business types and their descriptions
    [HttpGet("get-business-types")]
    public async Task<IActionResult> GetBusinessTypes()
    {
        var types = await _businessTypeService.GetAvailableBusinessTypes();
        return Ok(types);
    }

    [HttpGet("available-cities")]
    public async Task<IActionResult> GetCities()
    {
        var types = await _businessTypeService.GetAvailableCities();
        return Ok(types);
    }
}
