using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.System.Builds.Endpoints;

[Collection(AppHostCollection.Name)]
public class CreateBuildTests(AppHostFixture fixture)
{
    [Fact]
    public async Task PostBuild_ValidRequest_Returns201()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        var request = new
        {
            name = $"Test Build {Guid.NewGuid()}",
            userId = Guid.NewGuid()
        };

        string json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
        var response = await client.PostAsync("/api/builds", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostBuild_ResponseContainsIdAndLinks()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        var request = new
        {
            name = $"Test Build {Guid.NewGuid()}",
            userId = Guid.NewGuid()
        };

        string json = JsonSerializer.Serialize(request);
        var httpContent = new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
        var response = await client.PostAsync("/api/builds", httpContent);

        string responseBody = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(responseBody);
        
        Assert.True(doc.RootElement.TryGetProperty("id", out var idElement));
        Assert.NotNull(idElement.GetString());
        Assert.True(doc.RootElement.TryGetProperty("links", out _));
    }

    [Fact]
    public async Task PostBuild_MissingName_ReturnsBadRequest()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        var request = new
        {
            userId = Guid.NewGuid()
            // Missing name
        };

        string json = JsonSerializer.Serialize(request);
        var httpContent = new StringContent(json, Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
        var response = await client.PostAsync("/api/builds", httpContent);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
