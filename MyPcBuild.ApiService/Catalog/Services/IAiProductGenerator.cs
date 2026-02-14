using MyPcBuild.ApiService.Catalog.Models;

namespace MyPcBuild.ApiService.Catalog.Services;

/// <summary>
/// Service for generating product data using AI.
/// </summary>
public interface IAiProductGenerator
{
    /// <summary>
    /// Generates a product from a natural language description using AI.
    /// </summary>
    /// <param name="category">The product category (CPU, GPU, etc.)</param>
    /// <param name="description">Natural language description of the product</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A draft product generated from the AI response</returns>
    Task<Product> GenerateProductAsync(ProductCategory category, string description, CancellationToken cancellationToken = default);
}
