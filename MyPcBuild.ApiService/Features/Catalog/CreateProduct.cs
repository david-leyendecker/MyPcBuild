using Marten;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;
using System.Text.Json;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class CreateProduct
{
    public static IEndpointRouteBuilder MapCreateProductEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/catalog/products", async (
            IDocumentSession session,
            CreateProductRequest request) =>
        {
            Product product = request.Category switch
            {
                "CPU" => CreateCpuProduct(request),
                "Motherboard" => CreateMotherboardProduct(request),
                "GPU" => CreateGpuProduct(request),
                "RAM" => CreateRamProduct(request),
                "PCCase" => CreatePcCaseProduct(request),
                "PSU" => CreatePsuProduct(request),
                "Storage" => CreateStorageProduct(request),
                "Cooler" => CreateCoolerProduct(request),
                _ => throw new ArgumentException($"Unknown category: {request.Category}")
            };

            session.Store(product);
            await session.SaveChangesAsync();

            return Results.Created($"/api/catalog/{product.Id}", new CreateProductResponse(product.Id));
        })
        .WithName("CreateProduct")
        .WithTags("Catalog");

        return app;
    }

    private static CpuProduct CreateCpuProduct(CreateProductRequest request)
    {
        return new CpuProduct(
            Guid.NewGuid(),
            request.Name,
            request.Price,
            request.Manufacturer,
            ParseEnum<CpuSocket>(request.Fields.GetValueOrDefault("Socket", "AM5")),
            int.Parse(request.Fields.GetValueOrDefault("Cores", "8")),
            int.Parse(request.Fields.GetValueOrDefault("Threads", "16")),
            Frequency.FromGHz(decimal.Parse(request.Fields.GetValueOrDefault("BaseClock", "3.5"))),
            Frequency.FromGHz(decimal.Parse(request.Fields.GetValueOrDefault("BoostClock", "5.0"))),
            Power.FromWatts(int.Parse(request.Fields.GetValueOrDefault("TDP", "105"))),
            bool.Parse(request.Fields.GetValueOrDefault("IntegratedGraphics", "false"))
        );
    }

    private static MotherboardProduct CreateMotherboardProduct(CreateProductRequest request)
    {
        return new MotherboardProduct(
            Guid.NewGuid(),
            request.Name,
            request.Price,
            request.Manufacturer,
            ParseDimensions(request.Fields.GetValueOrDefault("Dimensions", "305,244,50")),
            ParseSlots(request.Fields.GetValueOrDefault("Slots", "[]")),
            ParseEnum<CpuSocket>(request.Fields.GetValueOrDefault("Socket", "AM5")),
            request.Fields.GetValueOrDefault("Chipset", "X670"),
            request.Fields.GetValueOrDefault("FormFactor", "ATX"),
            ParseEnum<MemoryType>(request.Fields.GetValueOrDefault("MemoryType", "DDR5")),
            StorageCapacity.FromGB(int.Parse(request.Fields.GetValueOrDefault("MaxMemory", "128")))
        );
    }

    private static GpuProduct CreateGpuProduct(CreateProductRequest request)
    {
        return new GpuProduct(
            Guid.NewGuid(),
            request.Name,
            request.Price,
            request.Manufacturer,
            ParseDimensions(request.Fields.GetValueOrDefault("Dimensions", "300,120,50")),
            ParseSlots(request.Fields.GetValueOrDefault("Slots", "[]")),
            request.Fields.GetValueOrDefault("ChipsetManufacturer", "NVIDIA"),
            request.Fields.GetValueOrDefault("Series", "RTX 4000"),
            StorageCapacity.FromGB(int.Parse(request.Fields.GetValueOrDefault("VRAM", "12"))),
            ParseEnum<MemoryType>(request.Fields.GetValueOrDefault("MemoryType", "GDDR6X")),
            Frequency.FromMHz(int.Parse(request.Fields.GetValueOrDefault("CoreClock", "2310"))),
            Frequency.FromMHz(int.Parse(request.Fields.GetValueOrDefault("BoostClock", "2535"))),
            Power.FromWatts(int.Parse(request.Fields.GetValueOrDefault("TDP", "320"))),
            Length.FromMm(int.Parse(request.Fields.GetValueOrDefault("Length", "304"))),
            request.Fields.GetValueOrDefault("PowerConnectors", "1x16-pin"),
            bool.Parse(request.Fields.GetValueOrDefault("RayTracing", "true"))
        );
    }

    private static RamProduct CreateRamProduct(CreateProductRequest request)
    {
        return new RamProduct(
            Guid.NewGuid(),
            request.Name,
            request.Price,
            request.Manufacturer,
            ParseEnum<MemoryType>(request.Fields.GetValueOrDefault("Type", "DDR5")),
            StorageCapacity.FromGB(int.Parse(request.Fields.GetValueOrDefault("Capacity", "32"))),
            request.Fields.GetValueOrDefault("Configuration", "2x16GB"),
            Frequency.FromMHz(int.Parse(request.Fields.GetValueOrDefault("Speed", "6000"))),
            request.Fields.GetValueOrDefault("CASLatency", "CL30"),
            Voltage.FromVolts(decimal.Parse(request.Fields.GetValueOrDefault("Voltage", "1.35")))
        );
    }

    private static PcCaseProduct CreatePcCaseProduct(CreateProductRequest request)
    {
        return new PcCaseProduct(
            Guid.NewGuid(),
            request.Name,
            request.Price,
            request.Manufacturer,
            ParseDimensions(request.Fields.GetValueOrDefault("Dimensions", "500,230,480")),
            ParseChambers(request.Fields.GetValueOrDefault("Chambers", "[]")),
            request.Fields.GetValueOrDefault("FormFactor", "ATX"),
            request.Fields.GetValueOrDefault("Color", "Black"),
            request.Fields.GetValueOrDefault("SidePanelWindow", "Tempered Glass"),
            Length.FromMm(int.Parse(request.Fields.GetValueOrDefault("MaxGPULength", "380"))),
            Length.FromMm(int.Parse(request.Fields.GetValueOrDefault("MaxCPUCoolerHeight", "170"))),
            Length.FromMm(int.Parse(request.Fields.GetValueOrDefault("MaxPSULength", "220")))
        );
    }

    private static PsuProduct CreatePsuProduct(CreateProductRequest request)
    {
        return new PsuProduct(
            Guid.NewGuid(),
            request.Name,
            request.Price,
            request.Manufacturer,
            Power.FromWatts(int.Parse(request.Fields.GetValueOrDefault("Wattage", "850"))),
            request.Fields.GetValueOrDefault("Efficiency", "80+ Gold"),
            request.Fields.GetValueOrDefault("Modular", "Fully Modular"),
            request.Fields.GetValueOrDefault("FormFactor", "ATX"),
            Length.FromMm(int.Parse(request.Fields.GetValueOrDefault("Length", "160"))),
            int.Parse(request.Fields.GetValueOrDefault("PCIe8Pin", "4"))
        );
    }

    private static StorageProduct CreateStorageProduct(CreateProductRequest request)
    {
        return new StorageProduct(
            Guid.NewGuid(),
            request.Name,
            request.Price,
            request.Manufacturer,
            request.Fields.GetValueOrDefault("Type", "SSD"),
            request.Fields.GetValueOrDefault("Interface", "NVMe"),
            request.Fields.GetValueOrDefault("StorageFormFactor", "M.2 2280"),
            StorageCapacity.FromGB(int.Parse(request.Fields.GetValueOrDefault("Capacity", "1024"))),
            DataSpeed.FromMBps(int.Parse(request.Fields.GetValueOrDefault("ReadSpeed", "7000"))),
            DataSpeed.FromMBps(int.Parse(request.Fields.GetValueOrDefault("WriteSpeed", "5000")))
        );
    }

    private static CoolerProduct CreateCoolerProduct(CreateProductRequest request)
    {
        string socketsStr = request.Fields.GetValueOrDefault("Sockets", "AM5,LGA1700");
        string[] socketArr = socketsStr.Split(',', StringSplitOptions.RemoveEmptyEntries);
        CpuSocket[] sockets = socketArr.Select(s => ParseEnum<CpuSocket>(s.Trim())).ToArray();

        return new CoolerProduct(
            Guid.NewGuid(),
            request.Name,
            request.Price,
            request.Manufacturer,
            ParseDimensions(request.Fields.GetValueOrDefault("Dimensions", "140,140,160")),
            request.Fields.GetValueOrDefault("CoolerType", "Air"),
            Length.FromMm(int.Parse(request.Fields.GetValueOrDefault("Height", "160"))),
            Power.FromWatts(int.Parse(request.Fields.GetValueOrDefault("TDP", "220"))),
            sockets
        );
    }

    private static Dimensions ParseDimensions(string value)
    {
        string[] parts = value.Split(',');
        if (parts.Length != 3)
        {
            throw new ArgumentException("Dimensions must be in format: length,width,height");
        }

        return new Dimensions(
            decimal.Parse(parts[0]),
            decimal.Parse(parts[1]),
            decimal.Parse(parts[2])
        );
    }

    private static List<Slot> ParseSlots(string json)
    {
        if (string.IsNullOrWhiteSpace(json) || json == "[]")
        {
            return [];
        }

        try
        {
            List<SlotData>? slotDataList = JsonSerializer.Deserialize<List<SlotData>>(json);
            if (slotDataList == null)
            {
                return [];
            }

            return slotDataList.Select(sd => new Slot(
                Guid.NewGuid(),
                sd.Name ?? "Unnamed Slot",
                sd.AllowedCategory ?? "Unknown",
                Vector3.Zero, // Position would need to be specified in a more advanced editor
                new Dimensions(100, 100, 50), // Default dimensions
                null // No sub-slots for now
            )).ToList();
        }
        catch
        {
            return [];
        }
    }

    private static List<Chamber> ParseChambers(string json)
    {
        if (string.IsNullOrWhiteSpace(json) || json == "[]")
        {
            return [];
        }

        try
        {
            List<ChamberData>? chamberDataList = JsonSerializer.Deserialize<List<ChamberData>>(json);
            if (chamberDataList == null)
            {
                return [];
            }

            return chamberDataList.Select(cd => new Chamber(
                Guid.NewGuid(),
                cd.Name ?? "Unnamed Chamber",
                new Dimensions(cd.Length, cd.Width, cd.Height),
                [] // Chambers would have their own slots in a more advanced implementation
            )).ToList();
        }
        catch
        {
            return [];
        }
    }

    private static T ParseEnum<T>(string value) where T : struct, Enum
    {
        if (Enum.TryParse<T>(value, ignoreCase: true, out T result))
        {
            return result;
        }
        throw new ArgumentException($"Invalid enum value: {value} for type {typeof(T).Name}");
    }
}

public record CreateProductRequest(
    string Category,
    string Name,
    decimal Price,
    string Manufacturer,
    Dictionary<string, string> Fields
);

public record CreateProductResponse(Guid Id);

// Helper records for JSON deserialization
internal record SlotData(string? Name, string? AllowedCategory);
internal record ChamberData(string? Name, decimal Length, decimal Width, decimal Height);
