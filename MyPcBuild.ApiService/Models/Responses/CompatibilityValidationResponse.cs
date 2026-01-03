namespace MyPcBuild.ApiService.Models.Responses;

/// <summary>
/// Response object for compatibility validation results
/// </summary>
public record CompatibilityValidationResponse
{
    /// <summary>
    /// Whether the build is compatible (no errors)
    /// </summary>
    public required bool IsCompatible { get; init; }
    
    /// <summary>
    /// Whether the validation found any errors
    /// </summary>
    public required bool HasErrors { get; init; }
    
    /// <summary>
    /// Whether the validation found any warnings
    /// </summary>
    public required bool HasWarnings { get; init; }
    
    /// <summary>
    /// List of compatibility issues found
    /// </summary>
    public required List<CompatibilityIssueDto> Issues { get; init; }
    
    /// <summary>
    /// Summary of components validated
    /// </summary>
    public ComponentSummaryDto? ComponentSummary { get; init; }
    
    /// <summary>
    /// Links to related resources (HATEOAS)
    /// </summary>
    public List<LinkDto> Links { get; init; } = new();
}

/// <summary>
/// Represents a single compatibility issue
/// </summary>
public record CompatibilityIssueDto
{
    /// <summary>
    /// Human-readable description of the issue
    /// </summary>
    public required string Message { get; init; }
    
    /// <summary>
    /// Severity level: Error or Warning
    /// </summary>
    public required string Severity { get; init; }
    
    /// <summary>
    /// Category of the compatibility check (e.g., "CPU/Motherboard")
    /// </summary>
    public required string Category { get; init; }
    
    /// <summary>
    /// Recommended action to resolve the issue
    /// </summary>
    public string? Recommendation { get; init; }
}

/// <summary>
/// Summary of components included in the validation
/// </summary>
public record ComponentSummaryDto
{
    public int TotalComponents { get; init; }
    public Dictionary<string, int> ComponentsByCategory { get; init; } = new();
    public bool HasCpu { get; init; }
    public bool HasMotherboard { get; init; }
    public bool HasGpu { get; init; }
    public bool HasRam { get; init; }
    public bool HasCase { get; init; }
    public bool HasPsu { get; init; }
    public bool HasCooler { get; init; }
    public bool HasStorage { get; init; }
}

/// <summary>
/// HATEOAS link for related resources
/// </summary>
public record LinkDto
{
    public required string Href { get; init; }
    public required string Rel { get; init; }
    public required string Method { get; init; }
}
