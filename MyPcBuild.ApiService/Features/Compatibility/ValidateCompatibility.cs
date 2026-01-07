using Marten;
using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Features.Compatibility;

public static class ValidateCompatibility
{
    public static IEndpointRouteBuilder MapValidateCompatibilityEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/compatibility/validate", async (
            ValidateCompatibilityRequest request,
            IDocumentSession session,
            ICompatibilityValidator validator) =>
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
                products.Select(p => new ProductInfo(p.Id, p.Name, p.CategoryName)).ToList()
            );

            return Results.Ok(response);
        })
        .WithName("ValidateCompatibility")
        .WithTags("Compatibility");

        return app;
    }
}

public record ValidateCompatibilityRequest(List<Guid> ProductIds);

public record ValidateCompatibilityResponse(
    bool IsCompatible,
    bool HasErrors,
    bool HasWarnings,
    List<CompatibilityIssueDto> Issues,
    List<ProductInfo> ValidatedProducts
);

public record CompatibilityIssueDto(
    string Message,
    string Severity,
    string Category
);

public record ProductInfo(
    Guid Id,
    string Name,
    string Category
);
