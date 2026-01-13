using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;

namespace MyPcBuild.ApiService.Features.Catalog.DTOs;

/// <summary>
/// Maps between API DTOs and domain models.
/// </summary>
public static class ProductDtoMapper
{
    /// <summary>
    /// Maps a domain Product to an API DTO.
    /// </summary>
    public static ProductDto ToDto(Product product)
    {
        return product switch
        {
            CpuProduct cpu => ToCpuDto(cpu),
            MotherboardProduct mb => ToMotherboardDto(mb),
            GpuProduct gpu => ToGpuDto(gpu),
            RamProduct ram => ToRamDto(ram),
            PcCaseProduct pcCase => ToPcCaseDto(pcCase),
            PsuProduct psu => ToPsuDto(psu),
            StorageProduct storage => ToStorageDto(storage),
            CoolerProduct cooler => ToCoolerDto(cooler),
            _ => throw new ArgumentException($"Unknown product type: {product.GetType().Name}")
        };
    }

    /// <summary>
    /// Maps an API DTO to a domain Product.
    /// </summary>
    public static Product ToDomain(ProductDto dto, Guid? id = null)
    {
        return dto switch
        {
            CpuDto cpu => ToCpuDomain(cpu, id ?? Guid.NewGuid()),
            MotherboardDto mb => ToMotherboardDomain(mb, id ?? Guid.NewGuid()),
            GpuDto gpu => ToGpuDomain(gpu, id ?? Guid.NewGuid()),
            RamDto ram => ToRamDomain(ram, id ?? Guid.NewGuid()),
            PcCaseDto pcCase => ToPcCaseDomain(pcCase, id ?? Guid.NewGuid()),
            PsuDto psu => ToPsuDomain(psu, id ?? Guid.NewGuid()),
            StorageDto storage => ToStorageDomain(storage, id ?? Guid.NewGuid()),
            CoolerDto cooler => ToCoolerDomain(cooler, id ?? Guid.NewGuid()),
            _ => throw new ArgumentException($"Unknown DTO type: {dto.GetType().Name}")
        };
    }

    // Domain to DTO mappings

    private static CpuDto ToCpuDto(CpuProduct cpu)
    {
        return new CpuDto
        {
            Id = cpu.Id,
            Name = cpu.Name,
            Price = cpu.Price,
            Manufacturer = cpu.Manufacturer,
            IsDraft = cpu.IsDraft,
            PublishedAt = cpu.PublishedAt,
            Socket = ToApiCpuSocket(cpu.Socket),
            Cores = cpu.Cores,
            Threads = cpu.Threads,
            BaseClock = cpu.BaseClock.ValueInGHz,
            BoostClock = cpu.BoostClock.ValueInGHz,
            TDP = cpu.TDP.ValueInWatts,
            IntegratedGraphics = cpu.IntegratedGraphics
        };
    }

    private static MotherboardDto ToMotherboardDto(MotherboardProduct mb)
    {
        return new MotherboardDto
        {
            Id = mb.Id,
            Name = mb.Name,
            Price = mb.Price,
            Manufacturer = mb.Manufacturer,
            IsDraft = mb.IsDraft,
            PublishedAt = mb.PublishedAt,
            Socket = ToApiCpuSocket(mb.Socket),
            Chipset = mb.Chipset,
            FormFactor = ToApiFormFactor(mb.FormFactor),
            MemoryType = ToApiMemoryType(mb.MemoryType),
            MaxMemory = mb.MaxMemory.ValueInGB,
            Dimensions = ToApiDimensions(mb.Dimensions),
            Slots = mb.Slots.Select(ToApiSlot).ToList()
        };
    }

    private static GpuDto ToGpuDto(GpuProduct gpu)
    {
        return new GpuDto
        {
            Id = gpu.Id,
            Name = gpu.Name,
            Price = gpu.Price,
            Manufacturer = gpu.Manufacturer,
            IsDraft = gpu.IsDraft,
            PublishedAt = gpu.PublishedAt,
            ChipsetManufacturer = gpu.ChipsetManufacturer,
            Series = gpu.Series,
            VRAM = gpu.VRAM.ValueInGB,
            MemoryType = ToApiMemoryType(gpu.MemoryType),
            CoreClock = (int)gpu.CoreClock.ToMHz(),
            BoostClock = (int)gpu.BoostClock.ToMHz(),
            TDP = gpu.TDP.ValueInWatts,
            Length = gpu.Length.ValueInMm,
            PowerConnectors = ToApiGpuPowerConnector(gpu.PowerConnectors),
            RayTracing = gpu.RayTracing,
            Dimensions = ToApiDimensions(gpu.Dimensions),
            Slots = gpu.Slots.Select(ToApiSlot).ToList()
        };
    }

    private static RamDto ToRamDto(RamProduct ram)
    {
        return new RamDto
        {
            Id = ram.Id,
            Name = ram.Name,
            Price = ram.Price,
            Manufacturer = ram.Manufacturer,
            IsDraft = ram.IsDraft,
            PublishedAt = ram.PublishedAt,
            Type = ToApiMemoryType(ram.Type),
            Capacity = ram.Capacity.ValueInGB,
            Configuration = ram.Configuration,
            Speed = (int)ram.Speed.ToMHz(),
            CASLatency = ram.CASLatency,
            Voltage = ram.Voltage.ValueInVolts
        };
    }

    private static PcCaseDto ToPcCaseDto(PcCaseProduct pcCase)
    {
        return new PcCaseDto
        {
            Id = pcCase.Id,
            Name = pcCase.Name,
            Price = pcCase.Price,
            Manufacturer = pcCase.Manufacturer,
            IsDraft = pcCase.IsDraft,
            PublishedAt = pcCase.PublishedAt,
            FormFactor = pcCase.FormFactor,
            Color = pcCase.Color,
            SidePanelWindow = pcCase.SidePanelWindow,
            Dimensions = ToApiDimensions(pcCase.Dimensions),
            Chambers = pcCase.Chambers.Select(ToApiChamber).ToList()
        };
    }

    private static PsuDto ToPsuDto(PsuProduct psu)
    {
        return new PsuDto
        {
            Id = psu.Id,
            Name = psu.Name,
            Price = psu.Price,
            Manufacturer = psu.Manufacturer,
            IsDraft = psu.IsDraft,
            PublishedAt = psu.PublishedAt,
            Wattage = psu.Wattage.ValueInWatts,
            Efficiency = psu.Efficiency,
            Modular = psu.Modular,
            FormFactor = psu.FormFactor,
            Length = psu.Length.ValueInMm,
            PCIe8Pin = psu.PCIe8Pin
        };
    }

    private static StorageDto ToStorageDto(StorageProduct storage)
    {
        return new StorageDto
        {
            Id = storage.Id,
            Name = storage.Name,
            Price = storage.Price,
            Manufacturer = storage.Manufacturer,
            IsDraft = storage.IsDraft,
            PublishedAt = storage.PublishedAt,
            Type = storage.Type,
            Interface = storage.Interface,
            StorageFormFactor = storage.StorageFormFactor,
            Capacity = storage.Capacity.ValueInGB,
            ReadSpeed = storage.ReadSpeed.ValueInMBps,
            WriteSpeed = storage.WriteSpeed.ValueInMBps
        };
    }

    private static CoolerDto ToCoolerDto(CoolerProduct cooler)
    {
        return new CoolerDto
        {
            Id = cooler.Id,
            Name = cooler.Name,
            Price = cooler.Price,
            Manufacturer = cooler.Manufacturer,
            IsDraft = cooler.IsDraft,
            PublishedAt = cooler.PublishedAt,
            CoolerType = ToApiCoolerType(cooler.CoolerType),
            Height = cooler.Height.ValueInMm,
            TDP = cooler.TDP.ValueInWatts,
            Sockets = cooler.Sockets.Select(ToApiCpuSocket).ToList(),
            Dimensions = ToApiDimensions(cooler.Dimensions)
        };
    }

    // DTO to Domain mappings

    private static CpuProduct ToCpuDomain(CpuDto dto, Guid id)
    {
        return new CpuProduct(
            id,
            dto.Name,
            dto.Price,
            dto.Manufacturer,
            ToDomainCpuSocket(dto.Socket),
            dto.Cores,
            dto.Threads,
            Frequency.FromGHz(dto.BaseClock),
            Frequency.FromGHz(dto.BoostClock),
            Power.FromWatts(dto.TDP),
            dto.IntegratedGraphics
        )
        {
            IsDraft = dto.IsDraft,
            PublishedAt = dto.PublishedAt
        };
    }

    private static MotherboardProduct ToMotherboardDomain(MotherboardDto dto, Guid id)
    {
        return new MotherboardProduct(
            id,
            dto.Name,
            dto.Price,
            dto.Manufacturer,
            ToDomainDimensions(dto.Dimensions),
            dto.Slots?.Select(ToDomainSlot).ToList() ?? [],
            ToDomainCpuSocket(dto.Socket),
            dto.Chipset,
            ToDomainFormFactor(dto.FormFactor),
            ToDomainMemoryType(dto.MemoryType),
            StorageCapacity.FromGB(dto.MaxMemory)
        )
        {
            IsDraft = dto.IsDraft,
            PublishedAt = dto.PublishedAt
        };
    }

    private static GpuProduct ToGpuDomain(GpuDto dto, Guid id)
    {
        return new GpuProduct(
            id,
            dto.Name,
            dto.Price,
            dto.Manufacturer,
            ToDomainDimensions(dto.Dimensions),
            dto.Slots?.Select(ToDomainSlot).ToList() ?? [],
            dto.ChipsetManufacturer,
            dto.Series,
            StorageCapacity.FromGB(dto.VRAM),
            ToDomainMemoryType(dto.MemoryType),
            Frequency.FromMHz(dto.CoreClock),
            Frequency.FromMHz(dto.BoostClock),
            Power.FromWatts(dto.TDP),
            Length.FromMm(dto.Length),
            ToDomainGpuPowerConnector(dto.PowerConnectors),
            dto.RayTracing
        )
        {
            IsDraft = dto.IsDraft,
            PublishedAt = dto.PublishedAt
        };
    }

    private static RamProduct ToRamDomain(RamDto dto, Guid id)
    {
        return new RamProduct(
            id,
            dto.Name,
            dto.Price,
            dto.Manufacturer,
            ToDomainMemoryType(dto.Type),
            StorageCapacity.FromGB(dto.Capacity),
            dto.Configuration,
            Frequency.FromMHz(dto.Speed),
            dto.CASLatency,
            Voltage.FromVolts(dto.Voltage)
        )
        {
            IsDraft = dto.IsDraft,
            PublishedAt = dto.PublishedAt
        };
    }

    private static PcCaseProduct ToPcCaseDomain(PcCaseDto dto, Guid id)
    {
        return new PcCaseProduct(
            id,
            dto.Name,
            dto.Price,
            dto.Manufacturer,
            ToDomainDimensions(dto.Dimensions),
            dto.Chambers?.Select(ToDomainChamber).ToList() ?? [],
            dto.FormFactor,
            dto.Color,
            dto.SidePanelWindow
        )
        {
            IsDraft = dto.IsDraft,
            PublishedAt = dto.PublishedAt
        };
    }

    private static PsuProduct ToPsuDomain(PsuDto dto, Guid id)
    {
        return new PsuProduct(
            id,
            dto.Name,
            dto.Price,
            dto.Manufacturer,
            Power.FromWatts(dto.Wattage),
            dto.Efficiency,
            dto.Modular,
            dto.FormFactor,
            Length.FromMm(dto.Length),
            dto.PCIe8Pin
        )
        {
            IsDraft = dto.IsDraft,
            PublishedAt = dto.PublishedAt
        };
    }

    private static StorageProduct ToStorageDomain(StorageDto dto, Guid id)
    {
        return new StorageProduct(
            id,
            dto.Name,
            dto.Price,
            dto.Manufacturer,
            dto.Type,
            dto.Interface,
            dto.StorageFormFactor,
            StorageCapacity.FromGB(dto.Capacity),
            DataSpeed.FromMBps(dto.ReadSpeed),
            DataSpeed.FromMBps(dto.WriteSpeed)
        )
        {
            IsDraft = dto.IsDraft,
            PublishedAt = dto.PublishedAt
        };
    }

    private static CoolerProduct ToCoolerDomain(CoolerDto dto, Guid id)
    {
        return new CoolerProduct(
            id,
            dto.Name,
            dto.Price,
            dto.Manufacturer,
            ToDomainDimensions(dto.Dimensions),
            ToDomainCoolerType(dto.CoolerType),
            Length.FromMm(dto.Height),
            Power.FromWatts(dto.TDP),
            dto.Sockets.Select(ToDomainCpuSocket).ToArray()
        )
        {
            IsDraft = dto.IsDraft,
            PublishedAt = dto.PublishedAt
        };
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

    private static ApiDimensions ToApiDimensions(Dimensions dimensions)
    {
        return new ApiDimensions
        {
            Length = dimensions.Length,
            Width = dimensions.Width,
            Height = dimensions.Height
        };
    }

    private static Dimensions ToDomainDimensions(ApiDimensions dimensions)
    {
        return new Dimensions(dimensions.Length, dimensions.Width, dimensions.Height);
    }

    private static ApiSlot ToApiSlot(Slot slot)
    {
        return new ApiSlot
        {
            Name = slot.Name,
            AllowedCategory = slot.AllowedProductCategory.ToString(),
            Location = new ApiVector3
            {
                X = slot.RelativePosition.X,
                Y = slot.RelativePosition.Y,
                Z = slot.RelativePosition.Z
            }
        };
    }

    private static Slot ToDomainSlot(ApiSlot slot)
    {
        ProductCategory category = Enum.Parse<ProductCategory>(slot.AllowedCategory, ignoreCase: true);
        Vector3 position = slot.Location != null
            ? new Vector3(slot.Location.X, slot.Location.Y, slot.Location.Z)
            : Vector3.Zero;

        return new Slot(
            Guid.NewGuid(),
            slot.Name,
            category,
            position,
            new Dimensions(100, 100, 50), // Default dimensions
            null
        );
    }

    private static ApiChamber ToApiChamber(Chamber chamber)
    {
        return new ApiChamber
        {
            Name = chamber.Name,
            Dimensions = ToApiDimensions(chamber.Dimensions)
        };
    }

    private static Chamber ToDomainChamber(ApiChamber chamber)
    {
        return new Chamber(
            Guid.NewGuid(),
            chamber.Name,
            ToDomainDimensions(chamber.Dimensions),
            []
        );
    }
}
