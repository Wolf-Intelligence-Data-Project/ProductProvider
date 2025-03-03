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

    public ReservationService(IReservationRepository reservationRepository, IProductRepository productRepository)
    {
        _reservationRepository = reservationRepository;
        _productRepository = productRepository;
    }

    public async Task<ReservationDto> ReserveProductsAsync(ProductReserveRequest request)
    {
        var companyId = request.CompanyId;
        var quantity = request.QuantityOfFiltered;

        if (quantity > 0 && companyId != Guid.Empty)
        {
            // First, remove existing reservations for this company
            await _reservationRepository.UpdateReservationsAsync(companyId);
            await DeleteReservationByUserIdAsync(companyId);

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

    public async Task<bool> DeleteReservationByUserIdAsync(Guid companyId)
    {
        var reservation = await _reservationRepository.GetReservationByUserIdAsync(companyId);
        if (reservation != null)
        {
            await _reservationRepository.DeleteReservationAsync(reservation.ReservationId);
            return true;
        }
        return false;
    }

    private async Task ReleaseExpiredReservationsAsync()
    {
        // Get the current Stockholm time directly in the service
        var stockholmTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

        // Calculate the cutoff time (15 minutes and 2 seconds)
        var cutoffTime = stockholmTime.AddMinutes(-15).AddSeconds(-2);

        // Call the repository to update expired reservations
        await _reservationRepository.UpdateExpiredReservationsAsync(cutoffTime);
    }

    // Auto Cleanup Reservation Services
    private void StartReservationReleaseTimer(Guid companyId)
    {
        _timer = new System.Timers.Timer(900000 + 3000); // 15 minutes + 3 seconds
        _timer.Elapsed += async (sender, e) => await TimerElapsedAsync(companyId);
        _timer.Start();
    }

    private async Task TimerElapsedAsync(Guid companyId)
    {
        try
        {
            await ReleaseExpiredReservationsAsync();

            await DeleteReservationByUserIdAsync(companyId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during timer elapsed: {ex.Message}");
        }
        finally
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }
}
