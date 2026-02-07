namespace MyPcBuild.ApiService.Infrastructure.Hateoas;

/// <summary>
/// Default implementation of ILinkGenerator using HttpContext to generate absolute URLs.
/// </summary>
public class HateoasLinkGenerator : ILinkGenerator
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HateoasLinkGenerator(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Link CreateLink(string path, string rel, string method = "GET")
    {
        string absoluteUrl = path.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? path
            : GetAbsoluteUrl(path);

        return new Link
        {
            Href = absoluteUrl,
            Rel = rel,
            Method = method
        };
    }

    public string GetBaseUrl()
    {
        HttpRequest? request = _httpContextAccessor.HttpContext?.Request;
        if (request == null)
        {
            return string.Empty;
        }

        return $"{request.Scheme}://{request.Host}";
    }

    private string GetAbsoluteUrl(string relativePath)
    {
        HttpRequest? request = _httpContextAccessor.HttpContext?.Request;
        if (request == null)
        {
            return relativePath;
        }

        string baseUrl = GetBaseUrl();
        string path = relativePath.StartsWith('/') ? relativePath : $"/{relativePath}";
        
        return $"{baseUrl}{path}";
    }
}
