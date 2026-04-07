using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.System.Catalog.Endpoints;

[Collection(AppHostCollection.Name)]
public class PublishProductTests(AppHostFixture fixture)
{
    [Fact]
    public async Task PostPublish_DraftProduct_ReturnsOk()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        // Create a draft product
        var createRequest = new
        {
            category = "cpu",
            name = $"Draft CPU {Guid.NewGuid()}",
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

        string createContent = await createResponse.Content.ReadAsStringAsync();
        using JsonDocument createDoc = JsonDocument.Parse(createContent);
        
        if (createDoc.RootElement.TryGetProperty("id", out var idElement))
        {
            string productId = idElement.GetString()!;

            // Publish it
            var publishResponse = await client.PostAsync(
                $"/api/catalog/products/{productId}/publish",
                new StringContent(string.Empty, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
            );

            Assert.Equal(HttpStatusCode.OK, publishResponse.StatusCode);
        }
    }

    [Fact]
    public async Task PostPublish_SetsPublishedAtTimestamp()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        var createRequest = new
        {
            category = "cpu",
            name = $"Draft CPU {Guid.NewGuid()}",
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

        string createContent = await createResponse.Content.ReadAsStringAsync();
        using JsonDocument createDoc = JsonDocument.Parse(createContent);
        
        if (createDoc.RootElement.TryGetProperty("id", out var idElement))
        {
            string productId = idElement.GetString()!;

            var publishResponse = await client.PostAsync(
                $"/api/catalog/products/{productId}/publish",
                new StringContent(string.Empty, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
            );

            string publishContent = await publishResponse.Content.ReadAsStringAsync();
            using JsonDocument publishDoc = JsonDocument.Parse(publishContent);
            
            // Should have publishedAt set
            Assert.True(publishDoc.RootElement.TryGetProperty("publishedAt", out var publishedAtElement));
            Assert.NotEqual(default, publishedAtElement.GetDateTime());
        }
    }

    [Fact]
    public async Task PostPublish_AlreadyPublished_ReturnsBadRequest()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        var createRequest = new
        {
            category = "cpu",
            name = $"Draft CPU {Guid.NewGuid()}",
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

        string createContent = await createResponse.Content.ReadAsStringAsync();
        using JsonDocument createDoc = JsonDocument.Parse(createContent);
        
        if (createDoc.RootElement.TryGetProperty("id", out var idElement))
        {
            string productId = idElement.GetString()!;

            // First publish
            await client.PostAsync(
                $"/api/catalog/products/{productId}/publish",
                new StringContent(string.Empty, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
            );

            // Try to publish again
            var publishAgainResponse = await client.PostAsync(
                $"/api/catalog/products/{productId}/publish",
                new StringContent(string.Empty, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
            );

            Assert.Equal(HttpStatusCode.BadRequest, publishAgainResponse.StatusCode);
        }
    }

    [Fact]
    public async Task PostPublish_NonExistentProduct_ReturnsNotFound()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        var response = await client.PostAsync(
            $"/api/catalog/products/{Guid.NewGuid()}/publish",
            new StringContent(string.Empty, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
