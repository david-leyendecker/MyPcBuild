using Marten;
using MyPcBuild.ApiService.Domain.Events;
using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Features.Builds;

public static class RemovePartFromBuild
{
    public static IEndpointRouteBuilder MapRemovePartEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/builds/{buildId:guid}/parts/{productId:guid}", async (
            Guid buildId,
            Guid productId,
            IDocumentSession session) =>
        {
            PartRemoved @event = new()
            {
                BuildId = buildId,
                ProductId = productId
            };

            session.Events.Append(buildId, @event);
            await session.SaveChangesAsync();

            return Results.Ok(new { Message = "Part removed successfully" });
        })
        .WithName("RemovePartFromBuild")
        .WithTags("Builds");

        return app;
    }
}
