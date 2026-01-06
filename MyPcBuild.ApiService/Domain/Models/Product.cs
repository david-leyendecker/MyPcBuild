using MyPcBuild.ApiService.Domain.Models.Spatial;

namespace MyPcBuild.ApiService.Domain.Models;

public record Product(
    Guid Id,
    string Name,
    ProductCategory Category,
    decimal Price,
    string Manufacturer,
    Dictionary<string, object> Specifications,
    List<Chamber>? Chambers = null,
    List<Slot>? Slots = null,
    Dimensions? Dimensions = null
);

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
