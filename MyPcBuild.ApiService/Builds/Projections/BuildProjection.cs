namespace MyPcBuild.ApiService.Builds.Projections;

public class BuildProjection
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public List<ProjectedPart> Parts { get; set; } = new();
    public decimal TotalPrice => Parts.Sum(p => p.PricePaid);
    public List<CompatibilityIssue> CompatibilityIssues { get; set; } = new();
    public int Version { get; set; }
}

public record ProjectedPart(
    Guid ProductId,
    string ProductName,
    string Category,
    decimal PricePaid
);

public record CompatibilityIssue(
    string Message,
    IssueSeverity Severity
);

public enum IssueSeverity
{
    Warning,
    Error
}
