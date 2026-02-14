namespace MyPcBuild.ApiService.Compatibility.Endpoints;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapCompatibilityEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapValidateCompatibilityEndpoint();
        app.MapGetBuildCompatibilityEndpoint();

        return app;
    }
}
