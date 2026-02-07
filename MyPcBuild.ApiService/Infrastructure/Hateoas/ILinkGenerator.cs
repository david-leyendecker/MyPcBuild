namespace MyPcBuild.ApiService.Infrastructure.Hateoas;

/// <summary>
/// Service for generating HATEOAS links with absolute URLs.
/// </summary>
public interface ILinkGenerator
{
    /// <summary>
    /// Creates a link with the given path, relation, and HTTP method.
    /// </summary>
    /// <param name="path">Relative or absolute path to the resource.</param>
    /// <param name="rel">The relationship of the link to the current resource.</param>
    /// <param name="method">The HTTP method to use when accessing this link.</param>
    /// <returns>A Link object with an absolute URL.</returns>
    Link CreateLink(string path, string rel, string method = "GET");

    /// <summary>
    /// Gets the base URL (scheme + host) for the current request.
    /// </summary>
    /// <returns>Base URL string (e.g., "https://api.example.com").</returns>
    string GetBaseUrl();
}
