namespace ProductProvider.Factories;

using ProductProvider.Models;
using ProductProvider.Models.Data.Entities;

public class ReservationFactory
{
    public static ReservationEntity CreateReservationEntity(ProductReserveRequest request)
    {
        return new ReservationEntity
        {
            ReservationId = Guid.NewGuid(),
            CustomerId = request.CompanyId,
            BusinessTypes = string.Join(",", request.BusinessTypes ?? new List<string>()),
            Regions = string.Join(",", request.Regions ?? new List<string>()),
            Cities = request.Cities != null ? string.Join(",", request.Cities) : null,  // Separate Cities
            CitiesByRegion = request.CitiesByRegion != null ? string.Join(",", request.CitiesByRegion) : null, // Separate CitiesByRegion
            PostalCodes = string.Join(",", request.PostalCodes ?? new List<string>()),
            MinRevenue = request.MinRevenue,
            MaxRevenue = request.MaxRevenue,
            MinNumberOfEmployees = request.MinNumberOfEmployees,
            MaxNumberOfEmployees = request.MaxNumberOfEmployees,
            Quantity = request.QuantityOfFiltered,
            ReservedFrom = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central European Time")),
            SoldFrom = null,
        };
    }

    public static ReservationDto CreateReservationDto(ReservationEntity reservation)
    {
        return new ReservationDto
        {
            CustomerId = reservation.CustomerId,
            BusinessTypes = reservation.BusinessTypes,
            Regions = reservation.Regions,
            Cities = reservation.Cities,
            CitiesByRegion = reservation.CitiesByRegion, // Include CitiesByRegion separately
            PostalCodes = reservation.PostalCodes,
            MinRevenue = reservation.MinRevenue,
            MaxRevenue = reservation.MaxRevenue,
            MinNumberOfEmployees = reservation.MinNumberOfEmployees,
            MaxNumberOfEmployees = reservation.MaxNumberOfEmployees,
            Quantity = reservation.Quantity,
            ReservedFrom = reservation.ReservedFrom,
            SoldFrom = null,
        };
    }
}
