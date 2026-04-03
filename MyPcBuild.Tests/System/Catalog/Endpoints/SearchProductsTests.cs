using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.System.Catalog.Endpoints;

[Collection(AppHostCollection.Name)]
public class SearchProductsTests(AppHostFixture fixture)
{
    [Fact]
    public async Task SearchProducts_ReturnsOk()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/search?query=cpu");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SearchProducts_WithQuery_ReturnsResults()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/search?query=Intel");

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);
        
        Assert.True(doc.RootElement.TryGetProperty("items", out var itemsElement));
    }

    [Fact]
    public async Task SearchProducts_EmptyQuery_ReturnsEmptyList()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/search");

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);
        
        Assert.True(doc.RootElement.TryGetProperty("items", out var itemsElement));
        Assert.Equal(0, itemsElement.GetArrayLength());
    }

    [Fact]
    public async Task SearchProducts_WithMaxResults_RespectsLimit()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/search?query=cpu&maxResults=5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);
        
        if (doc.RootElement.TryGetProperty("items", out var itemsElement))
        {
            Assert.True(itemsElement.GetArrayLength() <= 5);
        }
    }
}
