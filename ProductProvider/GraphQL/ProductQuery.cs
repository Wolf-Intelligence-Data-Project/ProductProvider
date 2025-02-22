using GraphQL;
using GraphQL.Types;
using ProductProvider.Interfaces;
using ProductProvider.Models;
using ProductProvider.Models.Data.Entities;
using ProductProvider.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductProvider.GraphQL
{
    public class ProductQuery : ObjectGraphType
    {
        public ProductQuery(IProductService productService)
        {
            Field<ListGraphType<ProductType>>(
                "products",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> { Name = "quantity" },
                    new QueryArgument<StringGraphType> { Name = "companyName" }
                ),
                resolve: context =>
                {
                    var quantity = context.GetArgument<int>("quantity");
                    var companyName = context.GetArgument<string>("companyName");

                    var filter = new ProductFilterRequest { CompanyName = companyName };
                    return productService.GetFilteredProductsAsync(filter, quantity);
                });
        }
    }

    public class ProductType : ObjectGraphType<ProductEntity>
    {
        public ProductType()
        {
            Field(x => x.ProductId);
            Field(x => x.CompanyName);
            Field(x => x.BusinessType);
            Field(x => x.Revenue);
            Field(x => x.NumberOfEmployees);
            Field(x => x.PhoneNumber);
        }
    }
}
