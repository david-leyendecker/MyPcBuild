using System.Text.Json;
using MyPcBuild.ApiService.Catalog.DTOs;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.Unit.Catalog.DTOs;

public class ApiEnumsTests
{
    private readonly JsonSerializerOptions _options = TestJsonOptions.CreateOptions();

    #region ApiGpuPowerConnector

    [Theory]
    [InlineData("One16Pin", ApiGpuPowerConnector.One16Pin)]
    [InlineData("one16pin", ApiGpuPowerConnector.One16Pin)]
    [InlineData("Dual8Pin", ApiGpuPowerConnector.Dual8Pin)]
    [InlineData("Triple8Pin", ApiGpuPowerConnector.Triple8Pin)]
    public void Deserialize_GpuPowerConnector_WithEnumNames_ReturnsCorrectValue(string json, ApiGpuPowerConnector expected)
    {
        string input = $"\"{json}\"";
        ApiGpuPowerConnector result = JsonSerializer.Deserialize<ApiGpuPowerConnector>(input, _options);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Serialize_ApiGpuPowerConnector_WritesEnumName()
    {
        string result = JsonSerializer.Serialize(ApiGpuPowerConnector.One16Pin, _options);
        Assert.Equal("\"One16Pin\"", result);
    }

    [Fact]
    public void Deserialize_GpuPowerConnector_WithOldAlias_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ApiGpuPowerConnector>("\"1x16-pin\"", _options));
    }

    #endregion

    #region ApiSidePanelType

    [Theory]
    [InlineData("TemperedGlass", ApiSidePanelType.TemperedGlass)]
    [InlineData("temperedglass", ApiSidePanelType.TemperedGlass)]
    [InlineData("Acrylic", ApiSidePanelType.Acrylic)]
    [InlineData("None", ApiSidePanelType.None)]
    public void Deserialize_SidePanelType_WithEnumNames_ReturnsCorrectValue(string json, ApiSidePanelType expected)
    {
        string input = $"\"{json}\"";
        ApiSidePanelType result = JsonSerializer.Deserialize<ApiSidePanelType>(input, _options);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Serialize_ApiSidePanelType_WritesEnumName()
    {
        string result = JsonSerializer.Serialize(ApiSidePanelType.TemperedGlass, _options);
        Assert.Equal("\"TemperedGlass\"", result);
    }

    [Fact]
    public void Deserialize_SidePanelType_WithOldAlias_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ApiSidePanelType>("\"Tempered Glass\"", _options));
    }

    #endregion

    #region ApiPsuEfficiency

    [Theory]
    [InlineData("Gold", ApiPsuEfficiency.Gold)]
    [InlineData("gold", ApiPsuEfficiency.Gold)]
    [InlineData("Silver", ApiPsuEfficiency.Silver)]
    [InlineData("Bronze", ApiPsuEfficiency.Bronze)]
    public void Deserialize_PsuEfficiency_WithEnumNames_ReturnsCorrectValue(string json, ApiPsuEfficiency expected)
    {
        string input = $"\"{json}\"";
        ApiPsuEfficiency result = JsonSerializer.Deserialize<ApiPsuEfficiency>(input, _options);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Serialize_ApiPsuEfficiency_WritesEnumName()
    {
        string result = JsonSerializer.Serialize(ApiPsuEfficiency.Gold, _options);
        Assert.Equal("\"Gold\"", result);
    }

    [Fact]
    public void Deserialize_PsuEfficiency_WithOldAlias_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ApiPsuEfficiency>("\"80+ Gold\"", _options));
    }

    #endregion

    #region ApiPsuModularity

    [Theory]
    [InlineData("SemiModular", ApiPsuModularity.SemiModular)]
    [InlineData("semimodular", ApiPsuModularity.SemiModular)]
    [InlineData("FullyModular", ApiPsuModularity.FullyModular)]
    [InlineData("NonModular", ApiPsuModularity.NonModular)]
    public void Deserialize_PsuModularity_WithEnumNames_ReturnsCorrectValue(string json, ApiPsuModularity expected)
    {
        string input = $"\"{json}\"";
        ApiPsuModularity result = JsonSerializer.Deserialize<ApiPsuModularity>(input, _options);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Serialize_ApiPsuModularity_WritesEnumName()
    {
        string result = JsonSerializer.Serialize(ApiPsuModularity.SemiModular, _options);
        Assert.Equal("\"SemiModular\"", result);
    }

    [Fact]
    public void Deserialize_PsuModularity_WithOldAlias_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ApiPsuModularity>("\"Semi-Modular\"", _options));
    }

    #endregion

    #region ApiPsuFormFactor

    [Theory]
    [InlineData("ATX", ApiPsuFormFactor.ATX)]
    [InlineData("SFX", ApiPsuFormFactor.SFX)]
    [InlineData("SFXL", ApiPsuFormFactor.SFXL)]
    [InlineData("sfxl", ApiPsuFormFactor.SFXL)]
    public void Deserialize_PsuFormFactor_WithEnumNames_ReturnsCorrectValue(string json, ApiPsuFormFactor expected)
    {
        string input = $"\"{json}\"";
        ApiPsuFormFactor result = JsonSerializer.Deserialize<ApiPsuFormFactor>(input, _options);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Serialize_ApiPsuFormFactor_WritesEnumName()
    {
        string result = JsonSerializer.Serialize(ApiPsuFormFactor.SFXL, _options);
        Assert.Equal("\"SFXL\"", result);
    }

    [Fact]
    public void Deserialize_PsuFormFactor_WithOldAlias_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ApiPsuFormFactor>("\"SFX-L\"", _options));
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
    [InlineData("NVMe", ApiStorageInterface.NVMe)]
    [InlineData("nvme", ApiStorageInterface.NVMe)]
    [InlineData("SATA", ApiStorageInterface.SATA)]
    [InlineData("sata", ApiStorageInterface.SATA)]
    public void Deserialize_StorageInterface_WithEnumNames_ReturnsCorrectValue(string json, ApiStorageInterface expected)
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
    [InlineData("M2_2280", ApiStorageFormFactor.M2_2280)]
    [InlineData("m2_2280", ApiStorageFormFactor.M2_2280)]
    [InlineData("TwoPointFiveInch", ApiStorageFormFactor.TwoPointFiveInch)]
    [InlineData("ThreePointFiveInch", ApiStorageFormFactor.ThreePointFiveInch)]
    public void Deserialize_StorageFormFactor_WithEnumNames_ReturnsCorrectValue(string json, ApiStorageFormFactor expected)
    {
        string input = $"\"{json}\"";
        ApiStorageFormFactor result = JsonSerializer.Deserialize<ApiStorageFormFactor>(input, _options);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Serialize_ApiStorageFormFactor_WritesEnumName()
    {
        string result = JsonSerializer.Serialize(ApiStorageFormFactor.M2_2280, _options);
        Assert.Equal("\"M2_2280\"", result);
    }

    [Fact]
    public void Deserialize_StorageFormFactor_WithOldAlias_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ApiStorageFormFactor>("\"M.2 2280\"", _options));
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
