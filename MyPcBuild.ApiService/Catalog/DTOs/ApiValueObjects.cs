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

/// <summary>
/// Represents CAS latency (API).
/// </summary>
[JsonConverter(typeof(ApiCasLatencyConverter))]
public record ApiCasLatency
{
    [Required]
    [Range(1, 50)]
    public required int Value { get; init; }

    public static ApiCasLatency FromInt(int value) => new() { Value = value };
    public override string ToString() => $"CL{Value}";
}

internal class ApiCasLatencyConverter : JsonConverter<ApiCasLatency>
{
    public override ApiCasLatency Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return ApiCasLatency.FromInt(reader.GetInt32());
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            string? s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s)) throw new JsonException("CAS latency string cannot be empty");
            string normalized = s.TrimStart('C', 'L', 'c', 'l');
            if (int.TryParse(normalized, out int val) && val >= 1 && val <= 50)
                return ApiCasLatency.FromInt(val);
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            if (doc.RootElement.TryGetProperty("value", out JsonElement valueElement) ||
                doc.RootElement.TryGetProperty("Value", out valueElement))
            {
                return ApiCasLatency.FromInt(valueElement.GetInt32());
            }
        }

        throw new JsonException($"Cannot convert to ApiCasLatency. Expected number, CL16-style string, or object with value.");
    }

    public override void Write(Utf8JsonWriter writer, ApiCasLatency value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

/// <summary>
/// Represents RAM configuration (API).
/// </summary>
[JsonConverter(typeof(ApiRamConfigurationConverter))]
public record ApiRamConfiguration
{
    [Required]
    [Range(1, 8)]
    public required int ModuleCount { get; init; }

    [Required]
    public required ApiStorageCapacity ModuleCapacity { get; init; }

    public static ApiRamConfiguration From(int moduleCount, int capacityGb) =>
        new() { ModuleCount = moduleCount, ModuleCapacity = ApiStorageCapacity.FromGB(capacityGb) };

    public int TotalCapacityGb => ModuleCount * ModuleCapacity.ValueInGB;
    public override string ToString() => $"{ModuleCount}x{ModuleCapacity.ValueInGB}GB";
}

internal class ApiRamConfigurationConverter : JsonConverter<ApiRamConfiguration>
{
    public override ApiRamConfiguration Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string? s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s)) throw new JsonException("RAM configuration string cannot be empty");
            string[] parts = s.ToUpperInvariant().Split(['X', 'x']);
            if (parts.Length != 2 ||
                !int.TryParse(parts[0].Trim(), out int count) ||
                count < 1 || count > 8 ||
                !int.TryParse(parts[1].Trim().Replace("GB", "").Trim(), out int gb) ||
                gb < 1)
            {
                throw new JsonException($"Invalid RAM configuration format: {s}. Expected e.g. 2x16GB");
            }
            return ApiRamConfiguration.From(count, gb);
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            int count = 1;
            int gb = 16;
            if (doc.RootElement.TryGetProperty("moduleCount", out JsonElement mc) || doc.RootElement.TryGetProperty("ModuleCount", out mc))
                count = mc.GetInt32();
            if (doc.RootElement.TryGetProperty("moduleCapacity", out JsonElement cap) || doc.RootElement.TryGetProperty("ModuleCapacity", out cap))
            {
                if (cap.ValueKind == JsonValueKind.Number)
                    gb = cap.GetInt32();
                else if (cap.TryGetProperty("valueInGB", out JsonElement gbEl) || cap.TryGetProperty("ValueInGB", out gbEl))
                    gb = gbEl.GetInt32();
            }
            return ApiRamConfiguration.From(count, gb);
        }

        throw new JsonException("Cannot convert to ApiRamConfiguration. Expected 2x16GB-style string or object.");
    }

    public override void Write(Utf8JsonWriter writer, ApiRamConfiguration value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
