using Marten;
using MyPcBuild.ApiService.Domain.Events;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;

namespace MyPcBuild.ApiService.Features.Builds;

public static class AddPartToSlot
{
    public static IEndpointRouteBuilder MapAddPartToSlotEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/builds/{buildId:guid}/parts/slot", async (
            Guid buildId,
            AddPartToSlotRequest request,
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

            PartAddedToSlot @event = new()
            {
                BuildId = buildId,
                ProductId = request.ProductId,
                PricePaid = request.PricePaid,
                SlotId = request.SlotId,
                Position = request.Position,
                Rotation = request.Rotation
            };

            session.Events.Append(buildId, @event);
            await session.SaveChangesAsync();

            return Results.Ok(new { Message = "Part added to slot successfully" });
        })
        .WithName("AddPartToSlot")
        .WithTags("Builds");

        return app;
    }
}

public record AddPartToSlotRequest(
    Guid ProductId,
    decimal PricePaid,
    Guid SlotId,
    Vector3 Position,
    Rotation? Rotation
);
