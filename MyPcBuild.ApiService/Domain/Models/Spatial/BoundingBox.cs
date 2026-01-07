namespace MyPcBuild.ApiService.Domain.Models.Spatial;

/// <summary>
/// Represents an Axis-Aligned Bounding Box (AABB) in 3D space.
/// </summary>
public record BoundingBox(
    Vector3 Position,
    Dimensions Size
)
{
    /// <summary>
    /// Gets the minimum corner of the bounding box.
    /// </summary>
    public Vector3 Min => Position;

    /// <summary>
    /// Gets the maximum corner of the bounding box.
    /// </summary>
    public Vector3 Max => new(
        Position.X + Size.Length,
        Position.Y + Size.Width,
        Position.Z + Size.Height
    );

    /// <summary>
    /// Checks if this bounding box intersects with another bounding box.
    /// Uses AABB collision detection.
    /// </summary>
    public bool Intersects(BoundingBox other)
    {
        Vector3 thisMin = Min;
        Vector3 thisMax = Max;
        Vector3 otherMin = other.Min;
        Vector3 otherMax = other.Max;

        return thisMin.X < otherMax.X && thisMax.X > otherMin.X &&
               thisMin.Y < otherMax.Y && thisMax.Y > otherMin.Y &&
               thisMin.Z < otherMax.Z && thisMax.Z > otherMin.Z;
    }

    /// <summary>
    /// Checks if this bounding box is completely contained within another bounding box.
    /// </summary>
    public bool IsContainedWithin(BoundingBox container)
    {
        Vector3 thisMin = Min;
        Vector3 thisMax = Max;
        Vector3 containerMin = container.Min;
        Vector3 containerMax = container.Max;

        return thisMin.X >= containerMin.X && thisMax.X <= containerMax.X &&
               thisMin.Y >= containerMin.Y && thisMax.Y <= containerMax.Y &&
               thisMin.Z >= containerMin.Z && thisMax.Z <= containerMax.Z;
    }
}
