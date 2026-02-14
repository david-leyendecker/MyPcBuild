using Marten;
using MyPcBuild.ApiService.Catalog.DTOs;
using MyPcBuild.ApiService.Catalog.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Catalog.Endpoints;

public static class CreateProduct
{
    public static IEndpointRouteBuilder MapCreateProductEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/catalog/products", async (
            IDocumentSession session,
            IHttpContextAccessor httpContextAccessor,
            ProductRequest request) =>
        {
            Product product = ProductDtoMapper.ToDomain(request);

            session.Store(product);
            await session.SaveChangesAsync();

            string baseUrl = httpContextAccessor.GetBaseUrl();

            CreateProductResponse response = new(
                product.Id,
                [
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products/{product.Id}"), "self", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products/{product.Id}"), "update", Infrastructure.HttpMethod.PUT),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products/{product.Id}/publish"), "publish", Infrastructure.HttpMethod.POST),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products"), "all-products", Infrastructure.HttpMethod.GET)
                ]
            );

            return Results.Created($"/api/catalog/products/{product.Id}", response);
        })
        .WithName("CreateProduct")
        .Produces<CreateProductResponse>(StatusCodes.Status201Created)
        .WithTags("Catalog");

        return app;
    }

}

public record CreateProductResponse(
    Guid Id,
    List<HateoasLink> Links
);
