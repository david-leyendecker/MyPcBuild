using System.Text.Json;
using MyPcBuild.ApiService.Catalog.DTOs;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.Unit.Catalog.DTOs;

public class DimensionsModelConverterTests
{
    private readonly JsonSerializerOptions _options = TestJsonOptions.CreateOptions();

    [Fact]
    public void Deserialize_DimensionsModel_FromObject_ReturnsCorrectDimensions()
    {
        string json = """
            {
                "length": 300,
                "width": 200,
                "height": 400
            }
            """;

        DimensionsModel result = JsonSerializer.Deserialize<DimensionsModel>(json, _options);
        
        Assert.NotNull(result);
        Assert.Equal(300, result.Length);
        Assert.Equal(200, result.Width);
        Assert.Equal(400, result.Height);
    }

    [Fact]
    public void Deserialize_DimensionsModel_FromCommaSeparatedString_ReturnsCorrectDimensions()
    {
        string json = "\"300,200,400\"";

        DimensionsModel result = JsonSerializer.Deserialize<DimensionsModel>(json, _options);
        
        Assert.NotNull(result);
        Assert.Equal(300, result.Length);
        Assert.Equal(200, result.Width);
        Assert.Equal(400, result.Height);
    }

    [Fact]
    public void Deserialize_DimensionsModel_FromStringWithSpaces_StillParses()
    {
        string json = "\"300, 200, 400\"";

        DimensionsModel result = JsonSerializer.Deserialize<DimensionsModel>(json, _options);
        
        Assert.NotNull(result);
        // Should parse correctly despite spaces
        Assert.True(result.Length > 0 && result.Width > 0 && result.Height > 0);
    }

    [Fact]
    public void Serialize_DimensionsModel_WritesObjectFormat()
    {
        var dimensions = new DimensionsModel { Length = 300, Width = 200, Height = 400 };
        string result = JsonSerializer.Serialize(dimensions, _options);
        
        Assert.Contains("300", result);
        Assert.Contains("200", result);
        Assert.Contains("400", result);
    }

    [Fact]
    public void RoundTrip_DimensionsModel_SerializeDeserialize_ReturnsOriginal()
    {
        var original = new DimensionsModel { Length = 500, Width = 350, Height = 650 };
        string serialized = JsonSerializer.Serialize(original, _options);
        DimensionsModel deserialized = JsonSerializer.Deserialize<DimensionsModel>(serialized, _options);
        
        Assert.NotNull(deserialized);
        Assert.Equal(original.Length, deserialized.Length);
        Assert.Equal(original.Width, deserialized.Width);
        Assert.Equal(original.Height, deserialized.Height);
    }
}
