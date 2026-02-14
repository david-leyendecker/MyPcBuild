using Marten;
using MyPcBuild.ApiService.Builds.Events;
using MyPcBuild.ApiService.Builds.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Builds.Endpoints;

public static class CreateBuild
{
    public static IEndpointRouteBuilder MapCreateBuildEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/builds", async (
            CreateBuildRequest request,
            IDocumentSession session,
            IHttpContextAccessor httpContextAccessor) =>
        {
            Guid buildId = Guid.NewGuid();
            BuildCreated @event = new()
            {
                BuildId = buildId,
                Name = request.Name,
                UserId = request.UserId
            };

            session.Events.StartStream<Build>(buildId, @event);
            await session.SaveChangesAsync();

            string baseUrl = httpContextAccessor.GetBaseUrl();

            CreateBuildResponse response = new(
                buildId,
                request.Name,
                request.UserId,
                [
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}"), "self", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/parts"), "add-part", Infrastructure.HttpMethod.POST),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/compatibility"), "validate", Infrastructure.HttpMethod.GET)
                ]
            );
            return Results.Created($"/api/builds/{buildId}", response);
        })
        .WithName("CreateBuild")
        .WithTags("Builds");

        return app;
    }

}

public record CreateBuildRequest(string Name, Guid UserId);

public record CreateBuildResponse(
    Guid Id,
    string Name,
    Guid UserId,
    List<HateoasLink> Links
);
