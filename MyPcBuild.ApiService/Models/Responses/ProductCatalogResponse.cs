namespace MyPcBuild.ApiService.Models.Responses;

/// <summary>
/// Response for paginated product catalog
/// </summary>
public record ProductCatalogResponse
{
    /// <summary>
    /// Total number of products matching the query
    /// </summary>
    public required int TotalCount { get; init; }
    
    /// <summary>
    /// Total number of pages available
    /// </summary>
    public required int TotalPages { get; init; }
    
    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public required int CurrentPage { get; init; }
    
    /// <summary>
    /// Number of items per page
    /// </summary>
    public required int PageSize { get; init; }
    
    /// <summary>
    /// Products in the current page
    /// </summary>
    public required List<ProductDto> Products { get; init; }
    
    /// <summary>
    /// Applied filters
    /// </summary>
    public FilterInfoDto? Filters { get; init; }
    
    /// <summary>
    /// Links to related resources (HATEOAS)
    /// </summary>
    public List<LinkDto> Links { get; init; } = new();
}

/// <summary>
/// Product data transfer object
/// </summary>
public record ProductDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Category { get; init; }
    public required int CategoryId { get; init; }
    public required decimal Price { get; init; }
    public required string Manufacturer { get; init; }
    public required Dictionary<string, object> Specifications { get; init; }
    
    /// <summary>
    /// Links to related resources
    /// </summary>
    public List<LinkDto> Links { get; init; } = new();
}

/// <summary>
/// Filter information applied to the query
/// </summary>
public record FilterInfoDto
{
    public string? Category { get; init; }
    public string? SearchTerm { get; init; }
    public string? Manufacturer { get; init; }
}
