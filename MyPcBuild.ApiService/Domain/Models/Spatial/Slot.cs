using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Domain.Models.Spatial;

/// <summary>
/// Represents a slot where a part can be installed.
/// Slots can contain sub-slots for recursive part installation (e.g., motherboard has CPU/RAM slots).
/// </summary>
public class Slot
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required ProductCategory AllowedCategory { get; set; }
    public required Vector3 RelativePosition { get; set; }
    public required Dimensions MaxDimensions { get; set; }
    public Guid? InstalledPartId { get; set; }
    public List<Slot> SubSlots { get; set; } = [];
    
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
