using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Features.Catalog.DTOs;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class CreateProduct
{
    public static IEndpointRouteBuilder MapCreateProductEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/catalog/products/cpu", CreateProductHandler<CpuDto>)
            .WithName("CreateCpuProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .WithTags("Catalog");

        app.MapPost("/api/catalog/products/motherboard", CreateProductHandler<MotherboardDto>)
            .WithName("CreateMotherboardProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .WithTags("Catalog");

        app.MapPost("/api/catalog/products/gpu", CreateProductHandler<GpuDto>)
            .WithName("CreateGpuProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .WithTags("Catalog");

        app.MapPost("/api/catalog/products/ram", CreateProductHandler<RamDto>)
            .WithName("CreateRamProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .WithTags("Catalog");

        app.MapPost("/api/catalog/products/case", CreateProductHandler<PcCaseDto>)
            .WithName("CreatePcCaseProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .WithTags("Catalog");

        app.MapPost("/api/catalog/products/powersupply", CreateProductHandler<PsuDto>)
            .WithName("CreatePsuProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .WithTags("Catalog");

        app.MapPost("/api/catalog/products/storage", CreateProductHandler<StorageDto>)
            .WithName("CreateStorageProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .WithTags("Catalog");

        app.MapPost("/api/catalog/products/cooler", CreateProductHandler<CoolerDto>)
            .WithName("CreateCoolerProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .WithTags("Catalog");

        return app;
    }

    private static async Task<IResult> CreateProductHandler<TDto>(
        IDocumentSession session,
        TDto dto) where TDto : ProductDto
    {
        Product product = ProductDtoMapper.ToDomain(dto);

        session.Store(product);
        await session.SaveChangesAsync();

        return Results.Created($"/api/catalog/products/{product.Id}", new CreateProductResponse(product.Id));
    }

}

public record CreateProductResponse(Guid Id);
