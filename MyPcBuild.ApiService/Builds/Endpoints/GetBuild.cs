using Marten;
using MyPcBuild.ApiService.Builds.Models;
using MyPcBuild.ApiService.Catalog.Models;
using MyPcBuild.ApiService.SharedDomain.Spatial;
using MyPcBuild.ApiService.Compatibility.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Builds.Endpoints;

public static class GetBuild
{
    public static IEndpointRouteBuilder MapGetBuildEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/builds/{buildId:guid}", async (
            Guid buildId,
            IDocumentSession session,
            ICompatibilityValidator validator,
            IHttpContextAccessor httpContextAccessor) =>
        {
            Build? build = await session.Events.AggregateStreamAsync<Build>(buildId);
            if (build is null)
            {
                return Results.NotFound();
            }

            string baseUrl = httpContextAccessor.GetBaseUrl();

            // Load products for the build
            List<ProductDetails> productDetails = [];
            foreach (BuildPart part in build.Parts)
            {
                Product? product = await session.LoadAsync<Product>(part.ProductId);
                if (product != null)
                {
                    // Get dimensions if product is spatial
                    DimensionsDto? dimensions = null;
                    if (product is ISpatialProduct spatialProduct)
                    {
                        dimensions = new DimensionsDto(
                            spatialProduct.Dimensions.Length,
                            spatialProduct.Dimensions.Width,
                            spatialProduct.Dimensions.Height
                        );
                    }

                    // Get slots if product has them
                    List<SlotDto>? slots = null;
                    if (product is ISlottedProduct slottedProduct)
                    {
                        slots = slottedProduct.Slots.Select(s => new SlotDto(
                            s.Id,
                            s.Name,
                            s.AllowedProductCategory.ToString(),
                            new Vector3Dto(s.RelativePosition.X, s.RelativePosition.Y, s.RelativePosition.Z),
                            new DimensionsDto(s.MaxDimensions.Length, s.MaxDimensions.Width, s.MaxDimensions.Height),
                            s.Rotation != Rotation.Identity 
                                ? new RotationDto(s.Rotation.X, s.Rotation.Y, s.Rotation.Z) 
                                : null
                        )).ToList();
                    }

                    // Get chambers if product has them
                    List<ChamberDto>? chambers = null;
                    if (product is IChamberedProduct chamberedProduct)
                    {
                        chambers = chamberedProduct.Chambers.Select(c => new ChamberDto(
                            c.Id,
                            c.Name,
                            new DimensionsDto(c.Dimensions.Length, c.Dimensions.Width, c.Dimensions.Height),
                            c.Slots.Select(s => new SlotDto(
                                s.Id,
                                s.Name,
                                s.AllowedProductCategory.ToString(),
                                new Vector3Dto(s.RelativePosition.X, s.RelativePosition.Y, s.RelativePosition.Z),
                                new DimensionsDto(s.MaxDimensions.Length, s.MaxDimensions.Width, s.MaxDimensions.Height),
                                s.Rotation != Rotation.Identity 
                                    ? new RotationDto(s.Rotation.X, s.Rotation.Y, s.Rotation.Z) 
                                    : null
                            )).ToList()
                        )).ToList();
                    }

                    Vector3Dto? position = part.Position != null 
                        ? new Vector3Dto(part.Position.X, part.Position.Y, part.Position.Z) 
                        : null;

                    RotationDto? rotation = part.Rotation != null
                        ? new RotationDto(part.Rotation.X, part.Rotation.Y, part.Rotation.Z)
                        : null;

                    productDetails.Add(new ProductDetails(
                        product.Id,
                        product.Name,
                        product.ProductCategory,
                        product.Manufacturer,
                        part.PricePaid,
                        part.SlotId,
                        position,
                        rotation,
                        dimensions,
                        slots,
                        chambers,
                        [
                            new HateoasLink(new Uri($"{baseUrl}/api/catalog/products/{product.Id}"), "product", Infrastructure.HttpMethod.GET),
                            new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/parts/{product.Id}"), "remove", Infrastructure.HttpMethod.DELETE)
                        ]
                    ));
                }
            }

            // Run compatibility validation
            List<Product> products = [];
            foreach (BuildPart part in build.Parts)
            {
                Product? product = await session.LoadAsync<Product>(part.ProductId);
                if (product != null)
                {
                    products.Add(product);
                }
            }

            CompatibilityResult? compatibilityResult = null;
            if (products.Any())
            {
                compatibilityResult = await validator.ValidateBuild(products);
            }

            GetBuildResponse response = new(
                build.Id,
                build.Name,
                build.UserId,
                productDetails,
                compatibilityResult?.IsCompatible ?? true,
                compatibilityResult?.Issues.Select(i => new CompatibilityIssueDto(
                    i.Message,
                    i.Severity.ToString(),
                    i.Category
                )).ToList() ?? [],
                DateTimeOffset.UtcNow,
                [
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}"), "self", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/parts"), "add-part", Infrastructure.HttpMethod.POST),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/compatibility"), "validate", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/builds/{buildId}/slots"), "available-slots", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products"), "catalog", Infrastructure.HttpMethod.GET)
                ]
            );

            return Results.Ok(response);
        })
        .WithName("GetBuild")
        .WithTags("Builds");

        return app;
    }

}

public record GetBuildResponse(
    Guid Id,
    string Name,
    Guid UserId,
    List<ProductDetails> Parts,
    bool IsCompatible,
    List<CompatibilityIssueDto> CompatibilityIssues,
    DateTimeOffset CreatedAt,
    List<HateoasLink> Links
);

public record ProductDetails(
    Guid Id,
    string Name,
    ProductCategory Category,
    string Manufacturer,
    decimal PricePaid,
    Guid? SlotId,
    Vector3Dto? Position,
    RotationDto? Rotation,
    DimensionsDto? Dimensions,
    List<SlotDto>? Slots,
    List<ChamberDto>? Chambers,
    List<HateoasLink> Links
);

public record CompatibilityIssueDto(
    string Message,
    string Severity,
    ProductCategory Category
);
