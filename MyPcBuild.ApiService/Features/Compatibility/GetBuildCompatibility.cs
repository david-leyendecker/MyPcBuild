using Marten;
using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Features.Compatibility;

public static class GetBuildCompatibility
{
    public static IEndpointRouteBuilder MapGetBuildCompatibilityEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/builds/{buildId:guid}/compatibility", async (
            Guid buildId,
            IDocumentSession session,
            ICompatibilityValidator validator) =>
        {
            // Load build
            Build? build = await session.Events.AggregateStreamAsync<Build>(buildId);
            if (build == null)
            {
                return Results.NotFound();
            }

            // Load all products in the build
            List<Product> products = [];
            foreach (BuildPart part in build.Parts)
            {
                Product? product = await session.LoadAsync<Product>(part.ProductId);
                if (product != null)
                {
                    products.Add(product);
                }
            }

            // Validate compatibility
            CompatibilityResult result = await validator.ValidateBuild(products);
            
            // Map to response DTO with build context
            GetBuildCompatibilityResponse response = new(
                buildId,
                build.Name,
                result.IsCompatible,
                result.HasErrors,
                result.HasWarnings,
                result.Issues.Select(i => new CompatibilityIssueDto(
                    i.Message,
                    i.Severity.ToString(),
                    i.Category
                )).ToList(),
                products.Select(p => new ProductInfo(p.Id, p.Name, p.Category.ToString())).ToList()
            );

            return Results.Ok(response);
        })
        .WithName("GetBuildCompatibility")
        .WithTags("Compatibility");

        return app;
    }
}

public record GetBuildCompatibilityResponse(
    Guid BuildId,
    string BuildName,
    bool IsCompatible,
    bool HasErrors,
    bool HasWarnings,
    List<CompatibilityIssueDto> Issues,
    List<ProductInfo> Products
);
