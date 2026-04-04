using System.Text.Json;
using MyPcBuild.ApiService.Catalog.DTOs;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.Unit.Catalog.DTOs;

public class ProductRequestJsonConverterTests
{
    private readonly JsonSerializerOptions _options = TestJsonOptions.CreateOptions();

    [Fact]
    public void Deserialize_ProductRequest_WithCpuCategory_ReturnsCpuProductRequest()
    {
        string json = """
            {
                "category": "cpu",
                "name": "Intel Core i9",
                "price": 589,
                "manufacturer": "Intel",
                "socket": "LGA1700",
                "cores": 24,
                "threads": 32,
                "baseClock": 2.0,
                "boostClock": 5.4,
                "tdp": 253,
                "integratedGraphics": false
            }
            """;

        ProductRequest result = JsonSerializer.Deserialize<ProductRequest>(json, _options)!;
        Assert.IsType<CpuProductRequest>(result);

        CpuProductRequest cpuRequest = Assert.IsType<CpuProductRequest>(result);
        Assert.Equal("Intel Core i9", cpuRequest.Name);
        Assert.Equal("cpu", cpuRequest.Category.ToString().ToLower());
    }

    [Fact]
    public void Deserialize_ProductRequest_WithGpuCategory_ReturnsGpuProductRequest()
    {
        string json = """
            {
                "category": "gpu",
                "name": "RTX 4090",
                "price": 1599,
                "manufacturer": "NVIDIA",
                "chipsetManufacturer": "NVIDIA",
                "series": "RTX 40",
                "vram": 24,
                "memoryType": "GDDR6X",
                "coreClock": 2.235,
                "boostClock": 2.52,
                "tdp": 450,
                "powerConnectors": "One16Pin",
                "rayTracing": true,
                "dimensions": { "length": 336, "width": 137, "height": 58 }
            }
            """;

        ProductRequest result = JsonSerializer.Deserialize<ProductRequest>(json, _options)!;
        Assert.IsType<GpuProductRequest>(result);
    }

    [Fact]
    public void Deserialize_ProductRequest_WithMotherboardCategory_ReturnsMotherboardProductRequest()
    {
        string json = """
            {
                "category": "motherboard",
                "name": "ROG STRIX Z790-E",
                "price": 399,
                "manufacturer": "ASUS",
                "socket": "LGA1700",
                "chipset": "Z790",
                "formFactor": "ATX",
                "memoryType": "DDR5",
                "maxMemory": 192,
                "dimensions": { "length": 305, "width": 244, "height": 53 }
            }
            """;

        ProductRequest result = JsonSerializer.Deserialize<ProductRequest>(json, _options)!;
        Assert.IsType<MotherboardProductRequest>(result);
    }

    [Fact]
    public void Deserialize_ProductRequest_WithRamCategory_ReturnsRamProductRequest()
    {
        string json = """
            {
                "category": "ram",
                "name": "Corsair Vengeance RGB",
                "price": 149,
                "manufacturer": "Corsair",
                "type": "DDR5",
                "capacity": 32,
                "configuration": "2x16GB",
                "speed": 5600,
                "casLatency": 36,
                "voltage": 1.1
            }
            """;

        ProductRequest result = JsonSerializer.Deserialize<ProductRequest>(json, _options)!;
        Assert.IsType<RamProductRequest>(result);
    }

    [Fact]
    public void Deserialize_ProductRequest_WithPcCaseCategory_ReturnsPcCaseProductRequest()
    {
        string json = """
            {
                "category": "case",
                "name": "NZXT H710",
                "price": 199,
                "manufacturer": "NZXT",
                "formFactor": "ATX",
                "color": "Black",
                "sidePanelWindow": "TemperedGlass",
                "dimensions": { "length": 494, "width": 230, "height": 516 }
            }
            """;

        ProductRequest result = JsonSerializer.Deserialize<ProductRequest>(json, _options)!;
        Assert.IsType<PcCaseProductRequest>(result);
    }

    [Fact]
    public void Deserialize_ProductRequest_WithPsuCategory_ReturnsPsuProductRequest()
    {
        string json = """
            {
                "category": "powersupply",
                "name": "Corsair RM850e",
                "price": 129,
                "manufacturer": "Corsair",
                "wattage": 850,
                "efficiency": "Gold",
                "modular": "FullyModular",
                "formFactor": "ATX",
                "length": 160,
                "pCIe8Pin": 2
            }
            """;

        ProductRequest result = JsonSerializer.Deserialize<ProductRequest>(json, _options)!;
        Assert.IsType<PsuProductRequest>(result);
    }

    [Fact]
    public void Deserialize_ProductRequest_WithStorageCategory_ReturnsStorageProductRequest()
    {
        string json = """
            {
                "category": "storage",
                "name": "Samsung 990 Pro",
                "price": 99,
                "manufacturer": "Samsung",
                "type": "SSD",
                "interface": "NVMe",
                "storageFormFactor": "M2_2280",
                "capacity": 1000,
                "readSpeed": 7450,
                "writeSpeed": 6900
            }
            """;

        ProductRequest result = JsonSerializer.Deserialize<ProductRequest>(json, _options)!;
        Assert.IsType<StorageProductRequest>(result);
    }

    [Fact]
    public void Deserialize_ProductRequest_WithCoolerCategory_ReturnsCoolerProductRequest()
    {
        string json = """
            {
                "category": "cooler",
                "name": "Noctua NH-D15",
                "price": 99,
                "manufacturer": "Noctua",
                "coolerType": "Air",
                "height": 165,
                "tdp": 250,
                "sockets": ["LGA1700", "AM5"],
                "dimensions": { "length": 165, "width": 150, "height": 161 }
            }
            """;

        ProductRequest result = JsonSerializer.Deserialize<ProductRequest>(json, _options)!;
        Assert.IsType<CoolerProductRequest>(result);
    }

    [Fact]
    public void Deserialize_ProductRequest_WithUppercaseCategory_StillDeserializes()
    {
        string json = """
            {
                "category": "CPU",
                "name": "Intel Core i9",
                "price": 589,
                "manufacturer": "Intel",
                "socket": "LGA1700",
                "cores": 24,
                "threads": 32,
                "baseClock": 2.0,
                "boostClock": 5.4,
                "tdp": 253,
                "integratedGraphics": false
            }
            """;

        ProductRequest result = JsonSerializer.Deserialize<ProductRequest>(json, _options)!;
        Assert.IsType<CpuProductRequest>(result);
    }

    [Fact]
    public void Deserialize_ProductRequest_WithUnknownCategory_ThrowsJsonException()
    {
        string json = """
            {
                "category": "unknown_category_xyz",
                "name": "Unknown Product",
                "manufacturer": "Unknown"
            }
            """;

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ProductRequest>(json, _options));
    }

    [Fact]
    public void Deserialize_ProductRequest_PreservesAllProperties()
    {
        string json = """
            {
                "category": "cpu",
                "name": "Test CPU",
                "price": 299,
                "manufacturer": "TestManufacturer",
                "socket": "LGA1700",
                "cores": 16,
                "threads": 32,
                "baseClock": 3.5,
                "boostClock": 4.8,
                "tdp": 125,
                "integratedGraphics": false
            }
            """;

        CpuProductRequest result = Assert.IsType<CpuProductRequest>(
            JsonSerializer.Deserialize<ProductRequest>(json, _options));

        Assert.Equal("Test CPU", result.Name);
        Assert.Equal("TestManufacturer", result.Manufacturer);
        Assert.Equal(3.5m, result.BaseClock.ValueInGHz);
        Assert.Equal(4.8m, result.BoostClock.ValueInGHz);
    }
}
