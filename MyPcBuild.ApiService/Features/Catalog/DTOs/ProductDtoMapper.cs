using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;

namespace MyPcBuild.ApiService.Features.Catalog.DTOs;

/// <summary>
/// Maps between API DTOs (Request/Response) and domain models.
/// </summary>
public static class ProductDtoMapper
{
    /// <summary>
    /// Maps a domain Product to an API Response.
    /// </summary>
    public static ProductResponse ToResponse(Product product)
    {
        return product switch
        {
            CpuProduct cpu => ToCpuResponse(cpu),
            MotherboardProduct mb => ToMotherboardResponse(mb),
            GpuProduct gpu => ToGpuResponse(gpu),
            RamProduct ram => ToRamResponse(ram),
            PcCaseProduct pcCase => ToPcCaseResponse(pcCase),
            PsuProduct psu => ToPsuResponse(psu),
            StorageProduct storage => ToStorageResponse(storage),
            CoolerProduct cooler => ToCoolerResponse(cooler),
            _ => throw new ArgumentException($"Unknown product type: {product.GetType().Name}")
        };
    }

    /// <summary>
    /// Maps an API Request to a domain Product.
    /// </summary>
    public static Product ToDomain(ProductRequest request, Guid? id = null)
    {
        Guid productId = id ?? Guid.NewGuid();
        
        return request switch
        {
            CpuProductRequest cpu => ToCpuDomain(cpu, productId),
            MotherboardProductRequest mb => ToMotherboardDomain(mb, productId),
            GpuProductRequest gpu => ToGpuDomain(gpu, productId),
            RamProductRequest ram => ToRamDomain(ram, productId),
            PcCaseProductRequest pcCase => ToPcCaseDomain(pcCase, productId),
            PsuProductRequest psu => ToPsuDomain(psu, productId),
            StorageProductRequest storage => ToStorageDomain(storage, productId),
            CoolerProductRequest cooler => ToCoolerDomain(cooler, productId),
            _ => throw new ArgumentException($"Unknown request type: {request.GetType().Name}")
        };
    }

    // Domain to Response mappings

    private static CpuProductResponse ToCpuResponse(CpuProduct cpu)
    {
        return new CpuProductResponse
        {
            Id = cpu.Id,
            Category = cpu.ProductCategory,
            Name = cpu.Name,
            Price = cpu.Price,
            Manufacturer = cpu.Manufacturer,
            IsDraft = cpu.IsDraft,
            PublishedAt = cpu.PublishedAt,
            Socket = ToApiCpuSocket(cpu.Socket),
            Cores = cpu.Cores,
            Threads = cpu.Threads,
            BaseClock = ApiFrequency.FromGHz(cpu.BaseClock.ValueInGHz),
            BoostClock = ApiFrequency.FromGHz(cpu.BoostClock.ValueInGHz),
            TDP = ApiPower.FromWatts(cpu.TDP.ValueInWatts),
            IntegratedGraphics = cpu.IntegratedGraphics
        };
    }

    private static MotherboardProductResponse ToMotherboardResponse(MotherboardProduct mb)
    {
        return new MotherboardProductResponse
        {
            Id = mb.Id,
            Category = mb.ProductCategory,
            Name = mb.Name,
            Price = mb.Price,
            Manufacturer = mb.Manufacturer,
            IsDraft = mb.IsDraft,
            PublishedAt = mb.PublishedAt,
            Socket = ToApiCpuSocket(mb.Socket),
            Chipset = mb.Chipset,
            FormFactor = ToApiFormFactor(mb.FormFactor),
            MemoryType = ToApiMemoryType(mb.MemoryType),
            MaxMemory = ApiStorageCapacity.FromGB(mb.MaxMemory.ValueInGB),
            Dimensions = ToDimensionsModel(mb.Dimensions),
            Slots = mb.Slots.Select(ToSlotModel).ToList()
        };
    }

    private static GpuProductResponse ToGpuResponse(GpuProduct gpu)
    {
        return new GpuProductResponse
        {
            Id = gpu.Id,
            Category = gpu.ProductCategory,
            Name = gpu.Name,
            Price = gpu.Price,
            Manufacturer = gpu.Manufacturer,
            IsDraft = gpu.IsDraft,
            PublishedAt = gpu.PublishedAt,
            ChipsetManufacturer = gpu.ChipsetManufacturer,
            Series = gpu.Series,
            VRAM = ApiStorageCapacity.FromGB(gpu.VRAM.ValueInGB),
            MemoryType = ToApiMemoryType(gpu.MemoryType),
            CoreClock = ApiFrequency.FromMHz((int)gpu.CoreClock.ToMHz()),
            BoostClock = ApiFrequency.FromMHz((int)gpu.BoostClock.ToMHz()),
            TDP = ApiPower.FromWatts(gpu.TDP.ValueInWatts),
            Length = ApiLength.FromMm(gpu.Length.ValueInMm),
            PowerConnectors = ToApiGpuPowerConnector(gpu.PowerConnectors),
            RayTracing = gpu.RayTracing,
            Dimensions = ToDimensionsModel(gpu.Dimensions),
            Slots = gpu.Slots.Select(ToSlotModel).ToList()
        };
    }

    private static RamProductResponse ToRamResponse(RamProduct ram)
    {
        return new RamProductResponse
        {
            Id = ram.Id,
            Category = ram.ProductCategory,
            Name = ram.Name,
            Price = ram.Price,
            Manufacturer = ram.Manufacturer,
            IsDraft = ram.IsDraft,
            PublishedAt = ram.PublishedAt,
            Type = ToApiMemoryType(ram.Type),
            Capacity = ApiStorageCapacity.FromGB(ram.Capacity.ValueInGB),
            Configuration = ram.Configuration,
            Speed = ApiFrequency.FromMHz((int)ram.Speed.ToMHz()),
            CASLatency = ram.CASLatency,
            Voltage = ApiVoltage.FromVolts(ram.Voltage.ValueInVolts)
        };
    }

    private static PcCaseProductResponse ToPcCaseResponse(PcCaseProduct pcCase)
    {
        return new PcCaseProductResponse
        {
            Id = pcCase.Id,
            Category = pcCase.ProductCategory,
            Name = pcCase.Name,
            Price = pcCase.Price,
            Manufacturer = pcCase.Manufacturer,
            IsDraft = pcCase.IsDraft,
            PublishedAt = pcCase.PublishedAt,
            FormFactor = pcCase.FormFactor,
            Color = pcCase.Color,
            SidePanelWindow = pcCase.SidePanelWindow,
            Dimensions = ToDimensionsModel(pcCase.Dimensions),
            Chambers = pcCase.Chambers.Select(ToChamberModel).ToList()
        };
    }

    private static PsuProductResponse ToPsuResponse(PsuProduct psu)
    {
        return new PsuProductResponse
        {
            Id = psu.Id,
            Category = psu.ProductCategory,
            Name = psu.Name,
            Price = psu.Price,
            Manufacturer = psu.Manufacturer,
            IsDraft = psu.IsDraft,
            PublishedAt = psu.PublishedAt,
            Wattage = ApiPower.FromWatts(psu.Wattage.ValueInWatts),
            Efficiency = psu.Efficiency,
            Modular = psu.Modular,
            FormFactor = psu.FormFactor,
            Length = ApiLength.FromMm(psu.Length.ValueInMm),
            PCIe8Pin = psu.PCIe8Pin
        };
    }

    private static StorageProductResponse ToStorageResponse(StorageProduct storage)
    {
        return new StorageProductResponse
        {
            Id = storage.Id,
            Category = storage.ProductCategory,
            Name = storage.Name,
            Price = storage.Price,
            Manufacturer = storage.Manufacturer,
            IsDraft = storage.IsDraft,
            PublishedAt = storage.PublishedAt,
            Type = storage.Type,
            Interface = storage.Interface,
            StorageFormFactor = storage.StorageFormFactor,
            Capacity = ApiStorageCapacity.FromGB(storage.Capacity.ValueInGB),
            ReadSpeed = ApiDataSpeed.FromMBps(storage.ReadSpeed.ValueInMBps),
            WriteSpeed = ApiDataSpeed.FromMBps(storage.WriteSpeed.ValueInMBps)
        };
    }

    private static CoolerProductResponse ToCoolerResponse(CoolerProduct cooler)
    {
        return new CoolerProductResponse
        {
            Id = cooler.Id,
            Category = cooler.ProductCategory,
            Name = cooler.Name,
            Price = cooler.Price,
            Manufacturer = cooler.Manufacturer,
            IsDraft = cooler.IsDraft,
            PublishedAt = cooler.PublishedAt,
            CoolerType = ToApiCoolerType(cooler.CoolerType),
            Height = ApiLength.FromMm(cooler.Height.ValueInMm),
            TDP = ApiPower.FromWatts(cooler.TDP.ValueInWatts),
            Sockets = cooler.Sockets.Select(ToApiCpuSocket).ToList(),
            Dimensions = ToDimensionsModel(cooler.Dimensions)
        };
    }

    // Request to Domain mappings

    private static CpuProduct ToCpuDomain(CpuProductRequest request, Guid id)
    {
        return new CpuProduct(
            id,
            request.Name,
            request.Price,
            request.Manufacturer,
            ToDomainCpuSocket(request.Socket),
            request.Cores,
            request.Threads,
            Frequency.FromGHz(request.BaseClock.ValueInGHz),
            Frequency.FromGHz(request.BoostClock.ValueInGHz),
            Power.FromWatts(request.TDP.ValueInWatts),
            request.IntegratedGraphics
        );
    }

    private static MotherboardProduct ToMotherboardDomain(MotherboardProductRequest request, Guid id)
    {
        return new MotherboardProduct(
            id,
            request.Name,
            request.Price,
            request.Manufacturer,
            ToDomainDimensions(request.Dimensions),
            request.Slots?.Select(ToDomainSlot).ToList() ?? [],
            ToDomainCpuSocket(request.Socket),
            request.Chipset,
            ToDomainFormFactor(request.FormFactor),
            ToDomainMemoryType(request.MemoryType),
            StorageCapacity.FromGB(request.MaxMemory.ValueInGB)
        );
    }

    private static GpuProduct ToGpuDomain(GpuProductRequest request, Guid id)
    {
        return new GpuProduct(
            id,
            request.Name,
            request.Price,
            request.Manufacturer,
            ToDomainDimensions(request.Dimensions),
            request.Slots?.Select(ToDomainSlot).ToList() ?? [],
            request.ChipsetManufacturer,
            request.Series,
            StorageCapacity.FromGB(request.VRAM.ValueInGB),
            ToDomainMemoryType(request.MemoryType),
            Frequency.FromMHz((int)request.CoreClock.ToMHz()),
            Frequency.FromMHz((int)request.BoostClock.ToMHz()),
            Power.FromWatts(request.TDP.ValueInWatts),
            Length.FromMm(request.Length.ValueInMm),
            ToDomainGpuPowerConnector(request.PowerConnectors),
            request.RayTracing
        );
    }

    private static RamProduct ToRamDomain(RamProductRequest request, Guid id)
    {
        return new RamProduct(
            id,
            request.Name,
            request.Price,
            request.Manufacturer,
            ToDomainMemoryType(request.Type),
            StorageCapacity.FromGB(request.Capacity.ValueInGB),
            request.Configuration,
            Frequency.FromMHz((int)request.Speed.ToMHz()),
            request.CASLatency,
            Voltage.FromVolts(request.Voltage.ValueInVolts)
        );
    }

    private static PcCaseProduct ToPcCaseDomain(PcCaseProductRequest request, Guid id)
    {
        return new PcCaseProduct(
            id,
            request.Name,
            request.Price,
            request.Manufacturer,
            ToDomainDimensions(request.Dimensions),
            request.Chambers?.Select(ToDomainChamber).ToList() ?? [],
            request.FormFactor,
            request.Color,
            request.SidePanelWindow
        );
    }

    private static PsuProduct ToPsuDomain(PsuProductRequest request, Guid id)
    {
        return new PsuProduct(
            id,
            request.Name,
            request.Price,
            request.Manufacturer,
            Power.FromWatts(request.Wattage.ValueInWatts),
            request.Efficiency,
            request.Modular,
            request.FormFactor,
            Length.FromMm(request.Length.ValueInMm),
            request.PCIe8Pin
        );
    }

    private static StorageProduct ToStorageDomain(StorageProductRequest request, Guid id)
    {
        return new StorageProduct(
            id,
            request.Name,
            request.Price,
            request.Manufacturer,
            request.Type,
            request.Interface,
            request.StorageFormFactor,
            StorageCapacity.FromGB(request.Capacity.ValueInGB),
            DataSpeed.FromMBps(request.ReadSpeed.ValueInMBps),
            DataSpeed.FromMBps(request.WriteSpeed.ValueInMBps)
        );
    }

    private static CoolerProduct ToCoolerDomain(CoolerProductRequest request, Guid id)
    {
        return new CoolerProduct(
            id,
            request.Name,
            request.Price,
            request.Manufacturer,
            ToDomainDimensions(request.Dimensions),
            ToDomainCoolerType(request.CoolerType),
            Length.FromMm(request.Height.ValueInMm),
            Power.FromWatts(request.TDP.ValueInWatts),
            request.Sockets.Select(ToDomainCpuSocket).ToArray()
        );
    }

    // Helper conversion methods

    private static ApiCpuSocket ToApiCpuSocket(CpuSocket socket) => socket switch
    {
        CpuSocket.LGA1700 => ApiCpuSocket.LGA1700,
        CpuSocket.LGA1200 => ApiCpuSocket.LGA1200,
        CpuSocket.LGA1151 => ApiCpuSocket.LGA1151,
        CpuSocket.LGA2066 => ApiCpuSocket.LGA2066,
        CpuSocket.AM5 => ApiCpuSocket.AM5,
        CpuSocket.AM4 => ApiCpuSocket.AM4,
        CpuSocket.sTRX4 => ApiCpuSocket.sTRX4,
        CpuSocket.TR4 => ApiCpuSocket.TR4,
        _ => throw new ArgumentException($"Unknown CPU socket: {socket}")
    };

    private static CpuSocket ToDomainCpuSocket(ApiCpuSocket socket) => socket switch
    {
        ApiCpuSocket.LGA1700 => CpuSocket.LGA1700,
        ApiCpuSocket.LGA1200 => CpuSocket.LGA1200,
        ApiCpuSocket.LGA1151 => CpuSocket.LGA1151,
        ApiCpuSocket.LGA2066 => CpuSocket.LGA2066,
        ApiCpuSocket.AM5 => CpuSocket.AM5,
        ApiCpuSocket.AM4 => CpuSocket.AM4,
        ApiCpuSocket.sTRX4 => CpuSocket.sTRX4,
        ApiCpuSocket.TR4 => CpuSocket.TR4,
        _ => throw new ArgumentException($"Unknown API CPU socket: {socket}")
    };

    private static ApiMemoryType ToApiMemoryType(MemoryType type) => type switch
    {
        MemoryType.DDR3 => ApiMemoryType.DDR3,
        MemoryType.DDR4 => ApiMemoryType.DDR4,
        MemoryType.DDR5 => ApiMemoryType.DDR5,
        MemoryType.GDDR5 => ApiMemoryType.GDDR5,
        MemoryType.GDDR5X => ApiMemoryType.GDDR5X,
        MemoryType.GDDR6 => ApiMemoryType.GDDR6,
        MemoryType.GDDR6X => ApiMemoryType.GDDR6X,
        MemoryType.HBM2 => ApiMemoryType.HBM2,
        MemoryType.HBM2E => ApiMemoryType.HBM2E,
        MemoryType.HBM3 => ApiMemoryType.HBM3,
        _ => throw new ArgumentException($"Unknown memory type: {type}")
    };

    private static MemoryType ToDomainMemoryType(ApiMemoryType type) => type switch
    {
        ApiMemoryType.DDR3 => MemoryType.DDR3,
        ApiMemoryType.DDR4 => MemoryType.DDR4,
        ApiMemoryType.DDR5 => MemoryType.DDR5,
        ApiMemoryType.GDDR5 => MemoryType.GDDR5,
        ApiMemoryType.GDDR5X => MemoryType.GDDR5X,
        ApiMemoryType.GDDR6 => MemoryType.GDDR6,
        ApiMemoryType.GDDR6X => MemoryType.GDDR6X,
        ApiMemoryType.HBM2 => MemoryType.HBM2,
        ApiMemoryType.HBM2E => MemoryType.HBM2E,
        ApiMemoryType.HBM3 => MemoryType.HBM3,
        _ => throw new ArgumentException($"Unknown API memory type: {type}")
    };

    private static ApiFormFactor ToApiFormFactor(FormFactor formFactor) => formFactor switch
    {
        FormFactor.ATX => ApiFormFactor.ATX,
        FormFactor.MicroATX => ApiFormFactor.MicroATX,
        FormFactor.MiniITX => ApiFormFactor.MiniITX,
        FormFactor.EATX => ApiFormFactor.EATX,
        _ => throw new ArgumentException($"Unknown form factor: {formFactor}")
    };

    private static FormFactor ToDomainFormFactor(ApiFormFactor formFactor) => formFactor switch
    {
        ApiFormFactor.ATX => FormFactor.ATX,
        ApiFormFactor.MicroATX => FormFactor.MicroATX,
        ApiFormFactor.MiniITX => FormFactor.MiniITX,
        ApiFormFactor.EATX => FormFactor.EATX,
        _ => throw new ArgumentException($"Unknown API form factor: {formFactor}")
    };

    private static ApiCoolerType ToApiCoolerType(CoolerType coolerType) => coolerType switch
    {
        CoolerType.Air => ApiCoolerType.Air,
        CoolerType.AIO => ApiCoolerType.AIO,
        CoolerType.CustomLoop => ApiCoolerType.CustomLoop,
        _ => throw new ArgumentException($"Unknown cooler type: {coolerType}")
    };

    private static CoolerType ToDomainCoolerType(ApiCoolerType coolerType) => coolerType switch
    {
        ApiCoolerType.Air => CoolerType.Air,
        ApiCoolerType.AIO => CoolerType.AIO,
        ApiCoolerType.CustomLoop => CoolerType.CustomLoop,
        _ => throw new ArgumentException($"Unknown API cooler type: {coolerType}")
    };

    private static ApiGpuPowerConnector ToApiGpuPowerConnector(GpuPowerConnector connector) => connector switch
    {
        GpuPowerConnector.Dual8Pin => ApiGpuPowerConnector.Dual8Pin,
        GpuPowerConnector.Triple8Pin => ApiGpuPowerConnector.Triple8Pin,
        GpuPowerConnector.One16Pin => ApiGpuPowerConnector.One16Pin,
        _ => throw new ArgumentException($"Unknown GPU power connector: {connector}")
    };

    private static GpuPowerConnector ToDomainGpuPowerConnector(ApiGpuPowerConnector connector) => connector switch
    {
        ApiGpuPowerConnector.Dual8Pin => GpuPowerConnector.Dual8Pin,
        ApiGpuPowerConnector.Triple8Pin => GpuPowerConnector.Triple8Pin,
        ApiGpuPowerConnector.One16Pin => GpuPowerConnector.One16Pin,
        _ => throw new ArgumentException($"Unknown API GPU power connector: {connector}")
    };

    private static DimensionsModel ToDimensionsModel(Dimensions dimensions)
    {
        return new DimensionsModel
        {
            Length = dimensions.Length,
            Width = dimensions.Width,
            Height = dimensions.Height
        };
    }

    private static Dimensions ToDomainDimensions(DimensionsModel dimensions)
    {
        return new Dimensions(dimensions.Length, dimensions.Width, dimensions.Height);
    }

    private static SlotModel ToSlotModel(Slot slot)
    {
        return new SlotModel
        {
            Name = slot.Name,
            AllowedCategory = slot.AllowedProductCategory.ToString(),
            RelativePosition = new Vector3Model
            {
                X = slot.RelativePosition.X,
                Y = slot.RelativePosition.Y,
                Z = slot.RelativePosition.Z
            },
            MaxDimensions = new DimensionsModel
            {
                Length = slot.MaxDimensions.Length,
                Width = slot.MaxDimensions.Width,
                Height = slot.MaxDimensions.Height
            },
            Rotation = slot.Rotation != Rotation.Identity 
                ? new RotationModel 
                { 
                    X = slot.Rotation.X, 
                    Y = slot.Rotation.Y, 
                    Z = slot.Rotation.Z 
                } 
                : null,
            SubSlots = slot.SubSlots?.Select(ToSlotModel).ToList()
        };
    }

    private static Slot ToDomainSlot(SlotModel slot)
    {
        ProductCategory category = Enum.Parse<ProductCategory>(slot.AllowedCategory, ignoreCase: true);
        Vector3 position = new Vector3(
            slot.RelativePosition.X, 
            slot.RelativePosition.Y, 
            slot.RelativePosition.Z
        );
        Dimensions maxDimensions = new Dimensions(
            slot.MaxDimensions.Length,
            slot.MaxDimensions.Width,
            slot.MaxDimensions.Height
        );
        Rotation rotation = slot.Rotation != null
            ? new Rotation(slot.Rotation.X, slot.Rotation.Y, slot.Rotation.Z)
            : Rotation.Identity;

        return new Slot(
            Guid.NewGuid(),
            slot.Name,
            category,
            position,
            maxDimensions,
            rotation,
            slot.SubSlots?.Select(ToDomainSlot).ToList()
        );
    }

    private static ChamberModel ToChamberModel(Chamber chamber)
    {
        return new ChamberModel
        {
            Name = chamber.Name,
            RelativePosition = new Vector3Model
            {
                X = chamber.RelativePosition.X,
                Y = chamber.RelativePosition.Y,
                Z = chamber.RelativePosition.Z
            },
            Dimensions = ToDimensionsModel(chamber.Dimensions),
            Slots = chamber.Slots.Select(ToSlotModel).ToList()
        };
    }

    private static Chamber ToDomainChamber(ChamberModel chamber)
    {
        return new Chamber(
            Guid.NewGuid(),
            chamber.Name,
            new Vector3(
                chamber.RelativePosition.X,
                chamber.RelativePosition.Y,
                chamber.RelativePosition.Z
            ),
            ToDomainDimensions(chamber.Dimensions),
            chamber.Slots.Select(ToDomainSlot).ToList()
        );
    }
}
