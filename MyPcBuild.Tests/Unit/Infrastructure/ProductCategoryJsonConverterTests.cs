using System.Text.Json;
using MyPcBuild.ApiService.Infrastructure;
using MyPcBuild.ApiService.Catalog.Models;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.Unit.Infrastructure;

public class ProductCategoryJsonConverterTests
{
    private readonly JsonSerializerOptions _options = TestJsonOptions.CreateOptions();

    [Theory]
    [InlineData("\"cpu\"", ProductCategory.CPU)]
    [InlineData("\"CPU\"", ProductCategory.CPU)]
    [InlineData("\"Cpu\"", ProductCategory.CPU)]
    [InlineData("\"gpu\"", ProductCategory.GPU)]
    [InlineData("\"GPU\"", ProductCategory.GPU)]
    [InlineData("\"motherboard\"", ProductCategory.Motherboard)]
    [InlineData("\"MOTHERBOARD\"", ProductCategory.Motherboard)]
    [InlineData("\"ram\"", ProductCategory.RAM)]
    [InlineData("\"RAM\"", ProductCategory.RAM)]
    [InlineData("\"case\"", ProductCategory.Case)]
    [InlineData("\"CASE\"", ProductCategory.Case)]
    [InlineData("\"powersupply\"", ProductCategory.PowerSupply)]
    [InlineData("\"POWERSUPPLY\"", ProductCategory.PowerSupply)]
    [InlineData("\"storage\"", ProductCategory.Storage)]
    [InlineData("\"STORAGE\"", ProductCategory.Storage)]
    [InlineData("\"cooler\"", ProductCategory.Cooler)]
    [InlineData("\"COOLER\"", ProductCategory.Cooler)]
    public void Deserialize_ProductCategory_WithVariousCases_ReturnsCorrectValue(string json, ProductCategory expected)
    {
        ProductCategory result = JsonSerializer.Deserialize<ProductCategory>(json, _options);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(ProductCategory.CPU, "\"cpu\"")]
    [InlineData(ProductCategory.GPU, "\"gpu\"")]
    [InlineData(ProductCategory.Motherboard, "\"motherboard\"")]
    [InlineData(ProductCategory.RAM, "\"ram\"")]
    [InlineData(ProductCategory.Case, "\"case\"")]
    [InlineData(ProductCategory.PowerSupply, "\"powersupply\"")]
    [InlineData(ProductCategory.Storage, "\"storage\"")]
    [InlineData(ProductCategory.Cooler, "\"cooler\"")]
    public void Serialize_ProductCategory_WritesLowercaseString(ProductCategory category, string expected)
    {
        string result = JsonSerializer.Serialize(category, _options);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Deserialize_ProductCategory_InvalidValue_ThrowsJsonException()
    {
        string invalidJson = "\"NotACategory\"";
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ProductCategory>(invalidJson, _options));
    }

    [Theory]
    [InlineData(ProductCategory.CPU)]
    [InlineData(ProductCategory.GPU)]
    [InlineData(ProductCategory.Motherboard)]
    [InlineData(ProductCategory.RAM)]
    [InlineData(ProductCategory.Case)]
    [InlineData(ProductCategory.PowerSupply)]
    [InlineData(ProductCategory.Storage)]
    [InlineData(ProductCategory.Cooler)]
    public void RoundTrip_ProductCategory_SerializeDeserialize_ReturnsOriginal(ProductCategory original)
    {
        string serialized = JsonSerializer.Serialize(original, _options);
        ProductCategory deserialized = JsonSerializer.Deserialize<ProductCategory>(serialized, _options);
        Assert.Equal(original, deserialized);
    }
}
