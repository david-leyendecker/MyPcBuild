using Marten;
using Microsoft.AspNetCore.Mvc;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Features.Catalog.DTOs;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class UpdateProduct
{
    public static IEndpointRouteBuilder MapUpdateProductEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/catalog/products/{id}", UpdateProductHandler)
            .WithName("UpdateProduct")
            .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("Catalog");

        return app;
    }

    private static async Task<IResult> UpdateProductHandler(
        [FromRoute] Guid id,
        IDocumentSession session,
        ProductDto dto)
    {
        Product? existingProduct = await session.LoadAsync<Product>(id);
        if (existingProduct == null)
        {
            return Results.NotFound();
        }

        // Convert DTO to domain model, preserving the existing ID
        Product updatedProduct = ProductDtoMapper.ToDomain(dto, id);

        // Validate that product category hasn't changed
        if (updatedProduct.ProductCategory != existingProduct.ProductCategory)
        {
            return Results.BadRequest(new { error = $"Cannot change product category from {existingProduct.ProductCategory} to {updatedProduct.ProductCategory}" });
        }

        session.Store(updatedProduct);
        await session.SaveChangesAsync();

        return Results.Ok(new UpdateProductResponse(updatedProduct.Id));
    }
}

public record UpdateProductResponse(Guid Id);
