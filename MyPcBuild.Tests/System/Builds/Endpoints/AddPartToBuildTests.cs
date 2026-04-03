using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.System.Builds.Endpoints;

[Collection(AppHostCollection.Name)]
public class AddPartToBuildTests(AppHostFixture fixture)
{
    [Fact]
    public async Task PostPart_ValidProduct_ReturnsOk()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        // First, create a published product
        var createProductRequest = new
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

        string createProductJson = JsonSerializer.Serialize(createProductRequest);
        var createProductResponse = await client.PostAsync(
            "/api/catalog/products",
            new StringContent(createProductJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        string productContent = await createProductResponse.Content.ReadAsStringAsync();
        using JsonDocument productDoc = JsonDocument.Parse(productContent);
        
        if (productDoc.RootElement.TryGetProperty("id", out var productIdElement))
        {
            string productId = productIdElement.GetString() ?? throw new InvalidOperationException("Product ID must not be null");

            // Publish the product
            await client.PostAsync(
                $"/api/catalog/products/{productId}/publish",
                new StringContent(string.Empty, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
            );

            // Create a build
            var createBuildRequest = new
            {
                name = $"Test Build {Guid.NewGuid()}",
                userId = "test-user"
            };

            string createBuildJson = JsonSerializer.Serialize(createBuildRequest);
            var createBuildResponse = await client.PostAsync(
                "/api/builds",
                new StringContent(createBuildJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
            );

            string buildContent = await createBuildResponse.Content.ReadAsStringAsync();
            using JsonDocument buildDoc = JsonDocument.Parse(buildContent);
            
            if (buildDoc.RootElement.TryGetProperty("id", out var buildIdElement))
            {
                string buildId = buildIdElement.GetString() ?? throw new InvalidOperationException("Build ID must not be null");

                // Add part to build
                var addPartRequest = new
                {
                    productId = productId,
                    pricePaid = 350.00m
                };

                string addPartJson = JsonSerializer.Serialize(addPartRequest);
                var addPartResponse = await client.PostAsync(
                    $"/api/builds/{buildId}/parts",
                    new StringContent(addPartJson, Encoding.UTF8, "application/json")
                );

                Assert.Equal(HttpStatusCode.OK, addPartResponse.StatusCode);
            }
        }
    }

    [Fact]
    public async Task PostPart_NonExistentProduct_ReturnsNotFound()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        // Create a build
        var createBuildRequest = new
        {
            name = $"Test Build {Guid.NewGuid()}",
            userId = "test-user"
        };

        string createBuildJson = JsonSerializer.Serialize(createBuildRequest);
        var createBuildResponse = await client.PostAsync(
            "/api/builds",
            new StringContent(createBuildJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        string buildContent = await createBuildResponse.Content.ReadAsStringAsync();
        using JsonDocument buildDoc = JsonDocument.Parse(buildContent);
        
        if (buildDoc.RootElement.TryGetProperty("id", out var buildIdElement))
        {
            string buildId = buildIdElement.GetString() ?? throw new InvalidOperationException("Build ID must not be null");

            // Try to add non-existent product
            var addPartRequest = new
            {
                productId = Guid.NewGuid().ToString(),
                pricePaid = 350.00m
            };

            string addPartJson = JsonSerializer.Serialize(addPartRequest);
            var addPartResponse = await client.PostAsync(
                $"/api/builds/{buildId}/parts",
                new StringContent(addPartJson, Encoding.UTF8, "application/json")
            );

            Assert.Equal(HttpStatusCode.NotFound, addPartResponse.StatusCode);
        }
    }
}
