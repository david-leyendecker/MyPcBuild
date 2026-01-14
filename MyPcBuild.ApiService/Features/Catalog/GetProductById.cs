using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Features.Catalog.DTOs;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class GetProductById
{
    /// <summary>
    /// Retrieves a product by its ID with strongly-typed response.
    /// </summary>
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

            ProductResponse response = ProductDtoMapper.ToResponse(product);
            return Results.Ok(response);
        })
        .WithName("GetProductById")
        .Produces<ProductResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithTags("Catalog");

        return app;
    }
}
