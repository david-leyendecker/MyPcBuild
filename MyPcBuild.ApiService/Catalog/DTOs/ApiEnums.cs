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

/// <summary>
/// GPU chipset manufacturer (API).
/// </summary>
[JsonConverter(typeof(ApiGpuChipsetManufacturerConverter))]
public enum ApiGpuChipsetManufacturer
{
    NVIDIA,
    AMD,
    Intel
}

internal class ApiGpuChipsetManufacturerConverter : JsonConverter<ApiGpuChipsetManufacturer>
{
    public override ApiGpuChipsetManufacturer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString() ?? string.Empty;
            return value.ToUpperInvariant() switch
            {
                "NVIDIA" => ApiGpuChipsetManufacturer.NVIDIA,
                "AMD" => ApiGpuChipsetManufacturer.AMD,
                "INTEL" => ApiGpuChipsetManufacturer.Intel,
                _ => throw new JsonException($"Unrecognized GPU chipset manufacturer: {value}. Expected: NVIDIA, AMD, Intel")
            };
        }
        if (reader.TokenType == JsonTokenType.Number)
            return (ApiGpuChipsetManufacturer)reader.GetInt32();
        throw new JsonException($"Cannot convert {reader.TokenType} to ApiGpuChipsetManufacturer");
    }

    public override void Write(Utf8JsonWriter writer, ApiGpuChipsetManufacturer value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}

/// <summary>
/// PC case side panel window type (API).
/// </summary>
[JsonConverter(typeof(ApiSidePanelTypeConverter))]
public enum ApiSidePanelType
{
    None,
    Acrylic,
    TemperedGlass
}

internal class ApiSidePanelTypeConverter : JsonConverter<ApiSidePanelType>
{
    public override ApiSidePanelType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString() ?? string.Empty;
            string normalized = value.Replace(" ", string.Empty).Replace("-", string.Empty).ToLowerInvariant();
            return normalized switch
            {
                "none" => ApiSidePanelType.None,
                "acrylic" => ApiSidePanelType.Acrylic,
                "temperedglass" => ApiSidePanelType.TemperedGlass,
                _ => throw new JsonException($"Unrecognized side panel type: {value}. Expected: None, Acrylic, Tempered Glass")
            };
        }
        if (reader.TokenType == JsonTokenType.Number)
            return (ApiSidePanelType)reader.GetInt32();
        throw new JsonException($"Cannot convert {reader.TokenType} to ApiSidePanelType");
    }

    public override void Write(Utf8JsonWriter writer, ApiSidePanelType value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            ApiSidePanelType.TemperedGlass => "Tempered Glass",
            _ => value.ToString()
        });
}

/// <summary>
/// PSU efficiency rating (API).
/// </summary>
[JsonConverter(typeof(ApiPsuEfficiencyConverter))]
public enum ApiPsuEfficiency
{
    Bronze,
    Silver,
    Gold,
    Platinum,
    Titanium
}internal class ApiPsuEfficiencyConverter : JsonConverter<ApiPsuEfficiency>
{
    private static readonly Dictionary<string, ApiPsuEfficiency> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["80+ Bronze"] = ApiPsuEfficiency.Bronze,
        ["80+Bronze"] = ApiPsuEfficiency.Bronze,
        ["Bronze"] = ApiPsuEfficiency.Bronze,
        ["80+ Silver"] = ApiPsuEfficiency.Silver,
        ["80+Silver"] = ApiPsuEfficiency.Silver,
        ["Silver"] = ApiPsuEfficiency.Silver,
        ["80+ Gold"] = ApiPsuEfficiency.Gold,
        ["80+Gold"] = ApiPsuEfficiency.Gold,
        ["Gold"] = ApiPsuEfficiency.Gold,
        ["80+ Platinum"] = ApiPsuEfficiency.Platinum,
        ["80+Platinum"] = ApiPsuEfficiency.Platinum,
        ["Platinum"] = ApiPsuEfficiency.Platinum,
        ["80+ Titanium"] = ApiPsuEfficiency.Titanium,
        ["80+Titanium"] = ApiPsuEfficiency.Titanium,
        ["Titanium"] = ApiPsuEfficiency.Titanium
    };

    public override ApiPsuEfficiency Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString() ?? string.Empty;
            string key = value.Replace(" ", string.Empty);
            if (Map.TryGetValue(value, out var result) || Map.TryGetValue(key, out result))
                return result;
            throw new JsonException($"Unrecognized PSU efficiency: {value}. Expected: 80+ Bronze, 80+ Silver, 80+ Gold, 80+ Platinum, 80+ Titanium");
        }
        if (reader.TokenType == JsonTokenType.Number)
            return (ApiPsuEfficiency)reader.GetInt32();
        throw new JsonException($"Cannot convert {reader.TokenType} to ApiPsuEfficiency");
    }

    public override void Write(Utf8JsonWriter writer, ApiPsuEfficiency value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            ApiPsuEfficiency.Bronze => "80+ Bronze",
            ApiPsuEfficiency.Silver => "80+ Silver",
            ApiPsuEfficiency.Gold => "80+ Gold",
            ApiPsuEfficiency.Platinum => "80+ Platinum",
            ApiPsuEfficiency.Titanium => "80+ Titanium",
            _ => value.ToString()
        });
}

/// <summary>
/// PSU modularity type (API).
/// </summary>
[JsonConverter(typeof(ApiPsuModularityConverter))]
public enum ApiPsuModularity
{
    NonModular,
    SemiModular,
    FullyModular
}

internal class ApiPsuModularityConverter : JsonConverter<ApiPsuModularity>
{
    public override ApiPsuModularity Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString() ?? string.Empty;
            string normalized = value.Replace(" ", string.Empty).Replace("-", string.Empty).ToLowerInvariant();
            return normalized switch
            {
                "nonmodular" or "non" => ApiPsuModularity.NonModular,
                "semimodular" or "semi" or "half" => ApiPsuModularity.SemiModular,
                "fullymodular" or "full" or "modular" => ApiPsuModularity.FullyModular,
                _ => throw new JsonException($"Unrecognized PSU modularity: {value}. Expected: Non-Modular, Semi-Modular, Fully Modular")
            };
        }
        if (reader.TokenType == JsonTokenType.Number)
            return (ApiPsuModularity)reader.GetInt32();
        throw new JsonException($"Cannot convert {reader.TokenType} to ApiPsuModularity");
    }

    public override void Write(Utf8JsonWriter writer, ApiPsuModularity value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            ApiPsuModularity.NonModular => "Non-Modular",
            ApiPsuModularity.SemiModular => "Semi-Modular",
            ApiPsuModularity.FullyModular => "Fully Modular",
            _ => value.ToString()
        });
}

/// <summary>
/// PSU form factor (API).
/// </summary>
[JsonConverter(typeof(ApiPsuFormFactorConverter))]
public enum ApiPsuFormFactor
{
    ATX,
    SFX,
    SFXL
}

internal class ApiPsuFormFactorConverter : JsonConverter<ApiPsuFormFactor>
{
    public override ApiPsuFormFactor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString() ?? string.Empty;
            string normalized = value.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpperInvariant();
            return normalized switch
            {
                "ATX" => ApiPsuFormFactor.ATX,
                "SFX" => ApiPsuFormFactor.SFX,
                "SFXL" or "SFX-L" => ApiPsuFormFactor.SFXL,
                _ => throw new JsonException($"Unrecognized PSU form factor: {value}. Expected: ATX, SFX, SFX-L")
            };
        }
        if (reader.TokenType == JsonTokenType.Number)
            return (ApiPsuFormFactor)reader.GetInt32();
        throw new JsonException($"Cannot convert {reader.TokenType} to ApiPsuFormFactor");
    }

    public override void Write(Utf8JsonWriter writer, ApiPsuFormFactor value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            ApiPsuFormFactor.SFXL => "SFX-L",
            _ => value.ToString()
        });
}

/// <summary>
/// Storage type (API).
/// </summary>
[JsonConverter(typeof(ApiStorageTypeConverter))]
public enum ApiStorageType
{
    SSD,
    HDD
}

internal class ApiStorageTypeConverter : JsonConverter<ApiStorageType>
{
    public override ApiStorageType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString() ?? string.Empty;
            return value.ToUpperInvariant() switch
            {
                "SSD" => ApiStorageType.SSD,
                "HDD" => ApiStorageType.HDD,
                _ => throw new JsonException($"Unrecognized storage type: {value}. Expected: SSD, HDD")
            };
        }
        if (reader.TokenType == JsonTokenType.Number)
            return (ApiStorageType)reader.GetInt32();
        throw new JsonException($"Cannot convert {reader.TokenType} to ApiStorageType");
    }

    public override void Write(Utf8JsonWriter writer, ApiStorageType value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}

/// <summary>
/// Storage interface (API).
/// </summary>
[JsonConverter(typeof(ApiStorageInterfaceConverter))]
public enum ApiStorageInterface
{
    NVMe,
    SATA
}

internal class ApiStorageInterfaceConverter : JsonConverter<ApiStorageInterface>
{
    public override ApiStorageInterface Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString() ?? string.Empty;
            string normalized = value.Replace(".", string.Empty).Replace("-", string.Empty).ToUpperInvariant();
            if (normalized == "SATA") return ApiStorageInterface.SATA;
            if (normalized == "NVME" || normalized == "M2" || value.Contains("NVMe", StringComparison.OrdinalIgnoreCase) || value.Contains("M.2", StringComparison.OrdinalIgnoreCase))
                return ApiStorageInterface.NVMe;
            throw new JsonException($"Unrecognized storage interface: {value}. Expected: NVMe, SATA");
        }
        if (reader.TokenType == JsonTokenType.Number)
            return (ApiStorageInterface)reader.GetInt32();
        throw new JsonException($"Cannot convert {reader.TokenType} to ApiStorageInterface");
    }

    public override void Write(Utf8JsonWriter writer, ApiStorageInterface value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}

/// <summary>
/// Storage form factor (API).
/// </summary>
[JsonConverter(typeof(ApiStorageFormFactorConverter))]
public enum ApiStorageFormFactor
{
    M2_2280,
    TwoPointFiveInch,
    ThreePointFiveInch
}

internal class ApiStorageFormFactorConverter : JsonConverter<ApiStorageFormFactor>
{
    public override ApiStorageFormFactor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString() ?? string.Empty;
            string normalized = value.Replace(".", string.Empty).Replace(" ", string.Empty).ToLowerInvariant();
            return normalized switch
            {
                "m22280" or "m.22280" or "m2280" => ApiStorageFormFactor.M2_2280,
                "2.5inch" or "25inch" or "2.5" => ApiStorageFormFactor.TwoPointFiveInch,
                "3.5inch" or "35inch" or "3.5" => ApiStorageFormFactor.ThreePointFiveInch,
                _ => throw new JsonException($"Unrecognized storage form factor: {value}. Expected: M.2 2280, 2.5 inch, 3.5 inch")
            };
        }
        if (reader.TokenType == JsonTokenType.Number)
            return (ApiStorageFormFactor)reader.GetInt32();
        throw new JsonException($"Cannot convert {reader.TokenType} to ApiStorageFormFactor");
    }

    public override void Write(Utf8JsonWriter writer, ApiStorageFormFactor value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            ApiStorageFormFactor.M2_2280 => "M.2 2280",
            ApiStorageFormFactor.TwoPointFiveInch => "2.5 inch",
            ApiStorageFormFactor.ThreePointFiveInch => "3.5 inch",
            _ => value.ToString()
        });
}