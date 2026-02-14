using Marten;
using MyPcBuild.ApiService.Builds.Events;
using MyPcBuild.ApiService.Catalog.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Builds.Endpoints;

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

            string baseUrl = httpContextAccessor.GetBaseUrl();

            AddPartResponse response = new(
                "Part added successfully",
                [
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}"), "build", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/parts/{request.ProductId}"), "remove", Infrastructure.HttpMethod.DELETE),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/compatibility"), "validate", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products/{request.ProductId}"), "product", Infrastructure.HttpMethod.GET)
                ]
            );

            return Results.Ok(response);
        })
        .WithName("AddPartToBuild")
        .WithTags("Builds");

        return app;
    }

}

public record AddPartRequest(Guid ProductId, decimal PricePaid);

public record AddPartResponse(
    string Message,
    List<HateoasLink> Links
);
