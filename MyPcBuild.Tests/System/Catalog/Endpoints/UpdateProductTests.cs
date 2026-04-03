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
            baseClockGHz = 3.0,
            boostClockGHz = 4.0,
            cores = 8,
            threads = 16,
            socket = "LGA1700",
            tdpWatts = 105,
            specs = new { }
        };

        string createJson = JsonSerializer.Serialize(createRequest);
        var createResponse = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(createJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        string content = await createResponse.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);
        
        if (doc.RootElement.TryGetProperty("id", out var idElement))
        {
            string productId = idElement.GetString() ?? throw new InvalidOperationException("Product ID must not be null");

            // Update the product
            var updateRequest = new
            {
                category = "cpu",
                name = $"Updated CPU {Guid.NewGuid()}",
                manufacturer = "Intel",
                baseClockGHz = 3.2,
                boostClockGHz = 4.5,
                cores = 12,
                threads = 24,
                socket = "LGA1700",
                tdpWatts = 125,
                specs = new { }
            };

            string updateJson = JsonSerializer.Serialize(updateRequest);
            var updateResponse = await client.PutAsync(
                $"/api/catalog/products/{productId}",
                new StringContent(updateJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
            );

            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        }
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
            baseClockGHz = 3.0,
            boostClockGHz = 4.0,
            cores = 8,
            threads = 16,
            socket = "LGA1700",
            tdpWatts = 105,
            specs = new { }
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
            baseClockGHz = 3.0,
            boostClockGHz = 4.0,
            cores = 8,
            threads = 16,
            socket = "LGA1700",
            tdpWatts = 105,
            specs = new { }
        };

        string createJson = JsonSerializer.Serialize(createRequest);
        var createResponse = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(createJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        string createContent = await createResponse.Content.ReadAsStringAsync();
        using JsonDocument createDoc = JsonDocument.Parse(createContent);
        
        if (createDoc.RootElement.TryGetProperty("id", out var idElement))
        {
            string productId = idElement.GetString() ?? throw new InvalidOperationException("Product ID must not be null");

            // Try to change category to GPU
            var updateRequest = new
            {
                category = "gpu",  // Changed!
                name = "Now a GPU",
                manufacturer = "NVIDIA",
                vramGb = 12,
                vramType = "GDDR6",
                chipsetManufacturer = "NVIDIA",
                specs = new { }
            };

            string updateJson = JsonSerializer.Serialize(updateRequest);
            var updateResponse = await client.PutAsync(
                $"/api/catalog/products/{productId}",
                new StringContent(updateJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
            );

            Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
        }
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
            baseClockGHz = 3.0,
            boostClockGHz = 4.0,
            cores = 8,
            threads = 16,
            socket = "LGA1700",
            tdpWatts = 105,
            specs = new { }
        };

        string createJson = JsonSerializer.Serialize(createRequest);
        var createResponse = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(createJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        string createContent = await createResponse.Content.ReadAsStringAsync();
        using JsonDocument createDoc = JsonDocument.Parse(createContent);
        
        if (createDoc.RootElement.TryGetProperty("id", out var createIdElement))
        {
            string originalId = createIdElement.GetString();

            // Update it
            var updateRequest = new
            {
                category = "cpu",
                name = $"Updated CPU {Guid.NewGuid()}",
                manufacturer = "Intel",
                baseClockGHz = 3.5,
                boostClockGHz = 4.5,
                cores = 8,
                threads = 16,
                socket = "LGA1700",
                tdpWatts = 105,
                specs = new { }
            };

            string updateJson = JsonSerializer.Serialize(updateRequest);
            var updateResponse = await client.PutAsync(
                $"/api/catalog/products/{originalId}",
                new StringContent(updateJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
            );

            string updateContent = await updateResponse.Content.ReadAsStringAsync();
            using JsonDocument updateDoc = JsonDocument.Parse(updateContent);
            
            if (updateDoc.RootElement.TryGetProperty("id", out var updateIdElement))
            {
                string updatedId = updateIdElement.GetString();
                Assert.Equal(originalId, updatedId);
            }
        }
    }
}
