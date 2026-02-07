using Marten;
using MyPcBuild.ApiService.Domain.Events;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Features.Builds;

public static class AddPartToBuild
{
    public static IEndpointRouteBuilder MapAddPartEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/builds/{buildId:guid}/parts", async (
            Guid buildId,
            AddPartRequest request,
            IDocumentSession session,
            IHttpContextAccessor httpContextAccessor) =>
        {
            // Validate that the product exists and is not a draft
            Product? product = await session.LoadAsync<Product>(request.ProductId);
            
            if (product == null)
            {
                return Results.NotFound(new { error = "Product not found" });
            }

            if (product.IsDraft)
            {
                return Results.BadRequest(new { error = "Draft products cannot be added to builds. Please publish the product first." });
            }

            PartAdded @event = new()
            {
                BuildId = buildId,
                ProductId = request.ProductId,
                PricePaid = request.PricePaid
            };

            session.Events.Append(buildId, @event);
            await session.SaveChangesAsync();

            string baseUrl = GetBaseUrl(httpContextAccessor);

            AddPartResponse response = new(
                "Part added successfully",
                [
                    new HateoasLink($"{baseUrl}/api/builds/{buildId}", "build", "GET"),
                    new HateoasLink($"{baseUrl}/api/builds/{buildId}/parts/{request.ProductId}", "remove", "DELETE"),
                    new HateoasLink($"{baseUrl}/api/builds/{buildId}/compatibility", "validate", "GET"),
                    new HateoasLink($"{baseUrl}/api/catalog/products/{request.ProductId}", "product", "GET")
                ]
            );

            return Results.Ok(response);
        })
        .WithName("AddPartToBuild")
        .WithTags("Builds");

        return app;
    }

    private static string GetBaseUrl(IHttpContextAccessor httpContextAccessor)
    {
        HttpRequest request = httpContextAccessor.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}";
    }
}

public record AddPartRequest(Guid ProductId, decimal PricePaid);

public record AddPartResponse(
    string Message,
    List<HateoasLink> Links
);
