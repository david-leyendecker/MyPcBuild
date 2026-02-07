using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Features.Builds;

public static class GetAvailableSlots
{
    public static IEndpointRouteBuilder MapGetAvailableSlotsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/builds/{buildId:guid}/slots", async (
            Guid buildId,
            IDocumentSession session,
            IHttpContextAccessor httpContextAccessor) =>
        {
            Build? build = await session.Events.AggregateStreamAsync<Build>(buildId);
            if (build is null)
            {
                return Results.NotFound();
            }

            string baseUrl = httpContextAccessor.GetBaseUrl();

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

            GetAvailableSlotsResponse response = new(
                availableSlots,
                [
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/slots"), "self", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}"), "build", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/parts/slot"), "add-part-to-slot", Infrastructure.HttpMethod.POST)
                ]
            );

            return Results.Ok(response);
        })
        .WithName("GetAvailableSlots")
        .WithTags("Builds");

        return app;
    }

}

public record GetAvailableSlotsResponse(
    List<AvailableSlotDto> Slots,
    List<HateoasLink> Links
);

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
