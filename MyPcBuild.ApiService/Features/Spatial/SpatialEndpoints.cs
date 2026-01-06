using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Marten;
using MyPcBuild.ApiService.Domain.Events;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;

namespace MyPcBuild.ApiService.Features.Spatial;

public static class SpatialEndpoints
{
    public static void MapSpatialEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/builds")
            .WithTags("Spatial Validation");

        group.MapPost("/{buildId:guid}/parts/validate", ValidatePartInstallation)
            .WithName("ValidatePartInstallation")
            .WithSummary("Validate adding a part to a build (optionally in a slot)");

        group.MapPost("/{buildId:guid}/validate", ValidateBuildSpatial)
            .WithName("ValidateBuildSpatial")
            .WithSummary("Validate entire build spatial configuration");
    }

    private static async Task<Results<Ok<SpatialValidationResponse>, NotFound>> ValidatePartInstallation(
        Guid buildId,
        [FromBody] ValidatePartInstallationRequest request,
        [FromServices] ISpatialValidator validator,
        [FromServices] IDocumentSession session)
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

        SpatialValidationResponse response = new(
            result.IsValid,
            result.HasErrors,
            result.HasWarnings,
            result.Issues.Select(i => new SpatialIssueDto(
                i.Message,
                i.Severity.ToString(),
                i.Category
            )).ToList()
        );

        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<SpatialValidationResponse>, NotFound>> ValidateBuildSpatial(
        Guid buildId,
        [FromServices] ISpatialValidator validator,
        [FromServices] IDocumentSession session)
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

        SpatialValidationResponse response = new(
            result.IsValid,
            result.HasErrors,
            result.HasWarnings,
            result.Issues.Select(i => new SpatialIssueDto(
                i.Message,
                i.Severity.ToString(),
                i.Category
            )).ToList()
        );

        return TypedResults.Ok(response);
    }
}
