using Marten;
using MyPcBuild.ApiService.Domain.Events;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Features.Builds;

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

            string baseUrl = GetBaseUrl(httpContextAccessor);

            CreateBuildResponse response = new(
                buildId,
                request.Name,
                request.UserId,
                [
                    new HateoasLink($"{baseUrl}/api/builds/{buildId}", "self", "GET"),
                    new HateoasLink($"{baseUrl}/api/builds/{buildId}/parts", "add-part", "POST"),
                    new HateoasLink($"{baseUrl}/api/builds/{buildId}/compatibility", "validate", "GET")
                ]
            );
            return Results.Created($"/api/builds/{buildId}", response);
        })
        .WithName("CreateBuild")
        .WithTags("Builds");

        return app;
    }

    private static string GetBaseUrl(IHttpContextAccessor httpContextAccessor)
    {
        HttpRequest request = httpContextAccessor.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}";
    }
}

public record CreateBuildRequest(string Name, Guid UserId);

public record CreateBuildResponse(
    Guid Id,
    string Name,
    Guid UserId,
    List<HateoasLink> Links
);
