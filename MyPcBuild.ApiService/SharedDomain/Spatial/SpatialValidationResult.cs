namespace MyPcBuild.ApiService.SharedDomain.Spatial;

/// <summary>
/// Represents a spatial validation issue.
/// </summary>
public record SpatialIssue(
    string Message,
    SpatialIssueSeverity Severity,
    string Category
);

public enum SpatialIssueSeverity
{
    Warning,
    Error
}

/// <summary>
/// Represents the result of a spatial validation.
/// </summary>
public record SpatialValidationResult(
    bool IsValid,
    List<SpatialIssue> Issues
)
{
    public bool HasErrors => Issues.Any(i => i.Severity == SpatialIssueSeverity.Error);
    public bool HasWarnings => Issues.Any(i => i.Severity == SpatialIssueSeverity.Warning);
}
