using Marten;
using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class GetProductById
{
    /// <summary>
    /// Retrieves a product by its ID with only the fields needed for the detail view.
    /// </summary>
    public static IEndpointRouteBuilder MapGetProductByIdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalog/products/{id:guid}", async (
            Guid id,
            IDocumentSession session) =>
        {
            Product? product = await session.LoadAsync<Product>(id);

            if (product is null)
            {
                return Results.NotFound();
            }

            ProductDetailResponse response = MapToDetailResponse(product);
            return Results.Ok(response);
        })
        .WithName("GetProductById")
        .Produces<ProductDetailResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithTags("Catalog");

        return app;
    }

    private static ProductDetailResponse MapToDetailResponse(Product product)
    {
        Dictionary<string, object> specifications = ExtractSpecifications(product);
        ProductDetailDimensions? dimensions = ExtractDimensions(product);
        List<ProductDetailSlot>? slots = ExtractSlots(product);
        List<ProductDetailChamber>? chambers = ExtractChambers(product);

        return new ProductDetailResponse(
            product.Id.ToString(),
            product.Name,
            product.ProductCategory.ToString(),
            product.Price,
            product.Manufacturer,
            product.IsDraft,
            product.PublishedAt?.ToString("O"),
            specifications,
            dimensions,
            slots,
            chambers
        );
    }

    private static Dictionary<string, object> ExtractSpecifications(Product product)
    {
        Dictionary<string, object> specs = [];

        return product switch
        {
            CpuProduct cpu => new()
            {
                [nameof(CpuProduct.Socket)] = cpu.Socket,
                [nameof(CpuProduct.Cores)] = cpu.Cores,
                [nameof(CpuProduct.Threads)] = cpu.Threads,
                [nameof(CpuProduct.BaseClock)] = cpu.BaseClock.ToString(),
                [nameof(CpuProduct.BoostClock)] = cpu.BoostClock.ToString(),
                [nameof(CpuProduct.TDP)] = cpu.TDP.ToString(),
                [nameof(CpuProduct.IntegratedGraphics)] = cpu.IntegratedGraphics
            },
            MotherboardProduct mb => new()
            {
                [nameof(MotherboardProduct.Socket)] = mb.Socket,
                [nameof(MotherboardProduct.Chipset)] = mb.Chipset,
                [nameof(MotherboardProduct.FormFactor)] = mb.FormFactor,
                [nameof(MotherboardProduct.MemoryType)] = mb.MemoryType,
                [nameof(MotherboardProduct.MaxMemory)] = mb.MaxMemory.ToString()
            },
            GpuProduct gpu => new()
            {
                [nameof(GpuProduct.ChipsetManufacturer)] = gpu.ChipsetManufacturer,
                [nameof(GpuProduct.Series)] = gpu.Series,
                [nameof(GpuProduct.VRAM)] = gpu.VRAM.ToString(),
                [nameof(GpuProduct.MemoryType)] = gpu.MemoryType,
                [nameof(GpuProduct.CoreClock)] = gpu.CoreClock.ToString(),
                [nameof(GpuProduct.BoostClock)] = gpu.BoostClock.ToString(),
                [nameof(GpuProduct.TDP)] = gpu.TDP.ToString(),
                [nameof(GpuProduct.Length)] = gpu.Length.ToString(),
                [nameof(GpuProduct.PowerConnectors)] = gpu.PowerConnectors.ToString(),
                [nameof(GpuProduct.RayTracing)] = gpu.RayTracing
            },
            RamProduct ram => new()
            {
                [nameof(RamProduct.Type)] = ram.Type,
                [nameof(RamProduct.Capacity)] = ram.Capacity.ToString(),
                [nameof(RamProduct.Configuration)] = ram.Configuration,
                [nameof(RamProduct.Speed)] = ram.Speed.ToString(),
                [nameof(RamProduct.CASLatency)] = ram.CASLatency,
                [nameof(RamProduct.Voltage)] = ram.Voltage.ToString()
            },
            PcCaseProduct @case => new()
            {
                [nameof(PcCaseProduct.FormFactor)] = @case.FormFactor,
                [nameof(PcCaseProduct.Color)] = @case.Color,
                [nameof(PcCaseProduct.SidePanelWindow)] = @case.SidePanelWindow
            },
            PsuProduct psu => new()
            {
                [nameof(PsuProduct.Wattage)] = psu.Wattage.ToString(),
                [nameof(PsuProduct.Efficiency)] = psu.Efficiency,
                [nameof(PsuProduct.Modular)] = psu.Modular,
                [nameof(PsuProduct.FormFactor)] = psu.FormFactor,
                [nameof(PsuProduct.Length)] = psu.Length.ToString(),
                [nameof(PsuProduct.PCIe8Pin)] = psu.PCIe8Pin
            },
            StorageProduct storage => new()
            {
                [nameof(StorageProduct.Type)] = storage.Type,
                [nameof(StorageProduct.Interface)] = storage.Interface,
                [nameof(StorageProduct.StorageFormFactor)] = storage.StorageFormFactor,
                [nameof(StorageProduct.Capacity)] = storage.Capacity.ToString(),
                [nameof(StorageProduct.ReadSpeed)] = storage.ReadSpeed.ToString(),
                [nameof(StorageProduct.WriteSpeed)] = storage.WriteSpeed.ToString()
            },
            CoolerProduct cooler => new()
            {
                [nameof(CoolerProduct.CoolerType)] = cooler.CoolerType,
                [nameof(CoolerProduct.Height)] = cooler.Height.ToString(),
                [nameof(CoolerProduct.TDP)] = cooler.TDP.ToString(),
                [nameof(CoolerProduct.Sockets)] = cooler.Sockets
            },
            _ => specs
        };
    }

    private static ProductDetailDimensions? ExtractDimensions(Product product)
    {
        return product switch
        {
            MotherboardProduct mb => new ProductDetailDimensions(mb.Dimensions.Length, mb.Dimensions.Width, mb.Dimensions.Height),
            GpuProduct gpu => new ProductDetailDimensions(gpu.Dimensions.Length, gpu.Dimensions.Width, gpu.Dimensions.Height),
            PcCaseProduct @case => new ProductDetailDimensions(@case.Dimensions.Length, @case.Dimensions.Width, @case.Dimensions.Height),
            CoolerProduct cooler => new ProductDetailDimensions(cooler.Dimensions.Length, cooler.Dimensions.Width, cooler.Dimensions.Height),
            _ => null
        };
    }

    private static List<ProductDetailSlot>? ExtractSlots(Product product)
    {
        return product switch
        {
            MotherboardProduct mb => mb.Slots.Select(s => new ProductDetailSlot(
                s.Name,
                s.AllowedProductCategory.ToString(),
                new ProductDetailLocation(s.RelativePosition.X, s.RelativePosition.Y, s.RelativePosition.Z)
            )).ToList(),
            GpuProduct gpu => gpu.Slots.Select(s => new ProductDetailSlot(
                s.Name,
                s.AllowedProductCategory.ToString(),
                new ProductDetailLocation(s.RelativePosition.X, s.RelativePosition.Y, s.RelativePosition.Z)
            )).ToList(),
            _ => null
        };
    }

    private static List<ProductDetailChamber>? ExtractChambers(Product product)
    {
        return product switch
        {
            PcCaseProduct @case => @case.Chambers.Select(c => new ProductDetailChamber(
                c.Name,
                new ProductDetailDimensions(c.Dimensions.Length, c.Dimensions.Width, c.Dimensions.Height)
            )).ToList(),
            _ => null
        };
    }
}

/// <summary>
/// Response DTO for product detail view, containing only the fields necessary for display.
/// </summary>
public record ProductDetailResponse(
    string Id,
    string Name,
    string Category,
    decimal Price,
    string Manufacturer,
    bool IsDraft,
    string? PublishedAt,
    Dictionary<string, object> Specifications,
    ProductDetailDimensions? Dimensions,
    List<ProductDetailSlot>? Slots,
    List<ProductDetailChamber>? Chambers
);

public record ProductDetailDimensions(decimal Length, decimal Width, decimal Height);
public record ProductDetailLocation(decimal X, decimal Y, decimal Z);
public record ProductDetailSlot(string Name, string AllowedCategory, ProductDetailLocation Location);
public record ProductDetailChamber(string Name, ProductDetailDimensions Dimensions);
