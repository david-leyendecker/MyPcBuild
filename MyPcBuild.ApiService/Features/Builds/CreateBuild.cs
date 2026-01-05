using Marten;
using MyPcBuild.ApiService.Domain.Events;
using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Features.Builds;

public static class CreateBuild
{
    public static IEndpointRouteBuilder MapCreateBuildEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/builds", async (
            CreateBuildRequest request,
            IDocumentSession session) =>
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

            CreateBuildResponse response = new(buildId, request.Name, request.UserId);
            return Results.Created($"/api/builds/{buildId}", response);
        })
        .WithName("CreateBuild")
        .WithTags("Builds");

        return app;
    }
}

public record CreateBuildRequest(string Name, Guid UserId);

public record CreateBuildResponse(Guid Id, string Name, Guid UserId);
