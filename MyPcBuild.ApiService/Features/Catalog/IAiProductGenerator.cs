using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Features.Catalog;

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
    Task<Product> GenerateProductAsync(string category, string description, CancellationToken cancellationToken = default);
}
