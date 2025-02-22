namespace ProductProvider.Models
{
    public class ProductFilterRequest
    {
        public string CompanyName { get; set; }
        public string BusinessType { get; set; }
        public decimal? MinRevenue { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string CEO { get; set; }
    }
}
