using ProductProvider.Factories;
using ProductProvider.Interfaces.Repositories;
using ProductProvider.Interfaces.Services;
using ProductProvider.Models;

namespace ProductProvider.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IProductRepository _productRepository;
    private System.Timers.Timer _timer;
    private readonly ILogger<ReservationService> _logger;

    public ReservationService(IReservationRepository reservationRepository, IProductRepository productRepository, ILogger<ReservationService> logger)
    {
        _reservationRepository = reservationRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ReservationDto> ReserveProductsAsync(ProductReserveRequest request)
    {
        var companyId = request.CompanyId;
        var quantity = request.QuantityOfFiltered;

        if (quantity > 0 && companyId != Guid.Empty)
        {
            await DeleteReservationNow(companyId);

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

    private async Task<bool> DeleteReservationByUserIdAsync(Guid companyId)
    {
        var reservation = await _reservationRepository.GetReservationByUserIdAsync(companyId);
        if (reservation != null)
        {
            await _reservationRepository.DeleteReservationImmediatelyAsync(reservation.ReservationId);
            return true;
        }
        return false;
    }


    public async Task<bool> DeleteReservationNow(Guid companyId)
    {
        try
        {
            await _reservationRepository.UpdateReservationsAsync(companyId);
            await DeleteReservationByUserIdAsync(companyId);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task ReleaseExpiredReservationsAsync()
    {
        // Get the current Stockholm time directly in the service
        var stockholmTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

        // Calculate the cutoff time (15 minutes and 2 seconds)
        var cutoffTime = stockholmTime.AddMinutes(-15).AddSeconds(-2);

        // Call the repository to update expired reservations (deletes expired reservations from both tables)
        await _reservationRepository.DeleteExpiredReservationsAsync(cutoffTime);
    }
    // Auto Cleanup Reservation Services
    private void StartReservationReleaseTimer(Guid companyId)
    {
        _timer?.Dispose();  // Dispose old timer if exists
        _timer = new System.Timers.Timer(900000 + 3000); // 15 minutes + 3 seconds
        _timer.Elapsed += async (sender, e) => await TimerElapsedAsync(companyId);
        _timer.AutoReset = false; // Ensure it runs only once
        _timer.Start();
        _logger.LogInformation("TIMER STARTED for Company ID: {CompanyId}", companyId);
    }

    private async Task TimerElapsedAsync(Guid companyId)
    {
        try
        {
            _logger.LogInformation("Timer Elapsed for Company ID: {CompanyId}. Executing cleanup.", companyId);

            await ReleaseExpiredReservationsAsync();

            _logger.LogInformation("RESERVATIONS DELETED for Company ID: {CompanyId}.", companyId);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error during timer elapsed for Company ID: {CompanyId}: {ErrorMessage}", companyId, ex.Message);
        }
        finally
        {
            _logger.LogInformation("Stopping timer for Company ID: {CompanyId}.", companyId);
            _timer?.Stop();
        }
    }
}
