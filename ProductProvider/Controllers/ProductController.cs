using Microsoft.AspNetCore.Mvc;
using ProductProvider.Interfaces;

namespace ProductProvider.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("filtered")]
    public async Task<IActionResult> GetFilteredProducts(
        [FromQuery] string? search, [FromQuery] string? businessType, [FromQuery] string? address,
        [FromQuery] string? postalCode, [FromQuery] string? city, [FromQuery] string? phoneNumber,
        [FromQuery] string? email, [FromQuery] string? revenue, [FromQuery] string? numberOfEmployees,
        [FromQuery] string? ceo)
    {
        var products = await _productService.GetFilteredProductsAsync(
            search, businessType, address, postalCode, city, phoneNumber, email, revenue,
            numberOfEmployees, ceo);

        return Ok(products);
    }
}