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
            await _reservationRepository.ReleaseExpiredReservationsAsync();
            await _reservationRepository.DeleteAsync(companyId);

            // Fetch only product IDs (no sensitive product data is returned)
            var productIds = await _productRepository.GetProductIdsForReservationAsync(request);

            // Reserve products in the repository by their IDs (without exposing product details)
            await _productRepository.ReserveProductsByIdsAsync(productIds, companyId);

            // Create and save reservation using factory
            var reservation = ReservationFactory.CreateReservationEntity(request);
            await _reservationRepository.AddAsync(reservation);

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
            await _reservationRepository.DeleteAsync(reservation.ReservationId);
            return true;
        }
        return false;
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
        await _reservationRepository.ReleaseExpiredReservationsAsync();

        // Call the method to delete the reservation
        await DeleteReservationByUserIdAsync(companyId);

        _timer.Stop();
        _timer.Dispose();
    }
}
