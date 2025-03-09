using Microsoft.AspNetCore.Mvc;
using ProductProvider.Services.Services;

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

    //// Endpoint to get Business Categories (e.g., "A", "B", etc.)
    //[HttpGet("business-categories")]
    //public IActionResult GetBusinessCategories()
    //{
    //    var categories = GetCategoriesWithDescription();
    //    return Ok(categories);
    //}

    //// Helper method to get Categories with Descriptions
    //public static List<KeyValuePair<int, string>> GetCategoriesWithDescription()
    //{
    //    return Enum.GetValues(typeof(BusinessCategory))
    //               .Cast<BusinessCategory>()
    //               .Select(e => new KeyValuePair<int, string>((int)e, GetEnumDescription(e)))
    //               .ToList();
    //}
}
