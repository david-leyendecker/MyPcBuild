using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyPcBuild.ApiService.Catalog.DTOs;

/// <summary>
/// Represents a frequency value (API).
/// Optimized for API consumption with simple numeric representation.
/// </summary>
[JsonConverter(typeof(ApiFrequencyConverter))]
public record ApiFrequency
{
    /// <summary>
    /// Frequency value in GHz.
    /// </summary>
    [Required]
    [Range(0.1, 10.0)]
    public required decimal ValueInGHz { get; init; }

    public static ApiFrequency FromGHz(decimal ghz) => new() { ValueInGHz = ghz };
    public static ApiFrequency FromMHz(decimal mhz) => new() { ValueInGHz = mhz / 1000m };
    public decimal ToMHz() => ValueInGHz * 1000m;
}

internal class ApiFrequencyConverter : JsonConverter<ApiFrequency>
{
    public override ApiFrequency Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return ApiFrequency.FromGHz(reader.GetDecimal());
        }
        
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            if (doc.RootElement.TryGetProperty("ValueInGHz", out JsonElement valueElement) ||
                doc.RootElement.TryGetProperty("valueInGHz", out valueElement))
            {
                return ApiFrequency.FromGHz(valueElement.GetDecimal());
            }
        }
        
        throw new JsonException($"Cannot convert {reader.TokenType} to ApiFrequency");
    }

    public override void Write(Utf8JsonWriter writer, ApiFrequency value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("valueInGHz", value.ValueInGHz);
        writer.WriteEndObject();
    }
}

/// <summary>
/// Represents a storage capacity value (API).
/// </summary>
[JsonConverter(typeof(ApiStorageCapacityConverter))]
public record ApiStorageCapacity
{
    /// <summary>
    /// Storage capacity in GB.
    /// </summary>
    [Required]
    [Range(1, 100000)]
    public required int ValueInGB { get; init; }

    public static ApiStorageCapacity FromGB(int gb) => new() { ValueInGB = gb };
}

internal class ApiStorageCapacityConverter : JsonConverter<ApiStorageCapacity>
{
    public override ApiStorageCapacity Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return ApiStorageCapacity.FromGB(reader.GetInt32());
        }
        
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            if (doc.RootElement.TryGetProperty("ValueInGB", out JsonElement valueElement) ||
                doc.RootElement.TryGetProperty("valueInGB", out valueElement))
            {
                return ApiStorageCapacity.FromGB(valueElement.GetInt32());
            }
        }
        
        throw new JsonException($"Cannot convert {reader.TokenType} to ApiStorageCapacity");
    }

    public override void Write(Utf8JsonWriter writer, ApiStorageCapacity value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("valueInGB", value.ValueInGB);
        writer.WriteEndObject();
    }
}

/// <summary>
/// Represents a power rating value (API).
/// </summary>
[JsonConverter(typeof(ApiPowerConverter))]
public record ApiPower
{
    /// <summary>
    /// Power in watts.
    /// </summary>
    [Required]
    [Range(1, 3000)]
    public required int ValueInWatts { get; init; }

    public static ApiPower FromWatts(int watts) => new() { ValueInWatts = watts };
}

internal class ApiPowerConverter : JsonConverter<ApiPower>
{
    public override ApiPower Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return ApiPower.FromWatts(reader.GetInt32());
        }
        
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            if (doc.RootElement.TryGetProperty("ValueInWatts", out JsonElement valueElement) ||
                doc.RootElement.TryGetProperty("valueInWatts", out valueElement))
            {
                return ApiPower.FromWatts(valueElement.GetInt32());
            }
        }
        
        throw new JsonException($"Cannot convert {reader.TokenType} to ApiPower");
    }

    public override void Write(Utf8JsonWriter writer, ApiPower value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("valueInWatts", value.ValueInWatts);
        writer.WriteEndObject();
    }
}

/// <summary>
/// Represents a voltage value (API).
/// </summary>
[JsonConverter(typeof(ApiVoltageConverter))]
public record ApiVoltage
{
    /// <summary>
    /// Voltage in volts.
    /// </summary>
    [Required]
    [Range(0.5, 3.0)]
    public required decimal ValueInVolts { get; init; }

    public static ApiVoltage FromVolts(decimal volts) => new() { ValueInVolts = volts };
}

internal class ApiVoltageConverter : JsonConverter<ApiVoltage>
{
    public override ApiVoltage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return ApiVoltage.FromVolts(reader.GetDecimal());
        }
        
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            if (doc.RootElement.TryGetProperty("ValueInVolts", out JsonElement valueElement) ||
                doc.RootElement.TryGetProperty("valueInVolts", out valueElement))
            {
                return ApiVoltage.FromVolts(valueElement.GetDecimal());
            }
        }
        
        throw new JsonException($"Cannot convert {reader.TokenType} to ApiVoltage");
    }

    public override void Write(Utf8JsonWriter writer, ApiVoltage value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("valueInVolts", value.ValueInVolts);
        writer.WriteEndObject();
    }
}

/// <summary>
/// Represents a length/distance value (API).
/// </summary>
[JsonConverter(typeof(ApiLengthConverter))]
public record ApiLength
{
    /// <summary>
    /// Length in millimeters.
    /// </summary>
    [Required]
    [Range(1, 1000)]
    public required int ValueInMm { get; init; }

    public static ApiLength FromMm(int mm) => new() { ValueInMm = mm };
}

internal class ApiLengthConverter : JsonConverter<ApiLength>
{
    public override ApiLength Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return ApiLength.FromMm(reader.GetInt32());
        }
        
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            if (doc.RootElement.TryGetProperty("ValueInMm", out JsonElement valueElement) ||
                doc.RootElement.TryGetProperty("valueInMm", out valueElement))
            {
                return ApiLength.FromMm(valueElement.GetInt32());
            }
        }
        
        throw new JsonException($"Cannot convert {reader.TokenType} to ApiLength");
    }

    public override void Write(Utf8JsonWriter writer, ApiLength value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("valueInMm", value.ValueInMm);
        writer.WriteEndObject();
    }
}

/// <summary>
/// Represents a data transfer speed value (API).
/// </summary>
[JsonConverter(typeof(ApiDataSpeedConverter))]
public record ApiDataSpeed
{
    /// <summary>
    /// Data speed in MB/s.
    /// </summary>
    [Required]
    [Range(1, 20000)]
    public required int ValueInMBps { get; init; }

    public static ApiDataSpeed FromMBps(int mbps) => new() { ValueInMBps = mbps };
}

internal class ApiDataSpeedConverter : JsonConverter<ApiDataSpeed>
{
    public override ApiDataSpeed Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return ApiDataSpeed.FromMBps(reader.GetInt32());
        }
        
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            if (doc.RootElement.TryGetProperty("ValueInMBps", out JsonElement valueElement) ||
                doc.RootElement.TryGetProperty("valueInMBps", out valueElement))
            {
                return ApiDataSpeed.FromMBps(valueElement.GetInt32());
            }
        }
        
        throw new JsonException($"Cannot convert {reader.TokenType} to ApiDataSpeed");
    }

    public override void Write(Utf8JsonWriter writer, ApiDataSpeed value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("valueInMBps", value.ValueInMBps);
        writer.WriteEndObject();
    }
}
