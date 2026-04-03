using System.Text.Json;
using MyPcBuild.ApiService.Catalog.DTOs;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.Unit.Catalog.DTOs;

public class ApiEnumsTests
{
    private readonly JsonSerializerOptions _options = TestJsonOptions.CreateOptions();

    #region ApiGpuPowerConnector

    [Theory]
    [InlineData("1x16-pin", ApiGpuPowerConnector.One16Pin)]
    [InlineData("2x8-pin", ApiGpuPowerConnector.Dual8Pin)]
    [InlineData("3x8-pin", ApiGpuPowerConnector.Triple8Pin)]
    public void Deserialize_GpuPowerConnector_WithVariousFormats_ReturnsCorrectValue(string json, ApiGpuPowerConnector expected)
    {
        string input = $"\"{json}\"";
        ApiGpuPowerConnector result = JsonSerializer.Deserialize<ApiGpuPowerConnector>(input, _options);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Serialize_ApiGpuPowerConnector_WritesCorrectString()
    {
        string result = JsonSerializer.Serialize(ApiGpuPowerConnector.One16Pin, _options);
        Assert.Equal("\"1x16-pin\"", result);
    }

    #endregion

    #region ApiSidePanelType

    [Theory]
    [InlineData("Tempered Glass", ApiSidePanelType.TemperedGlass)]
    [InlineData("Acrylic", ApiSidePanelType.Acrylic)]
    [InlineData("None", ApiSidePanelType.None)]
    public void Deserialize_SidePanelType_WithVariousFormats_ReturnsCorrectValue(string json, ApiSidePanelType expected)
    {
        string input = $"\"{json}\"";
        ApiSidePanelType result = JsonSerializer.Deserialize<ApiSidePanelType>(input, _options);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Serialize_ApiSidePanelType_WritesCorrectString()
    {
        string result = JsonSerializer.Serialize(ApiSidePanelType.TemperedGlass, _options);
        Assert.Equal("\"Tempered Glass\"", result);
    }

    #endregion

    #region ApiPsuEfficiency

    [Theory]
    [InlineData("80+ Gold", ApiPsuEfficiency.Gold)]
    [InlineData("80+Gold", ApiPsuEfficiency.Gold)]
    [InlineData("Gold", ApiPsuEfficiency.Gold)]
    [InlineData("80+ Silver", ApiPsuEfficiency.Silver)]
    [InlineData("80+ Bronze", ApiPsuEfficiency.Bronze)]
    public void Deserialize_PsuEfficiency_WithVariousFormats_ReturnsCorrectValue(string json, ApiPsuEfficiency expected)
    {
        string input = $"\"{json}\"";
        ApiPsuEfficiency result = JsonSerializer.Deserialize<ApiPsuEfficiency>(input, _options);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Serialize_ApiPsuEfficiency_WritesCorrectString()
    {
        string result = JsonSerializer.Serialize(ApiPsuEfficiency.Gold, _options);
        Assert.Equal("\"80+ Gold\"", result);
    }

    #endregion

    #region ApiPsuModularity

    [Theory]
    [InlineData("Semi-Modular", ApiPsuModularity.SemiModular)]
    [InlineData("SemiModular", ApiPsuModularity.SemiModular)]
    [InlineData("Fully-Modular", ApiPsuModularity.FullyModular)]
    [InlineData("Modular", ApiPsuModularity.FullyModular)]
    [InlineData("Non-Modular", ApiPsuModularity.NonModular)]
    public void Deserialize_PsuModularity_WithVariousFormats_ReturnsCorrectValue(string json, ApiPsuModularity expected)
    {
        string input = $"\"{json}\"";
        ApiPsuModularity result = JsonSerializer.Deserialize<ApiPsuModularity>(input, _options);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Serialize_ApiPsuModularity_WritesCorrectString()
    {
        string result = JsonSerializer.Serialize(ApiPsuModularity.SemiModular, _options);
        Assert.Equal("\"Semi-Modular\"", result);
    }

    #endregion

    #region ApiPsuFormFactor

    [Theory]
    [InlineData("ATX", ApiPsuFormFactor.ATX)]
    [InlineData("SFX", ApiPsuFormFactor.SFX)]
    [InlineData("SFX-L", ApiPsuFormFactor.SFXL)]
    [InlineData("SFXL", ApiPsuFormFactor.SFXL)]
    public void Deserialize_PsuFormFactor_WithVariousFormats_ReturnsCorrectValue(string json, ApiPsuFormFactor expected)
    {
        string input = $"\"{json}\"";
        ApiPsuFormFactor result = JsonSerializer.Deserialize<ApiPsuFormFactor>(input, _options);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Serialize_ApiPsuFormFactor_WritesCorrectString()
    {
        string result = JsonSerializer.Serialize(ApiPsuFormFactor.SFXL, _options);
        Assert.Equal("\"SFX-L\"", result);
    }

    #endregion

    #region ApiStorageType

    [Theory]
    [InlineData("SSD", ApiStorageType.SSD)]
    [InlineData("ssd", ApiStorageType.SSD)]
    [InlineData("HDD", ApiStorageType.HDD)]
    [InlineData("hdd", ApiStorageType.HDD)]
    public void Deserialize_StorageType_WithVariousFormats_ReturnsCorrectValue(string json, ApiStorageType expected)
    {
        string input = $"\"{json}\"";
        ApiStorageType result = JsonSerializer.Deserialize<ApiStorageType>(input, _options);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Serialize_ApiStorageType_WritesCorrectString()
    {
        string result = JsonSerializer.Serialize(ApiStorageType.SSD, _options);
        Assert.Equal("\"SSD\"", result);
    }

    #endregion

    #region ApiStorageInterface

    [Theory]
    [InlineData("M.2", ApiStorageInterface.NVMe)]
    [InlineData("NVMe", ApiStorageInterface.NVMe)]
    [InlineData("NVME", ApiStorageInterface.NVMe)]
    [InlineData("SATA", ApiStorageInterface.SATA)]
    [InlineData("sata", ApiStorageInterface.SATA)]
    public void Deserialize_StorageInterface_WithVariousFormats_ReturnsCorrectValue(string json, ApiStorageInterface expected)
    {
        string input = $"\"{json}\"";
        ApiStorageInterface result = JsonSerializer.Deserialize<ApiStorageInterface>(input, _options);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Serialize_ApiStorageInterface_WritesCorrectString()
    {
        string result = JsonSerializer.Serialize(ApiStorageInterface.NVMe, _options);
        Assert.Equal("\"NVMe\"", result);
    }

    #endregion

    #region ApiStorageFormFactor

    [Theory]
    [InlineData("M.2 2280", ApiStorageFormFactor.M2_2280)]
    [InlineData("2.5 inch", ApiStorageFormFactor.TwoPointFiveInch)]
    [InlineData("2.5\"", ApiStorageFormFactor.TwoPointFiveInch)]
    [InlineData("3.5 inch", ApiStorageFormFactor.ThreePointFiveInch)]
    public void Deserialize_StorageFormFactor_WithVariousFormats_ReturnsCorrectValue(string json, ApiStorageFormFactor expected)
    {
        string input = $"\"{json}\"";
        ApiStorageFormFactor result = JsonSerializer.Deserialize<ApiStorageFormFactor>(input, _options);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Serialize_ApiStorageFormFactor_WritesCorrectString()
    {
        string result = JsonSerializer.Serialize(ApiStorageFormFactor.M2_2280, _options);
        Assert.Equal("\"M.2 2280\"", result);
    }

    #endregion

    #region ApiGpuChipsetManufacturer

    [Theory]
    [InlineData("NVIDIA", ApiGpuChipsetManufacturer.NVIDIA)]
    [InlineData("nvidia", ApiGpuChipsetManufacturer.NVIDIA)]
    [InlineData("Nvidia", ApiGpuChipsetManufacturer.NVIDIA)]
    [InlineData("AMD", ApiGpuChipsetManufacturer.AMD)]
    [InlineData("amd", ApiGpuChipsetManufacturer.AMD)]
    [InlineData("Intel", ApiGpuChipsetManufacturer.Intel)]
    [InlineData("intel", ApiGpuChipsetManufacturer.Intel)]
    public void Deserialize_GpuChipsetManufacturer_WithVariousFormats_ReturnsCorrectValue(string json, ApiGpuChipsetManufacturer expected)
    {
        string input = $"\"{json}\"";
        ApiGpuChipsetManufacturer result = JsonSerializer.Deserialize<ApiGpuChipsetManufacturer>(input, _options);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Serialize_ApiGpuChipsetManufacturer_WritesCorrectString()
    {
        string result = JsonSerializer.Serialize(ApiGpuChipsetManufacturer.NVIDIA, _options);
        Assert.Equal("\"NVIDIA\"", result);
    }

    #endregion

    #region Round-trip Tests

    [Theory]
    [InlineData(ApiGpuPowerConnector.One16Pin)]
    [InlineData(ApiGpuPowerConnector.Dual8Pin)]
    [InlineData(ApiGpuPowerConnector.Triple8Pin)]
    public void RoundTrip_ApiGpuPowerConnector_SerializeDeserialize_ReturnsOriginal(ApiGpuPowerConnector original)
    {
        string serialized = JsonSerializer.Serialize(original, _options);
        ApiGpuPowerConnector deserialized = JsonSerializer.Deserialize<ApiGpuPowerConnector>(serialized, _options);
        Assert.Equal(original, deserialized);
    }

    [Theory]
    [InlineData(ApiSidePanelType.TemperedGlass)]
    [InlineData(ApiSidePanelType.Acrylic)]
    [InlineData(ApiSidePanelType.None)]
    public void RoundTrip_ApiSidePanelType_SerializeDeserialize_ReturnsOriginal(ApiSidePanelType original)
    {
        string serialized = JsonSerializer.Serialize(original, _options);
        ApiSidePanelType deserialized = JsonSerializer.Deserialize<ApiSidePanelType>(serialized, _options);
        Assert.Equal(original, deserialized);
    }

    [Theory]
    [InlineData(ApiPsuEfficiency.Gold)]
    [InlineData(ApiPsuEfficiency.Silver)]
    [InlineData(ApiPsuEfficiency.Bronze)]
    public void RoundTrip_ApiPsuEfficiency_SerializeDeserialize_ReturnsOriginal(ApiPsuEfficiency original)
    {
        string serialized = JsonSerializer.Serialize(original, _options);
        ApiPsuEfficiency deserialized = JsonSerializer.Deserialize<ApiPsuEfficiency>(serialized, _options);
        Assert.Equal(original, deserialized);
    }

    #endregion

    #region Error Cases

    [Fact]
    public void Deserialize_GpuChipsetManufacturer_InvalidValue_ThrowsJsonException()
    {
        string input = "\"InvalidManufacturer\"";
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ApiGpuChipsetManufacturer>(input, _options));
    }

    [Fact]
    public void Deserialize_StorageType_InvalidValue_ThrowsJsonException()
    {
        string input = "\"NotAStorageType\"";
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ApiStorageType>(input, _options));
    }

    #endregion
}
