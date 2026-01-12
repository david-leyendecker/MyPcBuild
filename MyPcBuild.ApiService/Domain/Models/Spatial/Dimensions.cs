namespace MyPcBuild.ApiService.Domain.Models.Spatial;

/// <summary>
/// Represents 3D dimensions (Length, Width, Height) in millimeters.
/// </summary>
public record struct Dimensions(
    decimal Length,
    decimal Width,
    decimal Height
)
{
    public static readonly Dimensions Zero = new(0, 0, 0);

    /// <summary>
    /// Checks if these dimensions fit within the given container dimensions.
    /// </summary>
    public bool FitsWithin(Dimensions container)
    {
        return Length <= container.Length && Width <= container.Width && Height <= container.Height;
    }
}