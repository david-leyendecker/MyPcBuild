namespace MyPcBuild.ApiService.Spatial.Endpoints;

public static class Endpoints
{
    public static void MapSpatialEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapValidatePartInstallationEndpoint();
        app.MapValidateBuildSpatialEndpoint();
    }
}
