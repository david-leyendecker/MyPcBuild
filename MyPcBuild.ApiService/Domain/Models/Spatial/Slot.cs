using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Domain.Models.Spatial;

/// <summary>
/// Represents a slot where a part can be installed.
/// Slots can contain sub-slots for recursive part installation (e.g., motherboard has CPU/RAM slots).
/// Used as a value object within Product/Chamber definitions.
/// </summary>
public record Slot(
    Guid Id,
    string Name,
    string AllowedCategoryName,
    Vector3 RelativePosition,
    Dimensions MaxDimensions,
    List<Slot>? SubSlots = null
)
{
    public List<Slot> SubSlots { get; init; } = SubSlots ?? [];
    
    /// <summary>
    /// Gets the bounding box for this slot at the given position.
    /// </summary>
    public BoundingBox GetBoundingBox(Vector3 absolutePosition)
    {
        return new BoundingBox(absolutePosition + RelativePosition, MaxDimensions);
    }
    
    /// <summary>
    /// Flattens all sub-slots to global coordinates relative to the given base position.
    /// </summary>
    public List<(Slot Slot, Vector3 GlobalPosition)> FlattenSlots(Vector3 basePosition)
    {
        List<(Slot, Vector3)> result = [(this, basePosition + RelativePosition)];
        
        foreach (Slot subSlot in SubSlots)
        {
            result.AddRange(subSlot.FlattenSlots(basePosition + RelativePosition));
        }
        
        return result;
    }
}
