namespace MyPcBuild.ApiService.Features.Compatibility;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapCompatibilityEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapValidateCompatibilityEndpoint();
        app.MapGetBuildCompatibilityEndpoint();

        return app;
    }
}
