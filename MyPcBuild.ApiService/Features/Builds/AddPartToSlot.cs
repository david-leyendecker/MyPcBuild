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

            // Load the product that contains the slot to get the slot's position
            Product? parentProduct = await session.LoadAsync<Product>(request.ParentProductId);
            
            if (parentProduct == null)
            {
                return Results.NotFound(new { error = "Parent product not found" });
            }

            // Find the slot in the parent product's slots/chambers to get the slot's position
            Slot? slot = null;

            // Try to find slot in direct slots first (slotted products like motherboards)
            if (parentProduct is ISlottedProduct slottedProduct)
            {
                slot = FindSlotInList(slottedProduct.Slots, request.SlotId);
            }

            // If not found, try to find in chambers (chambered products like cases)
            if (slot == null && parentProduct is IChamberedProduct chamberedProduct)
            {
                foreach (Chamber chamber in chamberedProduct.Chambers)
                {
                    slot = FindSlotInList(chamber.Slots, request.SlotId);
                    if (slot != null) break;
                }
            }

            if (slot == null)
            {
                return Results.NotFound(new { error = "Slot not found in parent product" });
            }

            // The position and rotation come from the slot definition, not from the client
            PartAddedToSlot @event = new()
            {
                BuildId = buildId,
                ProductId = request.ProductId,
                PricePaid = request.PricePaid,
                SlotId = request.SlotId,
                Position = slot.RelativePosition,
                Rotation = slot.Rotation
            };

            session.Events.Append(buildId, @event);
            await session.SaveChangesAsync();

            return Results.Ok(new { Message = "Part added to slot successfully" });
        })
        .WithName("AddPartToSlot")
        .WithTags("Builds");

        return app;
    }

    private static Slot? FindSlotInList(List<Slot> slots, Guid slotId)
    {
        foreach (Slot slot in slots)
        {
            if (slot.Id == slotId)
            {
                return slot;
            }

            // Recursively search in sub-slots
            Slot? found = FindSlotInList(slot.SubSlots, slotId);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }
}

public record AddPartToSlotRequest(
    Guid ProductId,
    decimal PricePaid,
    Guid SlotId,
    Guid ParentProductId
);
