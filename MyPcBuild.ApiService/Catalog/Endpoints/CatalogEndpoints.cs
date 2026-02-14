namespace MyPcBuild.ApiService.Catalog.Endpoints;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetProductsEndpoint();
        app.MapGetProductByIdEndpoint();
        app.MapGetCategoriesEndpoint();
        app.MapSearchProductsEndpoint();
        app.MapCreateProductEndpoint();
        app.MapUpdateProductEndpoint();
        app.MapGetFieldDefinitionsEndpoint();
        app.MapGenerateProductWithAiEndpoint();
        app.MapPublishProductEndpoint();

        return app;
    }
}
