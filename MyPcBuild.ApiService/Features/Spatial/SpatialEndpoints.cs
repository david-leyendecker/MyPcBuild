namespace MyPcBuild.ApiService.Features.Spatial;

public static class Endpoints
{
    public static void MapSpatialEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapValidatePartInstallationEndpoint();
        app.MapValidateBuildSpatialEndpoint();
    }
}
