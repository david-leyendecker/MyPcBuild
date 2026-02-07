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
        [nameof(Product.ProductCategory)] = p => p.ProductCategory,
        [nameof(Product.Price)] = p => p.Price,
        [nameof(Product.Manufacturer)] = p => p.Manufacturer
    };

    private static readonly Dictionary<string, Func<IQueryable<Product>, string, IQueryable<Product>>> _filterFunctions = new(StringComparer.OrdinalIgnoreCase)
    {
        [nameof(Product.ProductCategory)] = (q, v) => 
        {
            if (Enum.TryParse<ProductCategory>(v, ignoreCase: true, out ProductCategory category))
            {
                return q.Where(p => p.ProductCategory == category);
            }
            return q;
        },
        [nameof(Product.IsDraft)] = (q, v) => bool.TryParse(v, out bool isDraft) ? q.Where(p => p.IsDraft == isDraft) : q
    };

    public static IEndpointRouteBuilder MapGetProductsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalog/products", async (
            IDocumentSession session,
            IHttpContextAccessor httpContextAccessor,
            [AsParameters] QueryParameters queryParams) =>
        {
            IQueryable<Product> query = session.Query<Product>();

            // Apply generic filters
            query = ApplyFilters(query, queryParams.Filters);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                query = query.Where(p => p.Name.Contains(queryParams.Search, StringComparison.InvariantCultureIgnoreCase)
                    || p.Manufacturer.Contains(queryParams.Search, StringComparison.InvariantCultureIgnoreCase));
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

            string baseUrl = GetBaseUrl(httpContextAccessor);

            List<HateoasLink> links =
            [
                new HateoasLink($"{baseUrl}/api/catalog/products?page={queryParams.Page}&itemsPerPage={queryParams.ItemsPerPage}", "self", "GET"),
                new HateoasLink($"{baseUrl}/api/catalog/categories", "categories", "GET"),
                new HateoasLink($"{baseUrl}/api/catalog/products", "create-product", "POST")
            ];

            if (pagination.HasNextPage)
            {
                links.Add(new HateoasLink($"{baseUrl}/api/catalog/products?page={queryParams.Page + 1}&itemsPerPage={queryParams.ItemsPerPage}", "next", "GET"));
            }

            if (pagination.HasPreviousPage)
            {
                links.Add(new HateoasLink($"{baseUrl}/api/catalog/products?page={queryParams.Page - 1}&itemsPerPage={queryParams.ItemsPerPage}", "prev", "GET"));
            }

            GetProductsResponse response = new(
                productResults.Select(p => new ProductSummary(
                    p.Id,
                    p.Name,
                    p.ProductCategory.ToString(),
                    p.Price,
                    p.Manufacturer,
                    p.IsDraft,
                    p.PublishedAt,
                    [
                        new HateoasLink($"{baseUrl}/api/catalog/products/{p.Id}", "self", "GET"),
                        new HateoasLink($"{baseUrl}/api/catalog/products?filters=ProductCategory={p.ProductCategory}", "category", "GET")
                    ]
                )).ToList(),
                pagination,
                links
            );

            return Results.Ok(response);
        })
        .WithName("GetProducts")
        .WithTags("Catalog");

        return app;
    }

    private static IQueryable<Product> ApplyFilters(IQueryable<Product> query, string? filtersString)
    {
        if (string.IsNullOrWhiteSpace(filtersString))
        {
            return query;
        }

        string[] filterPairs = filtersString.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (string filterPair in filterPairs)
        {
            string[] parts = filterPair.Split('=', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                continue;
            }

            string fieldName = parts[0].Trim();
            string filterValue = parts[1].Trim();

            if (!_filterFunctions.TryGetValue(fieldName, out Func<IQueryable<Product>, string, IQueryable<Product>>? filterFunction))
            {
                throw new InvalidOperationException($"Filter '{fieldName}' is not supported. Supported filters: {string.Join(", ", _filterFunctions.Keys)}.");
            }

            query = filterFunction(query, filterValue);
        }

        return query;
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

    private static string GetBaseUrl(IHttpContextAccessor httpContextAccessor)
    {
        HttpRequest request = httpContextAccessor.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}";
    }
}

public record GetProductsResponse(
    List<ProductSummary> Items,
    PaginationMetadata Pagination,
    List<HateoasLink> Links
);

public record ProductSummary(
    Guid Id,
    string Name,
    string CategoryName,
    decimal Price,
    string Manufacturer,
    bool IsDraft,
    DateTime? PublishedAt,
    List<HateoasLink> Links
);
