using ProductProvider.Models.Data.Entities;

namespace ProductProvider.Interfaces.Repositories
{
    public interface IReservationRepository
    {
        Task AddAsync(ReservationEntity reservation);
        Task<ReservationEntity> GetReservationByUserIdAsync(Guid companyId);
        Task DeleteAsync(Guid companyId);

        // These handle ReservedUntil and ReservedBy in the Products table
        Task ReleaseExpiredReservationsAsync();
    }
}
