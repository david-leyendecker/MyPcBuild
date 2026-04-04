using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.System.Catalog.Endpoints;

[Collection(AppHostCollection.Name)]
public class CreateProductTests(AppHostFixture fixture)
{
    [Fact]
    public async Task PostProduct_ValidCpu_Returns201WithId()
    {
        HttpClient client = fixture.CreateApiServiceClient();

        var request = new
        {
            category = "cpu",
            name = $"Intel Core i9 {Guid.NewGuid()}",
            manufacturer = "Intel",
            price = 589m,
            baseClock = 3.2,
            boostClock = 5.0,
            cores = 24,
            threads = 32,
            socket = "LGA1700",
            tdp = 253,
            integratedGraphics = false,
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);

        Assert.True(doc.RootElement.TryGetProperty("id", out var idElement));
        Assert.NotEmpty(idElement.GetString() ?? string.Empty);
    }

    [Fact]
    public async Task PostProduct_ValidGpu_Returns201()
    {
        HttpClient client = fixture.CreateApiServiceClient();

        var request = new
        {
            category = "gpu",
            name = $"RTX 4090 {Guid.NewGuid()}",
            manufacturer = "NVIDIA",
            price = 799m,
            chipsetManufacturer = "NVIDIA",
            series = "RTX 4000",
            vram = 24,
            memoryType = "GDDR6X",
            coreClock = 2.23,
            boostClock = 2.52,
            tdp = 450,
            powerConnectors = "Triple8Pin",
            rayTracing = true,
            dimensions = new { length = 336, width = 140, height = 61 },
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_ValidMotherboard_Returns201()
    {
        HttpClient client = fixture.CreateApiServiceClient();

        var request = new
        {
            category = "motherboard",
            name = $"ROG STRIX {Guid.NewGuid()}",
            manufacturer = "ASUS",
            price = 449m,
            socket = "LGA1700",
            chipset = "Z790",
            formFactor = "ATX",
            memoryType = "DDR5",
            maxMemory = 192,
            dimensions = new { length = 305, width = 244, height = 60 },
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_ValidRam_Returns201()
    {
        HttpClient client = fixture.CreateApiServiceClient();

        var request = new
        {
            category = "ram",
            name = $"Corsair {Guid.NewGuid()}",
            manufacturer = "Corsair",
            price = 129m,
            type = "DDR5",
            capacity = 32,
            configuration = "2x16GB",
            speed = 5.6,
            casLatency = "CL36",
            voltage = 1.35,
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_ValidPcCase_Returns201()
    {
        HttpClient client = fixture.CreateApiServiceClient();

        var request = new
        {
            category = "case",
            name = $"NZXT H710 {Guid.NewGuid()}",
            manufacturer = "NZXT",
            price = 169m,
            formFactor = "ATX",
            color = "Black",
            sidePanelWindow = "TemperedGlass",
            dimensions = new { length = 494, width = 230, height = 516 },
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_ValidPsu_Returns201()
    {
        HttpClient client = fixture.CreateApiServiceClient();

        var request = new
        {
            category = "powersupply",
            name = $"Corsair RM850e {Guid.NewGuid()}",
            manufacturer = "Corsair",
            price = 129m,
            wattage = 850,
            efficiency = "Gold",
            modular = "FullyModular",
            formFactor = "ATX",
            length = 160,
            pcIe8Pin = 4,
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_ValidStorage_Returns201()
    {
        HttpClient client = fixture.CreateApiServiceClient();

        var request = new
        {
            category = "storage",
            name = $"Samsung 990 Pro {Guid.NewGuid()}",
            manufacturer = "Samsung",
            price = 109m,
            type = "SSD",
            @interface = "NVMe",
            storageFormFactor = "M2_2280",
            capacity = 1000,
            readSpeed = 7450,
            writeSpeed = 6900,
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_ValidCooler_Returns201()
    {
        HttpClient client = fixture.CreateApiServiceClient();

        var request = new
        {
            category = "cooler",
            name = $"Noctua NH-D15 {Guid.NewGuid()}",
            manufacturer = "Noctua",
            price = 99m,
            coolerType = "Air",
            height = 165,
            tdp = 220,
            sockets = new[] { "LGA1700", "AM5" },
            dimensions = new { length = 150, width = 166, height = 155 },
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_InvalidCategory_ReturnsBadRequest()
    {
        HttpClient client = fixture.CreateApiServiceClient();

        var request = new
        {
            category = "invalid_category",
            name = "Test Product",
            manufacturer = "Test"
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_MissingRequiredFields_ReturnsBadRequest()
    {
        HttpClient client = fixture.CreateApiServiceClient();

        var request = new
        {
            category = "cpu",
            // Missing name, manufacturer, etc.
            specs = new { }
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_StrictEnumFormats_OldPsuEfficiencyAlias_ReturnsBadRequest()
    {
        HttpClient client = fixture.CreateApiServiceClient();

        var request = new
        {
            category = "powersupply",
            name = $"Test PSU {Guid.NewGuid()}",
            manufacturer = "TestMfg",
            wattage = 750,
            efficiency = "80+Gold",  // Old alias format, no longer accepted
            modular = "SemiModular",
            formFactor = "ATX",
            specs = new { }
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_StrictEnumFormats_OldPsuFormFactorAlias_ReturnsBadRequest()
    {
        HttpClient client = fixture.CreateApiServiceClient();

        var request = new
        {
            category = "powersupply",
            name = $"Test PSU {Guid.NewGuid()}",
            manufacturer = "TestMfg",
            wattage = 600,
            efficiency = "Bronze",
            modular = "NonModular",
            formFactor = "SFX-L",  // Old alias, no longer accepted — use "SFXL"
            specs = new { }
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_CreatedProductIsDraft()
    {
        HttpClient client = fixture.CreateApiServiceClient();

        var request = new
        {
            category = "cpu",
            name = $"Draft CPU {Guid.NewGuid()}",
            manufacturer = "TestMfg",
            price = 299m,
            baseClock = 3.0,
            boostClock = 4.0,
            cores = 4,
            threads = 8,
            socket = "LGA1700",
            tdp = 65,
            integratedGraphics = false,
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);

        if (doc.RootElement.TryGetProperty("isDraft", out var isDraftElement))
        {
            Assert.True(isDraftElement.GetBoolean());
        }
    }

    [Fact]
    public async Task PostProduct_ResponseContainsLinks()
    {
        HttpClient client = fixture.CreateApiServiceClient();

        var request = new
        {
            category = "cpu",
            name = $"Test CPU {Guid.NewGuid()}",
            manufacturer = "TestMfg",
            price = 299m,
            baseClock = 3.0,
            boostClock = 4.0,
            cores = 4,
            threads = 8,
            socket = "LGA1700",
            tdp = 65,
            integratedGraphics = false,
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);

        Assert.True(doc.RootElement.TryGetProperty("links", out _));
    }
}
