using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Features.Catalog.DTOs;

namespace MyPcBuild.ApiService.Features.Catalog;

/// <summary>
/// Provides field schemas for AI product generation based on API DTOs.
/// </summary>
public class ProductCategoryPromptFields
{
    private static readonly Dictionary<ProductCategory, List<SystemPromptCategoryField>> _categoryStructures = new()
    {
        [ProductCategory.CPU] = [
            new(nameof(CpuDto.Name), "product name (string)"),
            new(nameof(CpuDto.Manufacturer), "manufacturer name (string)"),
            new(nameof(CpuDto.Price), "decimal price"),
            new(nameof(CpuDto.Socket), $"string: {string.Join(", ", Enum.GetNames(typeof(ApiCpuSocket)))}"),
            new(nameof(CpuDto.Cores), "integer cores"),
            new(nameof(CpuDto.Threads), "integer threads"),
            new(nameof(CpuDto.BaseClock), "decimal GHz"),
            new(nameof(CpuDto.BoostClock), "decimal GHz"),
            new(nameof(CpuDto.TDP), "integer watts"),
            new(nameof(CpuDto.IntegratedGraphics), "boolean")
        ],
        [ProductCategory.Motherboard] = [
            new(nameof(MotherboardDto.Name), "product name (string)"),
            new(nameof(MotherboardDto.Manufacturer), "manufacturer name (string)"),
            new(nameof(MotherboardDto.Price), "decimal price"),
            new(nameof(MotherboardDto.Socket), $"string: {string.Join(", ", Enum.GetNames(typeof(ApiCpuSocket)))}"),
            new(nameof(MotherboardDto.Chipset), "chipset name (string)"),
            new(nameof(MotherboardDto.FormFactor), $"string: {string.Join(", ", Enum.GetNames(typeof(ApiFormFactor)))}"),
            new(nameof(MotherboardDto.MemoryType), $"string: {ApiMemoryType.DDR3}, {ApiMemoryType.DDR4}, {ApiMemoryType.DDR5}"),
            new(nameof(MotherboardDto.MaxMemory), "integer GB"),
            new(nameof(MotherboardDto.Dimensions), "object with Length, Width, Height in mm (decimals)")
        ],
        [ProductCategory.GPU] = [
            new(nameof(GpuDto.Name), "product name (string)"),
            new(nameof(GpuDto.Manufacturer), "manufacturer name (string)"),
            new(nameof(GpuDto.Price), "decimal price"),
            new(nameof(GpuDto.ChipsetManufacturer), "string: NVIDIA, AMD, Intel"),
            new(nameof(GpuDto.Series), "series name (string)"),
            new(nameof(GpuDto.VRAM), "integer GB"),
            new(nameof(GpuDto.MemoryType), $"string: {ApiMemoryType.GDDR6}, {ApiMemoryType.GDDR6X}, {ApiMemoryType.GDDR5}"),
            new(nameof(GpuDto.CoreClock), "integer MHz"),
            new(nameof(GpuDto.BoostClock), "integer MHz"),
            new(nameof(GpuDto.TDP), "integer watts"),
            new(nameof(GpuDto.Length), "integer mm"),
            new(nameof(GpuDto.PowerConnectors), $"string: {string.Join(", ", Enum.GetNames(typeof(ApiGpuPowerConnector)))}"),
            new(nameof(GpuDto.RayTracing), "boolean"),
            new(nameof(GpuDto.Dimensions), "object with Length, Width, Height in mm (decimals)")
        ],
        [ProductCategory.RAM] = [
            new(nameof(RamDto.Name), "product name (string)"),
            new(nameof(RamDto.Manufacturer), "manufacturer name (string)"),
            new(nameof(RamDto.Price), "decimal price"),
            new(nameof(RamDto.Type), $"string: {ApiMemoryType.DDR5}, {ApiMemoryType.DDR4}, {ApiMemoryType.DDR3}"),
            new(nameof(RamDto.Capacity), "integer GB"),
            new(nameof(RamDto.Configuration), "string: e.g., 2x16GB"),
            new(nameof(RamDto.Speed), "integer MHz"),
            new(nameof(RamDto.CASLatency), "string: e.g., CL16"),
            new(nameof(RamDto.Voltage), "decimal volts")
        ],
        [ProductCategory.Case] = [
            new(nameof(PcCaseDto.Name), "product name (string)"),
            new(nameof(PcCaseDto.Manufacturer), "manufacturer name (string)"),
            new(nameof(PcCaseDto.Price), "decimal price"),
            new(nameof(PcCaseDto.FormFactor), "string form factor description"),
            new(nameof(PcCaseDto.Color), "color name (string)"),
            new(nameof(PcCaseDto.SidePanelWindow), "string: None, Acrylic, or Tempered Glass"),
            new(nameof(PcCaseDto.Dimensions), "object with Length, Width, Height in mm (decimals)")
        ],
        [ProductCategory.PowerSupply] = [
            new(nameof(PsuDto.Name), "product name (string)"),
            new(nameof(PsuDto.Manufacturer), "manufacturer name (string)"),
            new(nameof(PsuDto.Price), "decimal price"),
            new(nameof(PsuDto.Wattage), "integer watts"),
            new(nameof(PsuDto.Efficiency), "string: 80+ Bronze, 80+ Gold, 80+ Platinum, or 80+ Titanium"),
            new(nameof(PsuDto.Modular), "string: Non-Modular, Semi-Modular, or Fully Modular"),
            new(nameof(PsuDto.FormFactor), "string: ATX or SFX"),
            new(nameof(PsuDto.Length), "integer mm"),
            new(nameof(PsuDto.PCIe8Pin), "integer count")
        ],
        [ProductCategory.Storage] = [
            new(nameof(StorageDto.Name), "product name (string)"),
            new(nameof(StorageDto.Manufacturer), "manufacturer name (string)"),
            new(nameof(StorageDto.Price), "decimal price"),
            new(nameof(StorageDto.Type), "string: SSD or HDD"),
            new(nameof(StorageDto.Interface), "string: NVMe, SATA, or M.2"),
            new(nameof(StorageDto.StorageFormFactor), "string: M.2 2280, 2.5 inch, or 3.5 inch"),
            new(nameof(StorageDto.Capacity), "integer GB"),
            new(nameof(StorageDto.ReadSpeed), "integer MB/s"),
            new(nameof(StorageDto.WriteSpeed), "integer MB/s")
        ],
        [ProductCategory.Cooler] = [
            new(nameof(CoolerDto.Name), "product name (string)"),
            new(nameof(CoolerDto.Manufacturer), "manufacturer name (string)"),
            new(nameof(CoolerDto.Price), "decimal price"),
            new(nameof(CoolerDto.CoolerType), $"string: {string.Join(", ", Enum.GetNames(typeof(ApiCoolerType)))}"),
            new(nameof(CoolerDto.Height), "integer mm"),
            new(nameof(CoolerDto.TDP), "integer watts"),
            new(nameof(CoolerDto.Sockets), $"array of strings: {string.Join(", ", Enum.GetNames(typeof(ApiCpuSocket)))}"),
            new(nameof(CoolerDto.Dimensions), "object with Length, Width, Height in mm (decimals)")
        ]
    };

    public List<SystemPromptCategoryField> GetFieldsForCategory(ProductCategory category)
    {
        _categoryStructures.TryGetValue(category, out List<SystemPromptCategoryField>? fields);
        return fields ?? [];
    }
}

public record SystemPromptCategoryField(string Name, string values);