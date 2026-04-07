using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.System.Builds.Endpoints;

[Collection(AppHostCollection.Name)]
public class GetBuildsTests(AppHostFixture fixture)
{
    [Fact]
    public async Task GetBuilds_ReturnsOk()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/builds");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetBuilds_ReturnsJsonContent()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/builds");

        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetBuilds_ResponseContainsItems()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/builds");

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);
        
        Assert.True(doc.RootElement.TryGetProperty("items", out _));
    }

    [Fact]
    public async Task GetBuilds_ResponseContainsPaginationMetadata()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/builds");

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);
        
        Assert.True(doc.RootElement.TryGetProperty("paginationMetadata", out var paginationElement));
        Assert.True(paginationElement.TryGetProperty("totalCount", out _));
        Assert.True(paginationElement.TryGetProperty("totalPages", out _));
    }

    [Fact]
    public async Task GetBuilds_WithPagination_ReturnsPaginatedResults()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        HttpResponseMessage response1 = await client.GetAsync("/api/builds?page=1&itemsPerPage=5");
        HttpResponseMessage response2 = await client.GetAsync("/api/builds?page=2&itemsPerPage=5");

        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
    }

    [Fact]
    public async Task GetBuilds_ResponseContainsHateoasLinks()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/builds");

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);
        
        Assert.True(doc.RootElement.TryGetProperty("links", out var linksElement));
        Assert.True(linksElement.ValueKind == JsonValueKind.Array);
    }
}
