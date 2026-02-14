using Marten;
using MyPcBuild.ApiService.Catalog.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Catalog.Endpoints;

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

            string baseUrl = httpContextAccessor.GetBaseUrl();

            List<CategoryInfo> categories = [.. categoryInfoDict
                .Select(kvp => new CategoryInfo(
                    kvp.Key.ToString(),
                    kvp.Value.DisplayValue,
                    productCounts.GetValueOrDefault(kvp.Key, 0),
                    [
                        new HateoasLink(new Uri($"{baseUrl}/api/catalog/products?filters=ProductCategory={kvp.Key}"), "products", Infrastructure.HttpMethod.GET),
                        new HateoasLink(new Uri($"{baseUrl}/api/catalog/field-definitions/{kvp.Key}"), "field-definitions", Infrastructure.HttpMethod.GET)
                    ]
                ))];

            GetCategoriesResponse response = new(
                categories,
                [
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/categories"), "self", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products"), "all-products", Infrastructure.HttpMethod.GET)
                ]
            );
            return Results.Ok(response);
        })
        .WithName("GetCategories")
        .WithTags("Catalog");

        return app;
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
