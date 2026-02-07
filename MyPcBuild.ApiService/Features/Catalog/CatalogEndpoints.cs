using MyPcBuild.ApiService.Features.Catalog.Endpoints;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class CatalogEndpoints
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
