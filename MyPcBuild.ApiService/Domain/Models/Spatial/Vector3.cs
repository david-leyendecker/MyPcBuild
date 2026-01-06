namespace MyPcBuild.ApiService.Domain.Models.Spatial;

/// <summary>
/// Represents a 3D position in space (in millimeters).
/// </summary>
public record Vector3(
    decimal X,
    decimal Y,
    decimal Z
)
{
    public static Vector3 Zero { get; } = new(0, 0, 0);
    
    public static Vector3 operator +(Vector3 a, Vector3 b) =>
        new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    
    public static Vector3 operator -(Vector3 a, Vector3 b) =>
        new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
}
