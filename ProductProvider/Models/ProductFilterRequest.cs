using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProductProvider.Models
{
    public class ProductFilterRequest
    {
        [JsonPropertyName("businessTypes")]
        public List<string>? BusinessTypes { get; set; }

        [JsonPropertyName("cities")]
        public List<string>? Cities { get; set; }

        [JsonPropertyName("postalCodes")]
        public List<string>? PostalCodes { get; set; }

        [JsonPropertyName("minRevenue")]
        public int? MinRevenue { get; set; }

        [JsonPropertyName("maxRevenue")]
        public int? MaxRevenue { get; set; }

        [JsonPropertyName("minNumberOfEmployees")]
        public int? MinNumberOfEmployees { get; set; }

        [JsonPropertyName("maxNumberOfEmployees")]
        public int? MaxNumberOfEmployees { get; set; }

        [JsonPropertyName("quantity")]
        [Required]
        public int QuantityOfFiltered { get; set; }
    }
}


//using System.Text.Json.Serialization;

//namespace ProductProvider.Models;

//public class ProductFilterRequest
//{
//    [JsonPropertyName("businessTypes")]
//    public List<string>? BusinessTypes { get; set; }

//    [JsonPropertyName("businessTypesQuantities")]
//    public Dictionary<string, int>? BusinessTypesQuantities { get; set; } // Quantity for each business type

//    [JsonPropertyName("cities")]
//    public List<string>? Cities { get; set; }

//    [JsonPropertyName("citiesQuantities")]
//    public Dictionary<string, int>? CitiesQuantities { get; set; } // Quantity for each city

//    [JsonPropertyName("postalCodes")]
//    public List<string>? PostalCodes { get; set; }

//    [JsonPropertyName("postalCodesQuantities")]
//    public Dictionary<string, int>? PostalCodesQuantities { get; set; } // Quantity for each postal code

//    [JsonPropertyName("minRevenue")]
//    public int? MinRevenue { get; set; }

//    [JsonPropertyName("maxRevenue")]
//    public int? MaxRevenue { get; set; }

//    [JsonPropertyName("minNumberOfEmployees")]
//    public int? MinNumberOfEmployees { get; set; }

//    [JsonPropertyName("maxNumberOfEmployees")]
//    public int? MaxNumberOfEmployees { get; set; }
//}