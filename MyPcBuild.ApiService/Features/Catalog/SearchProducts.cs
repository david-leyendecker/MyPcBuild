using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Features.Catalog;

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
            string baseUrl = GetBaseUrl(httpContextAccessor);

            if (string.IsNullOrWhiteSpace(query))
            {
                SearchProductsResponse emptyResponse = new(
                    [],
                    [
                        new HateoasLink($"{baseUrl}/api/catalog/search", "self", "GET"),
                        new HateoasLink($"{baseUrl}/api/catalog/products", "all-products", "GET")
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
                    p.ProductCategory.ToString(),
                    p.Price,
                    p.Manufacturer,
                    [
                        new HateoasLink($"{baseUrl}/api/catalog/products/{p.Id}", "self", "GET"),
                        new HateoasLink($"{baseUrl}/api/catalog/products?filters=ProductCategory={p.ProductCategory}", "category", "GET")
                    ]
                )).ToList(),
                [
                    new HateoasLink($"{baseUrl}/api/catalog/search?query={Uri.EscapeDataString(query)}&maxResults={maxResults}", "self", "GET"),
                    new HateoasLink($"{baseUrl}/api/catalog/products", "all-products", "GET"),
                    new HateoasLink($"{baseUrl}/api/catalog/categories", "categories", "GET")
                ]
            );

            return Results.Ok(response);
        })
        .WithName("SearchProducts")
        .WithTags("Catalog");

        return app;
    }

    private static string GetBaseUrl(IHttpContextAccessor httpContextAccessor)
    {
        HttpRequest request = httpContextAccessor.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}";
    }
}

public record SearchProductsResponse(
    List<SearchProductResult> Items,
    List<HateoasLink> Links
);

public record SearchProductResult(
    Guid Id,
    string Name,
    string CategoryName,
    decimal Price,
    string Manufacturer,
    List<HateoasLink> Links
);
