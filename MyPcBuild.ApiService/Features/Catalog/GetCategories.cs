using Marten;
using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class GetCategories
{
    public static IEndpointRouteBuilder MapGetCategoriesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalog/categories", async (IDocumentSession session) =>
        {
            Dictionary<ProductCategory, ProductCategoryInfo> categoryInfoDict = ProductCategoryInfo.ByEnum();

            // Get product counts per category
            Dictionary<ProductCategory, int> productCounts = (await session.Query<Product>()
                .GroupBy(p => p.ProductCategory)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToListAsync())
                .ToDictionary(x => x.Category, x => x.Count);

            List<CategoryInfo> categories = [.. categoryInfoDict
                .Select(kvp => new CategoryInfo(
                    kvp.Key.ToString(),
                    kvp.Value.DisplayValue,
                    productCounts.GetValueOrDefault(kvp.Key, 0)
                ))];

            GetCategoriesResponse response = new(categories);
            return Results.Ok(response);
        })
        .WithName("GetCategories")
        .WithTags("Catalog");

        return app;
    }
}

public record GetCategoriesResponse(List<CategoryInfo> Categories);

public record CategoryInfo(
    string Name,
    string DisplayValue,
    int ProductCount
);
