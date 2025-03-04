namespace ProductProvider.Models;

public class ReservationDto
{
    public Guid UserId { get; set; }
    public string? BusinessTypes { get; set; }
    public string? Regions { get; set; }
    public string? CitiesByRegion { get; set; }  // I needed this field because of the service complexity (product data does not include region field)
    public string? Cities { get; set; }
    public string? PostalCodes { get; set; }
    public int? MinRevenue { get; set; }
    public int? MaxRevenue { get; set; }
    public int? MinNumberOfEmployees { get; set; }
    public int? MaxNumberOfEmployees { get; set; }
    public int Quantity { get; set; }
    public DateTime? ReservedTime { get; set; }
    public DateTime? SoldTime { get; set; }
}