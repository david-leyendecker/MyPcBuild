namespace MyPcBuild.ApiService.Domain.Models;

/// <summary>
/// CPU socket types.
/// </summary>
public enum CpuSocket
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
/// Extension methods for CpuSocket enum to allow custom socket definitions.
/// </summary>
public static class CpuSocketExtensions
{
    /// <summary>
    /// Checks if a socket string matches this CpuSocket enum value.
    /// </summary>
    public static bool Matches(this CpuSocket socket, string socketString)
    {
        return socket.ToString().Equals(socketString, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Parses a socket string to a CpuSocket enum value.
    /// </summary>
    public static CpuSocket Parse(string socketString)
    {
        if (Enum.TryParse<CpuSocket>(socketString, ignoreCase: true, out CpuSocket result))
        {
            return result;
        }
        throw new ArgumentException($"Unknown CPU socket: {socketString}", nameof(socketString));
    }

    /// <summary>
    /// Tries to parse a socket string to a CpuSocket enum value.
    /// </summary>
    public static bool TryParse(string socketString, out CpuSocket socket)
    {
        return Enum.TryParse(socketString, ignoreCase: true, out socket);
    }
}

/// <summary>
/// Memory types for RAM and VRAM.
/// </summary>
public enum MemoryType
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
/// Represents a frequency in gigahertz (GHz).
/// </summary>
public record Frequency
{
    public decimal ValueInGHz { get; }

    private Frequency(decimal valueInGHz)
    {
        if (valueInGHz < 0)
        {
            throw new ArgumentException("Frequency cannot be negative", nameof(valueInGHz));
        }

        ValueInGHz = valueInGHz;
    }

    public static Frequency FromGHz(decimal ghz) => new(ghz);
    public static Frequency FromMHz(decimal mhz) => new(mhz / 1000m);

    public decimal ToMHz() => ValueInGHz * 1000m;

    public override string ToString() => $"{ValueInGHz} GHz";
}

/// <summary>
/// Represents a storage capacity in gigabytes (GB).
/// </summary>
public record StorageCapacity
{
    public int ValueInGB { get; }

    private StorageCapacity(int valueInGB)
    {
        if (valueInGB < 0)
        {
            throw new ArgumentException("Storage capacity cannot be negative", nameof(valueInGB));
        }

        ValueInGB = valueInGB;
    }

    public static StorageCapacity FromGB(int gb) => new(gb);
    public static StorageCapacity FromTB(decimal tb) => new((int)(tb * 1024));

    public decimal ToTB() => ValueInGB / 1024m;

    public override string ToString() => ValueInGB >= 1024 ? $"{ToTB():F2} TB" : $"{ValueInGB} GB";
}

/// <summary>
/// Represents a power rating in watts (W).
/// </summary>
public record Power
{
    public int ValueInWatts { get; }

    private Power(int valueInWatts)
    {
        if (valueInWatts < 0)
        {
            throw new ArgumentException("Power cannot be negative", nameof(valueInWatts));
        }

        ValueInWatts = valueInWatts;
    }

    public static Power FromWatts(int watts) => new(watts);

    public override string ToString() => $"{ValueInWatts}W";
}

/// <summary>
/// Represents a voltage in volts (V).
/// </summary>
public record Voltage
{
    public decimal ValueInVolts { get; }

    private Voltage(decimal valueInVolts)
    {
        if (valueInVolts < 0)
        {
            throw new ArgumentException("Voltage cannot be negative", nameof(valueInVolts));
        }

        ValueInVolts = valueInVolts;
    }

    public static Voltage FromVolts(decimal volts) => new(volts);

    public override string ToString() => $"{ValueInVolts}V";
}

/// <summary>
/// Represents a length/distance in millimeters (mm).
/// </summary>
public record Length
{
    public int ValueInMm { get; }

    private Length(int valueInMm)
    {
        if (valueInMm < 0)
        {
            throw new ArgumentException("Length cannot be negative", nameof(valueInMm));
        }

        ValueInMm = valueInMm;
    }

    public static Length FromMm(int mm) => new(mm);
    public static Length FromCm(decimal cm) => new((int)(cm * 10));

    public decimal ToCm() => ValueInMm / 10m;

    public override string ToString() => $"{ValueInMm}mm";
}

/// <summary>
/// Represents a data transfer speed in MB/s.
/// </summary>
public record DataSpeed
{
    public int ValueInMBps { get; }

    private DataSpeed(int valueInMBps)
    {
        if (valueInMBps < 0)
        {
            throw new ArgumentException("Data speed cannot be negative", nameof(valueInMBps));
        }

        ValueInMBps = valueInMBps;
    }

    public static DataSpeed FromMBps(int mbps) => new(mbps);
    public static DataSpeed FromGBps(decimal gbps) => new((int)(gbps * 1000));

    public decimal ToGBps() => ValueInMBps / 1000m;

    public override string ToString() => ValueInMBps >= 1000 ? $"{ToGBps():F2} GB/s" : $"{ValueInMBps} MB/s";
}
