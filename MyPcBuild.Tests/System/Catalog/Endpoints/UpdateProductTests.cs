using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.System.Catalog.Endpoints;

[Collection(AppHostCollection.Name)]
public class UpdateProductTests(AppHostFixture fixture)
{
    [Fact]
    public async Task PutProduct_ExistingProduct_ReturnsOk()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        // Create a product first
        var createRequest = new
        {
            category = "cpu",
            name = $"Test CPU {Guid.NewGuid()}",
            manufacturer = "Intel",
            price = 299m,
            baseClock = 3.0,
            boostClock = 4.0,
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

        string content = await createResponse.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);
        
        Assert.True(doc.RootElement.TryGetProperty("id", out var idElement), "Create response must contain 'id'");
        string productId = idElement.GetString() ?? throw new InvalidOperationException("Product ID must not be null");

        // Update the product
        var updateRequest = new
        {
            category = "cpu",
            name = $"Updated CPU {Guid.NewGuid()}",
            manufacturer = "Intel",
            price = 349m,
            baseClock = 3.2,
            boostClock = 4.5,
            cores = 12,
            threads = 24,
            socket = "LGA1700",
            tdp = 125,
            integratedGraphics = false,
        };

        string updateJson = JsonSerializer.Serialize(updateRequest);
        var updateResponse = await client.PutAsync(
            $"/api/catalog/products/{productId}",
            new StringContent(updateJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
    }

    [Fact]
    public async Task PutProduct_NonExistentProduct_ReturnsNotFound()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        var updateRequest = new
        {
            category = "cpu",
            name = "Test CPU",
            manufacturer = "Intel",
            price = 299m,
            baseClock = 3.0,
            boostClock = 4.0,
            cores = 8,
            threads = 16,
            socket = "LGA1700",
            tdp = 105,
            integratedGraphics = false,
        };

        string json = JsonSerializer.Serialize(updateRequest);
        var response = await client.PutAsync(
            $"/api/catalog/products/{Guid.NewGuid()}",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutProduct_ChangedCategory_ReturnsBadRequest()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        // Create a CPU product
        var createRequest = new
        {
            category = "cpu",
            name = $"Test CPU {Guid.NewGuid()}",
            manufacturer = "Intel",
            price = 299m,
            baseClock = 3.0,
            boostClock = 4.0,
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
        
        Assert.True(createDoc.RootElement.TryGetProperty("id", out var idElement), "Create response must contain 'id'");
        string productId = idElement.GetString() ?? throw new InvalidOperationException("Product ID must not be null");

        // Try to change category to GPU
        var updateRequest = new
        {
            category = "gpu",  // Changed!
            name = "Now a GPU",
            manufacturer = "NVIDIA",
            price = 1599m,
            vramGb = 12,
            vramType = "GDDR6",
            chipsetManufacturer = "NVIDIA",
        };

        string updateJson = JsonSerializer.Serialize(updateRequest);
        var updateResponse = await client.PutAsync(
            $"/api/catalog/products/{productId}",
            new StringContent(updateJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }

    [Fact]
    public async Task PutProduct_PreservesId()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        // Create a product
        var createRequest = new
        {
            category = "cpu",
            name = $"Test CPU {Guid.NewGuid()}",
            manufacturer = "Intel",
            price = 299m,
            baseClock = 3.0,
            boostClock = 4.0,
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
        
        Assert.True(createDoc.RootElement.TryGetProperty("id", out var createIdElement), "Create response must contain 'id'");
        string originalId = createIdElement.GetString() ?? throw new InvalidOperationException("Product ID must not be null");

        // Update it
        var updateRequest = new
        {
            category = "cpu",
            name = $"Updated CPU {Guid.NewGuid()}",
            manufacturer = "Intel",
            price = 349m,
            baseClock = 3.5,
            boostClock = 4.5,
            cores = 8,
            threads = 16,
            socket = "LGA1700",
            tdp = 105,
            integratedGraphics = false,
        };

        string updateJson = JsonSerializer.Serialize(updateRequest);
        var updateResponse = await client.PutAsync(
            $"/api/catalog/products/{originalId}",
            new StringContent(updateJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        string updateContent = await updateResponse.Content.ReadAsStringAsync();
        using JsonDocument updateDoc = JsonDocument.Parse(updateContent);
        
        Assert.True(updateDoc.RootElement.TryGetProperty("id", out var updateIdElement), "Update response must contain 'id'");
        string updatedId = updateIdElement.GetString() ?? throw new InvalidOperationException("Updated product ID must not be null");
        Assert.Equal(originalId, updatedId);
    }
}
