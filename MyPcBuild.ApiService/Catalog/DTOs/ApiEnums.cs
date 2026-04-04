using System.Text.Json.Serialization;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.ApiService.Catalog.DTOs;

/// <summary>
/// CPU socket types for API consumption.
/// Optimized for external clients and AI-driven product creation.
/// </summary>
public enum ApiCpuSocket
{
    // Intel sockets
    LGA1700,
    LGA1200,
    LGA1151,
    LGA2066,

    // AMD sockets
    AM5,
    AM4,
    sTRX4,
    TR4
}

/// <summary>
/// Memory types for RAM and VRAM (API).
/// </summary>
public enum ApiMemoryType
{
    // DDR RAM types
    DDR3,
    DDR4,
    DDR5,

    // GDDR VRAM types
    GDDR5,
    GDDR5X,
    GDDR6,
    GDDR6X,

    // HBM types
    HBM2,
    HBM2E,
    HBM3
}

/// <summary>
/// Motherboard form factors (API).
/// </summary>
public enum ApiFormFactor
{
    ATX,
    MicroATX,
    MiniITX,
    EATX
}

/// <summary>
/// Cooler types (API).
/// </summary>
public enum ApiCoolerType
{
    Air,
    AIO,
    CustomLoop
}

/// <summary>
/// GPU power connector configurations (API).
/// </summary>
[JsonConverter(typeof(ApiGpuPowerConnectorConverter))]
public enum ApiGpuPowerConnector
{
    Dual8Pin,
    Triple8Pin,
    One16Pin
}

public class ApiGpuPowerConnectorConverter : EnumIgnoreCaseJsonConverter<ApiGpuPowerConnector> { }

/// <summary>
/// GPU chipset manufacturer (API).
/// </summary>
[JsonConverter(typeof(ApiGpuChipsetManufacturerConverter))]
public enum ApiGpuChipsetManufacturer
{
    NVIDIA,
    AMD,
    Intel
}

public class ApiGpuChipsetManufacturerConverter : EnumIgnoreCaseJsonConverter<ApiGpuChipsetManufacturer> { }

/// <summary>
/// PC case side panel window type (API).
/// </summary>
[JsonConverter(typeof(ApiSidePanelTypeConverter))]
public enum ApiSidePanelType
{
    None,
    Acrylic,
    TemperedGlass
}

public class ApiSidePanelTypeConverter : EnumIgnoreCaseJsonConverter<ApiSidePanelType> { }

/// <summary>
/// PSU efficiency rating (API).
/// </summary>
[JsonConverter(typeof(ApiPsuEfficiencyConverter))]
public enum ApiPsuEfficiency
{
    Bronze,
    Silver,
    Gold,
    Platinum,
    Titanium
}

public class ApiPsuEfficiencyConverter : EnumIgnoreCaseJsonConverter<ApiPsuEfficiency> { }

/// <summary>
/// PSU modularity type (API).
/// </summary>
[JsonConverter(typeof(ApiPsuModularityConverter))]
public enum ApiPsuModularity
{
    NonModular,
    SemiModular,
    FullyModular
}

public class ApiPsuModularityConverter : EnumIgnoreCaseJsonConverter<ApiPsuModularity> { }

/// <summary>
/// PSU form factor (API).
/// </summary>
[JsonConverter(typeof(ApiPsuFormFactorConverter))]
public enum ApiPsuFormFactor
{
    ATX,
    SFX,
    SFXL
}

public class ApiPsuFormFactorConverter : EnumIgnoreCaseJsonConverter<ApiPsuFormFactor> { }

/// <summary>
/// Storage type (API).
/// </summary>
[JsonConverter(typeof(ApiStorageTypeConverter))]
public enum ApiStorageType
{
    SSD,
    HDD
}

public class ApiStorageTypeConverter : EnumIgnoreCaseJsonConverter<ApiStorageType> { }

/// <summary>
/// Storage interface (API).
/// </summary>
[JsonConverter(typeof(ApiStorageInterfaceConverter))]
public enum ApiStorageInterface
{
    NVMe,
    SATA
}

public class ApiStorageInterfaceConverter : EnumIgnoreCaseJsonConverter<ApiStorageInterface> { }

/// <summary>
/// Storage form factor (API).
/// </summary>
[JsonConverter(typeof(ApiStorageFormFactorConverter))]
public enum ApiStorageFormFactor
{
    M2_2280,
    TwoPointFiveInch,
    ThreePointFiveInch
}

public class ApiStorageFormFactorConverter : EnumIgnoreCaseJsonConverter<ApiStorageFormFactor> { }