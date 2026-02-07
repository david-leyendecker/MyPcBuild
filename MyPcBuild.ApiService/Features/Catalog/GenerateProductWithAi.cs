using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class GenerateProductWithAi
{
    public static IEndpointRouteBuilder MapGenerateProductWithAiEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/catalog/products/generate-with-ai", async (
            IDocumentSession session,
            IAiProductGenerator aiGenerator,
            IHttpContextAccessor httpContextAccessor,
            GenerateProductRequest request,
            CancellationToken cancellationToken) =>
        {
            try
            {
                Product product = await aiGenerator.GenerateProductAsync(
                    request.Category,
                    request.Description,
                    cancellationToken
                );

                session.Store(product);
                await session.SaveChangesAsync(cancellationToken);

                string baseUrl = httpContextAccessor.GetBaseUrl();

                GenerateProductResponse response = new(
                    product.Id,
                    product,
                    [
                        new HateoasLink(new Uri($"{baseUrl}/api/catalog/products/{product.Id}"), "self", Infrastructure.HttpMethod.GET),
                        new HateoasLink(new Uri($"{baseUrl}/api/catalog/products/{product.Id}"), "update", Infrastructure.HttpMethod.PUT),
                        new HateoasLink(new Uri($"{baseUrl}/api/catalog/products/{product.Id}/publish"), "publish", Infrastructure.HttpMethod.POST),
                        new HateoasLink(new Uri($"{baseUrl}/api/catalog/products"), "all-products", Infrastructure.HttpMethod.GET)
                    ]
                );

                return Results.Created($"/api/catalog/{product.Id}", response);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("GenerateProductWithAi")
        .WithTags("Catalog");

        return app;
    }

}

public record GenerateProductRequest(
    ProductCategory Category,
    string Description
);

public record GenerateProductResponse(
    Guid Id,
    Product Product,
    List<HateoasLink> Links
);
