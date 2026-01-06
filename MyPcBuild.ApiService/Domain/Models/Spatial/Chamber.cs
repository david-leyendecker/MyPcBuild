namespace MyPcBuild.ApiService.Domain.Models.Spatial;

/// <summary>
/// Represents a 3D chamber (container) that can hold parts via slots.
/// Used for PC cases and other spatial containers.
/// </summary>
public class Chamber
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required Dimensions Dimensions { get; set; }
    public List<Slot> Slots { get; set; } = [];
    public List<InstalledPart> InstalledParts { get; set; } = [];
    
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
