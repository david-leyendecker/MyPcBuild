using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Features.Spatial;

public static class ValidateBuildSpatial
{
    public static IEndpointRouteBuilder MapValidateBuildSpatialEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/builds/{buildId:guid}/validate", async Task<Results<Ok<ValidateBuildSpatialResponse>, NotFound>> (
            Guid buildId,
            [FromServices] ISpatialValidator validator,
            [FromServices] IDocumentSession session,
            [FromServices] IHttpContextAccessor httpContextAccessor) =>
        {
            // Get the build
            Build? build = await session.LoadAsync<Build>(buildId);
            if (build == null)
            {
                return TypedResults.NotFound();
            }

            // Get all products
            List<Product> allProducts = (await session.Query<Product>().ToListAsync()).ToList();

            // Validate
            SpatialValidationResult result = validator.ValidateBuild(build, allProducts);

            string baseUrl = GetBaseUrl(httpContextAccessor);

            ValidateBuildSpatialResponse response = new(
                result.IsValid,
                result.HasErrors,
                result.HasWarnings,
                result.Issues.Select(i => new SpatialIssueDto(
                    i.Message,
                    i.Severity.ToString(),
                    i.Category
                )).ToList(),
                [
                    new HateoasLink($"{baseUrl}/api/builds/{buildId}/validate", "self", "POST"),
                    new HateoasLink($"{baseUrl}/api/builds/{buildId}", "build", "GET"),
                    new HateoasLink($"{baseUrl}/api/builds/{buildId}/parts", "add-part", "POST"),
                    new HateoasLink($"{baseUrl}/api/builds/{buildId}/slots", "available-slots", "GET"),
                    new HateoasLink($"{baseUrl}/api/builds/{buildId}/compatibility", "validate-compatibility", "GET")
                ]
            );

            return TypedResults.Ok(response);
        })
        .WithName("ValidateBuildSpatial")
        .WithSummary("Validate entire build spatial configuration")
        .WithTags("Spatial Validation");

        return app;
    }

    private static string GetBaseUrl(IHttpContextAccessor httpContextAccessor)
    {
        HttpRequest request = httpContextAccessor.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}";
    }
}

public record ValidateBuildSpatialResponse(
    bool IsValid,
    bool HasErrors,
    bool HasWarnings,
    List<SpatialIssueDto> Issues,
    List<HateoasLink> Links
);
