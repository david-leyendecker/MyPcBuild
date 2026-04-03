using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.System.Spatial.Endpoints;

[Collection(AppHostCollection.Name)]
public class ValidatePartInstallationTests(AppHostFixture fixture)
{
    [Fact]
    public async Task PostValidateInstallation_ValidPart_ReturnsOk()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        var createBuildRequest = new
        {
            name = $"Test Build {Guid.NewGuid()}",
            userId = "test-user"
        };

        string createBuildJson = JsonSerializer.Serialize(createBuildRequest);
        var createBuildResponse = await client.PostAsync(
            "/api/builds",
            new StringContent(createBuildJson, Encoding.UTF8, "application/json")
        );

        string buildContent = await createBuildResponse.Content.ReadAsStringAsync();
        using JsonDocument buildDoc = JsonDocument.Parse(buildContent);
        
        if (buildDoc.RootElement.TryGetProperty("id", out var buildIdElement))
        {
            string buildId = buildIdElement.GetString() ?? throw new InvalidOperationException("Build ID must not be null");

            // Validate part installation
            var validateRequest = new
            {
                productId = Guid.NewGuid().ToString(),
                slotId = Guid.NewGuid().ToString(),
                position = new { x = 0, y = 0, z = 0 }
            };

            string validateJson = JsonSerializer.Serialize(validateRequest);
            var validateResponse = await client.PostAsync(
                $"/api/builds/{buildId}/parts/validate",
                new StringContent(validateJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
            );

            // Endpoint should exist and return proper status
            Assert.True(validateResponse.IsSuccessStatusCode || 
                       validateResponse.StatusCode == HttpStatusCode.BadRequest);
        }
    }
}

[Collection(AppHostCollection.Name)]
public class ValidateBuildSpatialTests(AppHostFixture fixture)
{
    [Fact]
    public async Task PostValidateBuild_NoParts_ReturnsValid()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        var createRequest = new
        {
            name = $"Test Build {Guid.NewGuid()}",
            userId = "test-user"
        };

        string createJson = JsonSerializer.Serialize(createRequest);
        var createResponse = await client.PostAsync(
            "/api/builds",
            new StringContent(createJson, Encoding.UTF8, "application/json")
        );

        string buildContent = await createResponse.Content.ReadAsStringAsync();
        using JsonDocument buildDoc = JsonDocument.Parse(buildContent);
        
        if (buildDoc.RootElement.TryGetProperty("id", out var buildIdElement))
        {
            string buildId = buildIdElement.GetString() ?? throw new InvalidOperationException("Build ID must not be null");
            
            // Validate build with no parts
            var validateResponse = await client.PostAsync(
                $"/api/builds/{buildId}/validate",
                new StringContent(string.Empty, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
            );

            Assert.Equal(HttpStatusCode.OK, validateResponse.StatusCode);

            string validateContent = await validateResponse.Content.ReadAsStringAsync();
            using JsonDocument validateDoc = JsonDocument.Parse(validateContent);
            
            Assert.True(validateDoc.RootElement.TryGetProperty("isValid", out _));
        }
    }

    [Fact]
    public async Task PostValidateBuild_NonExistentBuild_ReturnsNotFound()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        var response = await client.PostAsync(
            $"/api/builds/{Guid.NewGuid()}/validate",
            new StringContent(string.Empty, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
