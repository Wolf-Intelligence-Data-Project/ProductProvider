using ProductProvider.Models.Data;
using ProductProvider.Models.Data.Entities;

namespace ProductProvider.Interfaces.Repositories;

public interface IReservationRepository
{
    Task AddReservationAsync(ReservationEntity reservation);
    Task<ReservationEntity> GetReservationByUserIdAsync(Guid companyId);
    Task ReserveProductsByIdsAsync(List<Guid> productIds, Guid companyId);
    Task DeleteReservationAsync(ProductDbContext dbContext, Guid companyId);
    Task UpdateExpiredReservationsAsync(ProductDbContext dbContext, DateTime cutoffTime);
    Task UpdateReservationsAsync(Guid companyId);
}
