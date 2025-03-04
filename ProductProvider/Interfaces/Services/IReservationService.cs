using ProductProvider.Models;
using ProductProvider.Models.Data;

namespace ProductProvider.Interfaces.Services;

public interface IReservationService
{
    Task<ReservationDto> ReserveProductsAsync(ProductDbContext dbContext, ProductReserveRequest request);
    Task<ReservationDto> GetReservationByUserIdAsync(Guid companyId);
    Task<bool> DeleteReservationByUserIdAsync(ProductDbContext dbContext, Guid companyId);
}
