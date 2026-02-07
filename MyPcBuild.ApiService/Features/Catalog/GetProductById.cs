using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Features.Catalog.DTOs;
using MyPcBuild.ApiService.Infrastructure;

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
            IDocumentSession session,
            IHttpContextAccessor httpContextAccessor) =>
        {
            Product? product = await session.LoadAsync<Product>(id);

            if (product is null)
            {
                return Results.NotFound();
            }

            string baseUrl = httpContextAccessor.GetBaseUrl();

            ProductResponse response = ProductDtoMapper.ToResponse(product);

            GetProductByIdResponse wrappedResponse = new(
                response,
                [
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products/{id}"), "self", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products/{id}"), "update", Infrastructure.HttpMethod.PUT),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products?filters=ProductCategory={product.ProductCategory}"), "category", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products"), "all-products", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/categories"), "categories", Infrastructure.HttpMethod.GET)
                ]
            );

            return Results.Ok(wrappedResponse);
        })
        .WithName("GetProductById")
        .Produces<GetProductByIdResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithTags("Catalog");

        return app;
    }

}

public record GetProductByIdResponse(
    ProductResponse Product,
    List<HateoasLink> Links
);
