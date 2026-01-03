using Marten;
using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class GetCategories
{
    public static IEndpointRouteBuilder MapGetCategoriesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalog/categories", async (IDocumentSession session) =>
        {
            // Get product counts per category
            IReadOnlyList<Product> allProducts = await session.Query<Product>().ToListAsync();
            Dictionary<ProductCategory, int> productCounts = allProducts
                .GroupBy(p => p.Category)
                .ToDictionary(g => g.Key, g => g.Count());
            
            GetCategoriesResponse response = new(
                productCounts.Select(kvp => new CategoryInfo(
                    kvp.Key,
                    kvp.Key.ToString(),
                    kvp.Value
                )).ToList()
            );

            return Results.Ok(response);
        })
        .WithName("GetCategories")
        .WithTags("Catalog");

        return app;
    }
}

public record GetCategoriesResponse(List<CategoryInfo> Categories);

public record CategoryInfo(
    ProductCategory Category,
    string Name,
    int ProductCount
);
