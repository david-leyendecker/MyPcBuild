using Marten;
using MyPcBuild.ApiService.Domain.Events;
using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Features.Builds;

public static class AddPartToBuild
{
    public static IEndpointRouteBuilder MapAddPartEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/builds/{buildId:guid}/parts", async (
            Guid buildId,
            AddPartRequest request,
            IDocumentSession session) =>
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

            return Results.Ok(new { Message = "Part added successfully" });
        })
        .WithName("AddPartToBuild")
        .WithTags("Builds");

        return app;
    }
}

public record AddPartRequest(Guid ProductId, decimal PricePaid);
