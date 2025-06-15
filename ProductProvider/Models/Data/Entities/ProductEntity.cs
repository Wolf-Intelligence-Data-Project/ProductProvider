using System.ComponentModel.DataAnnotations;

namespace ProductProvider.Models.Data.Entities;

public class ProductEntity
{
    [Required]
    public Guid ProductId { get; set; } = Guid.NewGuid();

    [Required]
    public string CompanyName { get; set; } = null!;
    public string OrganizationNumber { get; set; } = null!;
    public string Address { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string BusinessType { get; set; }
    public int Revenue { get; set; }
    public int NumberOfEmployees { get; set; }
    public string CEO { get; set; }
    public DateTime? SoldUntil { get; set; }
    public Guid? CustomerId { get; set; } 
    public DateTime? ReservedUntil { get; set; } 
}