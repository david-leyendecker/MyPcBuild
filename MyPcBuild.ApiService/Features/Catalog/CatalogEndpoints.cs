namespace MyPcBuild.ApiService.Features.Catalog;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetProductsEndpoint();
        app.MapGetProductByIdEndpoint();
        app.MapGetCategoriesEndpoint();
        app.MapSearchProductsEndpoint();
        app.MapCreateProductEndpoint();
        app.MapGetFieldDefinitionsEndpoint();

        return app;
    }
}
