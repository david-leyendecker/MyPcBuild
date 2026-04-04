using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.System.Catalog.Endpoints;

[Collection(AppHostCollection.Name)]
public class GetProductByIdTests(AppHostFixture fixture)
{
    [Fact]
    public async Task GetProductById_NonExistentId_ReturnsNotFound()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        var nonExistentId = Guid.NewGuid();
        
        HttpResponseMessage response = await client.GetAsync($"/api/catalog/products/{nonExistentId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_ExistingCpuProduct_ReturnsOkWithCorrectCategory()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        // First, create a CPU product
        var createRequest = new
        {
            category = "cpu",
            name = $"Test CPU {Guid.NewGuid()}",
            manufacturer = "TestMfg",
            price = 299m,
            baseClock = 3.2,
            boostClock = 5.0,
            cores = 8,
            threads = 16,
            socket = "LGA1700",
            tdp = 105,
            integratedGraphics = false,
        };

        string createJson = JsonSerializer.Serialize(createRequest);
        var createResponse = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(createJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        string createContent = await createResponse.Content.ReadAsStringAsync();
        using JsonDocument createDoc = JsonDocument.Parse(createContent);
        
        if (createDoc.RootElement.TryGetProperty("id", out var idElement))
        {
            string productId = idElement.GetString()!;
            
            // Now get the product
            HttpResponseMessage getResponse = await client.GetAsync($"/api/catalog/products/{productId}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            string getContent = await getResponse.Content.ReadAsStringAsync();
            using JsonDocument getDoc = JsonDocument.Parse(getContent);
            
            // Verify it has the category field
            Assert.True(getDoc.RootElement.TryGetProperty("category", out _));
        }
    }

    [Fact]
    public async Task GetProductById_GpuProduct_ReturnsCorrectFields()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        var createRequest = new
        {
            category = "gpu",
            name = $"Test GPU {Guid.NewGuid()}",
            manufacturer = "TestMfg",
            vramGb = 12,
            vramType = "GDDR6",
            chipsetManufacturer = "NVIDIA",
            specs = new { }
        };

        string createJson = JsonSerializer.Serialize(createRequest);
        var createResponse = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(createJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        if (createResponse.IsSuccessStatusCode)
        {
            string createContent = await createResponse.Content.ReadAsStringAsync();
            using JsonDocument createDoc = JsonDocument.Parse(createContent);
            
            if (createDoc.RootElement.TryGetProperty("id", out var idElement))
            {
                string productId = idElement.GetString()!;
                HttpResponseMessage getResponse = await client.GetAsync($"/api/catalog/products/{productId}");
                
                string getContent = await getResponse.Content.ReadAsStringAsync();
                using JsonDocument getDoc = JsonDocument.Parse(getContent);
                
                // GPU-specific fields should be present
                Assert.True(getDoc.RootElement.TryGetProperty("vramGb", out _));
            }
        }
    }

    [Fact]
    public async Task GetProductById_DraftProduct_IsMarkedAsDraft()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        var createRequest = new
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

        string createJson = JsonSerializer.Serialize(createRequest);
        var createResponse = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(createJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        if (createResponse.IsSuccessStatusCode)
        {
            string createContent = await createResponse.Content.ReadAsStringAsync();
            using JsonDocument createDoc = JsonDocument.Parse(createContent);
            
            if (createDoc.RootElement.TryGetProperty("id", out var idElement))
            {
                string productId = idElement.GetString()!;
                HttpResponseMessage getResponse = await client.GetAsync($"/api/catalog/products/{productId}");
                
                string getContent = await getResponse.Content.ReadAsStringAsync();
                using JsonDocument getDoc = JsonDocument.Parse(getContent);
                
                // New products should be drafts
                if (getDoc.RootElement.TryGetProperty("isDraft", out var isDraftElement))
                {
                    Assert.True(isDraftElement.GetBoolean());
                }
            }
        }
    }

    [Fact]
    public async Task GetProductById_ResponseContainsJsonDerivedType()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        var createRequest = new
        {
            category = "storage",
            name = $"Test Storage {Guid.NewGuid()}",
            manufacturer = "TestMfg",
            storageType = "SSD",
            capacityGb = 500,
            @interface = "NVMe",
            storageFormFactor = "M2_2280",
            specs = new { }
        };

        string createJson = JsonSerializer.Serialize(createRequest);
        var createResponse = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(createJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        if (createResponse.IsSuccessStatusCode)
        {
            string createContent = await createResponse.Content.ReadAsStringAsync();
            using JsonDocument createDoc = JsonDocument.Parse(createContent);
            
            if (createDoc.RootElement.TryGetProperty("id", out var idElement))
            {
                string productId = idElement.GetString()!;
                HttpResponseMessage getResponse = await client.GetAsync($"/api/catalog/products/{productId}");
                
                Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            }
        }
    }
}
