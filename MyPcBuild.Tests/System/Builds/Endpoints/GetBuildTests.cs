using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.System.Builds.Endpoints;

[Collection(AppHostCollection.Name)]
public class GetBuildTests(AppHostFixture fixture)
{
    [Fact]
    public async Task GetBuild_NonExistentBuild_ReturnsNotFound()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync($"/api/builds/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetBuild_ExistingBuild_ReturnsOk()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        // Create a build
        var createRequest = new
        {
            name = $"Test Build {Guid.NewGuid()}",
            userId = "test-user"
        };

        string createJson = JsonSerializer.Serialize(createRequest);
        var createResponse = await client.PostAsync(
            "/api/builds",
            new StringContent(createJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        string createContent = await createResponse.Content.ReadAsStringAsync();
        using JsonDocument createDoc = JsonDocument.Parse(createContent);
        
        if (createDoc.RootElement.TryGetProperty("id", out var idElement))
        {
            string buildId = idElement.GetString()!;
            HttpResponseMessage getResponse = await client.GetAsync($"/api/builds/{buildId}");

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        }
    }

    [Fact]
    public async Task GetBuild_ResponseContainsParts()
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
            new StringContent(createJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        string createContent = await createResponse.Content.ReadAsStringAsync();
        using JsonDocument createDoc = JsonDocument.Parse(createContent);
        
        if (createDoc.RootElement.TryGetProperty("id", out var idElement))
        {
            string buildId = idElement.GetString()!;
            HttpResponseMessage getResponse = await client.GetAsync($"/api/builds/{buildId}");

            string getContent = await getResponse.Content.ReadAsStringAsync();
            using JsonDocument getDoc = JsonDocument.Parse(getContent);
            
            Assert.True(getDoc.RootElement.TryGetProperty("parts", out _));
        }
    }

    [Fact]
    public async Task GetBuild_ResponseContainsCompatibilityInfo()
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
            new StringContent(createJson, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        string createContent = await createResponse.Content.ReadAsStringAsync();
        using JsonDocument createDoc = JsonDocument.Parse(createContent);
        
        if (createDoc.RootElement.TryGetProperty("id", out var idElement))
        {
            string buildId = idElement.GetString()!;
            HttpResponseMessage getResponse = await client.GetAsync($"/api/builds/{buildId}");

            string getContent = await getResponse.Content.ReadAsStringAsync();
            using JsonDocument getDoc = JsonDocument.Parse(getContent);
            
            Assert.True(getDoc.RootElement.TryGetProperty("isCompatible", out _));
        }
    }
}
