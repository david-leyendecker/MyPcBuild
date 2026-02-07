using System;
using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Features.Builds;

public static class GetBuilds
{
    public static IEndpointRouteBuilder MapGetBuildsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/builds", async (IDocumentSession session, IHttpContextAccessor httpContextAccessor) =>
        {
            IReadOnlyList<Build> builds = await session.Query<Build>().ToListAsync();

            string baseUrl = httpContextAccessor.GetBaseUrl();

            IReadOnlyList<GetBuildsResponseItem> items = builds.Select(build => new GetBuildsResponseItem(
                build.Id,
                build.Name,
                build.Parts.Sum(part => part.PricePaid),
                [
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{build.Id}"), "self", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{build.Id}/parts"), "add-part", Infrastructure.HttpMethod.POST),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{build.Id}/compatibility"), "validate", Infrastructure.HttpMethod.GET)
                ]
            )).ToList();

            GetBuildsResponse response = new(
                items,
                [
                    new HateoasLink(new Uri($"{baseUrl}/api/builds"), "self", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds"), "create-build", Infrastructure.HttpMethod.POST),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products"), "catalog", Infrastructure.HttpMethod.GET)
                ]
            );

            return Results.Ok(response);
        })
        .Produces<GetBuildsResponse>()
        .WithName("GetBuilds")
        .WithTags("Builds");

        return app;
    }

}

public record GetBuildsResponse(
    IReadOnlyList<GetBuildsResponseItem> Items,
    List<HateoasLink> Links
);

public record GetBuildsResponseItem(
    Guid Id,
    string Name,
    decimal TotalPrice,
    List<HateoasLink> Links
);