using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Features.Spatial;

public static class ValidatePartInstallation
{
    public static IEndpointRouteBuilder MapValidatePartInstallationEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/builds/{buildId:guid}/parts/validate", async Task<Results<Ok<ValidatePartInstallationResponse>, NotFound>> (
            Guid buildId,
            [FromBody] ValidatePartInstallationRequest request,
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
            SpatialValidationResult result = validator.ValidatePartInstallation(
                build,
                allProducts,
                request.ProductId,
                request.SlotId,
                request.Position
            );

            string baseUrl = httpContextAccessor.GetBaseUrl();

            ValidatePartInstallationResponse response = new(
                result.IsValid,
                result.HasErrors,
                result.HasWarnings,
                result.Issues.Select(i => new SpatialIssueDto(
                    i.Message,
                    i.Severity.ToString(),
                    i.Category
                )).ToList(),
                [
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/parts/validate"), "self", Infrastructure.HttpMethod.POST),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}"), "build", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/parts"), "add-part", Infrastructure.HttpMethod.POST),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/slots"), "available-slots", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/validate"), "validate-build", Infrastructure.HttpMethod.POST)
                ]
            );

            return TypedResults.Ok(response);
        })
        .WithName("ValidatePartInstallation")
        .WithSummary("Validate adding a part to a build (optionally in a slot)")
        .WithTags("Spatial Validation");

        return app;
    }

}

public record ValidatePartInstallationResponse(
    bool IsValid,
    bool HasErrors,
    bool HasWarnings,
    List<SpatialIssueDto> Issues,
    List<HateoasLink> Links
);
