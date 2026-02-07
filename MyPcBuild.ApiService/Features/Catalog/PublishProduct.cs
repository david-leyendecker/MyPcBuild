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

            string baseUrl = httpContextAccessor.GetBaseUrl();

            PublishProductResponse response = new(
                publishedProduct.Id,
                publishedProduct,
                [
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products/{publishedProduct.Id}"), "self", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products/{publishedProduct.Id}"), "update", Infrastructure.HttpMethod.PUT),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products"), "all-products", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/categories"), "categories", Infrastructure.HttpMethod.GET)
                ]
            );

            return Results.Ok(response);
        })
        .WithName("PublishProduct")
        .WithTags("Catalog");

        return app;
    }

}

public record PublishProductResponse(
    Guid Id,
    Product Product,
    List<HateoasLink> Links
);
