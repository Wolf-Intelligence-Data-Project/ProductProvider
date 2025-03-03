namespace ProductProvider.Models;

public class ReservationDto
{
    public string? BusinessTypes { get; set; }
    public string? Regions { get; set; }
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