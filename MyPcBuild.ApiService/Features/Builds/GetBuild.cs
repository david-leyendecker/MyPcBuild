using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Features.Compatibility;

namespace MyPcBuild.ApiService.Features.Builds;

public static class GetBuild
{
    public static IEndpointRouteBuilder MapGetBuildEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/builds/{buildId:guid}", async (
            Guid buildId,
            IDocumentSession session,
            ICompatibilityValidator validator) =>
        {
            Build? build = await session.Events.AggregateStreamAsync<Build>(buildId);
            if (build is null)
            {
                return Results.NotFound();
            }

            // Load products for the build
            List<ProductDetails> productDetails = [];
            foreach (BuildPart part in build.Parts)
            {
                Product? product = await session.LoadAsync<Product>(part.ProductId);
                if (product != null)
                {
                    productDetails.Add(new ProductDetails(
                        product.Id,
                        product.Name,
                        product.Category,
                        product.Manufacturer,
                        part.PricePaid
                    ));
                }
            }

            // Run compatibility validation
            List<Product> products = [];
            foreach (BuildPart part in build.Parts)
            {
                Product? product = await session.LoadAsync<Product>(part.ProductId);
                if (product != null)
                {
                    products.Add(product);
                }
            }

            CompatibilityResult? compatibilityResult = null;
            if (products.Any())
            {
                compatibilityResult = await validator.ValidateBuild(products);
            }

            GetBuildResponse response = new(
                build.Id,
                build.Name,
                build.UserId,
                productDetails,
                compatibilityResult?.IsCompatible ?? true,
                compatibilityResult?.Issues.Select(i => new CompatibilityIssueDto(
                    i.Message,
                    i.Severity.ToString(),
                    i.Category
                )).ToList() ?? [],
                DateTimeOffset.UtcNow
            );

            return Results.Ok(response);
        })
        .WithName("GetBuild")
        .WithTags("Builds");

        return app;
    }
}

public record GetBuildResponse(
    Guid Id,
    string Name,
    Guid UserId,
    List<ProductDetails> Parts,
    bool IsCompatible,
    List<CompatibilityIssueDto> CompatibilityIssues,
    DateTimeOffset CreatedAt
);

public record ProductDetails(
    Guid Id,
    string Name,
    ProductCategory Category,
    string Manufacturer,
    decimal PricePaid
);

public record CompatibilityIssueDto(
    string Message,
    string Severity,
    string Category
);
