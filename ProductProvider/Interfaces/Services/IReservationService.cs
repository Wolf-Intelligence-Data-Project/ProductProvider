using System;
using System.Threading.Tasks;
using ProductProvider.Models;

namespace ProductProvider.Interfaces.Services
{
    public interface IReservationService
    {
        Task<ReservationDto> ReserveProductsAsync(ProductReserveRequest request);
        Task<ReservationDto> GetReservationByUserIdAsync(Guid companyId);
        Task<bool> DeleteReservationByUserIdAsync(Guid companyId);
    }
}
