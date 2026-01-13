using Marten;
using Microsoft.AspNetCore.Mvc;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;
using System.Text.Json;

namespace MyPcBuild.ApiService.Features.Catalog;

public static class UpdateProduct
{
    public static IEndpointRouteBuilder MapUpdateProductEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/catalog/products/{id}", async (
            [FromRoute] Guid id,
            IDocumentSession session,
            UpdateProductRequest request) =>
        {
            Product? existingProduct = await session.LoadAsync<Product>(id);
            if (existingProduct == null)
            {
                return Results.NotFound();
            }

            // Create updated product based on category
            Product updatedProduct = request.Category switch
            {
                ProductCategory.CPU => UpdateCpuProduct((CpuProduct)existingProduct, request),
                ProductCategory.Motherboard => UpdateMotherboardProduct((MotherboardProduct)existingProduct, request),
                ProductCategory.GPU => UpdateGpuProduct((GpuProduct)existingProduct, request),
                ProductCategory.RAM => UpdateRamProduct((RamProduct)existingProduct, request),
                ProductCategory.Case => UpdatePcCaseProduct((PcCaseProduct)existingProduct, request),
                ProductCategory.PowerSupply => UpdatePsuProduct((PsuProduct)existingProduct, request),
                ProductCategory.Storage => UpdateStorageProduct((StorageProduct)existingProduct, request),
                ProductCategory.Cooler => UpdateCoolerProduct((CoolerProduct)existingProduct, request),
                _ => throw new ArgumentException($"Unknown category: {request.Category}")
            };

            session.Store(updatedProduct);
            await session.SaveChangesAsync();

            return Results.Ok(new UpdateProductResponse(updatedProduct.Id));
        })
        .WithName("UpdateProduct")
        .WithTags("Catalog");

        return app;
    }

    private static CpuProduct UpdateCpuProduct(CpuProduct existing, UpdateProductRequest request)
    {
        return existing with
        {
            Name = request.Name,
            Price = request.Price,
            Manufacturer = request.Manufacturer,
            Socket = ParseEnum<CpuSocket>(request.Fields.GetValueOrDefault("Socket", existing.Socket.ToString())),
            Cores = SafeParseInt(request.Fields.GetValueOrDefault("Cores", existing.Cores.ToString()), existing.Cores),
            Threads = SafeParseInt(request.Fields.GetValueOrDefault("Threads", existing.Threads.ToString()), existing.Threads),
            BaseClock = Frequency.FromGHz(SafeParseDecimal(request.Fields.GetValueOrDefault("BaseClock", existing.BaseClock.ValueInGHz.ToString()), existing.BaseClock.ValueInGHz)),
            BoostClock = Frequency.FromGHz(SafeParseDecimal(request.Fields.GetValueOrDefault("BoostClock", existing.BoostClock.ValueInGHz.ToString()), existing.BoostClock.ValueInGHz)),
            TDP = Power.FromWatts(SafeParseInt(request.Fields.GetValueOrDefault("TDP", existing.TDP.ValueInWatts.ToString()), existing.TDP.ValueInWatts)),
            IntegratedGraphics = bool.Parse(request.Fields.GetValueOrDefault("IntegratedGraphics", existing.IntegratedGraphics.ToString()))
        };
    }

    private static MotherboardProduct UpdateMotherboardProduct(MotherboardProduct existing, UpdateProductRequest request)
    {
        return existing with
        {
            Name = request.Name,
            Price = request.Price,
            Manufacturer = request.Manufacturer,
            Dimensions = ParseDimensions(request.Fields.GetValueOrDefault("Dimensions", $"{existing.Dimensions.Length},{existing.Dimensions.Width},{existing.Dimensions.Height}")),
            Slots = ParseSlots(request.Fields.GetValueOrDefault("Slots", "[]")),
            Socket = ParseEnum<CpuSocket>(request.Fields.GetValueOrDefault("Socket", existing.Socket.ToString())),
            Chipset = request.Fields.GetValueOrDefault("Chipset", existing.Chipset),
            FormFactor = ParseEnum<FormFactor>(request.Fields.GetValueOrDefault("FormFactor", existing.FormFactor.ToString())),
            MemoryType = ParseEnum<MemoryType>(request.Fields.GetValueOrDefault("MemoryType", existing.MemoryType.ToString())),
            MaxMemory = StorageCapacity.FromGB(SafeParseInt(request.Fields.GetValueOrDefault("MaxMemory", existing.MaxMemory.ValueInGB.ToString()), existing.MaxMemory.ValueInGB))
        };
    }

    private static GpuProduct UpdateGpuProduct(GpuProduct existing, UpdateProductRequest request)
    {
        return existing with
        {
            Name = request.Name,
            Price = request.Price,
            Manufacturer = request.Manufacturer,
            Dimensions = ParseDimensions(request.Fields.GetValueOrDefault("Dimensions", $"{existing.Dimensions.Length},{existing.Dimensions.Width},{existing.Dimensions.Height}")),
            Slots = ParseSlots(request.Fields.GetValueOrDefault("Slots", "[]")),
            ChipsetManufacturer = request.Fields.GetValueOrDefault("ChipsetManufacturer", existing.ChipsetManufacturer),
            Series = request.Fields.GetValueOrDefault("Series", existing.Series),
            VRAM = StorageCapacity.FromGB(SafeParseInt(request.Fields.GetValueOrDefault("VRAM", existing.VRAM.ValueInGB.ToString()), existing.VRAM.ValueInGB)),
            MemoryType = ParseEnum<MemoryType>(request.Fields.GetValueOrDefault("MemoryType", existing.MemoryType.ToString())),
            CoreClock = Frequency.FromMHz(SafeParseInt(request.Fields.GetValueOrDefault("CoreClock", existing.CoreClock.ToMHz().ToString()), (int)existing.CoreClock.ToMHz())),
            BoostClock = Frequency.FromMHz(SafeParseInt(request.Fields.GetValueOrDefault("BoostClock", existing.BoostClock.ToMHz().ToString()), (int)existing.BoostClock.ToMHz())),
            TDP = Power.FromWatts(SafeParseInt(request.Fields.GetValueOrDefault("TDP", existing.TDP.ValueInWatts.ToString()), existing.TDP.ValueInWatts)),
            Length = Length.FromMm(SafeParseInt(request.Fields.GetValueOrDefault("Length", existing.Length.ValueInMm.ToString()), existing.Length.ValueInMm)),
            PowerConnectors = ParseGpuPowerConnector(request.Fields.GetValueOrDefault("PowerConnectors", existing.PowerConnectors.ToString())),
            RayTracing = bool.Parse(request.Fields.GetValueOrDefault("RayTracing", existing.RayTracing.ToString()))
        };
    }

    private static RamProduct UpdateRamProduct(RamProduct existing, UpdateProductRequest request)
    {
        return existing with
        {
            Name = request.Name,
            Price = request.Price,
            Manufacturer = request.Manufacturer,
            Type = ParseEnum<MemoryType>(request.Fields.GetValueOrDefault("Type", existing.Type.ToString())),
            Capacity = StorageCapacity.FromGB(SafeParseInt(request.Fields.GetValueOrDefault("Capacity", existing.Capacity.ValueInGB.ToString()), existing.Capacity.ValueInGB)),
            Configuration = request.Fields.GetValueOrDefault("Configuration", existing.Configuration),
            Speed = Frequency.FromMHz(SafeParseInt(request.Fields.GetValueOrDefault("Speed", existing.Speed.ToMHz().ToString()), (int)existing.Speed.ToMHz())),
            CASLatency = request.Fields.GetValueOrDefault("CASLatency", existing.CASLatency),
            Voltage = Voltage.FromVolts(SafeParseDecimal(request.Fields.GetValueOrDefault("Voltage", existing.Voltage.ValueInVolts.ToString()), existing.Voltage.ValueInVolts))
        };
    }

    private static PcCaseProduct UpdatePcCaseProduct(PcCaseProduct existing, UpdateProductRequest request)
    {
        return existing with
        {
            Name = request.Name,
            Price = request.Price,
            Manufacturer = request.Manufacturer,
            Dimensions = ParseDimensions(request.Fields.GetValueOrDefault("Dimensions", $"{existing.Dimensions.Length},{existing.Dimensions.Width},{existing.Dimensions.Height}")),
            Chambers = ParseChambers(request.Fields.GetValueOrDefault("Chambers", "[]")),
            FormFactor = request.Fields.GetValueOrDefault("FormFactor", existing.FormFactor),
            Color = request.Fields.GetValueOrDefault("Color", existing.Color),
            SidePanelWindow = request.Fields.GetValueOrDefault("SidePanelWindow", existing.SidePanelWindow)
        };
    }

    private static PsuProduct UpdatePsuProduct(PsuProduct existing, UpdateProductRequest request)
    {
        return existing with
        {
            Name = request.Name,
            Price = request.Price,
            Manufacturer = request.Manufacturer,
            Wattage = Power.FromWatts(SafeParseInt(request.Fields.GetValueOrDefault("Wattage", existing.Wattage.ValueInWatts.ToString()), existing.Wattage.ValueInWatts)),
            Efficiency = request.Fields.GetValueOrDefault("Efficiency", existing.Efficiency),
            Modular = request.Fields.GetValueOrDefault("Modular", existing.Modular),
            FormFactor = request.Fields.GetValueOrDefault("FormFactor", existing.FormFactor),
            Length = Length.FromMm(SafeParseInt(request.Fields.GetValueOrDefault("Length", existing.Length.ValueInMm.ToString()), existing.Length.ValueInMm)),
            PCIe8Pin = SafeParseInt(request.Fields.GetValueOrDefault("PCIe8Pin", existing.PCIe8Pin.ToString()), existing.PCIe8Pin)
        };
    }

    private static StorageProduct UpdateStorageProduct(StorageProduct existing, UpdateProductRequest request)
    {
        return existing with
        {
            Name = request.Name,
            Price = request.Price,
            Manufacturer = request.Manufacturer,
            Type = request.Fields.GetValueOrDefault("Type", existing.Type),
            Interface = request.Fields.GetValueOrDefault("Interface", existing.Interface),
            StorageFormFactor = request.Fields.GetValueOrDefault("StorageFormFactor", existing.StorageFormFactor),
            Capacity = StorageCapacity.FromGB(SafeParseInt(request.Fields.GetValueOrDefault("Capacity", existing.Capacity.ValueInGB.ToString()), existing.Capacity.ValueInGB)),
            ReadSpeed = DataSpeed.FromMBps(SafeParseInt(request.Fields.GetValueOrDefault("ReadSpeed", existing.ReadSpeed.ValueInMBps.ToString()), existing.ReadSpeed.ValueInMBps)),
            WriteSpeed = DataSpeed.FromMBps(SafeParseInt(request.Fields.GetValueOrDefault("WriteSpeed", existing.WriteSpeed.ValueInMBps.ToString()), existing.WriteSpeed.ValueInMBps))
        };
    }

    private static CoolerProduct UpdateCoolerProduct(CoolerProduct existing, UpdateProductRequest request)
    {
        string socketsStr = request.Fields.GetValueOrDefault("Sockets", string.Join(",", existing.Sockets.Select(s => s.ToString())));
        string[] socketArr = socketsStr.Split(',', StringSplitOptions.RemoveEmptyEntries);
        CpuSocket[] sockets = socketArr.Select(s => ParseEnum<CpuSocket>(s.Trim())).ToArray();

        return existing with
        {
            Name = request.Name,
            Price = request.Price,
            Manufacturer = request.Manufacturer,
            Dimensions = ParseDimensions(request.Fields.GetValueOrDefault("Dimensions", $"{existing.Dimensions.Length},{existing.Dimensions.Width},{existing.Dimensions.Height}")),
            CoolerType = ParseEnum<CoolerType>(request.Fields.GetValueOrDefault("CoolerType", existing.CoolerType.ToString())),
            Height = Length.FromMm(SafeParseInt(request.Fields.GetValueOrDefault("Height", existing.Height.ValueInMm.ToString()), existing.Height.ValueInMm)),
            TDP = Power.FromWatts(SafeParseInt(request.Fields.GetValueOrDefault("TDP", existing.TDP.ValueInWatts.ToString()), existing.TDP.ValueInWatts)),
            Sockets = sockets
        };
    }

    // Helper methods (reused from CreateProduct)
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

    private static int SafeParseInt(string value, int defaultValue = 0)
    {
        if (string.IsNullOrWhiteSpace(value) || value == "NaN")
        {
            return defaultValue;
        }
        
        if (int.TryParse(value, out int result))
        {
            return result;
        }
        
        return defaultValue;
    }

    private static decimal SafeParseDecimal(string value, decimal defaultValue = 0m)
    {
        if (string.IsNullOrWhiteSpace(value) || value == "NaN")
        {
            return defaultValue;
        }
        
        if (decimal.TryParse(value, out decimal result))
        {
            return result;
        }
        
        return defaultValue;
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
                sd.AllowedCategory,
                sd.Location != null ? new Vector3(sd.Location.X, sd.Location.Y, sd.Location.Z) : Vector3.Zero,
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

    private static GpuPowerConnector ParseGpuPowerConnector(string value)
    {
        string normalized = value.Replace(" ", string.Empty).Replace("-", string.Empty).ToLowerInvariant();

        return normalized switch
        {
            "1x16pin" or "16pin" => GpuPowerConnector.One16Pin,
            "2x8pin" or "dual8pin" => GpuPowerConnector.Dual8Pin,
            "3x8pin" or "triple8pin" => GpuPowerConnector.Triple8Pin,
            _ => throw new ArgumentException($"Invalid GPU power connector: {value}")
        };
    }
}

public record UpdateProductRequest(
    ProductCategory Category,
    string Name,
    decimal Price,
    string Manufacturer,
    Dictionary<string, string> Fields
);

public record UpdateProductResponse(Guid Id);
