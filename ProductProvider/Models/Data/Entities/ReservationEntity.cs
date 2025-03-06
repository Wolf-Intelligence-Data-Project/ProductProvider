namespace ProductProvider.Models.Data.Entities;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class ReservationEntity
{
    [Required]
    public Guid ReservationId { get; set; }  // Primary key for this table

    [Required]
    public Guid CustomerId { get; set; }
    public string? BusinessTypes { get; set; }
    public string? Regions { get; set; }
    public string? CitiesByRegion { get; set; }
    public string? Cities { get; set; }
    public string? PostalCodes { get; set; }
    public int? MinRevenue { get; set; }
    public int? MaxRevenue { get; set; }
    public int? MinNumberOfEmployees { get; set; }
    public int? MaxNumberOfEmployees { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public DateTime? ReservedFrom { get; set; }

    public DateTime? SoldFrom { get; set; }
}
