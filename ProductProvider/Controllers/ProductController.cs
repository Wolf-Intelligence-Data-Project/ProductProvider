using Microsoft.AspNetCore.Mvc;
using ProductProvider.Interfaces.Services;
using ProductProvider.Models;

namespace ProductProvider.Controllers;

[Route("api/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
    }
    [HttpPost("filter")]
    public async Task<ActionResult<ProductFilterResponse>> GetFilteredProducts([FromBody] ProductFilterRequest request)
    {
        if (request == null)
        {
            _logger.LogWarning("Filters are null.");
            return BadRequest("Invalid filters.");
        }

        var response = await _productService.GetProductCountAsync(request);

        return Ok(response);
    }
}
