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
/// Standard product without spatial properties (CPU, RAM, PSU, Storage).
/// </summary>
public record StandardProduct(
    Guid Id,
    string Name,
    ProductCategory Category,
    decimal Price,
    string Manufacturer,
    Dictionary<string, object> Specifications
) : Product(Id, Name, Category, Price, Manufacturer, Specifications);

/// <summary>
/// Product with physical dimensions (all products that need spatial validation).
/// </summary>
public record SpatialProduct(
    Guid Id,
    string Name,
    ProductCategory Category,
    decimal Price,
    string Manufacturer,
    Dictionary<string, object> Specifications,
    Dimensions Dimensions
) : Product(Id, Name, Category, Price, Manufacturer, Specifications);

/// <summary>
/// Product that provides slots for other components (Motherboard, GPU).
/// </summary>
public record SlottedProduct(
    Guid Id,
    string Name,
    ProductCategory Category,
    decimal Price,
    string Manufacturer,
    Dictionary<string, object> Specifications,
    Dimensions Dimensions,
    List<Slot> Slots
) : SpatialProduct(Id, Name, Category, Price, Manufacturer, Specifications, Dimensions);

/// <summary>
/// Product that provides chambers with slots (PC Case).
/// </summary>
public record ChamberedProduct(
    Guid Id,
    string Name,
    ProductCategory Category,
    decimal Price,
    string Manufacturer,
    Dictionary<string, object> Specifications,
    Dimensions Dimensions,
    List<Chamber> Chambers
) : SpatialProduct(Id, Name, Category, Price, Manufacturer, Specifications, Dimensions);

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
