using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Features.Compatibility;

public static class GetBuildCompatibility
{
    public static IEndpointRouteBuilder MapGetBuildCompatibilityEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/builds/{buildId:guid}/compatibility", async (
            Guid buildId,
            IDocumentSession session,
            ICompatibilityValidator validator,
            IHttpContextAccessor httpContextAccessor) =>
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

            string baseUrl = httpContextAccessor.GetBaseUrl();

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
                products.Select(p => new ProductInfo(
                    p.Id,
                    p.Name,
                    p.ProductCategory,
                    [
                        new HateoasLink(new Uri($"{baseUrl}/api/catalog/products/{p.Id}"), "product", Infrastructure.HttpMethod.GET)
                    ]
                )).ToList(),
                [
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/compatibility"), "self", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}"), "build", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/parts"), "add-part", Infrastructure.HttpMethod.POST),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products"), "catalog", Infrastructure.HttpMethod.GET)
                ]
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
    List<ProductInfo> Products,
    List<HateoasLink> Links
);
