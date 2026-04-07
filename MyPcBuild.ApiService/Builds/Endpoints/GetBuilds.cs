using Marten;
using MyPcBuild.ApiService.Builds.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Builds.Endpoints;

public static class GetBuilds
{
    public static IEndpointRouteBuilder MapGetBuildsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/builds", async (
            IDocumentSession session,
            IHttpContextAccessor httpContextAccessor,
            [AsParameters] QueryParameters queryParams) =>
        {
            IQueryable<Build> query = session.Query<Build>();
            int totalCount = await query.CountAsync();

            IReadOnlyList<Build> builds = await query
                .Skip(queryParams.GetSkip())
                .Take(queryParams.ItemsPerPage ?? 20)
                .ToListAsync();

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

            PaginationMetadata paginationMetadata = new()
            {
                TotalCount = totalCount,
                PageNumber = queryParams.Page ?? 1,
                ItemsPerPage = queryParams.ItemsPerPage ?? 20
            };

            GetBuildsResponse response = new(
                items,
                paginationMetadata,
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
    PaginationMetadata PaginationMetadata,
    List<HateoasLink> Links
);

public record GetBuildsResponseItem(
    Guid Id,
    string Name,
    decimal TotalPrice,
    List<HateoasLink> Links
);
