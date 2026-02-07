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

                string baseUrl = GetBaseUrl(httpContextAccessor);

                GenerateProductResponse response = new(
                    product.Id,
                    product,
                    [
                        new HateoasLink($"{baseUrl}/api/catalog/products/{product.Id}", "self", "GET"),
                        new HateoasLink($"{baseUrl}/api/catalog/products/{product.Id}", "update", "PUT"),
                        new HateoasLink($"{baseUrl}/api/catalog/products/{product.Id}/publish", "publish", "POST"),
                        new HateoasLink($"{baseUrl}/api/catalog/products", "all-products", "GET")
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

    private static string GetBaseUrl(IHttpContextAccessor httpContextAccessor)
    {
        HttpRequest request = httpContextAccessor.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}";
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
