using System.Text.Json;
using MyPcBuild.ApiService.Catalog.DTOs;
using MyPcBuild.ApiService.Catalog.Models;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.Unit.Catalog.DTOs;

public class ProductResponseSerializationTests
{
    private readonly JsonSerializerOptions _options = TestJsonOptions.CreateOptions();

    [Fact]
    public void Serialize_CpuProductResponse_IncludesAllCpuFields()
    {
        CpuProductResponse cpuResponse = new()
        {
            Id = Guid.NewGuid(),
            Category = ProductCategory.CPU,
            Name = "Intel Core i9-13900K",
            Price = 589m,
            Manufacturer = "Intel",
            IsDraft = false,
            PublishedAt = DateTime.UtcNow,
            Socket = ApiCpuSocket.LGA1700,
            Cores = 24,
            Threads = 32,
            BaseClock = ApiFrequency.FromGHz(3.0m),
            BoostClock = ApiFrequency.FromGHz(5.8m),
            TDP = ApiPower.FromWatts(253),
            IntegratedGraphics = true,
        };

        string json = JsonSerializer.Serialize<ProductResponse>(cpuResponse, _options);

        Assert.Contains("Intel Core i9-13900K", json);
        Assert.Contains("3", json);
        Assert.Contains("5.8", json);
        Assert.Contains("24", json);
        Assert.Contains("32", json);
    }

    [Fact]
    public void Serialize_GpuProductResponse_IncludesAllGpuFields()
    {
        GpuProductResponse gpuResponse = new()
        {
            Id = Guid.NewGuid(),
            Category = ProductCategory.GPU,
            Name = "RTX 4090",
            Price = 1599m,
            Manufacturer = "NVIDIA",
            IsDraft = false,
            PublishedAt = DateTime.UtcNow,
            ChipsetManufacturer = ApiGpuChipsetManufacturer.NVIDIA,
            Series = "RTX 40",
            VRAM = ApiStorageCapacity.FromGB(24),
            MemoryType = ApiMemoryType.GDDR6X,
            CoreClock = ApiFrequency.FromGHz(2.235m),
            BoostClock = ApiFrequency.FromGHz(2.52m),
            TDP = ApiPower.FromWatts(450),
            PowerConnectors = ApiGpuPowerConnector.One16Pin,
            RayTracing = true,
            Dimensions = new DimensionsModel { Length = 336m, Width = 137m, Height = 58m },
        };

        string json = JsonSerializer.Serialize<ProductResponse>(gpuResponse, _options);

        Assert.Contains("RTX 4090", json);
        Assert.Contains("NVIDIA", json);
        Assert.Contains("24", json);
        Assert.Contains("GDDR6X", json);
    }

    [Fact]
    public void Serialize_MotherboardProductResponse_IncludesAllMotherboardFields()
    {
        MotherboardProductResponse mbResponse = new()
        {
            Id = Guid.NewGuid(),
            Category = ProductCategory.Motherboard,
            Name = "ROG STRIX Z790-E",
            Price = 399m,
            Manufacturer = "ASUS",
            IsDraft = false,
            PublishedAt = DateTime.UtcNow,
            Socket = ApiCpuSocket.LGA1700,
            Chipset = "Z790",
            FormFactor = ApiFormFactor.ATX,
            MemoryType = ApiMemoryType.DDR5,
            MaxMemory = ApiStorageCapacity.FromGB(192),
            Dimensions = new DimensionsModel { Length = 305m, Width = 244m, Height = 53m },
        };

        string json = JsonSerializer.Serialize<ProductResponse>(mbResponse, _options);

        Assert.Contains("ROG STRIX Z790-E", json);
        Assert.Contains("LGA1700", json);
        Assert.Contains("192", json);
    }

    [Fact]
    public void Serialize_RamProductResponse_IncludesAllRamFields()
    {
        RamProductResponse ramResponse = new()
        {
            Id = Guid.NewGuid(),
            Category = ProductCategory.RAM,
            Name = "Corsair Vengeance RGB",
            Price = 149m,
            Manufacturer = "Corsair",
            IsDraft = false,
            PublishedAt = DateTime.UtcNow,
            Type = ApiMemoryType.DDR5,
            Capacity = ApiStorageCapacity.FromGB(32),
            Configuration = ApiRamConfiguration.From(2, 16),
            Speed = ApiFrequency.FromGHz(5.6m),
            CASLatency = ApiCasLatency.FromInt(36),
            Voltage = ApiVoltage.FromVolts(1.1m),
        };

        string json = JsonSerializer.Serialize<ProductResponse>(ramResponse, _options);

        Assert.Contains("Corsair Vengeance RGB", json);
        Assert.Contains("DDR5", json);
        Assert.Contains("32", json);
    }

    [Fact]
    public void Serialize_PcCaseProductResponse_IncludesAllCaseFields()
    {
        PcCaseProductResponse caseResponse = new()
        {
            Id = Guid.NewGuid(),
            Category = ProductCategory.Case,
            Name = "NZXT H710",
            Price = 199m,
            Manufacturer = "NZXT",
            IsDraft = false,
            PublishedAt = DateTime.UtcNow,
            FormFactor = ApiFormFactor.ATX,
            Color = "Black",
            SidePanelWindow = ApiSidePanelType.TemperedGlass,
            Dimensions = new DimensionsModel { Length = 494m, Width = 230m, Height = 516m },
        };

        string json = JsonSerializer.Serialize<ProductResponse>(caseResponse, _options);

        Assert.Contains("NZXT H710", json);
        Assert.Contains("ATX", json);
        Assert.Contains("Black", json);
    }

    [Fact]
    public void Serialize_PsuProductResponse_IncludesAllPsuFields()
    {
        PsuProductResponse psuResponse = new()
        {
            Id = Guid.NewGuid(),
            Category = ProductCategory.PowerSupply,
            Name = "Corsair RM850e",
            Price = 129m,
            Manufacturer = "Corsair",
            IsDraft = false,
            PublishedAt = DateTime.UtcNow,
            Wattage = ApiPower.FromWatts(850),
            Efficiency = ApiPsuEfficiency.Gold,
            Modular = ApiPsuModularity.FullyModular,
            FormFactor = ApiPsuFormFactor.ATX,
            Length = ApiLength.FromMm(160),
            PCIe8Pin = 2,
        };

        string json = JsonSerializer.Serialize<ProductResponse>(psuResponse, _options);

        Assert.Contains("Corsair RM850e", json);
        Assert.Contains("850", json);
        Assert.Contains("Gold", json);
    }

    [Fact]
    public void Serialize_StorageProductResponse_IncludesAllStorageFields()
    {
        StorageProductResponse storageResponse = new()
        {
            Id = Guid.NewGuid(),
            Category = ProductCategory.Storage,
            Name = "Samsung 990 Pro",
            Price = 99m,
            Manufacturer = "Samsung",
            IsDraft = false,
            PublishedAt = DateTime.UtcNow,
            Type = ApiStorageType.SSD,
            Interface = ApiStorageInterface.NVMe,
            StorageFormFactor = ApiStorageFormFactor.M2_2280,
            Capacity = ApiStorageCapacity.FromGB(1000),
            ReadSpeed = ApiDataSpeed.FromMBps(7450),
            WriteSpeed = ApiDataSpeed.FromMBps(6900),
        };

        string json = JsonSerializer.Serialize<ProductResponse>(storageResponse, _options);

        Assert.Contains("Samsung 990 Pro", json);
        Assert.Contains("SSD", json);
        Assert.Contains("1000", json);
        Assert.Contains("NVMe", json);
    }

    [Fact]
    public void Serialize_CoolerProductResponse_IncludesAllCoolerFields()
    {
        CoolerProductResponse coolerResponse = new()
        {
            Id = Guid.NewGuid(),
            Category = ProductCategory.Cooler,
            Name = "Noctua NH-D15",
            Price = 99m,
            Manufacturer = "Noctua",
            IsDraft = false,
            PublishedAt = DateTime.UtcNow,
            CoolerType = ApiCoolerType.Air,
            Height = ApiLength.FromMm(165),
            TDP = ApiPower.FromWatts(220),
            Sockets = [ApiCpuSocket.LGA1700, ApiCpuSocket.AM5],
            Dimensions = new DimensionsModel { Length = 165m, Width = 150m, Height = 161m },
        };

        string json = JsonSerializer.Serialize<ProductResponse>(coolerResponse, _options);

        Assert.Contains("Noctua NH-D15", json);
        Assert.Contains("Air", json);
        Assert.Contains("220", json);
    }

    [Fact]
    public void RoundTrip_CpuProductResponse_SerializeDeserialize_ReturnsCorrectType()
    {
        CpuProductResponse original = new()
        {
            Id = Guid.NewGuid(),
            Category = ProductCategory.CPU,
            Name = "Test CPU",
            Price = 299m,
            Manufacturer = "TestMfg",
            IsDraft = false,
            PublishedAt = DateTime.UtcNow,
            Socket = ApiCpuSocket.AM5,
            Cores = 8,
            Threads = 16,
            BaseClock = ApiFrequency.FromGHz(3.0m),
            BoostClock = ApiFrequency.FromGHz(4.5m),
            TDP = ApiPower.FromWatts(105),
            IntegratedGraphics = false,
        };

        string serialized = JsonSerializer.Serialize<ProductResponse>(original, _options);
        ProductResponse deserialized = JsonSerializer.Deserialize<ProductResponse>(serialized, _options);

        Assert.NotNull(deserialized);
        Assert.IsType<CpuProductResponse>(deserialized);
        Assert.Equal(original.Id, deserialized.Id);
        Assert.Equal(original.Name, deserialized.Name);
    }

    [Fact]
    public void RoundTrip_GpuProductResponse_SerializeDeserialize_ReturnsCorrectType()
    {
        GpuProductResponse original = new()
        {
            Id = Guid.NewGuid(),
            Category = ProductCategory.GPU,
            Name = "Test GPU",
            Price = 499m,
            Manufacturer = "TestMfg",
            IsDraft = false,
            PublishedAt = DateTime.UtcNow,
            ChipsetManufacturer = ApiGpuChipsetManufacturer.AMD,
            Series = "RX 7000",
            VRAM = ApiStorageCapacity.FromGB(12),
            MemoryType = ApiMemoryType.GDDR6,
            CoreClock = ApiFrequency.FromGHz(2.3m),
            BoostClock = ApiFrequency.FromGHz(2.6m),
            TDP = ApiPower.FromWatts(320),
            PowerConnectors = ApiGpuPowerConnector.Dual8Pin,
            RayTracing = true,
            Dimensions = new DimensionsModel { Length = 267m, Width = 120m, Height = 50m },
        };

        string serialized = JsonSerializer.Serialize<ProductResponse>(original, _options);
        ProductResponse deserialized = JsonSerializer.Deserialize<ProductResponse>(serialized, _options);

        Assert.NotNull(deserialized);
        Assert.IsType<GpuProductResponse>(deserialized);
        Assert.Equal(original.Id, deserialized.Id);
    }

    [Fact]
    public void Serialize_AllResponseTypes_IncludeCommonFields()
    {
        Guid productId = Guid.NewGuid();

        CpuProductResponse cpuResponse = new()
        {
            Id = productId,
            Category = ProductCategory.CPU,
            Name = "Test CPU",
            Price = 299m,
            Manufacturer = "Test",
            IsDraft = false,
            PublishedAt = DateTime.UtcNow,
            Socket = ApiCpuSocket.LGA1700,
            Cores = 8,
            Threads = 16,
            BaseClock = ApiFrequency.FromGHz(3.0m),
            BoostClock = ApiFrequency.FromGHz(4.5m),
            TDP = ApiPower.FromWatts(105),
            IntegratedGraphics = false,
        };

        string json = JsonSerializer.Serialize<ProductResponse>(cpuResponse, _options);

        Assert.Contains(productId.ToString(), json);
        Assert.Contains("Test CPU", json);
        Assert.Contains("Test", json);
        Assert.Contains("false", json);
    }
}
