using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Features.Catalog;

public class ProductCategoryPromptFields
{
    private static readonly Dictionary<ProductCategory, List<SystemPromptCategoryField>> _categoryStructures = new()
    {
        [ProductCategory.CPU] = [
            new(nameof(CpuProduct.Name), "product name"),
            new(nameof(CpuProduct.Manufacturer), "manufacturer name"),
            new(nameof(CpuProduct.Price), "decimal_price"),
            new(nameof(CpuProduct.Socket), "AM5 or LGA1700 or AM4 or LGA1200"),
            new(nameof(CpuProduct.Cores), "integer_cores"),
            new(nameof(CpuProduct.Threads), "integer_threads"),
            new(nameof(CpuProduct.BaseClock), "decimal_ghz"),
            new(nameof(CpuProduct.BoostClock), "decimal_ghz"),
            new(nameof(CpuProduct.TDP), "integer_watts"),
            new(nameof(CpuProduct.IntegratedGraphics), "boolean")
        ],
        [ProductCategory.Motherboard] = [
            new(nameof(MotherboardProduct.Name), "product name"),
            new(nameof(MotherboardProduct.Manufacturer), "manufacturer name"),
            new(nameof(MotherboardProduct.Price), "decimal_price"),
            new(nameof(MotherboardProduct.Socket), "AM5 or LGA1700 or AM4 or LGA1200"),
            new(nameof(MotherboardProduct.Chipset), "chipset name"),
            new(nameof(MotherboardProduct.FormFactor), "ATX or MicroATX or MiniITX or EATX"),
            new(nameof(MotherboardProduct.MemoryType), "DDR4 or DDR5"),
            new(nameof(MotherboardProduct.MaxMemory), "integer_gb"),
            new(nameof(MotherboardProduct.Dimensions), "length,width,height in mm"),
            new(nameof(MotherboardProduct.Slots), "array of Slot objects")
        ],
        [ProductCategory.GPU] = [
            new(nameof(GpuProduct.Name), "product name"),
            new(nameof(GpuProduct.Manufacturer), "manufacturer name"),
            new(nameof(GpuProduct.Price), "decimal_price"),
            new(nameof(GpuProduct.ChipsetManufacturer), "NVIDIA or AMD or Intel"),
            new(nameof(GpuProduct.Series), "series name"),
            new(nameof(GpuProduct.VRAM), "integer_gb"),
            new(nameof(GpuProduct.MemoryType), "GDDR6 or GDDR6X or GDDR5"),
            new(nameof(GpuProduct.CoreClock), "integer_mhz"),
            new(nameof(GpuProduct.BoostClock), "integer_mhz"),
            new(nameof(GpuProduct.TDP), "integer_watts"),
            new(nameof(GpuProduct.Length), "integer_mm"),
            new(nameof(GpuProduct.PowerConnectors), "1x16-pin or 2x8-pin or 3x8-pin"),
            new(nameof(GpuProduct.RayTracing), "boolean"),
            new(nameof(GpuProduct.Dimensions), "length,width,height in mm"),
            new(nameof(GpuProduct.Slots), "array of Slot objects")
        ],
        [ProductCategory.RAM] = [
            new(nameof(RamProduct.Name), "product name"),
            new(nameof(RamProduct.Manufacturer), "manufacturer name"),
            new(nameof(RamProduct.Price), "decimal_price"),
            new(nameof(RamProduct.Type), "DDR4 or DDR5 or DDR3"),
            new(nameof(RamProduct.Capacity), "integer_gb"),
            new(nameof(RamProduct.Configuration), "e.g., 2x16GB"),
            new(nameof(RamProduct.Speed), "integer_mhz"),
            new(nameof(RamProduct.CASLatency), "e.g., CL16"),
            new(nameof(RamProduct.Voltage), "decimal_volts")
        ],
        [ProductCategory.Case] = [
            new(nameof(PcCaseProduct.Name), "product name"),
            new(nameof(PcCaseProduct.Manufacturer), "manufacturer name"),
            new(nameof(PcCaseProduct.Price), "decimal_price"),
            new(nameof(PcCaseProduct.FormFactor), "ATX or MicroATX or MiniITX or EATX"),
            new(nameof(PcCaseProduct.Color), "color name"),
            new(nameof(PcCaseProduct.SidePanelWindow), "None or Acrylic or Tempered Glass"),
            new(nameof(PcCaseProduct.Dimensions), "length,width,height in mm"),
            new(nameof(PcCaseProduct.Chambers), "array of Chamber objects")
        ],
        [ProductCategory.PowerSupply] = [
            new(nameof(PsuProduct.Name), "product name"),
            new(nameof(PsuProduct.Manufacturer), "manufacturer name"),
            new(nameof(PsuProduct.Price), "decimal_price"),
            new(nameof(PsuProduct.Wattage), "integer_watts"),
            new(nameof(PsuProduct.Efficiency), "80+ Bronze or 80+ Gold or 80+ Platinum or 80+ Titanium"),
            new(nameof(PsuProduct.Modular), "Non-Modular or Semi-Modular or Fully Modular"),
            new(nameof(PsuProduct.FormFactor), "ATX or SFX"),
            new(nameof(PsuProduct.Length), "integer_mm"),
            new(nameof(PsuProduct.PCIe8Pin), "integer_count")
        ],
        [ProductCategory.Storage] = [
            new(nameof(StorageProduct.Name), "product name"),
            new(nameof(StorageProduct.Manufacturer), "manufacturer name"),
            new(nameof(StorageProduct.Price), "decimal_price"),
            new(nameof(StorageProduct.Type), "SSD or HDD"),
            new(nameof(StorageProduct.Interface), "NVMe or SATA or M.2"),
            new(nameof(StorageProduct.StorageFormFactor), "M.2 2280 or 2.5 inch or 3.5 inch"),
            new(nameof(StorageProduct.Capacity), "integer_gb"),
            new(nameof(StorageProduct.ReadSpeed), "integer_mbps"),
            new(nameof(StorageProduct.WriteSpeed), "integer_mbps")
        ],
        [ProductCategory.Cooler] = [
            new(nameof(CoolerProduct.Name), "product name"),
            new(nameof(CoolerProduct.Manufacturer), "manufacturer name"),
            new(nameof(CoolerProduct.Price), "decimal_price"),
            new(nameof(CoolerProduct.CoolerType), "Air or AIO or CustomLoop"),
            new(nameof(CoolerProduct.Height), "integer_mm"),
            new(nameof(CoolerProduct.TDP), "integer_watts"),
            new(nameof(CoolerProduct.Sockets), "comma-separated list like AM5,LGA1700"),
            new(nameof(CoolerProduct.Dimensions), "length,width,height in mm")
        ]
    };

    public List<SystemPromptCategoryField> GetFieldsForCategory(ProductCategory category)
    {
        _categoryStructures.TryGetValue(category, out List<SystemPromptCategoryField>? fields);
        return fields ?? [];
    }
}

public record SystemPromptCategoryField(string Name, string values);