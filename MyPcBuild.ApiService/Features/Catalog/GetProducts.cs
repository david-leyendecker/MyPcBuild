using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class GetProducts
{
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
    PaginationMetadata Pagination
);

public record ProductSummary(
    Guid Id,
    string Name,
    string CategoryName,
    decimal Price,
    string Manufacturer
);
