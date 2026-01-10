namespace MyPcBuild.ApiService.Infrastructure;

/// <summary>
/// Pagination metadata for responses.
/// </summary>
public record PaginationMetadata
{
    /// <summary>
    /// Total number of items matching the query.
    /// </summary>
    public int Total { get; init; }

    /// <summary>
    /// Current page number (1-based).
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int ItemsPerPage { get; init; }

    /// <summary>
    /// Total number of pages available.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)Total / ItemsPerPage);

    /// <summary>
    /// Indicates if there is a next page.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Indicates if there is a previous page.
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}
