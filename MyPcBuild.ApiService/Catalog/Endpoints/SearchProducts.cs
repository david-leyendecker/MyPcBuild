using Marten;
using MyPcBuild.ApiService.Catalog.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Catalog.Endpoints;

public static class SearchProducts
{
    public static IEndpointRouteBuilder MapSearchProductsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalog/search", async (
            IDocumentSession session,
            IHttpContextAccessor httpContextAccessor,
            string? query = null,
            int maxResults = 10) =>
        {
            string baseUrl = httpContextAccessor.GetBaseUrl();

            if (string.IsNullOrWhiteSpace(query))
            {
                SearchProductsResponse emptyResponse = new(
                    [],
                    [
                        new HateoasLink(new Uri($"{baseUrl}/api/catalog/search"), "self", Infrastructure.HttpMethod.GET),
                        new HateoasLink(new Uri($"{baseUrl}/api/catalog/products"), "all-products", Infrastructure.HttpMethod.GET)
                    ]
                );
                return Results.Ok(emptyResponse);
            }

            IReadOnlyList<Product> results = await session.Query<Product>()
                .Where(p => p.Name.Contains(query) || p.Manufacturer.Contains(query))
                .Take(maxResults)
                .ToListAsync();

            SearchProductsResponse response = new(
                results.Select(p => new SearchProductResult(
                    p.Id,
                    p.Name,
                    p.ProductCategory,
                    p.Price,
                    p.Manufacturer,
                    [
                        new HateoasLink(new Uri($"{baseUrl}/api/catalog/products/{p.Id}"), "self", Infrastructure.HttpMethod.GET),
                        new HateoasLink(new Uri($"{baseUrl}/api/catalog/products?filters=ProductCategory={p.ProductCategory}"), "category", Infrastructure.HttpMethod.GET)
                    ]
                )).ToList(),
                [
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/search?query={Uri.EscapeDataString(query)}&maxResults={maxResults}"), "self", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products"), "all-products", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/categories"), "categories", Infrastructure.HttpMethod.GET)
                ]
            );

            return Results.Ok(response);
        })
        .WithName("SearchProducts")
        .WithTags("Catalog");

        return app;
    }

}

public record SearchProductsResponse(
    List<SearchProductResult> Items,
    List<HateoasLink> Links
);

public record SearchProductResult(
    Guid Id,
    string Name,
    ProductCategory CategoryName,
    decimal Price,
    string Manufacturer,
    List<HateoasLink> Links
);
