using Marten;
using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class GetProducts
{
    public static IEndpointRouteBuilder MapGetProductsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalog/products", async (
            IDocumentSession session,
            string? category = null,
            string? search = null,
            int page = 1,
            int itemsPerPage = 10,
            string sortBy = "name",
            bool sortDesc = false) =>
        {
            // Validate parameters
            if (page < 1)
            {
                return Results.BadRequest(new { error = "Page must be greater than or equal to 1" });
            }

            if (itemsPerPage < 1 || itemsPerPage > 100)
            {
                return Results.BadRequest(new { error = "ItemsPerPage must be between 1 and 100" });
            }

            IQueryable<Product> query = session.Query<Product>();

            // Apply category filter
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.CategoryName == category);
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Manufacturer.Contains(search));
            }

            // Get total count before pagination
            int totalCount = await query.CountAsync();

            // Apply sorting (always ensure ordering for pagination correctness)
            query = ApplySorting(query, sortBy, sortDesc);

            // Apply pagination
            IReadOnlyList<Product> productResults = await query
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();
            
            GetProductsResponse response = new(
                productResults.Select(p => new ProductSummary(
                    p.Id,
                    p.Name,
                    p.CategoryName,
                    p.Price,
                    p.Manufacturer
                )).ToList(),
                totalCount,
                page,
                itemsPerPage,
                sortBy,
                sortDesc,
                category,
                search
            );

            return Results.Ok(response);
        })
        .WithName("GetProducts")
        .WithTags("Catalog");

        return app;
    }

    private static IQueryable<Product> ApplySorting(IQueryable<Product> query, string sortBy, bool sortDesc)
    {
        // Normalize sortBy to lowercase for case-insensitive comparison
        string sortField = sortBy.ToLowerInvariant();

        return sortField switch
        {
            "name" => sortDesc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "category" or "categoryname" => sortDesc 
                ? query.OrderByDescending(p => p.CategoryName).ThenBy(p => p.Name)
                : query.OrderBy(p => p.CategoryName).ThenBy(p => p.Name),
            "price" => sortDesc 
                ? query.OrderByDescending(p => p.Price).ThenBy(p => p.Name)
                : query.OrderBy(p => p.Price).ThenBy(p => p.Name),
            "manufacturer" => sortDesc 
                ? query.OrderByDescending(p => p.Manufacturer).ThenBy(p => p.Name)
                : query.OrderBy(p => p.Manufacturer).ThenBy(p => p.Name),
            // Default to name sorting if invalid sortBy value
            _ => query.OrderBy(p => p.Name)
        };
    }
}

public record GetProductsResponse(
    List<ProductSummary> Items,
    int Total,
    int Page,
    int ItemsPerPage,
    string SortBy,
    bool SortDesc,
    string? Category,
    string? Search
);

public record ProductSummary(
    Guid Id,
    string Name,
    string CategoryName,
    decimal Price,
    string Manufacturer
);
