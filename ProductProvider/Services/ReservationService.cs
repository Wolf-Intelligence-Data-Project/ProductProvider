using ProductProvider.Factories;
using ProductProvider.Interfaces.Repositories;
using ProductProvider.Interfaces.Services;
using ProductProvider.Models;
using ProductProvider.Models.Data.Entities;

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
        var quantity = request.QuantityOfFiltered; // Ensuring it has a default value

        if (quantity > 0)
        {
            // Fetch only product IDs (no sensitive product data is returned)
            var productIds = await _productRepository.GetProductIdsForReservationAsync(request);

            // Reserve products in the repository by their IDs (without exposing product details)
            await _productRepository.ReserveProductsByIdsAsync(productIds, companyId);

            // Create and save reservation using factory
            var reservation = ReservationFactory.CreateReservationEntity(request);
            await _reservationRepository.AddAsync(reservation);

            // Start a timer to release expired reservations after 15 minutes and 3 seconds
            StartReservationReleaseTimer();

            // Create and return the ReservationDto from the reservation entity
            return ReservationFactory.CreateReservationDto(reservation);
        }

        // If quantity is not greater than 0, return null or handle the case as needed
        return null;
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
    private void StartReservationReleaseTimer()
    {
        _timer = new System.Timers.Timer(900000 + 3000); // 15 minutes + 3 seconds
        _timer.Elapsed += async (sender, e) => await TimerElapsedAsync();
        _timer.Start();
    }

    private async Task TimerElapsedAsync()
    {
        await ReleaseExpiredReservationsAsync();
        _timer.Stop();
        _timer.Dispose();
    }

    private async Task ReleaseExpiredReservationsAsync()
    {
        await _productRepository.ReleaseExpiredReservationsAsync();
    }
}
