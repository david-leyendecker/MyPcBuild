namespace MyPcBuild.ApiService.Infrastructure.Hateoas;

/// <summary>
/// Represents a hypermedia link in a HATEOAS response.
/// </summary>
public record Link
{
    /// <summary>
    /// The URI of the linked resource.
    /// </summary>
    public required string Href { get; init; }

    /// <summary>
    /// The relationship of the link to the current resource.
    /// </summary>
    public required string Rel { get; init; }

    /// <summary>
    /// The HTTP method to use when accessing this link.
    /// </summary>
    public required string Method { get; init; }
}
