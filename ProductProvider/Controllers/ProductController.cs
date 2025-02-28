using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using ProductProvider.Interfaces;
using ProductProvider.Models;
using System.Text.Json;

namespace ProductProvider.Controllers
{
    [Authorize]
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;  // Add logger field

        // Inject the logger into the constructor
        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;  // Assign the injected logger to the field
        }
        [HttpPost("filter")]
        public async Task<ActionResult<ProductFilterResponse>> GetFilteredProducts([FromBody] ProductFilterRequest request)
        {
            _logger.LogInformation("Received filter request: {Filters}", JsonSerializer.Serialize(request));

            if (request == null)
            {
                _logger.LogWarning("Filters are null.");
                return BadRequest("Invalid filters.");
            }

            // Call the service method and get the response
            var response = await _productService.GetProductCountAsync(request);

            return Ok(response);  // Return the response with available quantity and total price
        }


        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveProducts([FromBody] ProductFilterRequest request, [FromQuery] Guid userId)
        {
            await _productService.ReserveProductsAsync(request, userId);
            return Ok("Products reserved successfully.");
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportProducts([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            await _productService.ImportProductsFromExcelAsync(file);
            return Ok("Products imported successfully.");
        }

    }
}
