using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.System.Catalog.Endpoints;

[Collection(AppHostCollection.Name)]
public class GetCategoriesTests(AppHostFixture fixture)
{
    [Fact]
    public async Task GetCategories_ReturnsOk()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetCategories_ReturnsCategoryList()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/categories");

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);
        
        Assert.True(doc.RootElement.TryGetProperty("categories", out var categoriesElement));
        Assert.True(categoriesElement.ValueKind == JsonValueKind.Array);
        Assert.True(categoriesElement.GetArrayLength() > 0);
    }

    [Fact]
    public async Task GetCategories_EachCategoryHasRequiredFields()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/categories");

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);
        
        if (doc.RootElement.TryGetProperty("categories", out var categoriesElement) && 
            categoriesElement.GetArrayLength() > 0)
        {
            var firstCategory = categoriesElement[0];
            Assert.True(firstCategory.TryGetProperty("name", out _));
            Assert.True(firstCategory.TryGetProperty("displayValue", out _));
        }
    }

    [Fact]
    public async Task GetCategories_ResponseContainsLinks()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/categories");

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);
        
        Assert.True(doc.RootElement.TryGetProperty("links", out _));
    }
}
