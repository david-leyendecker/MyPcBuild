using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class PublishProduct
{
    public static IEndpointRouteBuilder MapPublishProductEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/catalog/products/{id:guid}/publish", async (
            IDocumentSession session,
            IHttpContextAccessor httpContextAccessor,
            Guid id,
            CancellationToken cancellationToken) =>
        {
            Product? product = await session.LoadAsync<Product>(id, cancellationToken);
            
            if (product == null)
            {
                return Results.NotFound(new { error = "Product not found" });
            }

            if (!product.IsDraft)
            {
                return Results.BadRequest(new { error = "Product is already published" });
            }

            // Create a new product instance with IsDraft = false and PublishedAt set
            Product publishedProduct = product with 
            { 
                IsDraft = false, 
                PublishedAt = DateTime.UtcNow 
            };

            session.Store(publishedProduct);
            await session.SaveChangesAsync(cancellationToken);

            string baseUrl = GetBaseUrl(httpContextAccessor);

            PublishProductResponse response = new(
                publishedProduct.Id,
                publishedProduct,
                [
                    new HateoasLink($"{baseUrl}/api/catalog/products/{publishedProduct.Id}", "self", "GET"),
                    new HateoasLink($"{baseUrl}/api/catalog/products/{publishedProduct.Id}", "update", "PUT"),
                    new HateoasLink($"{baseUrl}/api/catalog/products", "all-products", "GET"),
                    new HateoasLink($"{baseUrl}/api/catalog/categories", "categories", "GET")
                ]
            );

            return Results.Ok(response);
        })
        .WithName("PublishProduct")
        .WithTags("Catalog");

        return app;
    }

    private static string GetBaseUrl(IHttpContextAccessor httpContextAccessor)
    {
        HttpRequest request = httpContextAccessor.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}";
    }
}

public record PublishProductResponse(
    Guid Id,
    Product Product,
    List<HateoasLink> Links
);
