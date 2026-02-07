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

            string baseUrl = GetBaseUrl(httpContextAccessor);

            IReadOnlyList<GetBuildsResponseItem> items = builds.Select(build => new GetBuildsResponseItem(
                build.Id,
                build.Name,
                build.Parts.Sum(part => part.PricePaid),
                [
                    new HateoasLink($"{baseUrl}/api/builds/{build.Id}", "self", "GET"),
                    new HateoasLink($"{baseUrl}/api/builds/{build.Id}/parts", "add-part", "POST"),
                    new HateoasLink($"{baseUrl}/api/builds/{build.Id}/compatibility", "validate", "GET")
                ]
            )).ToList();

            GetBuildsResponse response = new(
                items,
                [
                    new HateoasLink($"{baseUrl}/api/builds", "self", "GET"),
                    new HateoasLink($"{baseUrl}/api/builds", "create-build", "POST"),
                    new HateoasLink($"{baseUrl}/api/catalog/products", "catalog", "GET")
                ]
            );

            return Results.Ok(response);
        })
        .Produces<GetBuildsResponse>()
        .WithName("GetBuilds")
        .WithTags("Builds");

        return app;
    }

    private static string GetBaseUrl(IHttpContextAccessor httpContextAccessor)
    {
        HttpRequest request = httpContextAccessor.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}";
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