namespace MyPcBuild.ApiService.Domain.Models;

public record Product(
    Guid Id,
    string Name,
    ProductCategory Category,
    decimal Price,
    string Manufacturer,
    Dictionary<string, object> Specifications
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
