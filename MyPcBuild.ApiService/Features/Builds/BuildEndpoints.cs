namespace MyPcBuild.ApiService.Features.Builds;

public static class BuildEndpoints
{
    public static IEndpointRouteBuilder MapBuildEndpoints(this IEndpointRouteBuilder app)
    {
        return app
            .MapCreateBuildEndpoint()
            .MapAddPartEndpoint()
            .MapRemovePartEndpoint()
            .MapGetBuildEndpoint();
    }
}
