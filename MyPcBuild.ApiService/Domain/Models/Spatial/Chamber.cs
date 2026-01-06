using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Domain.Models.Spatial;

/// <summary>
/// Represents a 3D chamber (container) that can hold parts via slots.
/// Used as a value object within Product definitions (e.g., PC Case).
/// </summary>
public record Chamber(
    Guid Id,
    string Name,
    Dimensions Dimensions,
    List<Slot> Slots
)
{
    /// <summary>
    /// Gets the bounding box for this chamber (always at origin).
    /// </summary>
    public BoundingBox GetBoundingBox()
    {
        return new BoundingBox(Vector3.Zero, Dimensions);
    }
    
    /// <summary>
    /// Gets all slots flattened to global coordinates.
    /// </summary>
    public List<(Slot Slot, Vector3 GlobalPosition)> GetAllSlots()
    {
        List<(Slot, Vector3)> result = [];
        
        foreach (Slot slot in Slots)
        {
            result.AddRange(slot.FlattenSlots(Vector3.Zero));
        }
        
        return result;
    }
}
