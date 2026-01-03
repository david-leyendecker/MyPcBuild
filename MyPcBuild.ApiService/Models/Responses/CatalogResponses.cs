namespace MyPcBuild.ApiService.Models.Responses;

/// <summary>
/// Response for a single product
/// </summary>
public record ProductResponse
{
    public required ProductDto Product { get; init; }
    
    /// <summary>
    /// Links to related resources (HATEOAS)
    /// </summary>
    public List<LinkDto> Links { get; init; } = new();
}

/// <summary>
/// Response for category list
/// </summary>
public record CategoryListResponse
{
    public required List<CategoryDto> Categories { get; init; }
    
    /// <summary>
    /// Links to related resources (HATEOAS)
    /// </summary>
    public List<LinkDto> Links { get; init; } = new();
}

/// <summary>
/// Category data transfer object
/// </summary>
public record CategoryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
    public int ProductCount { get; init; }
    
    /// <summary>
    /// Links to related resources
    /// </summary>
    public List<LinkDto> Links { get; init; } = new();
}
