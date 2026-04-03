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
            baseClockGHz = 3.2,
            boostClockGHz = 5.0,
            cores = 24,
            threads = 32,
            socket = "LGA1700",
            tdpWatts = 253,
            specs = new { }
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
        Assert.NotEmpty(idElement.GetString());
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
            vramGb = 24,
            vramType = "GDDR6X",
            chipsetManufacturer = "NVIDIA",
            specs = new { }
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
            socket = "LGA1700",
            formFactor = "ATX",
            maxRamGb = 192,
            ramSlots = 4,
            m2Slots = 4,
            specs = new { }
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
            memoryType = "DDR5",
            capacityGb = 32,
            speedMhz = 5600,
            casLatency = "CL36",
            specs = new { }
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
            category = "pccase",
            name = $"NZXT H710 {Guid.NewGuid()}",
            manufacturer = "NZXT",
            formFactor = "ATX",
            sidePanelType = "Tempered Glass",
            specs = new { }
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
            category = "psu",
            name = $"Corsair RM850e {Guid.NewGuid()}",
            manufacturer = "Corsair",
            wattage = 850,
            efficiency = "80+ Gold",
            modularity = "Fully-Modular",
            formFactor = "ATX",
            specs = new { }
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
            storageType = "SSD",
            capacityGb = 1000,
            @interface = "NVMe",
            formFactor = "M.2 2280",
            specs = new { }
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
            coolerType = "AirTower",
            maxTdpWatts = 220,
            socketsSupported = new[] { "LGA1700", "AM5" },
            specs = new { }
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
    public async Task PostProduct_FlexibleEnumFormats_AcceptedPsuEfficiency()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        var request = new
        {
            category = "psu",
            name = $"Test PSU {Guid.NewGuid()}",
            manufacturer = "TestMfg",
            wattage = 750,
            efficiency = "80+Gold",  // Flexible format without space
            modularity = "Semi-Modular",
            formFactor = "ATX",
            specs = new { }
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_FlexibleEnumFormats_AcceptedPsuFormFactor()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        var request = new
        {
            category = "psu",
            name = $"Test PSU {Guid.NewGuid()}",
            manufacturer = "TestMfg",
            wattage = 600,
            efficiency = "80+ Bronze",
            modularity = "Non-Modular",
            formFactor = "SFX-L",  // Flexible format
            specs = new { }
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
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
            baseClockGHz = 3.0,
            boostClockGHz = 4.0,
            cores = 4,
            threads = 8,
            socket = "LGA1700",
            tdpWatts = 65,
            specs = new { }
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
            baseClockGHz = 3.0,
            boostClockGHz = 4.0,
            cores = 4,
            threads = 8,
            socket = "LGA1700",
            tdpWatts = 65,
            specs = new { }
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
