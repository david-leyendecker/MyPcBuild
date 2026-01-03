namespace MyPcBuild.ApiService.Features.Catalog;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetProductsEndpoint();
        app.MapGetProductByIdEndpoint();
        app.MapGetCategoriesEndpoint();
        app.MapSearchProductsEndpoint();

        return app;
    }
}
