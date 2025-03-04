using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductProvider.Factories;
using ProductProvider.Interfaces.Repositories;
using ProductProvider.Interfaces.Services;
using ProductProvider.Models;
using ProductProvider.Models.Data;

namespace ProductProvider.Services;

public class ReservationService : IReservationService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IReservationRepository _reservationRepository;
    private readonly IProductRepository _productRepository;
    private System.Timers.Timer _timer;
    private readonly ILogger<ReservationService> _logger;

    public ReservationService(IReservationRepository reservationRepository, IProductRepository productRepository, IServiceScopeFactory serviceScopeFactory, ILogger<ReservationService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _reservationRepository = reservationRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ReservationDto> ReserveProductsAsync(ProductDbContext dbContext, ProductReserveRequest request)
    {
        var companyId = request.CompanyId;
        var quantity = request.QuantityOfFiltered;

        if (quantity > 0 && companyId != Guid.Empty)
        {
            // First, remove existing reservations for this company
            await _reservationRepository.UpdateReservationsAsync(companyId);
            await DeleteReservationByUserIdAsync(dbContext, companyId);

            // Fetch only product IDs (no sensitive product data is returned)
            var productIds = await _productRepository.GetProductIdsForReservationAsync(request);

            // Reserve products in the repository by their IDs (without exposing product details)
            await _reservationRepository.ReserveProductsByIdsAsync(productIds, companyId);

            // Create and save reservation using factory
            var reservation = ReservationFactory.CreateReservationEntity(request);
            await _reservationRepository.AddReservationAsync(reservation);

            // Start a timer to release expired reservations after 15 minutes and 3 seconds
            StartReservationReleaseTimer(companyId);

            // Create and return the ReservationDto from the reservation entity
            return ReservationFactory.CreateReservationDto(reservation);
        }

        return null!;
    }

    public async Task<ReservationDto> GetReservationByUserIdAsync(Guid companyId)
    {
        var reservation = await _reservationRepository.GetReservationByUserIdAsync(companyId);
        return reservation != null ? ReservationFactory.CreateReservationDto(reservation) : null;
    }

    public async Task<bool> DeleteReservationByUserIdAsync(ProductDbContext dbContext, Guid companyId)
    {
        var reservation = await _reservationRepository.GetReservationByUserIdAsync(companyId);
        if (reservation != null)
        {
            await _reservationRepository.DeleteReservationAsync(dbContext, reservation.ReservationId); // Pass dbContext to the repository method
            return true;
        }
        return false;
    }

    private async Task ReleaseExpiredReservationsAsync(ProductDbContext dbContext)
    {
        // Get the current Stockholm time directly in the service
        var stockholmTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

        // Calculate the cutoff time (15 minutes and 2 seconds)
        var cutoffTime = stockholmTime.AddMinutes(-15).AddSeconds(-2);

        // Call the repository to update expired reservations, pass dbContext to the repository method
        await _reservationRepository.UpdateExpiredReservationsAsync(dbContext, cutoffTime);
    }


    // Auto Cleanup Reservation Services
    private void StartReservationReleaseTimer(Guid companyId)
    {
        _timer = new System.Timers.Timer(900000 + 3000); // 15 minutes + 3 seconds
        _timer.Elapsed += async (sender, e) => await TimerElapsedAsync(companyId);
        _timer.Start();
        _logger.LogInformation("TIMER STARTED for Company ID: {CompanyId}", companyId);
    }

    private async Task TimerElapsedAsync(Guid companyId)
    {
        try
        {
            _logger.LogInformation("Timer Elapsed for Company ID: {CompanyId}. Executing cleanup.", companyId);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();

                // Execute your async methods here
                await ReleaseExpiredReservationsAsync(dbContext);
                await DeleteReservationByUserIdAsync(dbContext, companyId);
            }

            _logger.LogInformation("RESERVATIONS DELETED for Company ID: {CompanyId}.", companyId);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error during timer elapsed for Company ID: {CompanyId}: {ErrorMessage}", companyId, ex.Message);
        }
        finally
        {
            _logger.LogInformation("Stopping timer for Company ID: {CompanyId}.", companyId);
            _timer.Stop();
        }
    }
}
