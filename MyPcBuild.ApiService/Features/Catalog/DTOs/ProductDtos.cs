using System.ComponentModel.DataAnnotations;

namespace MyPcBuild.ApiService.Features.Catalog.DTOs;

/// <summary>
/// Base DTO for product requests and responses.
/// </summary>
public abstract record ProductDto
{
    /// <summary>
    /// Product ID (only in responses).
    /// </summary>
    public Guid? Id { get; init; }

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

    /// <summary>
    /// Indicates whether this product is a draft (AI-generated but not yet published).
    /// </summary>
    public bool IsDraft { get; init; }

    /// <summary>
    /// The timestamp when the product was published (null for draft products).
    /// </summary>
    public DateTime? PublishedAt { get; init; }
}

/// <summary>
/// DTO for CPU products (API).
/// </summary>
public record CpuDto : ProductDto
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
    /// Base clock frequency in GHz.
    /// </summary>
    [Required]
    [Range(0.1, 10.0)]
    public required decimal BaseClock { get; init; }

    /// <summary>
    /// Boost clock frequency in GHz.
    /// </summary>
    [Required]
    [Range(0.1, 10.0)]
    public required decimal BoostClock { get; init; }

    /// <summary>
    /// Thermal Design Power in watts.
    /// </summary>
    [Required]
    [Range(1, 1000)]
    public required int TDP { get; init; }

    /// <summary>
    /// Whether the CPU has integrated graphics.
    /// </summary>
    [Required]
    public required bool IntegratedGraphics { get; init; }
}

/// <summary>
/// DTO for motherboard products (API).
/// </summary>
public record MotherboardDto : ProductDto
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
    /// Maximum memory capacity in GB.
    /// </summary>
    [Required]
    [Range(1, 2048)]
    public required int MaxMemory { get; init; }

    /// <summary>
    /// Physical dimensions in millimeters.
    /// </summary>
    [Required]
    public required ApiDimensions Dimensions { get; init; }

    /// <summary>
    /// Installation slots (optional, usually empty for AI-generated products).
    /// </summary>
    public List<ApiSlot>? Slots { get; init; }
}

/// <summary>
/// DTO for GPU products (API).
/// </summary>
public record GpuDto : ProductDto
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
    /// Video RAM capacity in GB.
    /// </summary>
    [Required]
    [Range(1, 256)]
    public required int VRAM { get; init; }

    /// <summary>
    /// Memory type.
    /// </summary>
    [Required]
    public required ApiMemoryType MemoryType { get; init; }

    /// <summary>
    /// Core clock frequency in MHz.
    /// </summary>
    [Required]
    [Range(100, 5000)]
    public required int CoreClock { get; init; }

    /// <summary>
    /// Boost clock frequency in MHz.
    /// </summary>
    [Required]
    [Range(100, 5000)]
    public required int BoostClock { get; init; }

    /// <summary>
    /// Thermal Design Power in watts.
    /// </summary>
    [Required]
    [Range(1, 1000)]
    public required int TDP { get; init; }

    /// <summary>
    /// GPU length in millimeters.
    /// </summary>
    [Required]
    [Range(1, 1000)]
    public required int Length { get; init; }

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
    public required ApiDimensions Dimensions { get; init; }

    /// <summary>
    /// Installation slots (optional, usually empty for AI-generated products).
    /// </summary>
    public List<ApiSlot>? Slots { get; init; }
}

/// <summary>
/// DTO for RAM products (API).
/// </summary>
public record RamDto : ProductDto
{
    /// <summary>
    /// Memory type.
    /// </summary>
    [Required]
    public required ApiMemoryType Type { get; init; }

    /// <summary>
    /// Total capacity in GB.
    /// </summary>
    [Required]
    [Range(1, 512)]
    public required int Capacity { get; init; }

    /// <summary>
    /// Configuration description (e.g., "2x16GB").
    /// </summary>
    [Required]
    public required string Configuration { get; init; }

    /// <summary>
    /// Memory speed in MHz.
    /// </summary>
    [Required]
    [Range(800, 10000)]
    public required int Speed { get; init; }

    /// <summary>
    /// CAS latency (e.g., "CL16").
    /// </summary>
    [Required]
    public required string CASLatency { get; init; }

    /// <summary>
    /// Operating voltage in volts.
    /// </summary>
    [Required]
    [Range(0.5, 3.0)]
    public required decimal Voltage { get; init; }
}

/// <summary>
/// DTO for PC case products (API).
/// </summary>
public record PcCaseDto : ProductDto
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
    public required ApiDimensions Dimensions { get; init; }

    /// <summary>
    /// Internal chambers (optional, usually empty for AI-generated products).
    /// </summary>
    public List<ApiChamber>? Chambers { get; init; }
}

/// <summary>
/// DTO for PSU products (API).
/// </summary>
public record PsuDto : ProductDto
{
    /// <summary>
    /// Power rating in watts.
    /// </summary>
    [Required]
    [Range(200, 3000)]
    public required int Wattage { get; init; }

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
    /// PSU length in millimeters.
    /// </summary>
    [Required]
    [Range(1, 500)]
    public required int Length { get; init; }

    /// <summary>
    /// Number of PCIe 8-pin connectors.
    /// </summary>
    [Required]
    [Range(0, 16)]
    public required int PCIe8Pin { get; init; }
}

/// <summary>
/// DTO for storage products (API).
/// </summary>
public record StorageDto : ProductDto
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
    /// Storage capacity in GB.
    /// </summary>
    [Required]
    [Range(1, 100000)]
    public required int Capacity { get; init; }

    /// <summary>
    /// Read speed in MB/s.
    /// </summary>
    [Required]
    [Range(1, 20000)]
    public required int ReadSpeed { get; init; }

    /// <summary>
    /// Write speed in MB/s.
    /// </summary>
    [Required]
    [Range(1, 20000)]
    public required int WriteSpeed { get; init; }
}

/// <summary>
/// DTO for cooler products (API).
/// </summary>
public record CoolerDto : ProductDto
{
    /// <summary>
    /// Cooler type.
    /// </summary>
    [Required]
    public required ApiCoolerType CoolerType { get; init; }

    /// <summary>
    /// Cooler height in millimeters.
    /// </summary>
    [Required]
    [Range(1, 500)]
    public required int Height { get; init; }

    /// <summary>
    /// Thermal Design Power rating in watts.
    /// </summary>
    [Required]
    [Range(1, 1000)]
    public required int TDP { get; init; }

    /// <summary>
    /// Compatible CPU sockets.
    /// </summary>
    [Required]
    public required List<ApiCpuSocket> Sockets { get; init; }

    /// <summary>
    /// Physical dimensions in millimeters.
    /// </summary>
    [Required]
    public required ApiDimensions Dimensions { get; init; }
}

/// <summary>
/// Physical dimensions (API).
/// </summary>
public record ApiDimensions
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
/// Installation slot (API).
/// </summary>
public record ApiSlot
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
    public ApiVector3? Location { get; init; }
}

/// <summary>
/// Internal chamber (API).
/// </summary>
public record ApiChamber
{
    /// <summary>
    /// Chamber name.
    /// </summary>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    /// Chamber dimensions.
    /// </summary>
    [Required]
    public required ApiDimensions Dimensions { get; init; }
}

/// <summary>
/// 3D vector (API).
/// </summary>
public record ApiVector3
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
