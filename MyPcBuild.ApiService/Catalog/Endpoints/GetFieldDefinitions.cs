using MyPcBuild.ApiService.Catalog.Models;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Catalog.Endpoints;

public static class GetFieldDefinitions
{
    public static IEndpointRouteBuilder MapGetFieldDefinitionsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalog/field-definitions/{category}", (
            ProductCategory category,
            IHttpContextAccessor httpContextAccessor) =>
        {
            List<FieldDefinition> fields = category switch
            {
                ProductCategory.CPU => GetCpuFields(),
                ProductCategory.Motherboard => GetMotherboardFields(),
                ProductCategory.GPU => GetGpuFields(),
                ProductCategory.RAM => GetRamFields(),
                ProductCategory.Case => GetPcCaseFields(),
                ProductCategory.PowerSupply => GetPsuFields(),
                ProductCategory.Storage => GetStorageFields(),
                ProductCategory.Cooler => GetCoolerFields(),
                _ => throw new ArgumentException($"Unknown category: {category}")
            };

            string baseUrl = httpContextAccessor.GetBaseUrl();

            GetFieldDefinitionsResponse response = new(
                category,
                fields,
                [
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/field-definitions/{category}"), "self", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products?filters=ProductCategory={category}"), "products", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/categories"), "categories", Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri($"{baseUrl}/api/catalog/products"), "create-product", Infrastructure.HttpMethod.POST)
                ]
            );

            return Results.Ok(response);
        })
        .WithName("GetFieldDefinitions")
        .WithTags("Catalog");

        return app;
    }


    // Helper methods to create FieldDefinitions for each type
    private static FieldDefinition TextField(string name, bool required = false)
        => new(name, FieldDefinitionType.Text, required, null, null);

    private static FieldDefinition NumberField(string name, string? unit = null, bool required = false)
        => new(name, FieldDefinitionType.Number, required, unit, null);

    private static FieldDefinition BooleanField(string name, bool required = false)
        => new(name, FieldDefinitionType.Boolean, required, null, null);

    private static FieldDefinition SelectField(string name, List<string> options, bool required = false)
        => new(name, FieldDefinitionType.Select, required, null, options);

    private static FieldDefinition MultiSelectField(string name, List<string> options, bool required = false)
        => new(name, FieldDefinitionType.MultiSelect, required, null, options);

    private static FieldDefinition DimensionsField(string name, string unit, bool required = false)
        => new(name, FieldDefinitionType.Dimensions, required, unit, null);

    private static FieldDefinition SlotsField(string name, bool required = false)
        => new(name, FieldDefinitionType.Slots, required, null, null);

    private static FieldDefinition ChambersField(string name, bool required = false)
        => new(name, FieldDefinitionType.Chambers, required, null, null);

    private static List<FieldDefinition> GetCpuFields()
    {

        return
        [
            SelectField(nameof(CpuProduct.Socket), ["AM5", "AM4", "LGA1700", "LGA1200", "LGA1151"], required: true),
            NumberField(nameof(CpuProduct.Cores), required: true),
            NumberField(nameof(CpuProduct.Threads), required: true),
            NumberField(nameof(CpuProduct.BaseClock), Frequency.Unit, required: true),
            NumberField(nameof(CpuProduct.BoostClock), Frequency.Unit, required: true),
            NumberField(nameof(CpuProduct.TDP), Power.Unit, required: true),
            BooleanField(nameof(CpuProduct.IntegratedGraphics))
        ];
    }

    private static List<FieldDefinition> GetMotherboardFields()
    {
        return
        [
            SelectField(nameof(MotherboardProduct.Socket), ["AM5", "AM4", "LGA1700", "LGA1200", "LGA1151"], required: true),
            TextField(nameof(MotherboardProduct.Chipset), required: true),
            SelectField(nameof(MotherboardProduct.FormFactor), ["ATX", "MicroATX", "MiniITX", "EATX"], required: true),
            SelectField(nameof(MotherboardProduct.MemoryType), ["DDR5", "DDR4", "DDR3"], required: true),
            NumberField(nameof(MotherboardProduct.MaxMemory), StorageCapacity.Unit, required: true),
            DimensionsField(nameof(MotherboardProduct.Dimensions), Length.Unit, required: true),
            SlotsField(nameof(MotherboardProduct.Slots))
        ];
    }

    private static List<FieldDefinition> GetGpuFields()
    {
        return
        [
            SelectField(nameof(GpuProduct.ChipsetManufacturer), ["NVIDIA", "AMD", "Intel"], required: true),
            TextField(nameof(GpuProduct.Series), required: true),
            NumberField(nameof(GpuProduct.VRAM), StorageCapacity.Unit, required: true),
            SelectField(nameof(GpuProduct.MemoryType), ["GDDR6X", "GDDR6", "GDDR5"], required: true),
            NumberField(nameof(GpuProduct.CoreClock), Frequency.Unit, required: true),
            NumberField(nameof(GpuProduct.BoostClock), Frequency.Unit, required: true),
            NumberField(nameof(GpuProduct.TDP), Power.Unit, required: true),
            SelectField(nameof(GpuProduct.PowerConnectors), ["1x16-pin", "2x8-pin", "3x8-pin"], required: true),
            BooleanField(nameof(GpuProduct.RayTracing)),
            DimensionsField(nameof(GpuProduct.Dimensions), Length.Unit, required: true),
            SlotsField(nameof(GpuProduct.Slots))
        ];
    }

    private static List<FieldDefinition> GetRamFields()
    {
        return
        [
            SelectField(nameof(RamProduct.Type), ["DDR5", "DDR4", "DDR3"], required: true),
            NumberField(nameof(RamProduct.Capacity), StorageCapacity.Unit, required: true),
            TextField(nameof(RamProduct.Configuration), required: true),
            NumberField(nameof(RamProduct.Speed), Frequency.Unit, required: true),
            TextField(nameof(RamProduct.CASLatency), required: true),
            NumberField(nameof(RamProduct.Voltage), Voltage.Unit, required: true)
        ];
    }

    private static List<FieldDefinition> GetPcCaseFields()
    {
        return
        [
            SelectField(nameof(PcCaseProduct.FormFactor), ["ATX", "MicroATX", "MiniITX", "EATX"], required: true),
            TextField(nameof(PcCaseProduct.Color)),
            SelectField(nameof(PcCaseProduct.SidePanelWindow), ["None", "Acrylic", "Tempered Glass"]),
            DimensionsField(nameof(PcCaseProduct.Dimensions), Length.Unit, required: true),
            ChambersField(nameof(PcCaseProduct.Chambers))
        ];
    }

    private static List<FieldDefinition> GetPsuFields()
    {
        return
        [
            NumberField(nameof(PsuProduct.Wattage), Power.Unit, required: true),
            SelectField(nameof(PsuProduct.Efficiency), ["80+ Bronze", "80+ Silver", "80+ Gold", "80+ Platinum", "80+ Titanium"], required: true),
            SelectField(nameof(PsuProduct.Modular), ["Non-Modular", "Semi-Modular", "Fully Modular"], required: true),
            SelectField(nameof(PsuProduct.FormFactor), ["ATX", "SFX", "SFX-L"], required: true),
            NumberField(nameof(PsuProduct.Length), Length.Unit, required: true),
            NumberField(nameof(PsuProduct.PCIe8Pin), required: true)
        ];
    }

    private static List<FieldDefinition> GetStorageFields()
    {
        return
        [
            SelectField(nameof(StorageProduct.Type), ["SSD", "HDD"], required: true),
            SelectField(nameof(StorageProduct.Interface), ["NVMe", "SATA", "M.2"], required: true),
            TextField(nameof(StorageProduct.StorageFormFactor), required: true),
            NumberField(nameof(StorageProduct.Capacity), StorageCapacity.Unit, required: true),
            NumberField(nameof(StorageProduct.ReadSpeed), DataSpeed.Unit, required: true),
            NumberField(nameof(StorageProduct.WriteSpeed), DataSpeed.Unit, required: true)
        ];
    }

    private static List<FieldDefinition> GetCoolerFields()
    {
        return
        [
            SelectField(nameof(CoolerProduct.CoolerType), ["Air", "AIO", "CustomLoop"], required: true),
            NumberField(nameof(CoolerProduct.Height), Length.Unit, required: true),
            NumberField(nameof(CoolerProduct.TDP), Power.Unit, required: true),
            MultiSelectField(nameof(CoolerProduct.Sockets), ["AM5", "AM4", "LGA1700", "LGA1200", "LGA1151"], required: true),
            DimensionsField(nameof(CoolerProduct.Dimensions), Length.Unit, required: true)
        ];
    }
}

public record GetFieldDefinitionsResponse(
    ProductCategory Category,
    List<FieldDefinition> Fields,
    List<HateoasLink> Links
);

public record FieldDefinition(
    string Name,
    FieldDefinitionType Type,
    bool Required,
    string? Unit,
    List<string>? Options
);

public enum FieldDefinitionType
{
    Text,
    Number,
    Boolean,
    Select,
    MultiSelect,
    Dimensions,
    Slots,
    Chambers
}