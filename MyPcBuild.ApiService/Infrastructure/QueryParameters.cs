using System.ComponentModel.DataAnnotations;

namespace MyPcBuild.ApiService.Infrastructure;

/// <summary>
/// Common query parameters for paginated, searchable, and sortable endpoints.
/// </summary>
public class QueryParameters
{
    /// <summary>
    /// Page number (1-based).
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1.")]
    public int Page { get; init; } = 1;

    /// <summary>
    /// Number of items per page.
    /// </summary>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
    public int ItemsPerPage { get; init; } = 10;

    /// <summary>
    /// Search term to filter results.
    /// </summary>
    public string? Search { get; init; }
    
    /// <summary>
    /// Field to sort by.
    /// </summary>
    public string? SortBy { get; init; }

    /// <summary>
    /// Sort in descending order.
    /// </summary>
    public bool SortDesc { get; init; }

    /// <summary>
    /// Category filter.
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Calculates how many items to skip for pagination.
    /// </summary>
    public int GetSkip() => (Page - 1) * ItemsPerPage;
}
