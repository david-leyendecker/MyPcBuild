using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;

namespace MyPcBuild.ApiService.Features.Spatial;

public static class SpatialEndpoints
{
    public static void MapSpatialEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/spatial")
            .WithTags("Spatial Topology");

        group.MapPost("/validate", ValidatePartInstallation)
            .WithName("ValidatePartInstallation")
            .WithSummary("Validate a part installation in a chamber slot");

        group.MapPost("/chambers", CreateChamber)
            .WithName("CreateChamber")
            .WithSummary("Create a new chamber configuration");

        group.MapPost("/chambers/{chamberId:guid}/slots", AddSlot)
            .WithName("AddSlot")
            .WithSummary("Add a slot to a chamber");

        group.MapPost("/chambers/{chamberId:guid}/validate", ValidateChamber)
            .WithName("ValidateChamber")
            .WithSummary("Validate entire chamber configuration");

        group.MapGet("/chambers/{chamberId:guid}", GetChamber)
            .WithName("GetChamber")
            .WithSummary("Get chamber configuration");
    }

    private static Results<Ok<SpatialValidationResponse>, BadRequest<string>> ValidatePartInstallation(
        [FromBody] ValidatePartInstallationRequest request,
        [FromServices] ISpatialValidator validator)
    {
        // For this example, create a test chamber
        // In a real implementation, retrieve the chamber from a repository
        Chamber chamber = CreateTestChamber(request.ChamberId);

        SpatialValidationResult result = validator.ValidatePartInstallation(
            chamber,
            request.SlotId,
            request.Dimensions,
            request.Position
        );

        SpatialValidationResponse response = new(
            result.IsValid,
            result.HasErrors,
            result.HasWarnings,
            result.Issues.Select(i => new SpatialIssueDto(
                i.Message,
                i.Severity.ToString(),
                i.Category
            )).ToList()
        );

        return TypedResults.Ok(response);
    }

    private static Results<Ok<ChamberDto>, BadRequest<string>> CreateChamber(
        [FromBody] ConfigureChamberRequest request)
    {
        Guid chamberId = Guid.NewGuid();
        
        Chamber chamber = new()
        {
            Id = chamberId,
            Name = request.Name,
            Dimensions = request.Dimensions
        };

        ChamberDto dto = MapChamberToDto(chamber);
        
        return TypedResults.Ok(dto);
    }

    private static Results<Ok<SlotDto>, BadRequest<string>, NotFound> AddSlot(
        Guid chamberId,
        [FromBody] AddSlotRequest request)
    {
        // In a real implementation, retrieve the chamber from a repository
        Chamber chamber = CreateTestChamber(chamberId);

        Guid slotId = Guid.NewGuid();
        Slot slot = new()
        {
            Id = slotId,
            Name = request.SlotName,
            AllowedCategory = request.AllowedCategory,
            RelativePosition = request.RelativePosition,
            MaxDimensions = request.MaxDimensions
        };

        if (request.ParentSlotId.HasValue)
        {
            // Find parent slot and add as sub-slot
            Slot? parentSlot = FindSlotRecursive(chamber.Slots, request.ParentSlotId.Value);
            if (parentSlot != null)
            {
                parentSlot.SubSlots.Add(slot);
            }
            else
            {
                return TypedResults.NotFound();
            }
        }
        else
        {
            chamber.Slots.Add(slot);
        }

        SlotDto dto = MapSlotToDto(slot);
        
        return TypedResults.Ok(dto);
    }

    private static Results<Ok<SpatialValidationResponse>, NotFound> ValidateChamber(
        Guid chamberId,
        [FromServices] ISpatialValidator validator)
    {
        // In a real implementation, retrieve the chamber from a repository
        Chamber chamber = CreateTestChamber(chamberId);

        SpatialValidationResult result = validator.ValidateChamber(chamber);

        SpatialValidationResponse response = new(
            result.IsValid,
            result.HasErrors,
            result.HasWarnings,
            result.Issues.Select(i => new SpatialIssueDto(
                i.Message,
                i.Severity.ToString(),
                i.Category
            )).ToList()
        );

        return TypedResults.Ok(response);
    }

    private static Results<Ok<ChamberDto>, NotFound> GetChamber(Guid chamberId)
    {
        // In a real implementation, retrieve the chamber from a repository
        Chamber chamber = CreateTestChamber(chamberId);

        ChamberDto dto = MapChamberToDto(chamber);
        
        return TypedResults.Ok(dto);
    }

    // Helper methods

    private static Chamber CreateTestChamber(Guid chamberId)
    {
        // Create a test chamber with typical PC case dimensions (400x200x450mm)
        Chamber chamber = new()
        {
            Id = chamberId,
            Name = "Test PC Case",
            Dimensions = new Dimensions(400, 200, 450),
            Slots =
            [
                new Slot
                {
                    Id = Guid.NewGuid(),
                    Name = "Motherboard Slot",
                    AllowedCategory = ProductCategory.Motherboard,
                    RelativePosition = new Vector3(10, 10, 0),
                    MaxDimensions = new Dimensions(305, 244, 50) // ATX motherboard
                }
            ]
        };

        return chamber;
    }

    private static Slot? FindSlotRecursive(List<Slot> slots, Guid slotId)
    {
        foreach (Slot slot in slots)
        {
            if (slot.Id == slotId)
            {
                return slot;
            }

            Slot? found = FindSlotRecursive(slot.SubSlots, slotId);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private static ChamberDto MapChamberToDto(Chamber chamber)
    {
        return new ChamberDto(
            chamber.Id,
            chamber.Name,
            new DimensionsDto(chamber.Dimensions.Length, chamber.Dimensions.Width, chamber.Dimensions.Height),
            chamber.Slots.Select(MapSlotToDto).ToList(),
            chamber.InstalledParts.Select(MapInstalledPartToDto).ToList()
        );
    }

    private static SlotDto MapSlotToDto(Slot slot)
    {
        return new SlotDto(
            slot.Id,
            slot.Name,
            slot.AllowedCategory.ToString(),
            new Vector3Dto(slot.RelativePosition.X, slot.RelativePosition.Y, slot.RelativePosition.Z),
            new DimensionsDto(slot.MaxDimensions.Length, slot.MaxDimensions.Width, slot.MaxDimensions.Height),
            slot.InstalledPartId,
            slot.SubSlots.Select(MapSlotToDto).ToList()
        );
    }

    private static InstalledPartDto MapInstalledPartToDto(InstalledPart part)
    {
        return new InstalledPartDto(
            part.Id,
            part.ProductId,
            part.SlotId,
            new Vector3Dto(part.Position.X, part.Position.Y, part.Position.Z),
            new DimensionsDto(part.Dimensions.Length, part.Dimensions.Width, part.Dimensions.Height)
        );
    }
}
