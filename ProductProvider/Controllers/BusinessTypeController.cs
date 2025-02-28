using Microsoft.AspNetCore.Mvc;
using ProductProvider.Models.SNI_codes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ProductProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessTypeController : ControllerBase
    {
        // Endpoint to get Business Categories (e.g., "A", "B", etc.)
        [HttpGet("business-categories")]
        public IActionResult GetBusinessCategories()
        {
            var categories = GetCategoriesWithDescription();
            return Ok(categories);
        }

        // Helper method to get Categories with Descriptions
        public static List<KeyValuePair<int, string>> GetCategoriesWithDescription()
        {
            return Enum.GetValues(typeof(BusinessCategory))
                       .Cast<BusinessCategory>()
                       .Select(e => new KeyValuePair<int, string>((int)e, GetEnumDescription(e)))
                       .ToList();
        }

        // Helper method to get Description from Enum
        private static string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute?.Description ?? value.ToString();
        }

        // Endpoint to get Business Types (e.g., "01", "02", etc.)
        [HttpGet("get-business-types")]
        public IActionResult GetBusinessTypes()
        {
            var types = Enum.GetValues(typeof(BusinessType))
                .Cast<BusinessType>()
                .Select(t => new
                {
                    Value = t.ToString(),        // Enum value (e.g., "01", "02")
                    Description = GetEnumDescription(t)  // Description for human readability
                })
                .ToList();

            return Ok(types);
        }
    }
}
