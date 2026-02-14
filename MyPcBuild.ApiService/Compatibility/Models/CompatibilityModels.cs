using MyPcBuild.ApiService.Catalog.Models;

namespace MyPcBuild.ApiService.Compatibility.Models;

public record CompatibilityIssue(
    string Message,
    IssueSeverity Severity,
    ProductCategory Category
);

public enum IssueSeverity
{
    Warning,
    Error
}

public record CompatibilityResult(
    bool IsCompatible,
    List<CompatibilityIssue> Issues
)
{
    public bool HasErrors => Issues.Any(i => i.Severity == IssueSeverity.Error);
    public bool HasWarnings => Issues.Any(i => i.Severity == IssueSeverity.Warning);
}
