using MyPcBuild.ApiService.Catalog.Models;
using MyPcBuild.ApiService.Catalog.DTOs;

namespace MyPcBuild.ApiService.Catalog.Services;

/// <summary>
/// Provides field schemas for AI product generation based on API Request DTOs.
/// </summary>
public class ProductCategoryPromptFields
{
    private static readonly Dictionary<ProductCategory, List<SystemPromptCategoryField>> _categoryStructures = new()
    {
        [ProductCategory.CPU] = [
            new(nameof(CpuProductRequest.Category), $"string: {ProductCategory.CPU}"),
            new(nameof(CpuProductRequest.Name), "product name (string)"),
            new(nameof(CpuProductRequest.Manufacturer), "manufacturer name (string)"),
            new(nameof(CpuProductRequest.Price), "decimal price"),
            new(nameof(CpuProductRequest.Socket), $"string: {string.Join(", ", Enum.GetNames(typeof(ApiCpuSocket)))}"),
            new(nameof(CpuProductRequest.Cores), "integer cores"),
            new(nameof(CpuProductRequest.Threads), "integer threads"),
            new(nameof(CpuProductRequest.BaseClock), "decimal GHz"),
            new(nameof(CpuProductRequest.BoostClock), "decimal GHz"),
            new(nameof(CpuProductRequest.TDP), "integer watts"),
            new(nameof(CpuProductRequest.IntegratedGraphics), "boolean")
        ],
        [ProductCategory.Motherboard] = [
            new(nameof(MotherboardProductRequest.Category), $"string: {ProductCategory.Motherboard}"),
            new(nameof(MotherboardProductRequest.Name), "product name (string)"),
            new(nameof(MotherboardProductRequest.Manufacturer), "manufacturer name (string)"),
            new(nameof(MotherboardProductRequest.Price), "decimal price"),
            new(nameof(MotherboardProductRequest.Socket), $"string: {string.Join(", ", Enum.GetNames(typeof(ApiCpuSocket)))}"),
            new(nameof(MotherboardProductRequest.Chipset), "chipset name (string)"),
            new(nameof(MotherboardProductRequest.FormFactor), $"string: {string.Join(", ", Enum.GetNames(typeof(ApiFormFactor)))}"),
            new(nameof(MotherboardProductRequest.MemoryType), $"string: {ApiMemoryType.DDR3}, {ApiMemoryType.DDR4}, {ApiMemoryType.DDR5}"),
            new(nameof(MotherboardProductRequest.MaxMemory), "integer GB"),
            new(nameof(MotherboardProductRequest.Dimensions), "object with Length, Width, Height in mm (decimals)")
        ],
        [ProductCategory.GPU] = [
            new(nameof(GpuProductRequest.Category), $"string: {ProductCategory.GPU}"),
            new(nameof(GpuProductRequest.Name), "product name (string)"),
            new(nameof(GpuProductRequest.Manufacturer), "manufacturer name (string)"),
            new(nameof(GpuProductRequest.Price), "decimal price"),
            new(nameof(GpuProductRequest.ChipsetManufacturer), "string: NVIDIA, AMD, Intel"),
            new(nameof(GpuProductRequest.Series), "series name (string)"),
            new(nameof(GpuProductRequest.VRAM), "integer GB"),
            new(nameof(GpuProductRequest.MemoryType), $"string: {ApiMemoryType.GDDR6}, {ApiMemoryType.GDDR6X}, {ApiMemoryType.GDDR5}"),
            new(nameof(GpuProductRequest.CoreClock), "integer MHz"),
            new(nameof(GpuProductRequest.BoostClock), "integer MHz"),
            new(nameof(GpuProductRequest.TDP), "integer watts"),
            new(nameof(GpuProductRequest.PowerConnectors), $"string: {string.Join(", ", Enum.GetNames(typeof(ApiGpuPowerConnector)))}"),
            new(nameof(GpuProductRequest.RayTracing), "boolean"),
            new(nameof(GpuProductRequest.Dimensions), "object with Length, Width, Height in mm (decimals)")
        ],
        [ProductCategory.RAM] = [
            new(nameof(RamProductRequest.Category), $"string: {ProductCategory.RAM}"),
            new(nameof(RamProductRequest.Name), "product name (string)"),
            new(nameof(RamProductRequest.Manufacturer), "manufacturer name (string)"),
            new(nameof(RamProductRequest.Price), "decimal price"),
            new(nameof(RamProductRequest.Type), $"string: {ApiMemoryType.DDR5}, {ApiMemoryType.DDR4}, {ApiMemoryType.DDR3}"),
            new(nameof(RamProductRequest.Capacity), "integer GB"),
            new(nameof(RamProductRequest.Configuration), "string: e.g., 2x16GB"),
            new(nameof(RamProductRequest.Speed), "integer MHz"),
            new(nameof(RamProductRequest.CASLatency), "string: e.g., CL16"),
            new(nameof(RamProductRequest.Voltage), "decimal volts")
        ],
        [ProductCategory.Case] = [
            new(nameof(PcCaseProductRequest.Category), $"string: {ProductCategory.Case}"),
            new(nameof(PcCaseProductRequest.Name), "product name (string)"),
            new(nameof(PcCaseProductRequest.Manufacturer), "manufacturer name (string)"),
            new(nameof(PcCaseProductRequest.Price), "decimal price"),
            new(nameof(PcCaseProductRequest.FormFactor), "string form factor description"),
            new(nameof(PcCaseProductRequest.Color), "color name (string)"),
            new(nameof(PcCaseProductRequest.SidePanelWindow), "string: None, Acrylic, or Tempered Glass"),
            new(nameof(PcCaseProductRequest.Dimensions), "object with Length, Width, Height in mm (decimals)")
        ],
        [ProductCategory.PowerSupply] = [
            new(nameof(PsuProductRequest.Category), $"string: {ProductCategory.PowerSupply}"),
            new(nameof(PsuProductRequest.Name), "product name (string)"),
            new(nameof(PsuProductRequest.Manufacturer), "manufacturer name (string)"),
            new(nameof(PsuProductRequest.Price), "decimal price"),
            new(nameof(PsuProductRequest.Wattage), "integer watts"),
            new(nameof(PsuProductRequest.Efficiency), "string: 80+ Bronze, 80+ Gold, 80+ Platinum, or 80+ Titanium"),
            new(nameof(PsuProductRequest.Modular), "string: Non-Modular, Semi-Modular, or Fully Modular"),
            new(nameof(PsuProductRequest.FormFactor), "string: ATX or SFX"),
            new(nameof(PsuProductRequest.Length), "integer mm"),
            new(nameof(PsuProductRequest.PCIe8Pin), "integer count")
        ],
        [ProductCategory.Storage] = [
            new(nameof(StorageProductRequest.Category), $"string: {ProductCategory.Storage}"),
            new(nameof(StorageProductRequest.Name), "product name (string)"),
            new(nameof(StorageProductRequest.Manufacturer), "manufacturer name (string)"),
            new(nameof(StorageProductRequest.Price), "decimal price"),
            new(nameof(StorageProductRequest.Type), "string: SSD or HDD"),
            new(nameof(StorageProductRequest.Interface), "string: NVMe, SATA, or M.2"),
            new(nameof(StorageProductRequest.StorageFormFactor), "string: M.2 2280, 2.5 inch, or 3.5 inch"),
            new(nameof(StorageProductRequest.Capacity), "integer GB"),
            new(nameof(StorageProductRequest.ReadSpeed), "integer MB/s"),
            new(nameof(StorageProductRequest.WriteSpeed), "integer MB/s")
        ],
        [ProductCategory.Cooler] = [
            new(nameof(CoolerProductRequest.Category), $"string: {ProductCategory.Cooler}"),
            new(nameof(CoolerProductRequest.Name), "product name (string)"),
            new(nameof(CoolerProductRequest.Manufacturer), "manufacturer name (string)"),
            new(nameof(CoolerProductRequest.Price), "decimal price"),
            new(nameof(CoolerProductRequest.CoolerType), $"string: {string.Join(", ", Enum.GetNames(typeof(ApiCoolerType)))}"),
            new(nameof(CoolerProductRequest.Height), "integer mm"),
            new(nameof(CoolerProductRequest.TDP), "integer watts"),
            new(nameof(CoolerProductRequest.Sockets), $"array of strings: {string.Join(", ", Enum.GetNames(typeof(ApiCpuSocket)))}"),
            new(nameof(CoolerProductRequest.Dimensions), "object with Length, Width, Height in mm (decimals)")
        ]
    };

    public List<SystemPromptCategoryField> GetFieldsForCategory(ProductCategory category)
    {
        _categoryStructures.TryGetValue(category, out List<SystemPromptCategoryField>? fields);
        return fields ?? [];
    }
}

public record SystemPromptCategoryField(string Name, string values);
