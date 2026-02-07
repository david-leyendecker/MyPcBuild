namespace MyPcBuild.ApiService.Infrastructure;

/// <summary>
/// HTTP methods used in HATEOAS links.
/// </summary>
public enum HttpMethod
{
    GET,
    POST,
    PUT,
    DELETE,
    PATCH
}

/// <summary>
/// Represents a HATEOAS hypermedia link in API responses.
/// </summary>
/// <param name="Href">The URL of the linked resource.</param>
/// <param name="Rel">The relationship type (e.g., "self", "add-part", "validate").</param>
/// <param name="Method">The HTTP method to use.</param>
public record HateoasLink(Uri Href, string Rel, HttpMethod Method);

/// <summary>
/// Extension methods for <see cref="IHttpContextAccessor"/> to support HATEOAS link generation.
/// </summary>
public static class HttpContextAccessorExtensions
{
    /// <summary>
    /// Gets the base URL (scheme + host) from the current HTTP request.
    /// </summary>
    public static string GetBaseUrl(this IHttpContextAccessor httpContextAccessor)
    {
        HttpRequest request = httpContextAccessor.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}";
    }
}
