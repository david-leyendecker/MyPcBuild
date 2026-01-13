namespace MyPcBuild.ApiService.Features.Catalog.DTOs;

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
public enum ApiGpuPowerConnector
{
    Dual8Pin,
    Triple8Pin,
    One16Pin
}
