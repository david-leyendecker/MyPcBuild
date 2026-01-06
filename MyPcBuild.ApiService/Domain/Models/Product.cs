using MyPcBuild.ApiService.Domain.Models.Spatial;

namespace MyPcBuild.ApiService.Domain.Models;

/// <summary>
/// Base product record with common properties for all product types.
/// </summary>
public abstract record Product(
    Guid Id,
    string Name,
    ProductCategory Category,
    decimal Price,
    string Manufacturer,
    Dictionary<string, object> Specifications
);

/// <summary>
/// Marker interface for products with physical dimensions.
/// </summary>
public interface ISpatialProduct
{
    Dimensions Dimensions { get; }
}

/// <summary>
/// Marker interface for products that provide installation slots.
/// </summary>
public interface ISlottedProduct : ISpatialProduct
{
    List<Slot> Slots { get; }
}

/// <summary>
/// Marker interface for products with internal chambers.
/// </summary>
public interface IChamberedProduct : ISpatialProduct
{
    List<Chamber> Chambers { get; }
}

// Concrete product types by category

public record CpuProduct(
    Guid Id,
    string Name,
    decimal Price,
    string Manufacturer,
    Dictionary<string, object> Specifications,
    // CPU-specific properties
    string Socket,
    int Cores,
    int Threads,
    string BaseClock,
    string BoostClock,
    int TDP,
    bool IntegratedGraphics
) : Product(Id, Name, ProductCategory.CPU, Price, Manufacturer, Specifications);

public record MotherboardProduct(
    Guid Id,
    string Name,
    decimal Price,
    string Manufacturer,
    Dictionary<string, object> Specifications,
    Dimensions Dimensions,
    List<Slot> Slots,
    // Motherboard-specific properties
    string Socket,
    string Chipset,
    string FormFactor,
    string MemoryType,
    int MaxMemory,
    int MemorySlots,
    int PCIeSlots,
    int M2Slots
) : Product(Id, Name, ProductCategory.Motherboard, Price, Manufacturer, Specifications), ISlottedProduct;

public record GpuProduct(
    Guid Id,
    string Name,
    decimal Price,
    string Manufacturer,
    Dictionary<string, object> Specifications,
    Dimensions Dimensions,
    List<Slot> Slots,
    // GPU-specific properties
    string ChipsetManufacturer,
    string Series,
    int VRAM,
    string MemoryType,
    int CoreClock,
    int BoostClock,
    int TDP,
    int Length,
    int PCIeSlots,
    string PowerConnectors,
    bool RayTracing
) : Product(Id, Name, ProductCategory.GPU, Price, Manufacturer, Specifications), ISlottedProduct;

public record RamProduct(
    Guid Id,
    string Name,
    decimal Price,
    string Manufacturer,
    Dictionary<string, object> Specifications,
    // RAM-specific properties
    string Type,
    int Capacity,
    string Configuration,
    int Speed,
    string CASLatency,
    double Voltage
) : Product(Id, Name, ProductCategory.RAM, Price, Manufacturer, Specifications);

public record PcCaseProduct(
    Guid Id,
    string Name,
    decimal Price,
    string Manufacturer,
    Dictionary<string, object> Specifications,
    Dimensions Dimensions,
    List<Chamber> Chambers,
    // PC Case-specific properties
    string FormFactor,
    string Color,
    string SidePanelWindow,
    int MaxGPULength,
    int MaxCPUCoolerHeight,
    int MaxPSULength
) : Product(Id, Name, ProductCategory.PCCase, Price, Manufacturer, Specifications), IChamberedProduct;

public record PsuProduct(
    Guid Id,
    string Name,
    decimal Price,
    string Manufacturer,
    Dictionary<string, object> Specifications,
    // PSU-specific properties
    int Wattage,
    string Efficiency,
    string Modular,
    string FormFactor,
    int Length,
    int PCIe8Pin
) : Product(Id, Name, ProductCategory.PSU, Price, Manufacturer, Specifications);

public record StorageProduct(
    Guid Id,
    string Name,
    decimal Price,
    string Manufacturer,
    Dictionary<string, object> Specifications,
    // Storage-specific properties
    string Type,
    string Interface,
    string StorageFormFactor,
    int Capacity,
    int ReadSpeed,
    int WriteSpeed
) : Product(Id, Name, ProductCategory.Storage, Price, Manufacturer, Specifications);

public record CoolerProduct(
    Guid Id,
    string Name,
    decimal Price,
    string Manufacturer,
    Dictionary<string, object> Specifications,
    Dimensions Dimensions,
    // Cooler-specific properties
    string CoolerType,
    int Height,
    int TDP,
    string[] Sockets
) : Product(Id, Name, ProductCategory.Cooler, Price, Manufacturer, Specifications), ISpatialProduct;

public enum ProductCategory
{
    CPU,
    Motherboard,
    GPU,
    RAM,
    PCCase,
    PSU,
    Storage,
    Cooler
}
