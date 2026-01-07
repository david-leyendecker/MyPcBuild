using MyPcBuild.ApiService.Domain.Models.Spatial;

namespace MyPcBuild.ApiService.Domain.Models;

/// <summary>
/// Base product record with common properties for all product types.
/// </summary>
public abstract record Product(
    Guid Id,
    string Name,
    decimal Price,
    string Manufacturer
)
{
    /// <summary>
    /// Gets the category name for this product type.
    /// </summary>
    public abstract string CategoryName { get; }
};

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
    // CPU-specific properties
    CpuSocket Socket,
    int Cores,
    int Threads,
    Frequency BaseClock,
    Frequency BoostClock,
    Power TDP,
    bool IntegratedGraphics
) : Product(Id, Name, Price, Manufacturer)
{
    public override string CategoryName => "CPU";
};

public record MotherboardProduct(
    Guid Id,
    string Name,
    decimal Price,
    string Manufacturer,
    Dimensions Dimensions,
    List<Slot> Slots,
    // Motherboard-specific properties
    CpuSocket Socket,
    string Chipset,
    FormFactor FormFactor,
    MemoryType MemoryType,
    StorageCapacity MaxMemory
) : Product(Id, Name, Price, Manufacturer), ISlottedProduct
{
    public override string CategoryName => "Motherboard";
};

public record GpuProduct(
    Guid Id,
    string Name,
    decimal Price,
    string Manufacturer,
    Dimensions Dimensions,
    List<Slot> Slots,
    // GPU-specific properties
    string ChipsetManufacturer,
    string Series,
    StorageCapacity VRAM,
    MemoryType MemoryType,
    Frequency CoreClock,
    Frequency BoostClock,
    Power TDP,
    Length Length,
    GpuPowerConnector PowerConnectors,
    bool RayTracing
) : Product(Id, Name, Price, Manufacturer), ISlottedProduct
{
    public override string CategoryName => "GPU";
};

public record RamProduct(
    Guid Id,
    string Name,
    decimal Price,
    string Manufacturer,
    // RAM-specific properties
    MemoryType Type,
    StorageCapacity Capacity,
    string Configuration,
    Frequency Speed,
    string CASLatency,
    Voltage Voltage
) : Product(Id, Name, Price, Manufacturer)
{
    public override string CategoryName => "RAM";
};

public record PcCaseProduct(
    Guid Id,
    string Name,
    decimal Price,
    string Manufacturer,
    Dimensions Dimensions,
    List<Chamber> Chambers,
    // PC Case-specific properties
    string FormFactor,
    string Color,
    string SidePanelWindow
) : Product(Id, Name, Price, Manufacturer), IChamberedProduct
{
    public override string CategoryName => "PCCase";
};

public record PsuProduct(
    Guid Id,
    string Name,
    decimal Price,
    string Manufacturer,
    // PSU-specific properties
    Power Wattage,
    string Efficiency,
    string Modular,
    string FormFactor,
    Length Length,
    int PCIe8Pin
) : Product(Id, Name, Price, Manufacturer)
{
    public override string CategoryName => "PSU";
};

public record StorageProduct(
    Guid Id,
    string Name,
    decimal Price,
    string Manufacturer,
    // Storage-specific properties
    string Type,
    string Interface,
    string StorageFormFactor,
    StorageCapacity Capacity,
    DataSpeed ReadSpeed,
    DataSpeed WriteSpeed
) : Product(Id, Name, Price, Manufacturer)
{
    public override string CategoryName => "Storage";
};

public record CoolerProduct(
    Guid Id,
    string Name,
    decimal Price,
    string Manufacturer,
    Dimensions Dimensions,
    // Cooler-specific properties
    CoolerType CoolerType,
    Length Height,
    Power TDP,
    CpuSocket[] Sockets
) : Product(Id, Name, Price, Manufacturer), ISpatialProduct
{
    public override string CategoryName => "Cooler";
};

