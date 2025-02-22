using ProductProvider.Models.Data.Entities;

namespace ProductProvider.Models
{
    public class ProductFilterResponse
    {
        public int AvailableQuantity { get; set; }  // Available quantity of products matching the filter
        public List<ProductEntity> Products { get; set; }  // List of filtered products (up to requested quantity)
    }
}