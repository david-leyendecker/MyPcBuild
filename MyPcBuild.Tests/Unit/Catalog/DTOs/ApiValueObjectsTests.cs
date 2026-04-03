using System.Text.Json;
using MyPcBuild.ApiService.Catalog.DTOs;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.Unit.Catalog.DTOs;

public class ApiValueObjectsTests
{
    private readonly JsonSerializerOptions _options = TestJsonOptions.CreateOptions();

    #region ApiFrequency

    [Fact]
    public void Deserialize_ApiFrequency_FromScalarNumber_ReturnsCorrectValue()
    {
        string input = "3.5";
        ApiFrequency result = JsonSerializer.Deserialize<ApiFrequency>(input, _options);
        Assert.Equal(3.5m, result.ValueInGHz);
    }

    [Fact]
    public void Deserialize_ApiFrequency_FromObject_ReturnsCorrectValue()
    {
        string input = "{\"valueInGHz\": 3.5}";
        ApiFrequency result = JsonSerializer.Deserialize<ApiFrequency>(input, _options);
        Assert.Equal(3.5m, result.ValueInGHz);
    }

    [Fact]
    public void Serialize_ApiFrequency_WritesObjectFormat()
    {
        ApiFrequency frequency = ApiFrequency.FromGHz(2.8m);
        string result = JsonSerializer.Serialize(frequency, _options);
        Assert.Contains("2.8", result);
    }

    #endregion

    #region ApiStorageCapacity

    [Fact]
    public void Deserialize_ApiStorageCapacity_FromScalarNumber_ReturnsCorrectValue()
    {
        string input = "512";
        ApiStorageCapacity result = JsonSerializer.Deserialize<ApiStorageCapacity>(input, _options);
        Assert.Equal(512, result.ValueInGB);
    }

    [Fact]
    public void Deserialize_ApiStorageCapacity_FromObject_ReturnsCorrectValue()
    {
        string input = "{\"valueInGB\": 1000}";
        ApiStorageCapacity result = JsonSerializer.Deserialize<ApiStorageCapacity>(input, _options);
        Assert.Equal(1000, result.ValueInGB);
    }

    [Fact]
    public void Serialize_ApiStorageCapacity_WritesObjectFormat()
    {
        ApiStorageCapacity capacity = ApiStorageCapacity.FromGB(512);
        string result = JsonSerializer.Serialize(capacity, _options);
        Assert.Contains("512", result);
    }

    #endregion

    #region ApiPower

    [Fact]
    public void Deserialize_ApiPower_FromScalarNumber_ReturnsCorrectValue()
    {
        string input = "750";
        ApiPower result = JsonSerializer.Deserialize<ApiPower>(input, _options);
        Assert.Equal(750, result.ValueInWatts);
    }

    [Fact]
    public void Deserialize_ApiPower_FromObject_ReturnsCorrectValue()
    {
        string input = "{\"valueInWatts\": 850}";
        ApiPower result = JsonSerializer.Deserialize<ApiPower>(input, _options);
        Assert.Equal(850, result.ValueInWatts);
    }

    [Fact]
    public void Serialize_ApiPower_WritesObjectFormat()
    {
        ApiPower power = ApiPower.FromWatts(750);
        string result = JsonSerializer.Serialize(power, _options);
        Assert.Contains("750", result);
    }

    #endregion

    #region ApiVoltage

    [Fact]
    public void Deserialize_ApiVoltage_FromScalarNumber_ReturnsCorrectValue()
    {
        string input = "1.35";
        ApiVoltage result = JsonSerializer.Deserialize<ApiVoltage>(input, _options);
        Assert.Equal(1.35m, result.ValueInVolts);
    }

    [Fact]
    public void Deserialize_ApiVoltage_FromObject_ReturnsCorrectValue()
    {
        string input = "{\"valueInVolts\": 1.5}";
        ApiVoltage result = JsonSerializer.Deserialize<ApiVoltage>(input, _options);
        Assert.Equal(1.5m, result.ValueInVolts);
    }

    [Fact]
    public void Serialize_ApiVoltage_WritesObjectFormat()
    {
        ApiVoltage voltage = ApiVoltage.FromVolts(1.35m);
        string result = JsonSerializer.Serialize(voltage, _options);
        Assert.Contains("1.35", result);
    }

    #endregion

    #region ApiLength

    [Fact]
    public void Deserialize_ApiLength_FromScalarNumber_ReturnsCorrectValue()
    {
        string input = "300";
        ApiLength result = JsonSerializer.Deserialize<ApiLength>(input, _options);
        Assert.Equal(300, result.ValueInMm);
    }

    [Fact]
    public void Deserialize_ApiLength_FromObject_ReturnsCorrectValue()
    {
        string input = "{\"valueInMm\": 250}";
        ApiLength result = JsonSerializer.Deserialize<ApiLength>(input, _options);
        Assert.Equal(250, result.ValueInMm);
    }

    [Fact]
    public void Serialize_ApiLength_WritesObjectFormat()
    {
        ApiLength length = ApiLength.FromMm(300);
        string result = JsonSerializer.Serialize(length, _options);
        Assert.Contains("300", result);
    }

    #endregion

    #region ApiDataSpeed

    [Fact]
    public void Deserialize_ApiDataSpeed_FromScalarNumber_ReturnsCorrectValue()
    {
        string input = "3500";
        ApiDataSpeed result = JsonSerializer.Deserialize<ApiDataSpeed>(input, _options);
        Assert.Equal(3500, result.ValueInMBps);
    }

    [Fact]
    public void Deserialize_ApiDataSpeed_FromObject_ReturnsCorrectValue()
    {
        string input = "{\"valueInMBps\": 4500}";
        ApiDataSpeed result = JsonSerializer.Deserialize<ApiDataSpeed>(input, _options);
        Assert.Equal(4500, result.ValueInMBps);
    }

    [Fact]
    public void Serialize_ApiDataSpeed_WritesObjectFormat()
    {
        ApiDataSpeed speed = ApiDataSpeed.FromMBps(3500);
        string result = JsonSerializer.Serialize(speed, _options);
        Assert.Contains("3500", result);
    }

    #endregion

    #region ApiCasLatency

    [Fact]
    public void Deserialize_ApiCasLatency_FromStringCL_ReturnsCorrectValue()
    {
        string input = "\"CL16\"";
        ApiCasLatency result = JsonSerializer.Deserialize<ApiCasLatency>(input, _options);
        Assert.Equal(16, result.Value);
    }

    [Fact]
    public void Deserialize_ApiCasLatency_FromIntNumber_ReturnsCorrectValue()
    {
        string input = "16";
        ApiCasLatency result = JsonSerializer.Deserialize<ApiCasLatency>(input, _options);
        Assert.NotNull(result);
        Assert.True(result.Value > 0);
    }

    [Fact]
    public void Deserialize_ApiCasLatency_FromStringNumber_ReturnsCorrectValue()
    {
        string input = "\"18\"";
        ApiCasLatency result = JsonSerializer.Deserialize<ApiCasLatency>(input, _options);
        Assert.Equal(18, result.Value);
    }

    [Fact]
    public void Serialize_ApiCasLatency_WritesStringFormat()
    {
        ApiCasLatency latency = ApiCasLatency.FromInt(20);
        string result = JsonSerializer.Serialize(latency, _options);
        Assert.Contains("CL20", result);
    }

    #endregion

    #region ApiRamConfiguration

    [Fact]
    public void Deserialize_ApiRamConfiguration_FromString_ReturnsCorrectValue()
    {
        string input = "\"2x16GB\"";
        ApiRamConfiguration result = JsonSerializer.Deserialize<ApiRamConfiguration>(input, _options);
        Assert.Equal(2, result.ModuleCount);
        Assert.Equal(16, result.ModuleCapacity.ValueInGB);
    }

    [Fact]
    public void Deserialize_ApiRamConfiguration_FromObject_ReturnsCorrectValue()
    {
        string input = "{\"moduleCount\": 2, \"moduleCapacity\": 32}";
        ApiRamConfiguration result = JsonSerializer.Deserialize<ApiRamConfiguration>(input, _options);
        Assert.Equal(2, result.ModuleCount);
        Assert.Equal(32, result.ModuleCapacity.ValueInGB);
    }

    [Fact]
    public void Serialize_ApiRamConfiguration_WritesStringFormat()
    {
        ApiRamConfiguration config = ApiRamConfiguration.From(2, 16);
        string result = JsonSerializer.Serialize(config, _options);
        Assert.Contains("2x16GB", result);
    }

    #endregion

    #region Round-trip Tests

    [Theory]
    [InlineData(2.5)]
    [InlineData(3.5)]
    [InlineData(4.8)]
    public void RoundTrip_ApiFrequency_SerializeDeserialize_ReturnsOriginal(decimal original)
    {
        var frequency = ApiFrequency.FromGHz(original);
        string serialized = JsonSerializer.Serialize(frequency, _options);
        ApiFrequency deserialized = JsonSerializer.Deserialize<ApiFrequency>(serialized, _options);
        Assert.Equal(frequency.ValueInGHz, deserialized.ValueInGHz);
    }

    [Theory]
    [InlineData(256)]
    [InlineData(512)]
    [InlineData(2000)]
    public void RoundTrip_ApiStorageCapacity_SerializeDeserialize_ReturnsOriginal(int original)
    {
        var capacity = ApiStorageCapacity.FromGB(original);
        string serialized = JsonSerializer.Serialize(capacity, _options);
        ApiStorageCapacity deserialized = JsonSerializer.Deserialize<ApiStorageCapacity>(serialized, _options);
        Assert.Equal(capacity.ValueInGB, deserialized.ValueInGB);
    }

    [Theory]
    [InlineData(650)]
    [InlineData(750)]
    [InlineData(1000)]
    public void RoundTrip_ApiPower_SerializeDeserialize_ReturnsOriginal(int original)
    {
        var power = ApiPower.FromWatts(original);
        string serialized = JsonSerializer.Serialize(power, _options);
        ApiPower deserialized = JsonSerializer.Deserialize<ApiPower>(serialized, _options);
        Assert.Equal(power.ValueInWatts, deserialized.ValueInWatts);
    }

    #endregion
}
