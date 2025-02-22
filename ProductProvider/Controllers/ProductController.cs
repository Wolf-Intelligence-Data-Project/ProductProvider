using Microsoft.AspNetCore.Mvc;
using ProductProvider.Interfaces;
using ProductProvider.Models;
using ProductProvider.Models.Data.Entities;

namespace ProductProvider.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("filter")]
        public async Task<ActionResult<ProductFilterResponse>> GetFilteredProducts([FromBody] ProductFilterRequest request, [FromQuery] int quantity)
        {
            var filteredProducts = await _productService.GetFilteredProductsAsync(request, quantity);
            var availableQuantity = filteredProducts.Count;

            if (availableQuantity < quantity)
            {
                quantity = availableQuantity;
            }

            var response = new ProductFilterResponse
            {
                AvailableQuantity = availableQuantity,
                Products = filteredProducts.Take(quantity).ToList()
            };

            return Ok(response);
        }



        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveProducts([FromBody] ProductFilterRequest request, [FromQuery] int quantity, [FromHeader] Guid userId)
        {
            await _productService.ReserveProductsAsync(request, quantity, userId);
            return Ok("Products reserved successfully.");
        }

        [HttpGet("available-quantity")]
        public async Task<ActionResult<int>> GetAvailableProductsQuantity()
        {
            var availableProductsQuantity = await _productService.GetAvailableProductsQuantityAsync();
            return Ok(availableProductsQuantity);
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
