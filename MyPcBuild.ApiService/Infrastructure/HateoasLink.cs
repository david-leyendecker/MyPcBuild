namespace MyPcBuild.ApiService.Infrastructure;

/// <summary>
/// Represents a HATEOAS hypermedia link in API responses.
/// </summary>
/// <param name="Href">The URL of the linked resource.</param>
/// <param name="Rel">The relationship type (e.g., "self", "add-part", "validate").</param>
/// <param name="Method">The HTTP method to use (e.g., "GET", "POST", "DELETE").</param>
public record HateoasLink(string Href, string Rel, string Method);
