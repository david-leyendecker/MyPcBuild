using Marten;
using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class SearchProducts
{
    public static IEndpointRouteBuilder MapSearchProductsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalog/search", async (
            IDocumentSession session,
            string query,
            int maxResults = 10) =>
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Results.Ok(Array.Empty<Product>());
            }

            IReadOnlyList<Product> results = await session.Query<Product>()
                .Where(p => p.Name.Contains(query) || p.Manufacturer.Contains(query))
                .Take(maxResults)
                .ToListAsync();

            return Results.Ok(results);
        })
        .WithName("SearchProducts")
        .WithTags("Catalog");

        return app;
    }
}
