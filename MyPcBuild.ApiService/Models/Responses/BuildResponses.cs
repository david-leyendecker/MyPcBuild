namespace MyPcBuild.ApiService.Models.Responses;

/// <summary>
/// Response for build creation
/// </summary>
public record BuildCreatedResponse
{
    public required Guid BuildId { get; init; }
    public required string Name { get; init; }
    public required Guid UserId { get; init; }
    public required DateTime CreatedAt { get; init; }
    
    /// <summary>
    /// Links to related resources (HATEOAS)
    /// </summary>
    public List<LinkDto> Links { get; init; } = new();
}

/// <summary>
/// Response for build details
/// </summary>
public record BuildResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required Guid UserId { get; init; }
    public required List<BuildPartDto> Parts { get; init; }
    public required decimal TotalPrice { get; init; }
    public required int Version { get; init; }
    
    /// <summary>
    /// Quick compatibility status
    /// </summary>
    public CompatibilityStatusDto? CompatibilityStatus { get; init; }
    
    /// <summary>
    /// Links to related resources (HATEOAS)
    /// </summary>
    public List<LinkDto> Links { get; init; } = new();
}

/// <summary>
/// Build part data transfer object
/// </summary>
public record BuildPartDto
{
    public required Guid ProductId { get; init; }
    public required string ProductName { get; init; }
    public required string Category { get; init; }
    public required decimal PricePaid { get; init; }
    public required string Manufacturer { get; init; }
    
    /// <summary>
    /// Links to related resources
    /// </summary>
    public List<LinkDto> Links { get; init; } = new();
}

/// <summary>
/// Quick compatibility status summary
/// </summary>
public record CompatibilityStatusDto
{
    public required bool IsCompatible { get; init; }
    public required int ErrorCount { get; init; }
    public required int WarningCount { get; init; }
}
