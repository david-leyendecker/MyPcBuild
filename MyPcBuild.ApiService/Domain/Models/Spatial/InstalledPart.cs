namespace MyPcBuild.ApiService.Domain.Models.Spatial;

/// <summary>
/// Represents a part installed in a specific slot within a chamber.
/// </summary>
public class InstalledPart
{
    public Guid Id { get; set; }
    public required Guid ProductId { get; set; }
    public required Guid SlotId { get; set; }
    public required Vector3 Position { get; set; }
    public required Dimensions Dimensions { get; set; }
    
    /// <summary>
    /// Gets the bounding box for this installed part.
    /// </summary>
    public BoundingBox GetBoundingBox()
    {
        return new BoundingBox(Position, Dimensions);
    }
}
