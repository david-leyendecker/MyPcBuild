using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using MyPcBuild.ApiService.Catalog.Models;

namespace MyPcBuild.ApiService.Catalog.DTOs;

/// <summary>
/// Base request for creating/updating products.
/// </summary>
public abstract record ProductRequest
{
    /// <summary>
    /// Product category.
    /// </summary>
    [Required]
    public required ProductCategory Category { get; init; }

    /// <summary>
    /// Product name.
    /// </summary>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    /// Product price in USD.
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue)]
    public required decimal Price { get; init; }

    /// <summary>
    /// Manufacturer name.
    /// </summary>
    [Required]
    public required string Manufacturer { get; init; }
}

/// <summary>
/// Base response for product queries.
/// </summary>
[JsonDerivedType(typeof(CpuProductResponse), "cpu")]
[JsonDerivedType(typeof(GpuProductResponse), "gpu")]
[JsonDerivedType(typeof(MotherboardProductResponse), "motherboard")]
[JsonDerivedType(typeof(RamProductResponse), "ram")]
[JsonDerivedType(typeof(StorageProductResponse), "storage")]
[JsonDerivedType(typeof(PsuProductResponse), "psu")]
[JsonDerivedType(typeof(CoolerProductResponse), "cooler")]
[JsonDerivedType(typeof(PcCaseProductResponse), "case")]
public abstract record ProductResponse
{
    /// <summary>
    /// Product ID.
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    /// Product category.
    /// </summary>
    [Required]
    public required ProductCategory Category { get; init; }

    /// <summary>
    /// Product name.
    /// </summary>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    /// Product price in USD.
    /// </summary>
    [Required]
    public required decimal Price { get; init; }

    /// <summary>
    /// Manufacturer name.
    /// </summary>
    [Required]
    public required string Manufacturer { get; init; }

    /// <summary>
    /// Indicates whether this product is a draft (AI-generated but not yet published).
    /// </summary>
    public bool IsDraft { get; init; }

    /// <summary>
    /// The timestamp when the product was published (null for draft products).
    /// </summary>
    public DateTime? PublishedAt { get; init; }
}

// ========== CPU Product ==========

/// <summary>
/// Request for creating/updating CPU products.
/// </summary>
public record CpuProductRequest : ProductRequest
{
    /// <summary>
    /// CPU socket type.
    /// </summary>
    [Required]
    public required ApiCpuSocket Socket { get; init; }

    /// <summary>
    /// Number of CPU cores.
    /// </summary>
    [Required]
    [Range(1, 128)]
    public required int Cores { get; init; }

    /// <summary>
    /// Number of CPU threads.
    /// </summary>
    [Required]
    [Range(1, 256)]
    public required int Threads { get; init; }

    /// <summary>
    /// Base clock frequency.
    /// </summary>
    [Required]
    public required ApiFrequency BaseClock { get; init; }

    /// <summary>
    /// Boost clock frequency.
    /// </summary>
    [Required]
    public required ApiFrequency BoostClock { get; init; }

    /// <summary>
    /// Thermal Design Power.
    /// </summary>
    [Required]
    public required ApiPower TDP { get; init; }

    /// <summary>
    /// Whether the CPU has integrated graphics.
    /// </summary>
    [Required]
    public required bool IntegratedGraphics { get; init; }
}

/// <summary>
/// Response for CPU product queries.
/// </summary>
public record CpuProductResponse : ProductResponse
{
    /// <summary>
    /// CPU socket type.
    /// </summary>
    [Required]
    public required ApiCpuSocket Socket { get; init; }

    /// <summary>
    /// Number of CPU cores.
    /// </summary>
    [Required]
    public required int Cores { get; init; }

    /// <summary>
    /// Number of CPU threads.
    /// </summary>
    [Required]
    public required int Threads { get; init; }

    /// <summary>
    /// Base clock frequency.
    /// </summary>
    [Required]
    public required ApiFrequency BaseClock { get; init; }

    /// <summary>
    /// Boost clock frequency.
    /// </summary>
    [Required]
    public required ApiFrequency BoostClock { get; init; }

    /// <summary>
    /// Thermal Design Power.
    /// </summary>
    [Required]
    public required ApiPower TDP { get; init; }

    /// <summary>
    /// Whether the CPU has integrated graphics.
    /// </summary>
    [Required]
    public required bool IntegratedGraphics { get; init; }
}

// ========== Motherboard Product ==========

/// <summary>
/// Request for creating/updating motherboard products.
/// </summary>
public record MotherboardProductRequest : ProductRequest
{
    /// <summary>
    /// CPU socket type.
    /// </summary>
    [Required]
    public required ApiCpuSocket Socket { get; init; }

    /// <summary>
    /// Chipset name.
    /// </summary>
    [Required]
    public required string Chipset { get; init; }

    /// <summary>
    /// Motherboard form factor.
    /// </summary>
    [Required]
    public required ApiFormFactor FormFactor { get; init; }

    /// <summary>
    /// Memory type supported.
    /// </summary>
    [Required]
    public required ApiMemoryType MemoryType { get; init; }

    /// <summary>
    /// Maximum memory capacity.
    /// </summary>
    [Required]
    public required ApiStorageCapacity MaxMemory { get; init; }

    /// <summary>
    /// Physical dimensions in millimeters.
    /// </summary>
    [Required]
    public required DimensionsModel Dimensions { get; init; }

    /// <summary>
    /// Installation slots (optional, usually empty for AI-generated products).
    /// </summary>
    public List<SlotModel>? Slots { get; init; }
}

/// <summary>
/// Response for motherboard product queries.
/// </summary>
public record MotherboardProductResponse : ProductResponse
{
    /// <summary>
    /// CPU socket type.
    /// </summary>
    [Required]
    public required ApiCpuSocket Socket { get; init; }

    /// <summary>
    /// Chipset name.
    /// </summary>
    [Required]
    public required string Chipset { get; init; }

    /// <summary>
    /// Motherboard form factor.
    /// </summary>
    [Required]
    public required ApiFormFactor FormFactor { get; init; }

    /// <summary>
    /// Memory type supported.
    /// </summary>
    [Required]
    public required ApiMemoryType MemoryType { get; init; }

    /// <summary>
    /// Maximum memory capacity.
    /// </summary>
    [Required]
    public required ApiStorageCapacity MaxMemory { get; init; }

    /// <summary>
    /// Physical dimensions in millimeters.
    /// </summary>
    [Required]
    public required DimensionsModel Dimensions { get; init; }

    /// <summary>
    /// Installation slots.
    /// </summary>
    public List<SlotModel>? Slots { get; init; }
}

// ========== GPU Product ==========

/// <summary>
/// Request for creating/updating GPU products.
/// </summary>
public record GpuProductRequest : ProductRequest
{
    /// <summary>
    /// GPU chipset manufacturer (NVIDIA, AMD, Intel).
    /// </summary>
    [Required]
    public required string ChipsetManufacturer { get; init; }

    /// <summary>
    /// GPU series name.
    /// </summary>
    [Required]
    public required string Series { get; init; }

    /// <summary>
    /// Video RAM capacity.
    /// </summary>
    [Required]
    public required ApiStorageCapacity VRAM { get; init; }

    /// <summary>
    /// Memory type.
    /// </summary>
    [Required]
    public required ApiMemoryType MemoryType { get; init; }

    /// <summary>
    /// Core clock frequency.
    /// </summary>
    [Required]
    public required ApiFrequency CoreClock { get; init; }

    /// <summary>
    /// Boost clock frequency.
    /// </summary>
    [Required]
    public required ApiFrequency BoostClock { get; init; }

    /// <summary>
    /// Thermal Design Power.
    /// </summary>
    [Required]
    public required ApiPower TDP { get; init; }

    /// <summary>
    /// GPU length.
    /// </summary>
    [Required]
    public required ApiLength Length { get; init; }

    /// <summary>
    /// Power connector configuration.
    /// </summary>
    [Required]
    public required ApiGpuPowerConnector PowerConnectors { get; init; }

    /// <summary>
    /// Whether the GPU supports ray tracing.
    /// </summary>
    [Required]
    public required bool RayTracing { get; init; }

    /// <summary>
    /// Physical dimensions in millimeters.
    /// </summary>
    [Required]
    public required DimensionsModel Dimensions { get; init; }

    /// <summary>
    /// Installation slots (optional, usually empty for AI-generated products).
    /// </summary>
    public List<SlotModel>? Slots { get; init; }
}

/// <summary>
/// Response for GPU product queries.
/// </summary>
public record GpuProductResponse : ProductResponse
{
    /// <summary>
    /// GPU chipset manufacturer (NVIDIA, AMD, Intel).
    /// </summary>
    [Required]
    public required string ChipsetManufacturer { get; init; }

    /// <summary>
    /// GPU series name.
    /// </summary>
    [Required]
    public required string Series { get; init; }

    /// <summary>
    /// Video RAM capacity.
    /// </summary>
    [Required]
    public required ApiStorageCapacity VRAM { get; init; }

    /// <summary>
    /// Memory type.
    /// </summary>
    [Required]
    public required ApiMemoryType MemoryType { get; init; }

    /// <summary>
    /// Core clock frequency.
    /// </summary>
    [Required]
    public required ApiFrequency CoreClock { get; init; }

    /// <summary>
    /// Boost clock frequency.
    /// </summary>
    [Required]
    public required ApiFrequency BoostClock { get; init; }

    /// <summary>
    /// Thermal Design Power.
    /// </summary>
    [Required]
    public required ApiPower TDP { get; init; }

    /// <summary>
    /// GPU length.
    /// </summary>
    [Required]
    public required ApiLength Length { get; init; }

    /// <summary>
    /// Power connector configuration.
    /// </summary>
    [Required]
    public required ApiGpuPowerConnector PowerConnectors { get; init; }

    /// <summary>
    /// Whether the GPU supports ray tracing.
    /// </summary>
    [Required]
    public required bool RayTracing { get; init; }

    /// <summary>
    /// Physical dimensions in millimeters.
    /// </summary>
    [Required]
    public required DimensionsModel Dimensions { get; init; }

    /// <summary>
    /// Installation slots.
    /// </summary>
    public List<SlotModel>? Slots { get; init; }
}

// ========== RAM Product ==========

/// <summary>
/// Request for creating/updating RAM products.
/// </summary>
public record RamProductRequest : ProductRequest
{
    /// <summary>
    /// Memory type.
    /// </summary>
    [Required]
    public required ApiMemoryType Type { get; init; }

    /// <summary>
    /// Total capacity.
    /// </summary>
    [Required]
    public required ApiStorageCapacity Capacity { get; init; }

    /// <summary>
    /// Configuration description (e.g., "2x16GB").
    /// </summary>
    [Required]
    public required string Configuration { get; init; }

    /// <summary>
    /// Memory speed.
    /// </summary>
    [Required]
    public required ApiFrequency Speed { get; init; }

    /// <summary>
    /// CAS latency (e.g., "CL16").
    /// </summary>
    [Required]
    public required string CASLatency { get; init; }

    /// <summary>
    /// Operating voltage.
    /// </summary>
    [Required]
    public required ApiVoltage Voltage { get; init; }
}

/// <summary>
/// Response for RAM product queries.
/// </summary>
public record RamProductResponse : ProductResponse
{
    /// <summary>
    /// Memory type.
    /// </summary>
    [Required]
    public required ApiMemoryType Type { get; init; }

    /// <summary>
    /// Total capacity.
    /// </summary>
    [Required]
    public required ApiStorageCapacity Capacity { get; init; }

    /// <summary>
    /// Configuration description (e.g., "2x16GB").
    /// </summary>
    [Required]
    public required string Configuration { get; init; }

    /// <summary>
    /// Memory speed.
    /// </summary>
    [Required]
    public required ApiFrequency Speed { get; init; }

    /// <summary>
    /// CAS latency (e.g., "CL16").
    /// </summary>
    [Required]
    public required string CASLatency { get; init; }

    /// <summary>
    /// Operating voltage.
    /// </summary>
    [Required]
    public required ApiVoltage Voltage { get; init; }
}

// ========== PC Case Product ==========

/// <summary>
/// Request for creating/updating PC case products.
/// </summary>
public record PcCaseProductRequest : ProductRequest
{
    /// <summary>
    /// Case form factor description.
    /// </summary>
    [Required]
    public required string FormFactor { get; init; }

    /// <summary>
    /// Case color.
    /// </summary>
    [Required]
    public required string Color { get; init; }

    /// <summary>
    /// Side panel window type (e.g., "Tempered Glass", "Acrylic", "None").
    /// </summary>
    [Required]
    public required string SidePanelWindow { get; init; }

    /// <summary>
    /// Physical dimensions in millimeters.
    /// </summary>
    [Required]
    public required DimensionsModel Dimensions { get; init; }

    /// <summary>
    /// Internal chambers (optional, usually empty for AI-generated products).
    /// </summary>
    public List<ChamberModel>? Chambers { get; init; }
}

/// <summary>
/// Response for PC case product queries.
/// </summary>
public record PcCaseProductResponse : ProductResponse
{
    /// <summary>
    /// Case form factor description.
    /// </summary>
    [Required]
    public required string FormFactor { get; init; }

    /// <summary>
    /// Case color.
    /// </summary>
    [Required]
    public required string Color { get; init; }

    /// <summary>
    /// Side panel window type (e.g., "Tempered Glass", "Acrylic", "None").
    /// </summary>
    [Required]
    public required string SidePanelWindow { get; init; }

    /// <summary>
    /// Physical dimensions in millimeters.
    /// </summary>
    [Required]
    public required DimensionsModel Dimensions { get; init; }

    /// <summary>
    /// Internal chambers.
    /// </summary>
    public List<ChamberModel>? Chambers { get; init; }
}

// ========== PSU Product ==========

/// <summary>
/// Request for creating/updating PSU products.
/// </summary>
public record PsuProductRequest : ProductRequest
{
    /// <summary>
    /// Power rating.
    /// </summary>
    [Required]
    public required ApiPower Wattage { get; init; }

    /// <summary>
    /// Efficiency rating (e.g., "80+ Gold").
    /// </summary>
    [Required]
    public required string Efficiency { get; init; }

    /// <summary>
    /// Modularity type (e.g., "Fully Modular", "Semi-Modular", "Non-Modular").
    /// </summary>
    [Required]
    public required string Modular { get; init; }

    /// <summary>
    /// PSU form factor (e.g., "ATX", "SFX").
    /// </summary>
    [Required]
    public required string FormFactor { get; init; }

    /// <summary>
    /// PSU length.
    /// </summary>
    [Required]
    public required ApiLength Length { get; init; }

    /// <summary>
    /// Number of PCIe 8-pin connectors.
    /// </summary>
    [Required]
    [Range(0, 16)]
    public required int PCIe8Pin { get; init; }
}

/// <summary>
/// Response for PSU product queries.
/// </summary>
public record PsuProductResponse : ProductResponse
{
    /// <summary>
    /// Power rating.
    /// </summary>
    [Required]
    public required ApiPower Wattage { get; init; }

    /// <summary>
    /// Efficiency rating (e.g., "80+ Gold").
    /// </summary>
    [Required]
    public required string Efficiency { get; init; }

    /// <summary>
    /// Modularity type (e.g., "Fully Modular", "Semi-Modular", "Non-Modular").
    /// </summary>
    [Required]
    public required string Modular { get; init; }

    /// <summary>
    /// PSU form factor (e.g., "ATX", "SFX").
    /// </summary>
    [Required]
    public required string FormFactor { get; init; }

    /// <summary>
    /// PSU length.
    /// </summary>
    [Required]
    public required ApiLength Length { get; init; }

    /// <summary>
    /// Number of PCIe 8-pin connectors.
    /// </summary>
    [Required]
    public required int PCIe8Pin { get; init; }
}

// ========== Storage Product ==========

/// <summary>
/// Request for creating/updating storage products.
/// </summary>
public record StorageProductRequest : ProductRequest
{
    /// <summary>
    /// Storage type (e.g., "SSD", "HDD").
    /// </summary>
    [Required]
    public required string Type { get; init; }

    /// <summary>
    /// Interface type (e.g., "NVMe", "SATA").
    /// </summary>
    [Required]
    public required string Interface { get; init; }

    /// <summary>
    /// Storage form factor (e.g., "M.2 2280", "2.5 inch").
    /// </summary>
    [Required]
    public required string StorageFormFactor { get; init; }

    /// <summary>
    /// Storage capacity.
    /// </summary>
    [Required]
    public required ApiStorageCapacity Capacity { get; init; }

    /// <summary>
    /// Read speed.
    /// </summary>
    [Required]
    public required ApiDataSpeed ReadSpeed { get; init; }

    /// <summary>
    /// Write speed.
    /// </summary>
    [Required]
    public required ApiDataSpeed WriteSpeed { get; init; }
}

/// <summary>
/// Response for storage product queries.
/// </summary>
public record StorageProductResponse : ProductResponse
{
    /// <summary>
    /// Storage type (e.g., "SSD", "HDD").
    /// </summary>
    [Required]
    public required string Type { get; init; }

    /// <summary>
    /// Interface type (e.g., "NVMe", "SATA").
    /// </summary>
    [Required]
    public required string Interface { get; init; }

    /// <summary>
    /// Storage form factor (e.g., "M.2 2280", "2.5 inch").
    /// </summary>
    [Required]
    public required string StorageFormFactor { get; init; }

    /// <summary>
    /// Storage capacity.
    /// </summary>
    [Required]
    public required ApiStorageCapacity Capacity { get; init; }

    /// <summary>
    /// Read speed.
    /// </summary>
    [Required]
    public required ApiDataSpeed ReadSpeed { get; init; }

    /// <summary>
    /// Write speed.
    /// </summary>
    [Required]
    public required ApiDataSpeed WriteSpeed { get; init; }
}

// ========== Cooler Product ==========

/// <summary>
/// Request for creating/updating cooler products.
/// </summary>
public record CoolerProductRequest : ProductRequest
{
    /// <summary>
    /// Cooler type.
    /// </summary>
    [Required]
    public required ApiCoolerType CoolerType { get; init; }

    /// <summary>
    /// Cooler height.
    /// </summary>
    [Required]
    public required ApiLength Height { get; init; }

    /// <summary>
    /// Thermal Design Power rating.
    /// </summary>
    [Required]
    public required ApiPower TDP { get; init; }

    /// <summary>
    /// Compatible CPU sockets.
    /// </summary>
    [Required]
    public required List<ApiCpuSocket> Sockets { get; init; }

    /// <summary>
    /// Physical dimensions in millimeters.
    /// </summary>
    [Required]
    public required DimensionsModel Dimensions { get; init; }
}

/// <summary>
/// Response for cooler product queries.
/// </summary>
public record CoolerProductResponse : ProductResponse
{
    /// <summary>
    /// Cooler type.
    /// </summary>
    [Required]
    public required ApiCoolerType CoolerType { get; init; }

    /// <summary>
    /// Cooler height.
    /// </summary>
    [Required]
    public required ApiLength Height { get; init; }

    /// <summary>
    /// Thermal Design Power rating.
    /// </summary>
    [Required]
    public required ApiPower TDP { get; init; }

    /// <summary>
    /// Compatible CPU sockets.
    /// </summary>
    [Required]
    public required List<ApiCpuSocket> Sockets { get; init; }

    /// <summary>
    /// Physical dimensions in millimeters.
    /// </summary>
    [Required]
    public required DimensionsModel Dimensions { get; init; }
}

// ========== Supporting Models ==========

/// <summary>
/// Physical dimensions model.
/// </summary>
public record DimensionsModel
{
    /// <summary>
    /// Length in millimeters.
    /// </summary>
    [Required]
    [Range(0.1, 10000)]
    public required decimal Length { get; init; }

    /// <summary>
    /// Width in millimeters.
    /// </summary>
    [Required]
    [Range(0.1, 10000)]
    public required decimal Width { get; init; }

    /// <summary>
    /// Height in millimeters.
    /// </summary>
    [Required]
    [Range(0.1, 10000)]
    public required decimal Height { get; init; }
}

/// <summary>
/// JSON converter for DimensionsModel that handles various input formats (object, string).
/// </summary>
internal class DimensionsModelConverter : JsonConverter<DimensionsModel>
{
    public override DimensionsModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            decimal length = 0, width = 0, height = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString()!;
                    reader.Read();

                    decimal value = reader.TokenType == JsonTokenType.Number
                        ? reader.GetDecimal()
                        : 0;

                    if (propertyName.Equals(nameof(DimensionsModel.Length), StringComparison.OrdinalIgnoreCase))
                    {
                        length = value;
                    }
                    else if (propertyName.Equals(nameof(DimensionsModel.Width), StringComparison.OrdinalIgnoreCase))
                    {
                        width = value;
                    }
                    else if (propertyName.Equals(nameof(DimensionsModel.Height), StringComparison.OrdinalIgnoreCase))
                    {
                        height = value;
                    }
                }
            }

            return new DimensionsModel { Length = length, Width = width, Height = height };
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            string? str = reader.GetString();
            if (!string.IsNullOrEmpty(str))
            {
                string[] parts = str.Split(',');
                if (parts.Length == 3 &&
                    decimal.TryParse(parts[0].Trim(), out decimal length) &&
                    decimal.TryParse(parts[1].Trim(), out decimal width) &&
                    decimal.TryParse(parts[2].Trim(), out decimal height))
                {
                    return new DimensionsModel { Length = length, Width = width, Height = height };
                }
            }
        }

        throw new JsonException("Invalid dimensions format");
    }

    public override void Write(Utf8JsonWriter writer, DimensionsModel value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber(nameof(DimensionsModel.Length), value.Length);
        writer.WriteNumber(nameof(DimensionsModel.Width), value.Width);
        writer.WriteNumber(nameof(DimensionsModel.Height), value.Height);
        writer.WriteEndObject();
    }
}

/// <summary>
/// Installation slot model.
/// </summary>
public record SlotModel
{
    /// <summary>
    /// Slot name.
    /// </summary>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    /// Allowed product category name.
    /// </summary>
    [Required]
    public required string AllowedCategory { get; init; }

    /// <summary>
    /// Relative position.
    /// </summary>
    [Required]
    public required Vector3Model RelativePosition { get; init; }

    /// <summary>
    /// Maximum dimensions that can fit in this slot.
    /// </summary>
    [Required]
    public required DimensionsModel MaxDimensions { get; init; }

    /// <summary>
    /// Rotation to apply to parts placed in this slot (optional).
    /// </summary>
    public RotationModel? Rotation { get; init; }

    /// <summary>
    /// Sub-slots (optional, for nested slot hierarchies).
    /// </summary>
    public List<SlotModel>? SubSlots { get; init; }
}

/// <summary>
/// JSON converter for SlotModel lists.
/// For AI-generated products, returns an empty list since spatial data is not provided by the AI.
/// </summary>
internal class SlotModelListConverter : JsonConverter<List<SlotModel>?>
{
    public override List<SlotModel>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Skip the value (could be array or string like "[]")
        reader.Skip();
        // Return empty list for AI-generated draft products (spatial data not provided)
        return [];
    }

    public override void Write(Utf8JsonWriter writer, List<SlotModel>? value, JsonSerializerOptions options)
    {
        if (value == null || value.Count == 0)
        {
            writer.WriteStartArray();
            writer.WriteEndArray();
        }
        else
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}

/// <summary>
/// Internal chamber model.
/// </summary>
public record ChamberModel
{
    /// <summary>
    /// Chamber name.
    /// </summary>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    /// Relative position of the chamber within its parent (e.g., PC case).
    /// </summary>
    [Required]
    public required Vector3Model RelativePosition { get; init; }

    /// <summary>
    /// Chamber dimensions.
    /// </summary>
    [Required]
    public required DimensionsModel Dimensions { get; init; }

    /// <summary>
    /// Slots within this chamber.
    /// </summary>
    [Required]
    public required List<SlotModel> Slots { get; init; }
}

/// <summary>
/// JSON converter for ChamberModel lists.
/// For AI-generated products, returns an empty list since spatial data is not provided by the AI.
/// </summary>
internal class ChamberModelListConverter : JsonConverter<List<ChamberModel>?>
{
    public override List<ChamberModel>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Skip the value (could be array or nested object)
        reader.Skip();
        // Return empty list for AI-generated draft products (spatial data not provided)
        return [];
    }

    public override void Write(Utf8JsonWriter writer, List<ChamberModel>? value, JsonSerializerOptions options)
    {
        if (value == null || value.Count == 0)
        {
            writer.WriteStartArray();
            writer.WriteEndArray();
        }
        else
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}

/// <summary>
/// 3D vector model.
/// </summary>
public record Vector3Model
{
    /// <summary>
    /// X coordinate.
    /// </summary>
    [Required]
    public required decimal X { get; init; }

    /// <summary>
    /// Y coordinate.
    /// </summary>
    [Required]
    public required decimal Y { get; init; }

    /// <summary>
    /// Z coordinate.
    /// </summary>
    [Required]
    public required decimal Z { get; init; }
}

/// <summary>
/// 3D rotation model using Euler angles (in degrees).
/// </summary>
public record RotationModel
{
    /// <summary>
    /// X rotation (pitch) in degrees.
    /// </summary>
    public decimal X { get; init; }

    /// <summary>
    /// Y rotation (yaw) in degrees.
    /// </summary>
    public decimal Y { get; init; }

    /// <summary>
    /// Z rotation (roll) in degrees.
    /// </summary>
    public decimal Z { get; init; }
}
