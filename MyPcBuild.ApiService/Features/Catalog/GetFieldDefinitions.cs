namespace MyPcBuild.ApiService.Features.Catalog;

public static class GetFieldDefinitions
{
    public static IEndpointRouteBuilder MapGetFieldDefinitionsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalog/field-definitions/{category}", (string category) =>
        {
            List<FieldDefinition> fields = category switch
            {
                "CPU" => GetCpuFields(),
                "Motherboard" => GetMotherboardFields(),
                "GPU" => GetGpuFields(),
                "RAM" => GetRamFields(),
                "PCCase" => GetPcCaseFields(),
                "PSU" => GetPsuFields(),
                "Storage" => GetStorageFields(),
                "Cooler" => GetCoolerFields(),
                _ => throw new ArgumentException($"Unknown category: {category}")
            };

            return Results.Ok(new GetFieldDefinitionsResponse(category, fields));
        })
        .WithName("GetFieldDefinitions")
        .WithTags("Catalog");

        return app;
    }

    private static List<FieldDefinition> GetCpuFields()
    {
        return
        [
            new FieldDefinition("Socket", "select", true, null, ["AM5", "AM4", "LGA1700", "LGA1200", "LGA1151"]),
            new FieldDefinition("Cores", "number", true, null, null),
            new FieldDefinition("Threads", "number", true, null, null),
            new FieldDefinition("BaseClock", "number", true, "GHz", null),
            new FieldDefinition("BoostClock", "number", true, "GHz", null),
            new FieldDefinition("TDP", "number", true, "W", null),
            new FieldDefinition("IntegratedGraphics", "boolean", false, null, null)
        ];
    }

    private static List<FieldDefinition> GetMotherboardFields()
    {
        return
        [
            new FieldDefinition("Socket", "select", true, null, ["AM5", "AM4", "LGA1700", "LGA1200", "LGA1151"]),
            new FieldDefinition("Chipset", "text", true, null, null),
            new FieldDefinition("FormFactor", "select", true, null, ["ATX", "Micro-ATX", "Mini-ITX", "E-ATX"]),
            new FieldDefinition("MemoryType", "select", true, null, ["DDR5", "DDR4", "DDR3"]),
            new FieldDefinition("MaxMemory", "number", true, "GB", null),
            new FieldDefinition("Dimensions", "dimensions", true, "mm", null),
            new FieldDefinition("Slots", "slots", false, null, null)
        ];
    }

    private static List<FieldDefinition> GetGpuFields()
    {
        return
        [
            new FieldDefinition("ChipsetManufacturer", "select", true, null, ["NVIDIA", "AMD", "Intel"]),
            new FieldDefinition("Series", "text", true, null, null),
            new FieldDefinition("VRAM", "number", true, "GB", null),
            new FieldDefinition("MemoryType", "select", true, null, ["GDDR6X", "GDDR6", "GDDR5"]),
            new FieldDefinition("CoreClock", "number", true, "MHz", null),
            new FieldDefinition("BoostClock", "number", true, "MHz", null),
            new FieldDefinition("TDP", "number", true, "W", null),
            new FieldDefinition("Length", "number", true, "mm", null),
            new FieldDefinition("PowerConnectors", "text", true, null, null),
            new FieldDefinition("RayTracing", "boolean", false, null, null),
            new FieldDefinition("Dimensions", "dimensions", true, "mm", null),
            new FieldDefinition("Slots", "slots", false, null, null)
        ];
    }

    private static List<FieldDefinition> GetRamFields()
    {
        return
        [
            new FieldDefinition("Type", "select", true, null, ["DDR5", "DDR4", "DDR3"]),
            new FieldDefinition("Capacity", "number", true, "GB", null),
            new FieldDefinition("Configuration", "text", true, null, null),
            new FieldDefinition("Speed", "number", true, "MHz", null),
            new FieldDefinition("CASLatency", "text", true, null, null),
            new FieldDefinition("Voltage", "number", true, "V", null)
        ];
    }

    private static List<FieldDefinition> GetPcCaseFields()
    {
        return
        [
            new FieldDefinition("FormFactor", "select", true, null, ["ATX", "Micro-ATX", "Mini-ITX", "E-ATX"]),
            new FieldDefinition("Color", "text", false, null, null),
            new FieldDefinition("SidePanelWindow", "select", false, null, ["None", "Acrylic", "Tempered Glass"]),
            new FieldDefinition("MaxGPULength", "number", true, "mm", null),
            new FieldDefinition("MaxCPUCoolerHeight", "number", true, "mm", null),
            new FieldDefinition("MaxPSULength", "number", true, "mm", null),
            new FieldDefinition("Dimensions", "dimensions", true, "mm", null),
            new FieldDefinition("Chambers", "chambers", false, null, null)
        ];
    }

    private static List<FieldDefinition> GetPsuFields()
    {
        return
        [
            new FieldDefinition("Wattage", "number", true, "W", null),
            new FieldDefinition("Efficiency", "select", true, null, ["80+ Bronze", "80+ Silver", "80+ Gold", "80+ Platinum", "80+ Titanium"]),
            new FieldDefinition("Modular", "select", true, null, ["Non-Modular", "Semi-Modular", "Fully Modular"]),
            new FieldDefinition("FormFactor", "select", true, null, ["ATX", "SFX", "SFX-L"]),
            new FieldDefinition("Length", "number", true, "mm", null),
            new FieldDefinition("PCIe8Pin", "number", true, null, null)
        ];
    }

    private static List<FieldDefinition> GetStorageFields()
    {
        return
        [
            new FieldDefinition("Type", "select", true, null, ["SSD", "HDD"]),
            new FieldDefinition("Interface", "select", true, null, ["NVMe", "SATA", "M.2"]),
            new FieldDefinition("StorageFormFactor", "text", true, null, null),
            new FieldDefinition("Capacity", "number", true, "GB", null),
            new FieldDefinition("ReadSpeed", "number", true, "MB/s", null),
            new FieldDefinition("WriteSpeed", "number", true, "MB/s", null)
        ];
    }

    private static List<FieldDefinition> GetCoolerFields()
    {
        return
        [
            new FieldDefinition("CoolerType", "select", true, null, ["Air", "AIO", "Custom Loop"]),
            new FieldDefinition("Height", "number", true, "mm", null),
            new FieldDefinition("TDP", "number", true, "W", null),
            new FieldDefinition("Sockets", "multi-select", true, null, ["AM5", "AM4", "LGA1700", "LGA1200", "LGA1151"]),
            new FieldDefinition("Dimensions", "dimensions", true, "mm", null)
        ];
    }
}

public record GetFieldDefinitionsResponse(
    string Category,
    List<FieldDefinition> Fields
);

public record FieldDefinition(
    string Name,
    string Type,
    bool Required,
    string? Unit,
    List<string>? Options
);
