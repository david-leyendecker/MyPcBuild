using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Infrastructure;
using System.Linq.Expressions;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class GetProducts
{
    private static readonly Dictionary<string, Expression<Func<Product, object>>> _sortKeySelectors = new(StringComparer.OrdinalIgnoreCase)
    {
        [nameof(Product.Name)] = p => p.Name,
        [nameof(Product.CategoryName)] = p => p.CategoryName,
        [nameof(Product.Price)] = p => p.Price,
        [nameof(Product.Manufacturer)] = p => p.Manufacturer
    };

    public static IEndpointRouteBuilder MapGetProductsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalog/products", async (
            IDocumentSession session,
            [AsParameters] QueryParameters queryParams) =>
        {
            IQueryable<Product> query = session.Query<Product>();

            // Apply category filter
            if (!string.IsNullOrWhiteSpace(queryParams.Category))
            {
                query = query.Where(p => p.CategoryName == queryParams.Category);
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                query = query.Where(p => p.Name.Contains(queryParams.Search) || p.Manufacturer.Contains(queryParams.Search));
            }

            // Get total count before pagination
            int totalCount = await query.CountAsync();

            // Apply sorting (always ensure ordering for pagination correctness)
            string sortBy = queryParams.SortBy ?? "name";
            query = ApplySorting(query, sortBy, queryParams.SortDesc);

            // Apply pagination
            IReadOnlyList<Product> productResults = await query
                .Skip(queryParams.GetSkip())
                .Take(queryParams.ItemsPerPage)
                .ToListAsync();

            PaginationMetadata pagination = new()
            {
                Total = totalCount,
                Page = queryParams.Page,
                ItemsPerPage = queryParams.ItemsPerPage
            };

            GetProductsResponse response = new(
                productResults.Select(p => new ProductSummary(
                    p.Id,
                    p.Name,
                    p.CategoryName,
                    p.Price,
                    p.Manufacturer
                )).ToList(),
                pagination
            );

            return Results.Ok(response);
        })
        .WithName("GetProducts")
        .WithTags("Catalog");

        return app;
    }

    private static IQueryable<Product> ApplySorting(IQueryable<Product> query, string sortBy, bool sortDesc)
    {
        Expression<Func<Product, object>> keySelector = _sortKeySelectors.TryGetValue(sortBy, out Expression<Func<Product, object>>? selector)
            ? selector
            : _sortKeySelectors[nameof(Product.Name)];

        return sortDesc
            ? query.OrderByDescending(keySelector).ThenBy(p => p.Name)
            : query.OrderBy(keySelector).ThenBy(p => p.Name);
    }
}

public record GetProductsResponse(
    List<ProductSummary> Items,
    PaginationMetadata Pagination
);

public record ProductSummary(
    Guid Id,
    string Name,
    string CategoryName,
    decimal Price,
    string Manufacturer
);
