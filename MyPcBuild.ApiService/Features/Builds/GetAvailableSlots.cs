using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;

namespace MyPcBuild.ApiService.Features.Builds;

public static class GetAvailableSlots
{
    public static IEndpointRouteBuilder MapGetAvailableSlotsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/builds/{buildId:guid}/slots", async (
            Guid buildId,
            IDocumentSession session) =>
        {
            Build? build = await session.Events.AggregateStreamAsync<Build>(buildId);
            if (build is null)
            {
                return Results.NotFound();
            }

            List<AvailableSlotDto> availableSlots = [];

            // Get all products in the build
            foreach (BuildPart part in build.Parts)
            {
                Product? product = await session.LoadAsync<Product>(part.ProductId);
                if (product == null) continue;

                Vector3 basePosition = part.Position ?? Vector3.Zero;

                // Add slots from chambered products (PC cases)
                if (product is IChamberedProduct chamberedProduct)
                {
                    foreach (Chamber chamber in chamberedProduct.Chambers)
                    {
                        foreach (Slot slot in chamber.Slots)
                        {
                            bool isOccupied = build.Parts.Any(p => p.SlotId == slot.Id);
                            availableSlots.Add(new AvailableSlotDto(
                                slot.Id,
                                slot.Name,
                                slot.AllowedProductCategory.ToString(),
                                new Vector3Dto(
                                    basePosition.X + slot.RelativePosition.X,
                                    basePosition.Y + slot.RelativePosition.Y,
                                    basePosition.Z + slot.RelativePosition.Z
                                ),
                                new DimensionsDto(
                                    slot.MaxDimensions.Length,
                                    slot.MaxDimensions.Width,
                                    slot.MaxDimensions.Height
                                ),
                                slot.Rotation != Rotation.Identity 
                                    ? new RotationDto(slot.Rotation.X, slot.Rotation.Y, slot.Rotation.Z)
                                    : null,
                                isOccupied,
                                product.Id,
                                product.Name
                            ));
                        }
                    }
                }

                // Add slots from slotted products (motherboards, GPUs)
                if (product is ISlottedProduct slottedProduct)
                {
                    foreach (Slot slot in slottedProduct.Slots)
                    {
                        bool isOccupied = build.Parts.Any(p => p.SlotId == slot.Id);
                        availableSlots.Add(new AvailableSlotDto(
                            slot.Id,
                            slot.Name,
                            slot.AllowedProductCategory.ToString(),
                            new Vector3Dto(
                                basePosition.X + slot.RelativePosition.X,
                                basePosition.Y + slot.RelativePosition.Y,
                                basePosition.Z + slot.RelativePosition.Z
                            ),
                            new DimensionsDto(
                                slot.MaxDimensions.Length,
                                slot.MaxDimensions.Width,
                                slot.MaxDimensions.Height
                            ),
                            slot.Rotation != Rotation.Identity 
                                ? new RotationDto(slot.Rotation.X, slot.Rotation.Y, slot.Rotation.Z)
                                : null,
                            isOccupied,
                            product.Id,
                            product.Name
                        ));
                    }
                }
            }

            return Results.Ok(availableSlots);
        })
        .WithName("GetAvailableSlots")
        .WithTags("Builds");

        return app;
    }
}

public record AvailableSlotDto(
    Guid Id,
    string Name,
    string AllowedCategory,
    Vector3Dto AbsolutePosition,
    DimensionsDto MaxDimensions,
    RotationDto? Rotation,
    bool IsOccupied,
    Guid ParentProductId,
    string ParentProductName
);
