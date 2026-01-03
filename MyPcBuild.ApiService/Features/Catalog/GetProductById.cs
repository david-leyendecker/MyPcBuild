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
            
            GetProductByIdResponse response = new(
                product.Id,
                product.Name,
                product.Category,
                product.Price,
                product.Manufacturer,
                product.Specifications
            );

            return Results.Ok(response);
        })
        .WithName("GetProductById")
        .WithTags("Catalog");

        return app;
    }
}

public record GetProductByIdResponse(
    Guid Id,
    string Name,
    ProductCategory Category,
    decimal Price,
    string Manufacturer,
    Dictionary<string, object> Specifications
);
