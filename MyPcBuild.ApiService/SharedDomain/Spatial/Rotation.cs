namespace MyPcBuild.ApiService.SharedDomain.Spatial;

/// <summary>
/// Represents a 3D rotation using Euler angles (in degrees).
/// Rotations are applied in the order: Y (yaw), X (pitch), Z (roll).
/// </summary>
public record Rotation(
    decimal X,
    decimal Y,
    decimal Z
)
{
    /// <summary>
    /// No rotation (identity).
    /// </summary>
    public static Rotation Identity { get; } = new(0, 0, 0);

    /// <summary>
    /// Rotation of 90 degrees around Y axis (common for motherboard in case).
    /// </summary>
    public static Rotation Rotate90Y { get; } = new(0, 90, 0);

    /// <summary>
    /// Rotation of 180 degrees around Y axis.
    /// </summary>
    public static Rotation Rotate180Y { get; } = new(0, 180, 0);

    /// <summary>
    /// Rotation of 270 degrees around Y axis.
    /// </summary>
    public static Rotation Rotate270Y { get; } = new(0, 270, 0);
}
