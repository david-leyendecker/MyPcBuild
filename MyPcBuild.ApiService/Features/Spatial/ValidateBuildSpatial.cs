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

            string baseUrl = httpContextAccessor.GetBaseUrl();

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
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/validate"), "self", Infrastructure.HttpMethod.POST),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}"), "build", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/parts"), "add-part", Infrastructure.HttpMethod.POST),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/slots"), "available-slots", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/compatibility"), "validate-compatibility", Infrastructure.HttpMethod.GET)
                ]
            );

            return TypedResults.Ok(response);
        })
        .WithName("ValidateBuildSpatial")
        .WithSummary("Validate entire build spatial configuration")
        .WithTags("Spatial Validation");

        return app;
    }

}

public record ValidateBuildSpatialResponse(
    bool IsValid,
    bool HasErrors,
    bool HasWarnings,
    List<SpatialIssueDto> Issues,
    List<HateoasLink> Links
);
