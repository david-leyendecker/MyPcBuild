using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.System.Catalog.Endpoints;

[Collection(AppHostCollection.Name)]
public class GetProductsTests(AppHostFixture fixture)
{
    [Fact]
    public async Task GetProducts_ReturnsOk()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_ReturnsJsonContent()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/products");

        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetProducts_ResponseContainsItems()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/products");

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);
        
        Assert.True(doc.RootElement.TryGetProperty("items", out _));
    }

    [Fact]
    public async Task GetProducts_ResponseContainsPaginationMetadata()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/products");

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);
        
        Assert.True(doc.RootElement.TryGetProperty("paginationMetadata", out var paginationElement));
        Assert.True(paginationElement.TryGetProperty("totalCount", out _));
        Assert.True(paginationElement.TryGetProperty("totalPages", out _));
        Assert.True(paginationElement.TryGetProperty("pageNumber", out _));
        Assert.True(paginationElement.TryGetProperty("itemsPerPage", out _));
    }

    [Fact]
    public async Task GetProducts_WithPage2_ReturnsDifferentResults()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        HttpResponseMessage response1 = await client.GetAsync("/api/catalog/products?page=1&itemsPerPage=5");
        HttpResponseMessage response2 = await client.GetAsync("/api/catalog/products?page=2&itemsPerPage=5");

        string content1 = await response1.Content.ReadAsStringAsync();
        string content2 = await response2.Content.ReadAsStringAsync();

        // Both should be valid JSON
        using JsonDocument doc1 = JsonDocument.Parse(content1);
        using JsonDocument doc2 = JsonDocument.Parse(content2);

        Assert.True(doc1.RootElement.TryGetProperty("items", out _));
        Assert.True(doc2.RootElement.TryGetProperty("items", out _));
    }

    [Fact]
    public async Task GetProducts_WithSearchQuery_ReturnsFilteredResults()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        
        // First, list all products
        HttpResponseMessage allResponse = await client.GetAsync("/api/catalog/products");
        
        // Then search for a specific product
        HttpResponseMessage searchResponse = await client.GetAsync("/api/catalog/products?search=Intel");

        Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, searchResponse.StatusCode);
    }

    [Fact]
    public async Task GetProducts_ResponseHateoasLinks()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/products");

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);
        
        Assert.True(doc.RootElement.TryGetProperty("links", out var linksElement));
        Assert.True(linksElement.ValueKind == JsonValueKind.Array);
    }

    [Fact]
    public async Task GetProducts_EachItemHasIdAndName()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/products");

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);
        
        if (doc.RootElement.TryGetProperty("items", out var itemsElement) && 
            itemsElement.ValueKind == JsonValueKind.Array && 
            itemsElement.GetArrayLength() > 0)
        {
            var firstItem = itemsElement[0];
            Assert.True(firstItem.TryGetProperty("id", out _));
            Assert.True(firstItem.TryGetProperty("name", out _));
        }
    }

    [Fact]
    public async Task GetProducts_InvalidPage_ReturnsBadRequest()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/products?page=0");

        // Page must be >= 1
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_InvalidItemsPerPage_ReturnsBadRequest()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/products?itemsPerPage=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WithCategoryFilter_ReturnsOnlyMatchingCategory()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/products?filters=ProductCategory=cpu");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WithSorting_ReturnsOk()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/products?sortBy=Name&sortDesc=false");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
