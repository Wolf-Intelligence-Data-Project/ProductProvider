using ProductProvider.Interfaces;
using ProductProvider.Models.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;
using ProductProvider.Models;

namespace ProductProvider.Services
{
    public class ElasticSearchService : IElasticSearchService
    {
        // Assuming you have some ElasticSearch client dependency injected
        private readonly IElasticClient _elasticClient;

        public ElasticSearchService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<List<ProductEntity>> SearchProductsAsync(ProductFilterRequest filters, int quantity)
        {
            // Your ElasticSearch query logic goes here
            var result = await _elasticClient.SearchAsync<ProductEntity>(s => s
                .Index("products")
                .Query(q => q
                    .Bool(b => b
                        .Must(mu => mu
                            .Match(m => m.Field(f => f.CompanyName).Query(filters.CompanyName)) // Example filter
                        )
                    )
                )
                .Size(quantity)
            );

            return result.Documents.ToList();
        }

        public async Task IndexProductAsync(ProductEntity product)
        {
            // Your indexing logic goes here
            await _elasticClient.IndexDocumentAsync(product);
        }

        public async Task DeleteProductAsync(string productId)
        {
            // Your delete logic goes here
            await _elasticClient.DeleteAsync<ProductEntity>(productId);
        }
    }
}
