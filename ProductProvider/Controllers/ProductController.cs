using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Newtonsoft.Json;
using ProductProvider.Interfaces;
using ProductProvider.Interfaces.Services;
using ProductProvider.Models;
using ProductProvider.Services;
using System.Text.Json;
using System.Xml;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ProductProvider.Controllers
{
    [Authorize]
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;  // Add logger field

        // Inject the logger into the constructor
        public ProductController(IProductService productService, ILogger<ProductController> logger, IReservationService reservationService)
        {
            _productService = productService;
            _logger = logger;  // Assign the injected logger to the field*
            _reservationService = reservationService;
        }
        [HttpPost("filter")]
        public async Task<ActionResult<ProductFilterResponse>> GetFilteredProducts([FromBody] ProductFilterRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("Filters are null.");
                return BadRequest("Invalid filters.");
            }

            // Call the service method to get the response
            var response = await _productService.GetProductCountAsync(request);

            // Return the response with the available quantity and total price
            return Ok(response);  // The response is of type ProductFilterResponse
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveProducts([FromBody] ProductReserveRequest request)
        {
            // Log the raw incoming request (request body) before any processing or conversion
            _logger.LogInformation("Raw request body: {RawRequest}", JsonConvert.SerializeObject(request));

            if (request == null)
            {
                _logger.LogWarning("Request body is null.");
                return BadRequest(new { message = "Invalid request." });  // Return a JSON response
            }

            if (request.QuantityOfFiltered == null)
            {
                _logger.LogWarning("QuantityOfFiltered is null.");
            }
            else if (request.QuantityOfFiltered == 0)
            {
                _logger.LogWarning("QuantityOfFiltered is 0.");
            }
            else
            {
                _logger.LogInformation("Valid QuantityOfFiltered: {Quantity}", request.QuantityOfFiltered);
            }

            // Log the entire request object after any null checks or required modifications
            _logger.LogInformation("Received reservation request: {Request}", JsonConvert.SerializeObject(request));

            // Check and handle null or empty fields
            if (request.BusinessTypes == null || request.BusinessTypes.Count == 0)
            {
                _logger.LogInformation("No BusinessTypes provided, defaulting to empty array.");
                request.BusinessTypes = new List<string>(); // or handle as needed
            }

            if (request.Cities == null || request.Cities.Count == 0)
            {
                _logger.LogInformation("No Cities provided, defaulting to empty array.");
                request.Cities = new List<string>(); // or handle as needed
            }

            // Process reservation and get the full ReservationDto
            var reservation = await _reservationService.ReserveProductsAsync(request);

            if (reservation == null)
            {
                _logger.LogWarning("Reservation failed or returned null.");
                return BadRequest(new { message = "Reservation failed." });
            }

            // Log reserved details
            _logger.LogInformation("Reservation successful: {@Reservation}", reservation);

            // Return a JSON response with the full reservation details
            return Ok(new
            {
                message = "Products reserved successfully",
                reservation.Quantity
            });
        }

        [HttpGet("get-reservation")]
        public async Task<IActionResult> GetReservation(Guid userId)
        {
            var reservation = await _reservationService.GetReservationByUserIdAsync(userId);

            if (reservation == null)
            {
                _logger.LogWarning("No reservation found for user ID: {UserId}", userId);
                return NotFound(new { message = "No reservation found." });
            }

            return Ok(new { reservation });
        }

        [HttpDelete("delete-reservation")]
        public async Task<IActionResult> DeleteReservation(Guid userId)
        {
            var isDeleted = await _reservationService.DeleteReservationByUserIdAsync(userId);

            if (!isDeleted)
            {
                _logger.LogWarning("No reservation found to delete for user ID: {UserId}", userId);
                return NotFound(new { message = "Reservation not found or could not be deleted." });
            }

            return Ok(new { message = "Reservation deleted successfully." });
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
