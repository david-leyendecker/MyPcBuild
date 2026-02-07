using Marten;
using Microsoft.AspNetCore.Mvc;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Features.Catalog.DTOs;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class UpdateProduct
{
    public static IEndpointRouteBuilder MapUpdateProductEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/catalog/products/{id}", async (
            [FromRoute] Guid id,
            IDocumentSession session,
            IHttpContextAccessor httpContextAccessor,
            ProductRequest request) =>
        {
            Product? existingProduct = await session.LoadAsync<Product>(id);
            if (existingProduct == null)
            {
                return Results.NotFound();
            }

            // Convert request to domain model, preserving the existing ID
            Product updatedProduct = ProductDtoMapper.ToDomain(request, id);

            // Validate that product category hasn't changed
            if (updatedProduct.ProductCategory != existingProduct.ProductCategory)
            {
                return Results.BadRequest(new { error = $"Cannot change product category from {existingProduct.ProductCategory} to {updatedProduct.ProductCategory}" });
            }

            session.Store(updatedProduct);
            await session.SaveChangesAsync();

            string baseUrl = httpContextAccessor.GetBaseUrl();

            List<HateoasLink> links =
            [
                new HateoasLink(new Uri($"{baseUrl}/api/catalog/products/{updatedProduct.Id}"), "self", Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri($"{baseUrl}/api/catalog/products/{updatedProduct.Id}"), "update", Infrastructure.HttpMethod.PUT),
                new HateoasLink(new Uri($"{baseUrl}/api/catalog/products"), "all-products", Infrastructure.HttpMethod.GET)
            ];

            if (updatedProduct.IsDraft)
            {
                links.Add(new HateoasLink(new Uri($"{baseUrl}/api/catalog/products/{updatedProduct.Id}/publish"), "publish", Infrastructure.HttpMethod.POST));
            }

            UpdateProductResponse response = new(updatedProduct.Id, links);

            return Results.Ok(response);
        })
        .WithName("UpdateProduct")
        .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithTags("Catalog");

        return app;
    }

}

public record UpdateProductResponse(
    Guid Id,
    List<HateoasLink> Links
);
