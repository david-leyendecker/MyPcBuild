using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Features.Compatibility;

public static class ValidateCompatibility
{
    public static IEndpointRouteBuilder MapValidateCompatibilityEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/compatibility/validate", async (
            ValidateCompatibilityRequest request,
            IDocumentSession session,
            ICompatibilityValidator validator,
            IHttpContextAccessor httpContextAccessor) =>
        {
            if (request.ProductIds == null || !request.ProductIds.Any())
            {
                return Results.BadRequest("Product IDs are required");
            }

            // Load all products
            List<Product> products = [];
            foreach (Guid productId in request.ProductIds)
            {
                Product? product = await session.LoadAsync<Product>(productId);
                if (product != null)
                {
                    products.Add(product);
                }
            }

            if (!products.Any())
            {
                return Results.BadRequest("No valid products found");
            }

            // Validate compatibility
            CompatibilityResult result = await validator.ValidateBuild(products);

            string baseUrl = GetBaseUrl(httpContextAccessor);

            // Map to response DTO
            ValidateCompatibilityResponse response = new(
                result.IsCompatible,
                result.HasErrors,
                result.HasWarnings,
                result.Issues.Select(i => new CompatibilityIssueDto(
                    i.Message,
                    i.Severity.ToString(),
                    i.Category
                )).ToList(),
                products.Select(p => new ProductInfo(
                    p.Id,
                    p.Name,
                    p.ProductCategory,
                    [
                        new HateoasLink($"{baseUrl}/api/catalog/products/{p.Id}", "product", "GET")
                    ]
                )).ToList(),
                [
                    new HateoasLink($"{baseUrl}/api/compatibility/validate", "self", "POST"),
                    new HateoasLink($"{baseUrl}/api/catalog/products", "catalog", "GET")
                ]
            );

            return Results.Ok(response);
        })
        .WithName("ValidateCompatibility")
        .WithTags("Compatibility");

        return app;
    }

    private static string GetBaseUrl(IHttpContextAccessor httpContextAccessor)
    {
        HttpRequest request = httpContextAccessor.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}";
    }
}

public record ValidateCompatibilityRequest(List<Guid> ProductIds);

public record ValidateCompatibilityResponse(
    bool IsCompatible,
    bool HasErrors,
    bool HasWarnings,
    List<CompatibilityIssueDto> Issues,
    List<ProductInfo> ValidatedProducts,
    List<HateoasLink> Links
);

public record CompatibilityIssueDto(
    string Message,
    string Severity,
    ProductCategory Category
);

public record ProductInfo(
    Guid Id,
    string Name,
    ProductCategory Category,
    List<HateoasLink> Links
);
