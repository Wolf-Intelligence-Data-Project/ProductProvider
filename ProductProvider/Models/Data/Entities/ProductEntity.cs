using Nest;

namespace ProductProvider.Models.Data.Entities;

public class ProductEntity
{
    public int Id { get; set; }  // Primary key for EF Core

    [Text]
    public string CompanyName { get; set; } = null!;

    [Keyword]
    public string OrganizationNumber { get; set; } = null!;

    [Text]
    public string Address { get; set; } = null!;

    [Keyword]
    public string PostalCode { get; set; } = null!;

    [Text]
    public string City { get; set; } = null!;

    [Keyword]
    public string PhoneNumber { get; set; } = null!;

    [Keyword]
    public string Email { get; set; } = null!;

    [Text]
    public string BusinessType { get; set; } = null!;

    [Keyword]
    public string Revenue { get; set; } = null!;

    [Keyword]
    public string NumberOfEmployees { get; set; } = null!;

    [Text]
    public string CEO { get; set; } = null!;

    public DateTime? SoldUntil { get; set; }
    public DateTime? ReservedUntil { get; set; }
}