using Marten;
using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Infrastructure;

public static class ProductSeeder
{
    public static async Task SeedProducts(IDocumentStore documentStore)
    {
        await using IDocumentSession session = documentStore.LightweightSession();

        // Check if products already exist
        int existingCount = await session.Query<Product>().CountAsync();
        if (existingCount > 0)
        {
            return; // Already seeded
        }

        List<Product> products =
        [
            // CPUs - AMD
            new Product(
                Guid.NewGuid(),
                "AMD Ryzen 9 7950X",
                ProductCategory.CPU,
                549.99m,
                "AMD",
                new Dictionary<string, object>
                {
                    ["Socket"] = "AM5",
                    ["Cores"] = 16,
                    ["Threads"] = 32,
                    ["BaseClock"] = "4.5 GHz",
                    ["BoostClock"] = "5.7 GHz",
                    ["TDP"] = 170,
                    ["IntegratedGraphics"] = false
                }
            ),
            new Product(
                Guid.NewGuid(),
                "AMD Ryzen 7 7800X3D",
                ProductCategory.CPU,
                399.99m,
                "AMD",
                new Dictionary<string, object>
                {
                    ["Socket"] = "AM5",
                    ["Cores"] = 8,
                    ["Threads"] = 16,
                    ["BaseClock"] = "4.2 GHz",
                    ["BoostClock"] = "5.0 GHz",
                    ["TDP"] = 120,
                    ["IntegratedGraphics"] = false,
                    ["3DCache"] = true
                }
            ),
            
            // CPUs - Intel
            new Product(
                Guid.NewGuid(),
                "Intel Core i9-14900K",
                ProductCategory.CPU,
                589.99m,
                "Intel",
                new Dictionary<string, object>
                {
                    ["Socket"] = "LGA1700",
                    ["Cores"] = 24,
                    ["Threads"] = 32,
                    ["BaseClock"] = "3.2 GHz",
                    ["BoostClock"] = "6.0 GHz",
                    ["TDP"] = 125,
                    ["IntegratedGraphics"] = true
                }
            ),
            new Product(
                Guid.NewGuid(),
                "Intel Core i5-14600K",
                ProductCategory.CPU,
                319.99m,
                "Intel",
                new Dictionary<string, object>
                {
                    ["Socket"] = "LGA1700",
                    ["Cores"] = 14,
                    ["Threads"] = 20,
                    ["BaseClock"] = "3.5 GHz",
                    ["BoostClock"] = "5.3 GHz",
                    ["TDP"] = 125,
                    ["IntegratedGraphics"] = true
                }
            ),

            // Motherboards - AMD
            new Product(
                Guid.NewGuid(),
                "ASUS ROG Strix X670E-E Gaming WiFi",
                ProductCategory.Motherboard,
                499.99m,
                "ASUS",
                new Dictionary<string, object>
                {
                    ["Socket"] = "AM5",
                    ["Chipset"] = "X670E",
                    ["FormFactor"] = "ATX",
                    ["MemoryType"] = "DDR5",
                    ["MaxMemory"] = 128,
                    ["MemorySlots"] = 4,
                    ["PCIeSlots"] = 3,
                    ["M2Slots"] = 4,
                    ["WiFi"] = true,
                    ["RGB"] = true
                }
            ),
            new Product(
                Guid.NewGuid(),
                "MSI MAG B650 TOMAHAWK WiFi",
                ProductCategory.Motherboard,
                229.99m,
                "MSI",
                new Dictionary<string, object>
                {
                    ["Socket"] = "AM5",
                    ["Chipset"] = "B650",
                    ["FormFactor"] = "ATX",
                    ["MemoryType"] = "DDR5",
                    ["MaxMemory"] = 128,
                    ["MemorySlots"] = 4,
                    ["PCIeSlots"] = 2,
                    ["M2Slots"] = 3,
                    ["WiFi"] = true,
                    ["RGB"] = true
                }
            ),

            // Motherboards - Intel
            new Product(
                Guid.NewGuid(),
                "ASUS ROG Maximus Z790 Hero",
                ProductCategory.Motherboard,
                629.99m,
                "ASUS",
                new Dictionary<string, object>
                {
                    ["Socket"] = "LGA1700",
                    ["Chipset"] = "Z790",
                    ["FormFactor"] = "ATX",
                    ["MemoryType"] = "DDR5",
                    ["MaxMemory"] = 192,
                    ["MemorySlots"] = 4,
                    ["PCIeSlots"] = 3,
                    ["M2Slots"] = 5,
                    ["WiFi"] = true,
                    ["RGB"] = true
                }
            ),
            new Product(
                Guid.NewGuid(),
                "Gigabyte B760M DS3H",
                ProductCategory.Motherboard,
                129.99m,
                "Gigabyte",
                new Dictionary<string, object>
                {
                    ["Socket"] = "LGA1700",
                    ["Chipset"] = "B760",
                    ["FormFactor"] = "MicroATX",
                    ["MemoryType"] = "DDR4",
                    ["MaxMemory"] = 128,
                    ["MemorySlots"] = 4,
                    ["PCIeSlots"] = 2,
                    ["M2Slots"] = 2,
                    ["WiFi"] = false,
                    ["RGB"] = false
                }
            ),

            // GPUs - NVIDIA
            new Product(
                Guid.NewGuid(),
                "NVIDIA GeForce RTX 4090",
                ProductCategory.GPU,
                1599.99m,
                "NVIDIA",
                new Dictionary<string, object>
                {
                    ["ChipsetManufacturer"] = "NVIDIA",
                    ["Series"] = "RTX 4090",
                    ["VRAM"] = 24,
                    ["MemoryType"] = "GDDR6X",
                    ["CoreClock"] = 2230,
                    ["BoostClock"] = 2520,
                    ["TDP"] = 450,
                    ["Length"] = 304,
                    ["Slots"] = 3,
                    ["PowerConnectors"] = "1x 16-pin",
                    ["RayTracing"] = true,
                    ["DLSS"] = true
                }
            ),
            new Product(
                Guid.NewGuid(),
                "NVIDIA GeForce RTX 4070 Ti",
                ProductCategory.GPU,
                799.99m,
                "NVIDIA",
                new Dictionary<string, object>
                {
                    ["ChipsetManufacturer"] = "NVIDIA",
                    ["Series"] = "RTX 4070 Ti",
                    ["VRAM"] = 12,
                    ["MemoryType"] = "GDDR6X",
                    ["CoreClock"] = 2310,
                    ["BoostClock"] = 2610,
                    ["TDP"] = 285,
                    ["Length"] = 285,
                    ["Slots"] = 2,
                    ["PowerConnectors"] = "1x 16-pin",
                    ["RayTracing"] = true,
                    ["DLSS"] = true
                }
            ),

            // GPUs - AMD
            new Product(
                Guid.NewGuid(),
                "AMD Radeon RX 7900 XTX",
                ProductCategory.GPU,
                999.99m,
                "AMD",
                new Dictionary<string, object>
                {
                    ["ChipsetManufacturer"] = "AMD",
                    ["Series"] = "RX 7900 XTX",
                    ["VRAM"] = 24,
                    ["MemoryType"] = "GDDR6",
                    ["CoreClock"] = 1900,
                    ["BoostClock"] = 2500,
                    ["TDP"] = 355,
                    ["Length"] = 287,
                    ["Slots"] = 2,
                    ["PowerConnectors"] = "2x 8-pin",
                    ["RayTracing"] = true,
                    ["FSR"] = true
                }
            ),

            // RAM - DDR5
            new Product(
                Guid.NewGuid(),
                "G.Skill Trident Z5 RGB 32GB (2x16GB) DDR5-6000",
                ProductCategory.RAM,
                129.99m,
                "G.Skill",
                new Dictionary<string, object>
                {
                    ["Type"] = "DDR5",
                    ["Capacity"] = 32,
                    ["Configuration"] = "2x16GB",
                    ["Speed"] = 6000,
                    ["CASLatency"] = "CL30",
                    ["Voltage"] = 1.35,
                    ["RGB"] = true,
                    ["HeatSpreader"] = true
                }
            ),
            new Product(
                Guid.NewGuid(),
                "Corsair Vengeance 64GB (2x32GB) DDR5-5600",
                ProductCategory.RAM,
                189.99m,
                "Corsair",
                new Dictionary<string, object>
                {
                    ["Type"] = "DDR5",
                    ["Capacity"] = 64,
                    ["Configuration"] = "2x32GB",
                    ["Speed"] = 5600,
                    ["CASLatency"] = "CL36",
                    ["Voltage"] = 1.25,
                    ["RGB"] = false,
                    ["HeatSpreader"] = true
                }
            ),

            // RAM - DDR4
            new Product(
                Guid.NewGuid(),
                "Corsair Vengeance LPX 16GB (2x8GB) DDR4-3200",
                ProductCategory.RAM,
                44.99m,
                "Corsair",
                new Dictionary<string, object>
                {
                    ["Type"] = "DDR4",
                    ["Capacity"] = 16,
                    ["Configuration"] = "2x8GB",
                    ["Speed"] = 3200,
                    ["CASLatency"] = "CL16",
                    ["Voltage"] = 1.35,
                    ["RGB"] = false,
                    ["HeatSpreader"] = true
                }
            ),

            // PC Cases
            new Product(
                Guid.NewGuid(),
                "Lian Li O11 Dynamic EVO",
                ProductCategory.PCCase,
                169.99m,
                "Lian Li",
                new Dictionary<string, object>
                {
                    ["FormFactor"] = "ATX",
                    ["Color"] = "Black",
                    ["SidePanelWindow"] = "Tempered Glass",
                    ["MaxGPULength"] = 420,
                    ["MaxCPUCoolerHeight"] = 167,
                    ["MaxPSULength"] = 225,
                    ["DriveBays2_5"] = 6,
                    ["DriveBays3_5"] = 4,
                    ["FanMounts"] = 13,
                    ["PreinstalledFans"] = 0,
                    ["RGBController"] = false,
                    ["USBPorts"] = "2x USB 3.0, 1x USB-C"
                }
            ),
            new Product(
                Guid.NewGuid(),
                "Fractal Design Meshify C",
                ProductCategory.PCCase,
                109.99m,
                "Fractal Design",
                new Dictionary<string, object>
                {
                    ["FormFactor"] = "ATX",
                    ["Color"] = "Black",
                    ["SidePanelWindow"] = "Tempered Glass",
                    ["MaxGPULength"] = 315,
                    ["MaxCPUCoolerHeight"] = 172,
                    ["MaxPSULength"] = 175,
                    ["DriveBays2_5"] = 3,
                    ["DriveBays3_5"] = 2,
                    ["FanMounts"] = 7,
                    ["PreinstalledFans"] = 2,
                    ["RGBController"] = false,
                    ["USBPorts"] = "2x USB 3.0"
                }
            ),
            new Product(
                Guid.NewGuid(),
                "NZXT H510i",
                ProductCategory.PCCase,
                99.99m,
                "NZXT",
                new Dictionary<string, object>
                {
                    ["FormFactor"] = "MicroATX",
                    ["Color"] = "White",
                    ["SidePanelWindow"] = "Tempered Glass",
                    ["MaxGPULength"] = 381,
                    ["MaxCPUCoolerHeight"] = 165,
                    ["MaxPSULength"] = 180,
                    ["DriveBays2_5"] = 4,
                    ["DriveBays3_5"] = 2,
                    ["FanMounts"] = 5,
                    ["PreinstalledFans"] = 2,
                    ["RGBController"] = true,
                    ["USBPorts"] = "1x USB 3.1, 1x USB-C"
                }
            ),

            // PSUs
            new Product(
                Guid.NewGuid(),
                "Corsair RM850x (2021)",
                ProductCategory.PSU,
                129.99m,
                "Corsair",
                new Dictionary<string, object>
                {
                    ["Wattage"] = 850,
                    ["Efficiency"] = "80+ Gold",
                    ["Modular"] = "Fully Modular",
                    ["FormFactor"] = "ATX",
                    ["Length"] = 160,
                    ["FanSize"] = 135,
                    ["PCIe8Pin"] = 6,
                    ["SATA"] = 9,
                    ["Molex"] = 6,
                    ["ATX12V"] = "2x 4+4 pin"
                }
            ),
            new Product(
                Guid.NewGuid(),
                "EVGA SuperNOVA 1000 G6",
                ProductCategory.PSU,
                199.99m,
                "EVGA",
                new Dictionary<string, object>
                {
                    ["Wattage"] = 1000,
                    ["Efficiency"] = "80+ Gold",
                    ["Modular"] = "Fully Modular",
                    ["FormFactor"] = "ATX",
                    ["Length"] = 150,
                    ["FanSize"] = 135,
                    ["PCIe8Pin"] = 8,
                    ["SATA"] = 10,
                    ["Molex"] = 6,
                    ["ATX12V"] = "2x 4+4 pin"
                }
            ),
            new Product(
                Guid.NewGuid(),
                "Seasonic Focus GX-650",
                ProductCategory.PSU,
                89.99m,
                "Seasonic",
                new Dictionary<string, object>
                {
                    ["Wattage"] = 650,
                    ["Efficiency"] = "80+ Gold",
                    ["Modular"] = "Fully Modular",
                    ["FormFactor"] = "ATX",
                    ["Length"] = 140,
                    ["FanSize"] = 120,
                    ["PCIe8Pin"] = 4,
                    ["SATA"] = 6,
                    ["Molex"] = 3,
                    ["ATX12V"] = "1x 4+4 pin"
                }
            ),

            // Storage - NVMe
            new Product(
                Guid.NewGuid(),
                "Samsung 990 Pro 2TB",
                ProductCategory.Storage,
                169.99m,
                "Samsung",
                new Dictionary<string, object>
                {
                    ["Type"] = "SSD",
                    ["Interface"] = "NVMe",
                    ["FormFactor"] = "M.2 2280",
                    ["Capacity"] = 2000,
                    ["ReadSpeed"] = 7450,
                    ["WriteSpeed"] = 6900,
                    ["TBW"] = 1200,
                    ["DRAM"] = true,
                    ["PCIeGen"] = 4
                }
            ),
            new Product(
                Guid.NewGuid(),
                "WD Black SN850X 1TB",
                ProductCategory.Storage,
                99.99m,
                "Western Digital",
                new Dictionary<string, object>
                {
                    ["Type"] = "SSD",
                    ["Interface"] = "NVMe",
                    ["FormFactor"] = "M.2 2280",
                    ["Capacity"] = 1000,
                    ["ReadSpeed"] = 7300,
                    ["WriteSpeed"] = 6300,
                    ["TBW"] = 600,
                    ["DRAM"] = true,
                    ["PCIeGen"] = 4
                }
            ),

            // Coolers
            new Product(
                Guid.NewGuid(),
                "Noctua NH-D15",
                ProductCategory.Cooler,
                109.99m,
                "Noctua",
                new Dictionary<string, object>
                {
                    ["Type"] = "Air",
                    ["Height"] = 165,
                    ["FanSize"] = 140,
                    ["FanCount"] = 2,
                    ["TDP"] = 250,
                    ["Sockets"] = new[] { "AM5", "AM4", "LGA1700", "LGA1200" },
                    ["RGB"] = false,
                    ["NoiseLevel"] = 24.6
                }
            ),
            new Product(
                Guid.NewGuid(),
                "Corsair iCUE H150i Elite LCD XT",
                ProductCategory.Cooler,
                289.99m,
                "Corsair",
                new Dictionary<string, object>
                {
                    ["Type"] = "AIO",
                    ["RadiatorSize"] = 360,
                    ["FanSize"] = 120,
                    ["FanCount"] = 3,
                    ["TDP"] = 300,
                    ["Sockets"] = new[] { "AM5", "AM4", "LGA1700", "LGA1200" },
                    ["RGB"] = true,
                    ["LCD"] = true,
                    ["NoiseLevel"] = 36.0
                }
            )
        ];

        foreach (Product product in products)
        {
            session.Store(product);
        }

        await session.SaveChangesAsync();
    }
}
