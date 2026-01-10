using Marten;
using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class GenerateProductWithAi
{
    public static IEndpointRouteBuilder MapGenerateProductWithAiEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/catalog/products/generate-with-ai", async (
            IDocumentSession session,
            IAiProductGenerator aiGenerator,
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

                return Results.Created($"/api/catalog/{product.Id}", new GenerateProductResponse(product.Id, product));
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
    string Category,
    string Description
);

public record GenerateProductResponse(
    Guid Id,
    Product Product
);
