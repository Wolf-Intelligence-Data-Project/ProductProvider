using ProductProvider.Models.Data;
using ProductProvider.Models.Data.Entities;

namespace ProductProvider.Services.Repositories;

public interface IReservationRepository
{
    Task AddReservationAsync(ReservationEntity reservation);
    Task<ReservationEntity> GetReservationByUserIdAsync(Guid companyId);
    Task ReserveProductsByIdsAsync(List<Guid> productIds, Guid companyId);
    Task DeleteExpiredReservationsAsync(ProductDbContext context, DateTime cutoffTime, Guid companyId);
    Task DeleteReservationImmediatelyAsync(Guid reservationId);
    Task UpdateReservationsAsync(Guid companyId);
}
