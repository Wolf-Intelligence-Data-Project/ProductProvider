namespace ProductProvider.Factories
{
    using ProductProvider.Models;
    using ProductProvider.Models.Data.Entities;

    public class ReservationFactory
    {
        public static ReservationEntity CreateReservationEntity(ProductReserveRequest request)
        {
            return new ReservationEntity
            {
                ReservationId = Guid.NewGuid(),
                UserId = request.CompanyId,
                BusinessTypes = string.Join(",", request.BusinessTypes ?? new List<string>()),
                Regions = string.Join(",", request.Regions ?? new List<string>()),
                Cities = string.Join(",", request.Cities ?? new List<string>()),
                PostalCodes = string.Join(",", request.PostalCodes ?? new List<string>()),
                MinRevenue = request.MinRevenue,
                MaxRevenue = request.MaxRevenue,
                MinNumberOfEmployees = request.MinNumberOfEmployees,
                MaxNumberOfEmployees = request.MaxNumberOfEmployees,
                Quantity = request.QuantityOfFiltered,
                ReservedTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time")),
                SoldTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time")).AddMinutes(15)

            };
        }

        public static ReservationDto CreateReservationDto(ReservationEntity reservation)
        {
            return new ReservationDto
            {
                UserId = reservation.UserId,
                BusinessTypes = reservation.BusinessTypes,
                Regions = reservation.Regions,
                Cities = reservation.Cities,
                PostalCodes = reservation.PostalCodes,
                MinRevenue = reservation.MinRevenue,
                MaxRevenue = reservation.MaxRevenue,
                MinNumberOfEmployees = reservation.MinNumberOfEmployees,
                MaxNumberOfEmployees = reservation.MaxNumberOfEmployees,
                Quantity = reservation.Quantity,
                ReservedTime = reservation.ReservedTime,
                SoldTime = reservation.SoldTime
            };
        }
    }
}
