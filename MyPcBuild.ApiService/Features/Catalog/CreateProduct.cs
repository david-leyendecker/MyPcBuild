using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Features.Catalog.DTOs;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class CreateProduct
{
    public static IEndpointRouteBuilder MapCreateProductEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/catalog/products", async (
            IDocumentSession session,
            ProductRequest request) =>
        {
            Product product = ProductDtoMapper.ToDomain(request);

            session.Store(product);
            await session.SaveChangesAsync();

            return Results.Created($"/api/catalog/products/{product.Id}", new CreateProductResponse(product.Id));
        })
        .WithName("CreateProduct")
        .Produces<CreateProductResponse>(StatusCodes.Status201Created)
        .WithTags("Catalog");

        return app;
    }
}

public record CreateProductResponse(Guid Id);
