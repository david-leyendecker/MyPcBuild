namespace MyPcBuild.ApiService.Builds.Endpoints;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapBuildEndpoints(this IEndpointRouteBuilder app)
    {
        return app
            .MapGetBuildsEndpoint()
            .MapGetBuildEndpoint()
            .MapCreateBuildEndpoint()
            .MapAddPartEndpoint()
            .MapAddPartToSlotEndpoint()
            .MapGetAvailableSlotsEndpoint()
            .MapRemovePartEndpoint();
    }
}
