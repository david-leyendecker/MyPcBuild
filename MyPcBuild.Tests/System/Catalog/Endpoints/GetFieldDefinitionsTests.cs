using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.System.Catalog.Endpoints;

[Collection(AppHostCollection.Name)]
public class GetFieldDefinitionsTests(AppHostFixture fixture)
{
    [Theory]
    [InlineData("cpu")]
    [InlineData("gpu")]
    [InlineData("motherboard")]
    [InlineData("ram")]
    [InlineData("case")]
    [InlineData("powersupply")]
    [InlineData("storage")]
    [InlineData("cooler")]
    public async Task GetFieldDefinitions_ValidCategory_ReturnsOk(string category)
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync($"/api/catalog/field-definitions/{category}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("cpu")]
    [InlineData("gpu")]
    [InlineData("motherboard")]
    public async Task GetFieldDefinitions_Category_ContainsFields(string category)
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync($"/api/catalog/field-definitions/{category}");

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);
        
        Assert.True(doc.RootElement.TryGetProperty("fields", out var fieldsElement));
        Assert.True(fieldsElement.ValueKind == JsonValueKind.Array);
        Assert.True(fieldsElement.GetArrayLength() > 0);
    }

    [Fact]
    public async Task GetFieldDefinitions_CPUCategory_HasSocketField()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/field-definitions/cpu");

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(content);
        
        if (doc.RootElement.TryGetProperty("fields", out var fieldsElement))
        {
            bool hasSocketField = false;
            foreach (var field in fieldsElement.EnumerateArray())
            {
                if (field.TryGetProperty("name", out var nameElement) && 
                    string.Equals(nameElement.GetString(), "socket", StringComparison.OrdinalIgnoreCase))
                {
                    hasSocketField = true;
                    break;
                }
            }
            Assert.True(hasSocketField);
        }
    }

    [Fact]
    public async Task GetFieldDefinitions_InvalidCategory_ReturnsBadRequest()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/api/catalog/field-definitions/invalid-category");

        Assert.True(response.StatusCode == HttpStatusCode.BadRequest || 
                   response.StatusCode == HttpStatusCode.NotFound);
    }
}
