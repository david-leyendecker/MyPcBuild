using System;
using Marten;
using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Features.Builds;

public static class GetBuilds
{
    public static IEndpointRouteBuilder MapGetBuildsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/builds", async (IDocumentSession session) =>
        {
            IReadOnlyList<Build> builds = await session.Query<Build>().ToListAsync();

            IReadOnlyList<GetBuildsResponse> response = builds.Select(build => new GetBuildsResponse(
                build.Id,
                build.Name,
                build.Parts.Sum(part => part.PricePaid)
            )).ToList();

            return Results.Ok(response);
        })
        .Produces<IReadOnlyList<GetBuildsResponse>>()
        .WithName("GetBuilds")
        .WithTags("Builds");

        return app;
    }
}

public record GetBuildsResponse(
    Guid Id,
    string Name,
    decimal TotalPrice
);