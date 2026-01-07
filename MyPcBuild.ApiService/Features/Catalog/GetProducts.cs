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
            int pageSize = 20) =>
        {
            IQueryable<Product> query = session.Query<Product>();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.CategoryName == category);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Manufacturer.Contains(search));
            }

            int totalCount = await query.CountAsync();
            IReadOnlyList<Product> productResults = await query
                .OrderBy(p => p.CategoryName)
                .ThenBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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
                pageSize,
                category,
                search
            );

            return Results.Ok(response);
        })
        .WithName("GetProducts")
        .WithTags("Catalog");

        return app;
    }
}

public record GetProductsResponse(
    List<ProductSummary> Products,
    int TotalCount,
    int CurrentPage,
    int PageSize,
    string? FilteredCategory,
    string? SearchTerm
);

public record ProductSummary(
    Guid Id,
    string Name,
    string CategoryName,
    decimal Price,
    string Manufacturer
);
