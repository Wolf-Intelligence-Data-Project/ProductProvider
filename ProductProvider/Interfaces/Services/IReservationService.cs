using ProductProvider.Models;
using ProductProvider.Models.Data;

namespace ProductProvider.Services.Services;

public interface IReservationService
{
    Task<ReservationDto> ReserveProductsAsync(ProductReserveRequest request);
    Task<ReservationDto> GetReservationByUserIdAsync(Guid companyId);
    Task<bool> DeleteReservationNow(Guid companyId);
}
