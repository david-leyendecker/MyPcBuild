using Marten;
using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class GetProductById
{
    public static IEndpointRouteBuilder MapGetProductByIdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalog/products/{id:guid}", async (
            Guid id,
            IDocumentSession session) =>
        {
            Product? product = await session.LoadAsync<Product>(id);
            if (product is null)
            {
                return Results.NotFound();
            }
            
            // Return the full product object which will include all category-specific properties
            return Results.Ok(product);
        })
        .WithName("GetProductById")
        .WithTags("Catalog");

        return app;
    }
}
