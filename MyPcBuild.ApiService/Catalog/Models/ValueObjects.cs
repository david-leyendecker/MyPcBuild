using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyPcBuild.ApiService.Catalog.Models;

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
    public static readonly ProductCategoryInfo PcCase = new("case", "Case");


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
[JsonConverter(typeof(CpuSocketJsonConverter))]
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

internal class CpuSocketJsonConverter : EnumIgnoreCaseJsonConverter<CpuSocket> { }

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
[JsonConverter(typeof(MemoryTypeJsonConverter))]
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

internal class MemoryTypeJsonConverter : EnumIgnoreCaseJsonConverter<MemoryType> { }

/// <summary>
/// Motherboard form factors.
/// </summary>
[JsonConverter(typeof(FormFactorJsonConverter))]
public enum FormFactor
{
    ATX,
    MicroATX,
    MiniITX,
    EATX
}

internal class FormFactorJsonConverter : EnumIgnoreCaseJsonConverter<FormFactor> { }

/// <summary>
/// Cooler types
/// </summary>
[JsonConverter(typeof(CoolerTypeJsonConverter))]
public enum CoolerType
{
    Air,
    AIO,
    CustomLoop
}

internal class CoolerTypeJsonConverter : EnumIgnoreCaseJsonConverter<CoolerType> { }

/// <summary>
/// GPU power connector configurations.
/// </summary>
[JsonConverter(typeof(GpuPowerConnectorJsonConverter))]
public enum GpuPowerConnector
{
    Dual8Pin,
    Triple8Pin,
    One16Pin
}

internal class GpuPowerConnectorJsonConverter : EnumIgnoreCaseJsonConverter<GpuPowerConnector> { }

/// <summary>
/// GPU chipset manufacturer (NVIDIA, AMD, Intel).
/// </summary>
[JsonConverter(typeof(GpuChipsetManufacturerJsonConverter))]
public enum GpuChipsetManufacturer
{
    NVIDIA,
    AMD,
    Intel
}

internal class GpuChipsetManufacturerJsonConverter : EnumIgnoreCaseJsonConverter<GpuChipsetManufacturer> { }

/// <summary>
/// PC case side panel window type.
/// </summary>
[JsonConverter(typeof(SidePanelTypeJsonConverter))]
public enum SidePanelType
{
    None,
    Acrylic,
    TemperedGlass
}

internal class SidePanelTypeJsonConverter : EnumIgnoreCaseJsonConverter<SidePanelType> { }

/// <summary>
/// PSU efficiency rating (80+ tier).
/// </summary>
[JsonConverter(typeof(PsuEfficiencyJsonConverter))]
public enum PsuEfficiency
{
    Bronze,
    Silver,
    Gold,
    Platinum,
    Titanium
}

internal class PsuEfficiencyJsonConverter : EnumIgnoreCaseJsonConverter<PsuEfficiency> { }

/// <summary>
/// PSU modularity type.
/// </summary>
[JsonConverter(typeof(PsuModularityJsonConverter))]
public enum PsuModularity
{
    NonModular,
    SemiModular,
    FullyModular
}

internal class PsuModularityJsonConverter : EnumIgnoreCaseJsonConverter<PsuModularity> { }

/// <summary>
/// PSU form factor (ATX, SFX, SFX-L).
/// </summary>
[JsonConverter(typeof(PsuFormFactorJsonConverter))]
public enum PsuFormFactor
{
    ATX,
    SFX,
    SFXL
}

internal class PsuFormFactorJsonConverter : EnumIgnoreCaseJsonConverter<PsuFormFactor> { }

/// <summary>
/// Storage device type.
/// </summary>
[JsonConverter(typeof(StorageTypeJsonConverter))]
public enum StorageType
{
    SSD,
    HDD
}

internal class StorageTypeJsonConverter : EnumIgnoreCaseJsonConverter<StorageType> { }

/// <summary>
/// Storage interface type.
/// </summary>
[JsonConverter(typeof(StorageInterfaceJsonConverter))]
public enum StorageInterface
{
    NVMe,
    SATA
}

internal class StorageInterfaceJsonConverter : EnumIgnoreCaseJsonConverter<StorageInterface> { }

/// <summary>
/// Storage form factor.
/// </summary>
[JsonConverter(typeof(StorageFormFactorJsonConverter))]
public enum StorageFormFactor
{
    M2_2280,
    TwoPointFiveInch,
    ThreePointFiveInch
}

internal class StorageFormFactorJsonConverter : EnumIgnoreCaseJsonConverter<StorageFormFactor> { }

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
/// Represents CAS latency (e.g., CL16).
/// </summary>
[JsonConverter(typeof(CasLatencyConverter))]
public record CasLatency
{
    public int Value { get; }

    [JsonConstructor]
    public CasLatency(int value)
    {
        if (value < 1 || value > 50)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "CAS latency must be between 1 and 50");
        }

        Value = value;
    }

    public static CasLatency FromInt(int value) => new(value);
    public static CasLatency Parse(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            throw new ArgumentException("CAS latency string cannot be empty", nameof(s));
        }

        string normalized = s.TrimStart('C', 'L', 'c', 'l');
        if (int.TryParse(normalized, out int val))
        {
            return new(val);
        }

        throw new ArgumentException($"Invalid CAS latency format: {s}. Expected e.g. CL16", nameof(s));
    }

    public static bool TryParse(string? s, out CasLatency? result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(s))
        {
            return false;
        }

        string normalized = s.TrimStart('C', 'L', 'c', 'l');
        if (!int.TryParse(normalized, out int val) || val < 1 || val > 50)
        {
            return false;
        }

        result = new(val);
        return true;
    }

    public override string ToString() => $"CL{Value}";
}

/// <summary>
/// Represents RAM configuration (e.g., 2x16GB).
/// </summary>
[JsonConverter(typeof(RamConfigurationConverter))]
public record RamConfiguration
{
    public int ModuleCount { get; }
    public StorageCapacity ModuleCapacity { get; }

    [JsonConstructor]
    public RamConfiguration(int moduleCount, StorageCapacity moduleCapacity)
    {
        if (moduleCount < 1 || moduleCount > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(moduleCount), "Module count must be between 1 and 8");
        }

        ModuleCount = moduleCount;
        ModuleCapacity = moduleCapacity;
    }

    public static RamConfiguration From(int moduleCount, int capacityGb) =>
        new(moduleCount, StorageCapacity.FromGB(capacityGb));

    public static RamConfiguration Parse(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            throw new ArgumentException("RAM configuration string cannot be empty", nameof(s));
        }

        string[] parts = s.ToUpperInvariant().Split('X', 'x');
        if (parts.Length != 2 ||
            !int.TryParse(parts[0].Trim(), out int count) ||
            !int.TryParse(parts[1].Trim().Replace("GB", "").Trim(), out int gb))
        {
            throw new ArgumentException($"Invalid RAM configuration format: {s}. Expected e.g. 2x16GB", nameof(s));
        }
        return From(count, gb);
    }

    public static bool TryParse(string? s, out RamConfiguration? result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(s))
        {
            return false;
        }

        string[] parts = s.ToUpperInvariant().Split('X', 'x');
        if (parts.Length != 2 ||
            !int.TryParse(parts[0].Trim(), out int count) ||
            count < 1 || count > 8 ||
            !int.TryParse(parts[1].Trim().Replace("GB", "").Trim(), out int gb) ||
            gb < 1)
        {
            return false;
        }
        result = From(count, gb);
        return true;
    }

    public int TotalCapacityGb => ModuleCount * ModuleCapacity.ValueInGB;
    public override string ToString() => $"{ModuleCount}x{ModuleCapacity.ValueInGB}GB";
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

internal abstract class ScalarValueObjectConverter<TRecord, TScalar> : JsonConverter<TRecord>
{
    protected abstract string PropertyName { get; }
    protected abstract TScalar ReadScalar(ref Utf8JsonReader reader);
    protected abstract TScalar GetElementValue(JsonElement element);
    protected abstract TRecord FromScalar(TScalar scalar);
    protected abstract void WriteScalar(Utf8JsonWriter writer, TRecord value);

    public sealed override TRecord Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return FromScalar(ReadScalar(ref reader));
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            if (doc.RootElement.TryGetProperty(PropertyName, out JsonElement el))
            {
                return FromScalar(GetElementValue(el));
            }
        }

        throw new JsonException($"Cannot convert {reader.TokenType} to {typeof(TRecord).Name}");
    }

    public sealed override void Write(Utf8JsonWriter writer, TRecord value, JsonSerializerOptions options)
        => WriteScalar(writer, value);
}

internal class FrequencyConverter : ScalarValueObjectConverter<Frequency, decimal>
{
    protected override string PropertyName => "ValueInGHz";
    protected override decimal ReadScalar(ref Utf8JsonReader reader) => reader.GetDecimal();
    protected override decimal GetElementValue(JsonElement element) => element.GetDecimal();
    protected override Frequency FromScalar(decimal scalar) => Frequency.FromGHz(scalar);
    protected override void WriteScalar(Utf8JsonWriter writer, Frequency value) => writer.WriteNumberValue(value.ValueInGHz);
}

internal class StorageCapacityConverter : ScalarValueObjectConverter<StorageCapacity, int>
{
    protected override string PropertyName => "ValueInGB";
    protected override int ReadScalar(ref Utf8JsonReader reader) => reader.GetInt32();
    protected override int GetElementValue(JsonElement element) => element.GetInt32();
    protected override StorageCapacity FromScalar(int scalar) => StorageCapacity.FromGB(scalar);
    protected override void WriteScalar(Utf8JsonWriter writer, StorageCapacity value) => writer.WriteNumberValue(value.ValueInGB);
}

internal class PowerConverter : ScalarValueObjectConverter<Power, int>
{
    protected override string PropertyName => "ValueInWatts";
    protected override int ReadScalar(ref Utf8JsonReader reader) => reader.GetInt32();
    protected override int GetElementValue(JsonElement element) => element.GetInt32();
    protected override Power FromScalar(int scalar) => Power.FromWatts(scalar);
    protected override void WriteScalar(Utf8JsonWriter writer, Power value) => writer.WriteNumberValue(value.ValueInWatts);
}

internal class VoltageConverter : ScalarValueObjectConverter<Voltage, decimal>
{
    protected override string PropertyName => "ValueInVolts";
    protected override decimal ReadScalar(ref Utf8JsonReader reader) => reader.GetDecimal();
    protected override decimal GetElementValue(JsonElement element) => element.GetDecimal();
    protected override Voltage FromScalar(decimal scalar) => Voltage.FromVolts(scalar);
    protected override void WriteScalar(Utf8JsonWriter writer, Voltage value) => writer.WriteNumberValue(value.ValueInVolts);
}

internal class LengthConverter : ScalarValueObjectConverter<Length, int>
{
    protected override string PropertyName => "ValueInMm";
    protected override int ReadScalar(ref Utf8JsonReader reader) => reader.GetInt32();
    protected override int GetElementValue(JsonElement element) => element.GetInt32();
    protected override Length FromScalar(int scalar) => Length.FromMm(scalar);
    protected override void WriteScalar(Utf8JsonWriter writer, Length value) => writer.WriteNumberValue(value.ValueInMm);
}

internal class DataSpeedConverter : ScalarValueObjectConverter<DataSpeed, int>
{
    protected override string PropertyName => "ValueInMBps";
    protected override int ReadScalar(ref Utf8JsonReader reader) => reader.GetInt32();
    protected override int GetElementValue(JsonElement element) => element.GetInt32();
    protected override DataSpeed FromScalar(int scalar) => DataSpeed.FromMBps(scalar);
    protected override void WriteScalar(Utf8JsonWriter writer, DataSpeed value) => writer.WriteNumberValue(value.ValueInMBps);
}

internal class CasLatencyConverter : JsonConverter<CasLatency>
{
    public override CasLatency Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return CasLatency.FromInt(reader.GetInt32());
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            string? s = reader.GetString();
            if (CasLatency.TryParse(s, out CasLatency? result) && result != null)
            {
                return result;
            }
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            if (doc.RootElement.TryGetProperty("value", out JsonElement valueElement) ||
                doc.RootElement.TryGetProperty("Value", out valueElement))
            {
                return CasLatency.FromInt(valueElement.GetInt32());
            }
        }

        throw new JsonException($"Cannot convert {reader.TokenType} to CasLatency");
    }

    public override void Write(Utf8JsonWriter writer, CasLatency value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

internal class RamConfigurationConverter : JsonConverter<RamConfiguration>
{
    public override RamConfiguration Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string? s = reader.GetString();
            if (RamConfiguration.TryParse(s, out RamConfiguration? result) && result != null)
            {
                return result;
            }
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            int count = 1;
            int gb = 16;
            if (doc.RootElement.TryGetProperty("moduleCount", out JsonElement mc) || doc.RootElement.TryGetProperty("ModuleCount", out mc))
            {
                count = mc.GetInt32();
            }

            if (doc.RootElement.TryGetProperty("moduleCapacity", out JsonElement cap) || doc.RootElement.TryGetProperty("ModuleCapacity", out cap))
            {
                if (cap.ValueKind == JsonValueKind.Number)
                {
                    gb = cap.GetInt32();
                }
                else if (cap.TryGetProperty("valueInGB", out JsonElement gbEl) || cap.TryGetProperty("ValueInGB", out gbEl))
                {
                    gb = gbEl.GetInt32();
                }
            }
            return RamConfiguration.From(count, gb);
        }

        throw new JsonException($"Cannot convert {reader.TokenType} to RamConfiguration");
    }

    public override void Write(Utf8JsonWriter writer, RamConfiguration value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

internal class EnumIgnoreCaseJsonConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string? value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new JsonException($"{typeof(T).Name} value cannot be null or empty");
            }

            if (Enum.TryParse<T>(value, ignoreCase: true, out T parsed))
            {
                return parsed;
            }

            throw new JsonException($"Unrecognized {typeof(T).Name} value: {value}. Expected one of: {string.Join(", ", Enum.GetNames<T>())}");
        }
        if (reader.TokenType == JsonTokenType.Number)
        {
            return (T)(object)reader.GetInt32();
        }

        throw new JsonException($"Cannot convert {reader.TokenType} to {typeof(T).Name}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}