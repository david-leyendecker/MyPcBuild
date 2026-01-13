using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyPcBuild.ApiService.Domain.Models;

public enum ProductCategory
{
    CPU,
    GPU,
    Motherboard,
    RAM,
    Storage,
    PowerSupply,
    Cooler,
    Case
}

public readonly record struct ProductCategoryInfo
{
    public static readonly ProductCategoryInfo Cpu = new("cpu", "Processor");
    public static readonly ProductCategoryInfo Gpu = new("gpu", "Graphics Card");
    public static readonly ProductCategoryInfo Motherboard = new("motherboard", "Motherboard");
    public static readonly ProductCategoryInfo Ram = new("ram", "Memory");
    public static readonly ProductCategoryInfo Storage = new("storage", "Storage");
    public static readonly ProductCategoryInfo PowerSupply = new("powersupply", "Power Supply");
    public static readonly ProductCategoryInfo Cooler = new("cooler", "Cooler");
    public static readonly ProductCategoryInfo PcCase = new("pccase", "Case");


    public string Name { get; }
    public string DisplayValue { get; }

    private ProductCategoryInfo(string name, string displayValue)
    {
        Name = name;
        DisplayValue = displayValue;
    }

    public override string ToString() => Name;
    
    public static ProductCategoryInfo FromEnum(ProductCategory category) => category switch
    {
        ProductCategory.CPU => Cpu,
        ProductCategory.GPU => Gpu,
        ProductCategory.Motherboard => Motherboard,
        ProductCategory.RAM => Ram,
        ProductCategory.Storage => Storage,
        ProductCategory.PowerSupply => PowerSupply,
        ProductCategory.Cooler => Cooler,
        ProductCategory.Case => PcCase,
        _ => throw new ArgumentException($"Unknown category: {category}")
    };

    public static Dictionary<ProductCategory, ProductCategoryInfo> ByEnum()
    {
        return new Dictionary<ProductCategory, ProductCategoryInfo>
        {
            [ProductCategory.CPU] = Cpu,
            [ProductCategory.GPU] = Gpu,
            [ProductCategory.Motherboard] = Motherboard,
            [ProductCategory.RAM] = Ram,
            [ProductCategory.Storage] = Storage,
            [ProductCategory.PowerSupply] = PowerSupply,
            [ProductCategory.Cooler] = Cooler,
            [ProductCategory.Case] = PcCase
        };
    }
}

/// <summary>
/// CPU socket types.
/// </summary>
public enum CpuSocket
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
/// Extension methods for CpuSocket enum to allow custom socket definitions.
/// </summary>
public static class CpuSocketExtensions
{
    /// <summary>
    /// Checks if a socket string matches this CpuSocket enum value.
    /// </summary>
    public static bool Matches(this CpuSocket socket, string socketString)
    {
        return socket.ToString().Equals(socketString, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Parses a socket string to a CpuSocket enum value.
    /// </summary>
    public static CpuSocket Parse(string socketString)
    {
        if (Enum.TryParse<CpuSocket>(socketString, ignoreCase: true, out CpuSocket result))
        {
            return result;
        }
        throw new ArgumentException($"Unknown CPU socket: {socketString}", nameof(socketString));
    }

    /// <summary>
    /// Tries to parse a socket string to a CpuSocket enum value.
    /// </summary>
    public static bool TryParse(string socketString, out CpuSocket socket)
    {
        return Enum.TryParse(socketString, ignoreCase: true, out socket);
    }
}

/// <summary>
/// Memory types for RAM and VRAM.
/// </summary>
public enum MemoryType
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
/// Motherboard form factors.
/// </summary>
public enum FormFactor
{
    ATX,
    MicroATX,
    MiniITX,
    EATX
}

/// <summary>
/// Cooler types
/// </summary>
public enum CoolerType
{
    Air,
    AIO,
    CustomLoop
}

/// <summary>
/// GPU power connector configurations.
/// </summary>
public enum GpuPowerConnector
{
    Dual8Pin,
    Triple8Pin,
    One16Pin
}

/// <summary>
/// JSON converter for GpuPowerConnector to handle string values like "1x16-pin", "2x8pin", etc.
/// </summary>
internal class GpuPowerConnectorConverter : JsonConverter<GpuPowerConnector>
{
    public override GpuPowerConnector Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString() ?? string.Empty;
            string normalized = value.Replace(" ", string.Empty).Replace("-", string.Empty).ToLowerInvariant();

            return normalized switch
            {
                "1x16pin" or "16pin" or "one16pin" => GpuPowerConnector.One16Pin,
                "2x8pin" or "dual8pin" => GpuPowerConnector.Dual8Pin,
                "3x8pin" or "triple8pin" => GpuPowerConnector.Triple8Pin,
                _ => GpuPowerConnector.Dual8Pin // Default fallback
            };
        }

        // Try to parse as standard enum format
        if (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.Number)
        {
            // Fall back to default enum deserialization
            return GpuPowerConnector.Dual8Pin;
        }

        throw new JsonException($"Cannot convert {reader.TokenType} to GpuPowerConnector");
    }

    public override void Write(Utf8JsonWriter writer, GpuPowerConnector value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}


/// <summary>
/// Represents a frequency in gigahertz (GHz).
/// </summary>
[JsonConverter(typeof(FrequencyConverter))]
public record Frequency
{
    public static readonly string Unit = "GHz";
    public decimal ValueInGHz { get; }

    [JsonConstructor]
    public Frequency(decimal valueInGHz)
    {
        if (valueInGHz < 0)
        {
            throw new ArgumentException("Frequency cannot be negative", nameof(valueInGHz));
        }

        ValueInGHz = valueInGHz;
    }

    public static Frequency FromGHz(decimal ghz) => new(ghz);
    public static Frequency FromMHz(decimal mhz) => new(mhz / 1000m);

    public decimal ToMHz() => ValueInGHz * 1000m;

    public override string ToString() => $"{ValueInGHz} {Unit}";
}

/// <summary>
/// Represents a storage capacity in gigabytes (GB).
/// </summary>
[JsonConverter(typeof(StorageCapacityConverter))]
public record StorageCapacity
{
    public static readonly string Unit = "GB";

    public int ValueInGB { get; }

    [JsonConstructor]
    public StorageCapacity(int valueInGB)
    {
        if (valueInGB < 0)
        {
            throw new ArgumentException("Storage capacity cannot be negative", nameof(valueInGB));
        }

        ValueInGB = valueInGB;
    }

    public static StorageCapacity FromGB(int gb) => new(gb);
    public static StorageCapacity FromTB(decimal tb) => new((int)(tb * 1024));

    public decimal ToTB() => ValueInGB / 1024m;

    public override string ToString() => ValueInGB >= 1024 ? $"{ToTB():F2} TB" : $"{ValueInGB} {Unit}";
}

/// <summary>
/// Represents a power rating in watts (W).
/// </summary>
[JsonConverter(typeof(PowerConverter))]
public record Power
{
    public static readonly string Unit = "W";
    public int ValueInWatts { get; }

    [JsonConstructor]
    public Power(int valueInWatts)
    {
        if (valueInWatts < 0)
        {
            throw new ArgumentException("Power cannot be negative", nameof(valueInWatts));
        }

        ValueInWatts = valueInWatts;
    }

    public static Power FromWatts(int watts) => new(watts);

    public override string ToString() => $"{ValueInWatts}{Unit}";
}

/// <summary>
/// Represents a voltage in volts (V).
/// </summary>
[JsonConverter(typeof(VoltageConverter))]
public record Voltage
{
    public static readonly string Unit = "V";
    public decimal ValueInVolts { get; }

    [JsonConstructor]
    public Voltage(decimal valueInVolts)
    {
        if (valueInVolts < 0)
        {
            throw new ArgumentException("Voltage cannot be negative", nameof(valueInVolts));
        }

        ValueInVolts = valueInVolts;
    }

    public static Voltage FromVolts(decimal volts) => new(volts);

    public override string ToString() => $"{ValueInVolts}{Unit}";
}

/// <summary>
/// Represents a length/distance in millimeters (mm).
/// </summary>
[JsonConverter(typeof(LengthConverter))]
public record Length
{
    public static readonly string Unit = "mm";

    public int ValueInMm { get; }

    [JsonConstructor]
    public Length(int valueInMm)
    {
        if (valueInMm < 0)
        {
            throw new ArgumentException("Length cannot be negative", nameof(valueInMm));
        }

        ValueInMm = valueInMm;
    }

    public static Length FromMm(int mm) => new(mm);
    public static Length FromCm(decimal cm) => new((int)(cm * 10));

    public decimal ToCm() => ValueInMm / 10m;

    public override string ToString() => $"{ValueInMm}{Unit}";
}

/// <summary>
/// Represents a data transfer speed in MB/s.
/// </summary>
[JsonConverter(typeof(DataSpeedConverter))]
public record DataSpeed
{
    public static readonly string Unit = "MB/s";
    public int ValueInMBps { get; }

    [JsonConstructor]
    public DataSpeed(int valueInMBps)
    {
        if (valueInMBps < 0)
        {
            throw new ArgumentException("Data speed cannot be negative", nameof(valueInMBps));
        }

        ValueInMBps = valueInMBps;
    }

    public static DataSpeed FromMBps(int mbps) => new(mbps);
    public static DataSpeed FromGBps(decimal gbps) => new((int)(gbps * 1000));

    public decimal ToGBps() => ValueInMBps / 1000m;

    public override string ToString() => ValueInMBps >= 1000 ? $"{ToGBps():F2} GB/s" : $"{ValueInMBps} {Unit}";
}

// JSON Converters for value objects

internal class FrequencyConverter : JsonConverter<Frequency>
{
    public override Frequency Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return Frequency.FromGHz(reader.GetDecimal());
        }
        throw new JsonException($"Cannot convert {reader.TokenType} to Frequency");
    }

    public override void Write(Utf8JsonWriter writer, Frequency value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ValueInGHz);
    }
}

internal class StorageCapacityConverter : JsonConverter<StorageCapacity>
{
    public override StorageCapacity Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return StorageCapacity.FromGB(reader.GetInt32());
        }
        throw new JsonException($"Cannot convert {reader.TokenType} to StorageCapacity");
    }

    public override void Write(Utf8JsonWriter writer, StorageCapacity value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ValueInGB);
    }
}

internal class PowerConverter : JsonConverter<Power>
{
    public override Power Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return Power.FromWatts(reader.GetInt32());
        }
        throw new JsonException($"Cannot convert {reader.TokenType} to Power");
    }

    public override void Write(Utf8JsonWriter writer, Power value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ValueInWatts);
    }
}

internal class VoltageConverter : JsonConverter<Voltage>
{
    public override Voltage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return Voltage.FromVolts(reader.GetDecimal());
        }
        throw new JsonException($"Cannot convert {reader.TokenType} to Voltage");
    }

    public override void Write(Utf8JsonWriter writer, Voltage value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ValueInVolts);
    }
}

internal class LengthConverter : JsonConverter<Length>
{
    public override Length Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return Length.FromMm(reader.GetInt32());
        }
        throw new JsonException($"Cannot convert {reader.TokenType} to Length");
    }

    public override void Write(Utf8JsonWriter writer, Length value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ValueInMm);
    }
}

internal class DataSpeedConverter : JsonConverter<DataSpeed>
{
    public override DataSpeed Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return DataSpeed.FromMBps(reader.GetInt32());
        }
        throw new JsonException($"Cannot convert {reader.TokenType} to DataSpeed");
    }

    public override void Write(Utf8JsonWriter writer, DataSpeed value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ValueInMBps);
    }
}
