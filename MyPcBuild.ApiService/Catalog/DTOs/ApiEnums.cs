using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyPcBuild.ApiService.Catalog.DTOs;

/// <summary>
/// CPU socket types for API consumption.
/// Optimized for external clients and AI-driven product creation.
/// </summary>
public enum ApiCpuSocket
{
    // Intel sockets
    LGA1700,
    LGA1200,
    LGA1151,
    LGA2066,

    // AMD sockets
    AM5,
    AM4,
    sTRX4,
    TR4
}

/// <summary>
/// Memory types for RAM and VRAM (API).
/// </summary>
public enum ApiMemoryType
{
    // DDR RAM types
    DDR3,
    DDR4,
    DDR5,

    // GDDR VRAM types
    GDDR5,
    GDDR5X,
    GDDR6,
    GDDR6X,

    // HBM types
    HBM2,
    HBM2E,
    HBM3
}

/// <summary>
/// Motherboard form factors (API).
/// </summary>
public enum ApiFormFactor
{
    ATX,
    MicroATX,
    MiniITX,
    EATX
}

/// <summary>
/// Cooler types (API).
/// </summary>
public enum ApiCoolerType
{
    Air,
    AIO,
    CustomLoop
}

/// <summary>
/// GPU power connector configurations (API).
/// </summary>
public enum ApiGpuPowerConnector
{
    Dual8Pin,
    Triple8Pin,
    One16Pin
}

/// <summary>
/// JSON converter for ApiGpuPowerConnector to handle string values like "1x16-pin", "2x8pin", etc.
/// </summary>
internal class ApiGpuPowerConnectorConverter : JsonConverter<ApiGpuPowerConnector>
{
    public override ApiGpuPowerConnector Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString() ?? string.Empty;
            string normalized = value.Replace(" ", string.Empty).Replace("-", string.Empty).ToLowerInvariant();

            return normalized switch
            {
                "1x16pin" or "16pin" or "one16pin" => ApiGpuPowerConnector.One16Pin,
                "2x8pin" or "dual8pin" => ApiGpuPowerConnector.Dual8Pin,
                "3x8pin" or "triple8pin" => ApiGpuPowerConnector.Triple8Pin,
                _ => throw new JsonException($"Unrecognized GPU power connector value: {value}. Expected one of: 1x16-pin, 2x8-pin, 3x8-pin")
            };
        }

        // Try to parse as standard enum format
        if (reader.TokenType == JsonTokenType.Number)
        {
            return (ApiGpuPowerConnector)reader.GetInt32();
        }

        throw new JsonException($"Cannot convert {reader.TokenType} to ApiGpuPowerConnector");
    }

    public override void Write(Utf8JsonWriter writer, ApiGpuPowerConnector value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

