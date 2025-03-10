//using Microsoft.Extensions.Options;
//using ProductProvider.Factories;
//using ProductProvider.Services.Repositories;
//using ProductProvider.Services.Services;
//using ProductProvider.Models;
//using ProductProvider.Models.Data;

//namespace ProductProvider.Services;

//public class ReservationService : IReservationService
//{
//    private readonly IReservationRepository _reservationRepository;
//    private readonly IProductRepository _productRepository;
//    private System.Timers.Timer _timer;
//    private readonly IServiceScopeFactory _serviceScopeFactory;
//    private readonly ILogger<ReservationService> _logger;
//    private readonly IOptions<PriceSettings> _priceSettings;

//    public ReservationService(IReservationRepository reservationRepository, IProductRepository productRepository, IServiceScopeFactory serviceScopeFactory, IOptions<PriceSettings> priceSettings, ILogger<ReservationService> logger)
//    {
//        _reservationRepository = reservationRepository;
//        _productRepository = productRepository;
//        _serviceScopeFactory = serviceScopeFactory;
//        _priceSettings = priceSettings;
//        _logger = logger;
//    }

//    public async Task<ReservationDto> ReserveProductsAsync(ProductReserveRequest request)
//    {
//        var companyId = request.CompanyId;
//        var quantity = request.QuantityOfFiltered;

//        if (quantity > 0 && companyId != Guid.Empty)
//        {
//            await DeleteReservationNow(companyId);

//            // Fetch only product IDs (no sensitive product data is returned)
//            var productIds = await _productRepository.GetProductIdsForReservationAsync(request);

//            // Reserve products in the repository by their IDs (without exposing product details)
//            await _reservationRepository.ReserveProductsByIdsAsync(productIds, companyId);

//            // Create and save reservation using factory
//            var reservation = ReservationFactory.CreateReservationEntity(request);
//            await _reservationRepository.AddReservationAsync(reservation);

//            // Start a timer to release expired reservations after 15 minutes and 3 seconds
//            StartReservationReleaseTimer(companyId);

//            // Create and return the ReservationDto from the reservation entity
//            return ReservationFactory.CreateReservationDto(reservation);
//        }

//        return null!;
//    }
//    private (decimal priceWithoutVat, decimal totalPrice) CalculatePrice(int count)
//    {
//        // Get price and VAT rate from configuration
//        decimal pricePerProduct = _priceSettings.Value.PricePerProduct; // SEK per product
//        decimal vatRate = _priceSettings.Value.VatRate; // VAT rate (e.g., 25 for 25%)

//        // Ensure price settings are being loaded
//        if (pricePerProduct <= 0 || vatRate <= 0)
//        {
//            _logger.LogError("Invalid price settings: PricePerProduct = {PricePerProduct}, VatRate = {VatRate}",
//                pricePerProduct, vatRate);
//            throw new InvalidOperationException("Invalid price settings.");
//        }

//        // Calculate the total price before VAT
//        decimal baseTotalPrice = count * pricePerProduct;

//        // Calculate the total price after VAT
//        decimal totalPriceWithVat = baseTotalPrice * (1 + vatRate / 100); // Apply VAT to total price

//        return (baseTotalPrice, totalPriceWithVat);
//    }

//    public async Task<ReservationDto> GetReservationByUserIdAsync(Guid companyId)
//    {
//        var reservation = await _reservationRepository.GetReservationByUserIdAsync(companyId);

//        if (reservation == null)
//        {
//            return null;
//        }

//        // Calculate prices based on reservation quantity
//        var (priceWithoutVat, totalPriceWithVat) = CalculatePrice(reservation.Quantity);

//        // Create ReservationDto and include calculated prices
//        var reservationDto = ReservationFactory.CreateReservationDto(reservation);

//        // Add the calculated prices to the ReservationDto
//        reservationDto.PriceWithoutVat = priceWithoutVat;
//        reservationDto.TotalPrice = totalPriceWithVat;

//        return reservationDto;

//    }


//    private async Task<bool> DeleteReservationByUserIdAsync(Guid companyId)
//    {
//        var reservation = await _reservationRepository.GetReservationByUserIdAsync(companyId);
//        if (reservation != null)
//        {
//            await _reservationRepository.DeleteReservationImmediatelyAsync(reservation.ReservationId);
//            return true;
//        }
//        return false;
//    }


//    public async Task<bool> DeleteReservationNow(Guid companyId)
//    {
//        try
//        {
//            await _reservationRepository.UpdateReservationsAsync(companyId);
//            await DeleteReservationByUserIdAsync(companyId);
//            return true;
//        }
//        catch
//        {
//            return false;
//        }
//    }

//    //private async Task ReleaseExpiredReservationsAsync(ProductDbContext context)
//    //{
//    //    var stockholmTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central European Time"));
//    //    var cutoffTime = stockholmTime.AddMinutes(-15).AddSeconds(-2);

//    //    // Now using the passed context
//    //    await _reservationRepository.DeleteExpiredReservationsAsync(context, cutoffTime);
//    //}
//    // Auto Cleanup Reservation Services
//    private void StartReservationReleaseTimer(Guid companyId)
//    {
//        _timer?.Dispose();  // Dispose old timer if exists
//        _timer = new System.Timers.Timer(900000 + 3000); // 15 minutes + 3 seconds
//        _timer.Elapsed += async (sender, e) => await TimerElapsedAsync(companyId);
//        _timer.AutoReset = false; // Ensure it runs only once
//        _timer.Start();
//        _logger.LogInformation("TIMER STARTED for Company ID: {CompanyId}", companyId);
//    }

//    private async Task TimerElapsedAsync(Guid companyId)
//    {
//        try
//        {
//            _logger.LogInformation("Timer Elapsed for Company ID: {CompanyId}. Executing cleanup.", companyId);

//            // Create a new scope for this operation to keep the context alive
//            using (var scope = _serviceScopeFactory.CreateScope())
//            {
//                var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
//                var cutoffTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central European Time")).AddMinutes(-15.2);

//                // Pass the context to the repository method
//                await _reservationRepository.DeleteExpiredReservationsAsync(context, cutoffTime, companyId);
//            }

//            _logger.LogInformation("RESERVATIONS DELETED for Company ID: {CompanyId}.", companyId);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError("Error during timer elapsed for Company ID: {CompanyId}: {ErrorMessage}", companyId, ex.Message);
//        }
//        finally
//        {
//            _logger.LogInformation("Stopping timer for Company ID: {CompanyId}.", companyId);
//            _timer?.Stop();
//        }
//    }

//}
