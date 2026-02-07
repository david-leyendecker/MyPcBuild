using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class GetCategories
{
    public static IEndpointRouteBuilder MapGetCategoriesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalog/categories", async (
            IDocumentSession session,
            IHttpContextAccessor httpContextAccessor) =>
        {
            Dictionary<ProductCategory, ProductCategoryInfo> categoryInfoDict = ProductCategoryInfo.ByEnum();

            // Get product counts per category
            Dictionary<ProductCategory, int> productCounts = (await session.Query<Product>()
                .GroupBy(p => p.ProductCategory)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToListAsync())
                .ToDictionary(x => x.Category, x => x.Count);

            string baseUrl = GetBaseUrl(httpContextAccessor);

            List<CategoryInfo> categories = [.. categoryInfoDict
                .Select(kvp => new CategoryInfo(
                    kvp.Key.ToString(),
                    kvp.Value.DisplayValue,
                    productCounts.GetValueOrDefault(kvp.Key, 0),
                    [
                        new HateoasLink($"{baseUrl}/api/catalog/products?filters=ProductCategory={kvp.Key}", "products", "GET"),
                        new HateoasLink($"{baseUrl}/api/catalog/field-definitions/{kvp.Key}", "field-definitions", "GET")
                    ]
                ))];

            GetCategoriesResponse response = new(
                categories,
                [
                    new HateoasLink($"{baseUrl}/api/catalog/categories", "self", "GET"),
                    new HateoasLink($"{baseUrl}/api/catalog/products", "all-products", "GET")
                ]
            );
            return Results.Ok(response);
        })
        .WithName("GetCategories")
        .WithTags("Catalog");

        return app;
    }

    private static string GetBaseUrl(IHttpContextAccessor httpContextAccessor)
    {
        HttpRequest request = httpContextAccessor.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}";
    }
}

public record GetCategoriesResponse(
    List<CategoryInfo> Categories,
    List<HateoasLink> Links
);

public record CategoryInfo(
    string Name,
    string DisplayValue,
    int ProductCount,
    List<HateoasLink> Links
);
