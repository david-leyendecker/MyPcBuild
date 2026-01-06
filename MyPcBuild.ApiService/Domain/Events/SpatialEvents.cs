using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;

namespace MyPcBuild.ApiService.Domain.Events;

/// <summary>
/// Event raised when a chamber is configured.
/// </summary>
public record ChamberConfigured : BuildEvent
{
    public required Guid ChamberId { get; init; }
    public required string ChamberName { get; init; }
    public required Dimensions Dimensions { get; init; }
}

/// <summary>
/// Event raised when a part is installed in a slot.
/// </summary>
public record PartInstalledInSlot : BuildEvent
{
    public required Guid ChamberId { get; init; }
    public required Guid SlotId { get; init; }
    public required Guid ProductId { get; init; }
    public required Vector3 Position { get; init; }
    public required Dimensions Dimensions { get; init; }
}

/// <summary>
/// Event raised when a part is removed from a slot.
/// </summary>
public record PartRemovedFromSlot : BuildEvent
{
    public required Guid ChamberId { get; init; }
    public required Guid SlotId { get; init; }
    public required Guid ProductId { get; init; }
}

/// <summary>
/// Event raised when a slot is added to a chamber.
/// </summary>
public record SlotAddedToChamber : BuildEvent
{
    public required Guid ChamberId { get; init; }
    public required Guid SlotId { get; init; }
    public required string SlotName { get; init; }
    public required ProductCategory AllowedCategory { get; init; }
    public required Vector3 RelativePosition { get; init; }
    public required Dimensions MaxDimensions { get; init; }
    public Guid? ParentSlotId { get; init; }
}
