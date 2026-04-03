using MyPcBuild.ApiService.Catalog.Models;
using MyPcBuild.ApiService.SharedDomain.Spatial;

namespace MyPcBuild.ApiService.Catalog.DTOs;

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
            Dimensions = mb.Dimensions.ToDimensionsModel(),
            Slots = [.. mb.Slots.Select(s => s.ToSlotModel())]
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
            ChipsetManufacturer = ToApiGpuChipsetManufacturer(gpu.ChipsetManufacturer),
            Series = gpu.Series,
            VRAM = ApiStorageCapacity.FromGB(gpu.VRAM.ValueInGB),
            MemoryType = ToApiMemoryType(gpu.MemoryType),
            CoreClock = ApiFrequency.FromMHz((int)gpu.CoreClock.ToMHz()),
            BoostClock = ApiFrequency.FromMHz((int)gpu.BoostClock.ToMHz()),
            TDP = ApiPower.FromWatts(gpu.TDP.ValueInWatts),
            PowerConnectors = ToApiGpuPowerConnector(gpu.PowerConnectors),
            RayTracing = gpu.RayTracing,
            Dimensions = gpu.Dimensions.ToDimensionsModel(),
            Slots = [.. gpu.Slots.Select(s => s.ToSlotModel())]
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
            Configuration = ToApiRamConfiguration(ram.Configuration),
            Speed = ApiFrequency.FromMHz((int)ram.Speed.ToMHz()),
            CASLatency = ToApiCasLatency(ram.CASLatency),
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
            FormFactor = ToApiFormFactor(pcCase.FormFactor),
            Color = pcCase.Color,
            SidePanelWindow = ToApiSidePanelType(pcCase.SidePanelWindow),
            Dimensions = pcCase.Dimensions.ToDimensionsModel(),
            Chambers = [.. pcCase.Chambers.Select(c => c.ToChamberModel())]
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
            Efficiency = ToApiPsuEfficiency(psu.Efficiency),
            Modular = ToApiPsuModularity(psu.Modular),
            FormFactor = ToApiPsuFormFactor(psu.FormFactor),
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
            Type = ToApiStorageType(storage.Type),
            Interface = ToApiStorageInterface(storage.Interface),
            StorageFormFactor = ToApiStorageFormFactor(storage.StorageFormFactor),
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
            Dimensions = cooler.Dimensions.ToDimensionsModel()
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
            request.Dimensions.ToDomainDimensions(),
            request.Slots?.Select(s => s.ToDomainSlot()).ToList() ?? [],
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
            request.Dimensions.ToDomainDimensions(),
            request.Slots?.Select(s => s.ToDomainSlot()).ToList() ?? [],
            ToDomainGpuChipsetManufacturer(request.ChipsetManufacturer),
            request.Series,
            StorageCapacity.FromGB(request.VRAM.ValueInGB),
            ToDomainMemoryType(request.MemoryType),
            Frequency.FromMHz((int)request.CoreClock.ToMHz()),
            Frequency.FromMHz((int)request.BoostClock.ToMHz()),
            Power.FromWatts(request.TDP.ValueInWatts),
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
            ToDomainRamConfiguration(request.Configuration),
            Frequency.FromMHz((int)request.Speed.ToMHz()),
            ToDomainCasLatency(request.CASLatency),
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
            request.Dimensions.ToDomainDimensions(),
            request.Chambers?.Select(c => c.ToDomainChamber()).ToList() ?? [],
            ToDomainFormFactor(request.FormFactor),
            request.Color,
            ToDomainSidePanelType(request.SidePanelWindow)
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
            ToDomainPsuEfficiency(request.Efficiency),
            ToDomainPsuModularity(request.Modular),
            ToDomainPsuFormFactor(request.FormFactor),
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
            ToDomainStorageType(request.Type),
            ToDomainStorageInterface(request.Interface),
            ToDomainStorageFormFactor(request.StorageFormFactor),
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
            request.Dimensions.ToDomainDimensions(),
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

    private static ApiGpuChipsetManufacturer ToApiGpuChipsetManufacturer(GpuChipsetManufacturer m) => m switch
    {
        GpuChipsetManufacturer.NVIDIA => ApiGpuChipsetManufacturer.NVIDIA,
        GpuChipsetManufacturer.AMD => ApiGpuChipsetManufacturer.AMD,
        GpuChipsetManufacturer.Intel => ApiGpuChipsetManufacturer.Intel,
        _ => throw new ArgumentException($"Unknown GPU chipset manufacturer: {m}")
    };

    private static GpuChipsetManufacturer ToDomainGpuChipsetManufacturer(ApiGpuChipsetManufacturer m) => m switch
    {
        ApiGpuChipsetManufacturer.NVIDIA => GpuChipsetManufacturer.NVIDIA,
        ApiGpuChipsetManufacturer.AMD => GpuChipsetManufacturer.AMD,
        ApiGpuChipsetManufacturer.Intel => GpuChipsetManufacturer.Intel,
        _ => throw new ArgumentException($"Unknown API GPU chipset manufacturer: {m}")
    };

    private static ApiSidePanelType ToApiSidePanelType(SidePanelType t) => t switch
    {
        SidePanelType.None => ApiSidePanelType.None,
        SidePanelType.Acrylic => ApiSidePanelType.Acrylic,
        SidePanelType.TemperedGlass => ApiSidePanelType.TemperedGlass,
        _ => throw new ArgumentException($"Unknown side panel type: {t}")
    };

    private static SidePanelType ToDomainSidePanelType(ApiSidePanelType t) => t switch
    {
        ApiSidePanelType.None => SidePanelType.None,
        ApiSidePanelType.Acrylic => SidePanelType.Acrylic,
        ApiSidePanelType.TemperedGlass => SidePanelType.TemperedGlass,
        _ => throw new ArgumentException($"Unknown API side panel type: {t}")
    };

    private static ApiPsuEfficiency ToApiPsuEfficiency(PsuEfficiency e) => e switch
    {
        PsuEfficiency.Bronze => ApiPsuEfficiency.Bronze,
        PsuEfficiency.Silver => ApiPsuEfficiency.Silver,
        PsuEfficiency.Gold => ApiPsuEfficiency.Gold,
        PsuEfficiency.Platinum => ApiPsuEfficiency.Platinum,
        PsuEfficiency.Titanium => ApiPsuEfficiency.Titanium,
        _ => throw new ArgumentException($"Unknown PSU efficiency: {e}")
    };

    private static PsuEfficiency ToDomainPsuEfficiency(ApiPsuEfficiency e) => e switch
    {
        ApiPsuEfficiency.Bronze => PsuEfficiency.Bronze,
        ApiPsuEfficiency.Silver => PsuEfficiency.Silver,
        ApiPsuEfficiency.Gold => PsuEfficiency.Gold,
        ApiPsuEfficiency.Platinum => PsuEfficiency.Platinum,
        ApiPsuEfficiency.Titanium => PsuEfficiency.Titanium,
        _ => throw new ArgumentException($"Unknown API PSU efficiency: {e}")
    };

    private static ApiPsuModularity ToApiPsuModularity(PsuModularity m) => m switch
    {
        PsuModularity.NonModular => ApiPsuModularity.NonModular,
        PsuModularity.SemiModular => ApiPsuModularity.SemiModular,
        PsuModularity.FullyModular => ApiPsuModularity.FullyModular,
        _ => throw new ArgumentException($"Unknown PSU modularity: {m}")
    };

    private static PsuModularity ToDomainPsuModularity(ApiPsuModularity m) => m switch
    {
        ApiPsuModularity.NonModular => PsuModularity.NonModular,
        ApiPsuModularity.SemiModular => PsuModularity.SemiModular,
        ApiPsuModularity.FullyModular => PsuModularity.FullyModular,
        _ => throw new ArgumentException($"Unknown API PSU modularity: {m}")
    };

    private static ApiPsuFormFactor ToApiPsuFormFactor(PsuFormFactor f) => f switch
    {
        PsuFormFactor.ATX => ApiPsuFormFactor.ATX,
        PsuFormFactor.SFX => ApiPsuFormFactor.SFX,
        PsuFormFactor.SFXL => ApiPsuFormFactor.SFXL,
        _ => throw new ArgumentException($"Unknown PSU form factor: {f}")
    };

    private static PsuFormFactor ToDomainPsuFormFactor(ApiPsuFormFactor f) => f switch
    {
        ApiPsuFormFactor.ATX => PsuFormFactor.ATX,
        ApiPsuFormFactor.SFX => PsuFormFactor.SFX,
        ApiPsuFormFactor.SFXL => PsuFormFactor.SFXL,
        _ => throw new ArgumentException($"Unknown API PSU form factor: {f}")
    };

    private static ApiStorageType ToApiStorageType(StorageType t) => t switch
    {
        StorageType.SSD => ApiStorageType.SSD,
        StorageType.HDD => ApiStorageType.HDD,
        _ => throw new ArgumentException($"Unknown storage type: {t}")
    };

    private static StorageType ToDomainStorageType(ApiStorageType t) => t switch
    {
        ApiStorageType.SSD => StorageType.SSD,
        ApiStorageType.HDD => StorageType.HDD,
        _ => throw new ArgumentException($"Unknown API storage type: {t}")
    };

    private static ApiStorageInterface ToApiStorageInterface(StorageInterface i) => i switch
    {
        StorageInterface.NVMe => ApiStorageInterface.NVMe,
        StorageInterface.SATA => ApiStorageInterface.SATA,
        _ => throw new ArgumentException($"Unknown storage interface: {i}")
    };

    private static StorageInterface ToDomainStorageInterface(ApiStorageInterface i) => i switch
    {
        ApiStorageInterface.NVMe => StorageInterface.NVMe,
        ApiStorageInterface.SATA => StorageInterface.SATA,
        _ => throw new ArgumentException($"Unknown API storage interface: {i}")
    };

    private static ApiStorageFormFactor ToApiStorageFormFactor(StorageFormFactor f) => f switch
    {
        StorageFormFactor.M2_2280 => ApiStorageFormFactor.M2_2280,
        StorageFormFactor.TwoPointFiveInch => ApiStorageFormFactor.TwoPointFiveInch,
        StorageFormFactor.ThreePointFiveInch => ApiStorageFormFactor.ThreePointFiveInch,
        _ => throw new ArgumentException($"Unknown storage form factor: {f}")
    };

    private static StorageFormFactor ToDomainStorageFormFactor(ApiStorageFormFactor f) => f switch
    {
        ApiStorageFormFactor.M2_2280 => StorageFormFactor.M2_2280,
        ApiStorageFormFactor.TwoPointFiveInch => StorageFormFactor.TwoPointFiveInch,
        ApiStorageFormFactor.ThreePointFiveInch => StorageFormFactor.ThreePointFiveInch,
        _ => throw new ArgumentException($"Unknown API storage form factor: {f}")
    };

    private static ApiRamConfiguration ToApiRamConfiguration(RamConfiguration c) =>
        ApiRamConfiguration.From(c.ModuleCount, c.ModuleCapacity.ValueInGB);

    private static RamConfiguration ToDomainRamConfiguration(ApiRamConfiguration c) =>
        RamConfiguration.From(c.ModuleCount, c.ModuleCapacity.ValueInGB);

    private static ApiCasLatency ToApiCasLatency(CasLatency c) => ApiCasLatency.FromInt(c.Value);

    private static CasLatency ToDomainCasLatency(ApiCasLatency c) => CasLatency.FromInt(c.Value);
}

public static class MapperExtensions
{
    public static Vector3Model ToVector3Model(this Vector3? relativePosition)
    {
        return relativePosition is null
            ? new Vector3Model { X = 0, Y = 0, Z = 0 }
            : new Vector3Model
            {
                X = relativePosition.X,
                Y = relativePosition.Y,
                Z = relativePosition.Z
            };
    }

    public static Vector3 ToDomainVector3(this Vector3Model? model)
    {
        return model is null
            ? Vector3.Zero
            : new Vector3(
                model.X,
                model.Y,
                model.Z
            );
    }

    public static ChamberModel ToChamberModel(this Chamber chamber)
    {
        return new ChamberModel
        {
            Name = chamber.Name,
            RelativePosition = chamber.RelativePosition.ToVector3Model(),
            Dimensions = chamber.Dimensions.ToDimensionsModel(),
            Slots = chamber.Slots.Select(ToSlotModel).ToList() ?? []
        };
    }

    public static Chamber ToDomainChamber(this ChamberModel chamber)
    {
        return new Chamber(
            Guid.NewGuid(),
            chamber.Name,
            chamber.RelativePosition.ToDomainVector3(),
            chamber.Dimensions.ToDomainDimensions(),
            [.. chamber.Slots.Select(ToDomainSlot)]
        );
    }

    public static DimensionsModel ToDimensionsModel(this Dimensions dimensions)
    {
        return new DimensionsModel
        {
            Length = dimensions.Length,
            Width = dimensions.Width,
            Height = dimensions.Height
        };
    }

    public static Dimensions ToDomainDimensions(this DimensionsModel dimensions)
    {
        return new Dimensions(dimensions.Length, dimensions.Width, dimensions.Height);
    }

    public static SlotModel ToSlotModel(this Slot slot)
    {
        return new SlotModel
        {
            Name = slot.Name,
            AllowedCategory = slot.AllowedProductCategory,
            RelativePosition = slot.RelativePosition.ToVector3Model(),
            MaxDimensions = slot.MaxDimensions.ToDimensionsModel(),
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

    public static Slot ToDomainSlot(this SlotModel slot)
    {
        return new Slot(
            Guid.NewGuid(),
            slot.Name,
            slot.AllowedCategory,
            slot.RelativePosition.ToDomainVector3(),
            slot.MaxDimensions.ToDomainDimensions(),
            slot.Rotation.ToDomainRotation(),
            slot.SubSlots?.Select(ToDomainSlot).ToList()
        );
    }

    public static Rotation ToDomainRotation(this RotationModel? rotation)
    {
        return rotation is null ?
            Rotation.Identity :
            new Rotation(rotation.X, rotation.Y, rotation.Z);
    }
}
