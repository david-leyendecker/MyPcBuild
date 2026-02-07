using Marten;
using MyPcBuild.ApiService.Domain.Events;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Features.Builds;

public static class RemovePartFromBuild
{
    public static IEndpointRouteBuilder MapRemovePartEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/builds/{buildId:guid}/parts/{productId:guid}", async (
            Guid buildId,
            Guid productId,
            IDocumentSession session,
            IHttpContextAccessor httpContextAccessor) =>
        {
            PartRemoved @event = new()
            {
                BuildId = buildId,
                ProductId = productId
            };

            _ = session.Events.Append(buildId, @event);
            await session.SaveChangesAsync();

            string baseUrl = httpContextAccessor.GetBaseUrl();

            RemovePartResponse response = new(
                "Part removed successfully",
                [
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}"), "build", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/parts"), "add-part", Infrastructure.HttpMethod.POST),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/compatibility"), "validate", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products"), "catalog", Infrastructure.HttpMethod.GET)
                ]
            );

            return Results.Ok(response);
        })
        .WithName("RemovePartFromBuild")
        .WithTags("Builds");

        return app;
    }

}

public record RemovePartResponse(
    string Message,
    List<HateoasLink> Links
);
