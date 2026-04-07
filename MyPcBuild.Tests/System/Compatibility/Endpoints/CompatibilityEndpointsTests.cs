using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.System.Compatibility.Endpoints;

[Collection(AppHostCollection.Name)]
public class ValidateCompatibilityTests(AppHostFixture fixture)
{
    [Fact]
    public async Task PostValidate_WithProductIds_ReturnsCompatibilityResult()
    {
        HttpClient client = fixture.CreateApiServiceClient();

        var request = new
        {
            productIds = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() }
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/compatibility/validate",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        // Non-existent products should still return a structured result
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);

        Assert.True(doc.RootElement.TryGetProperty("isCompatible", out _), "Response must contain 'isCompatible'");
        Assert.True(doc.RootElement.TryGetProperty("issues", out _), "Response must contain 'issues'");
    }

    [Fact]
    public async Task PostValidate_EmptyProductIds_ReturnsBadRequest()
    {
        HttpClient client = fixture.CreateApiServiceClient();

        var request = new
        {
            productIds = new string[] { }
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/compatibility/validate",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostValidate_ResponseSerializesIssues()
    {
        HttpClient client = fixture.CreateApiServiceClient();

        var request = new
        {
            productIds = new[] { Guid.NewGuid().ToString() }
        };

        string json = JsonSerializer.Serialize(request);
        var response = await client.PostAsync(
            "/api/compatibility/validate",
            new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);

        Assert.True(doc.RootElement.TryGetProperty("isCompatible", out _), "Response must contain 'isCompatible'");
        Assert.True(doc.RootElement.TryGetProperty("issues", out _), "Response must contain 'issues'");
    }
}

[Collection(AppHostCollection.Name)]
public class GetBuildCompatibilityTests(AppHostFixture fixture)
{
    [Fact]
    public async Task GetCompatibility_NonExistentBuild_ReturnsNotFound()
    {
        HttpClient client = fixture.CreateApiServiceClient();

        HttpResponseMessage response = await client.GetAsync($"/api/builds/{Guid.NewGuid()}/compatibility");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCompatibility_ExistingBuild_ReturnsOk()
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

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        string content = await createResponse.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);

        Assert.True(doc.RootElement.TryGetProperty("id", out var idElement), "Build creation response must contain 'id'");
        string buildId = idElement.GetString() ?? throw new InvalidOperationException("Build ID must not be null");

        HttpResponseMessage getResponse = await client.GetAsync($"/api/builds/{buildId}/compatibility");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetCompatibility_IncludesBuildId()
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

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        string buildContent = await createResponse.Content.ReadAsStringAsync();
        using JsonDocument buildDoc = JsonDocument.Parse(buildContent);

        Assert.True(buildDoc.RootElement.TryGetProperty("id", out var buildIdElement), "Build creation response must contain 'id'");
        string buildId = buildIdElement.GetString() ?? throw new InvalidOperationException("Build ID must not be null");

        HttpResponseMessage getResponse = await client.GetAsync($"/api/builds/{buildId}/compatibility");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        string getContent = await getResponse.Content.ReadAsStringAsync();
        using JsonDocument getDoc = JsonDocument.Parse(getContent);

        Assert.True(getDoc.RootElement.TryGetProperty("buildId", out _), "Compatibility response must contain 'buildId'");
    }
}
