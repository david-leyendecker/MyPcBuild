using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Features.Catalog.DTOs;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Features.Catalog;

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

            string baseUrl = GetBaseUrl(httpContextAccessor);

            CreateProductResponse response = new(
                product.Id,
                [
                    new HateoasLink($"{baseUrl}/api/catalog/products/{product.Id}", "self", "GET"),
                    new HateoasLink($"{baseUrl}/api/catalog/products/{product.Id}", "update", "PUT"),
                    new HateoasLink($"{baseUrl}/api/catalog/products/{product.Id}/publish", "publish", "POST"),
                    new HateoasLink($"{baseUrl}/api/catalog/products", "all-products", "GET")
                ]
            );

            return Results.Created($"/api/catalog/products/{product.Id}", response);
        })
        .WithName("CreateProduct")
        .Produces<CreateProductResponse>(StatusCodes.Status201Created)
        .WithTags("Catalog");

        return app;
    }

    private static string GetBaseUrl(IHttpContextAccessor httpContextAccessor)
    {
        HttpRequest request = httpContextAccessor.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}";
    }
}

public record CreateProductResponse(
    Guid Id,
    List<HateoasLink> Links
);
