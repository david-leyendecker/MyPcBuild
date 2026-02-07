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

            string baseUrl = GetBaseUrl(httpContextAccessor);

            RemovePartResponse response = new(
                "Part removed successfully",
                [
                    new HateoasLink($"{baseUrl}/api/builds/{buildId}", "build", "GET"),
                    new HateoasLink($"{baseUrl}/api/builds/{buildId}/parts", "add-part", "POST"),
                    new HateoasLink($"{baseUrl}/api/builds/{buildId}/compatibility", "validate", "GET"),
                    new HateoasLink($"{baseUrl}/api/catalog/products", "catalog", "GET")
                ]
            );

            return Results.Ok(response);
        })
        .WithName("RemovePartFromBuild")
        .WithTags("Builds");

        return app;
    }

    private static string GetBaseUrl(IHttpContextAccessor httpContextAccessor)
    {
        HttpRequest request = httpContextAccessor.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}";
    }
}

public record RemovePartResponse(
    string Message,
    List<HateoasLink> Links
);
