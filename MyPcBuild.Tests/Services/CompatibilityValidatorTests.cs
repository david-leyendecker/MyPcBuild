using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;
using MyPcBuild.ApiService.Features.Compatibility;

namespace MyPcBuild.Tests.Services;

public class CompatibilityValidatorTests
{
    private readonly ICompatibilityValidator _validator;

    public CompatibilityValidatorTests()
    {
        _validator = new CompatibilityValidator();
    }

    #region CPU/Motherboard Compatibility Tests

    [Fact]
    public async Task ValidateBuild_MatchingCpuAndMotherboardSockets_ReturnsCompatible()
    {
        // Arrange
        List<Product> products =
        [
            CreateCpu("AMD Ryzen 9 7950X", "AM5"),
            CreateMotherboard("ASUS X670E", "AM5", "DDR5", "ATX")
        ];

        // Act
        CompatibilityResult result = await _validator.ValidateBuild(products);

        // Assert
        Assert.True(result.IsCompatible);
        Assert.False(result.HasErrors);
        Assert.Empty(result.Issues);
    }

    [Fact]
    public async Task ValidateBuild_MismatchedCpuAndMotherboardSockets_ReturnsError()
    {
        // Arrange
        List<Product> products =
        [
            CreateCpu("Intel i9-14900K", "LGA1700"),
            CreateMotherboard("ASUS X670E", "AM5", "DDR5", "ATX")
        ];

        // Act
        CompatibilityResult result = await _validator.ValidateBuild(products);

        // Assert
        Assert.False(result.IsCompatible);
        Assert.True(result.HasErrors);
        Assert.Single(result.Issues);
        Assert.Equal("CPU/Motherboard", result.Issues[0].Category);
        Assert.Equal(IssueSeverity.Error, result.Issues[0].Severity);
        Assert.Contains("LGA1700", result.Issues[0].Message);
        Assert.Contains("AM5", result.Issues[0].Message);
    }

    #endregion

    #region RAM Compatibility Tests

    [Fact]
    public async Task ValidateBuild_MatchingRamAndMotherboardDdrType_ReturnsCompatible()
    {
        // Arrange
        List<Product> products =
        [
            CreateMotherboard("ASUS X670E", "AM5", "DDR5", "ATX"),
            CreateRam("G.Skill Trident", "DDR5", 32, "2x16GB")
        ];

        // Act
        CompatibilityResult result = await _validator.ValidateBuild(products);

        // Assert
        Assert.True(result.IsCompatible);
        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateBuild_MismatchedRamAndMotherboardDdrType_ReturnsError()
    {
        // Arrange
        List<Product> products =
        [
            CreateMotherboard("ASUS X670E", "AM5", "DDR5", "ATX"),
            CreateRam("Corsair LPX", "DDR4", 16, "2x8GB")
        ];

        // Act
        CompatibilityResult result = await _validator.ValidateBuild(products);

        // Assert
        Assert.False(result.IsCompatible);
        Assert.True(result.HasErrors);
        Assert.Contains(result.Issues, i => i.Category == "RAM/Motherboard" && i.Message.Contains("DDR4") && i.Message.Contains("DDR5"));
    }

    [Fact]
    public async Task ValidateBuild_RamExceedsMotherboardCapacity_ReturnsError()
    {
        // Arrange
        List<Product> products =
        [
            CreateMotherboard("Budget Board", "AM5", "DDR5", "ATX", maxMemory: 64),
            CreateRam("G.Skill 64GB", "DDR5", 64, "2x32GB"),
            CreateRam("G.Skill 32GB", "DDR5", 32, "2x16GB") // Total 96GB
        ];

        // Act
        CompatibilityResult result = await _validator.ValidateBuild(products);

        // Assert
        Assert.False(result.IsCompatible);
        Assert.True(result.HasErrors);
        Assert.Contains(result.Issues, i => i.Category == "RAM/Motherboard" && i.Message.Contains("96") && i.Message.Contains("64"));
    }

    [Fact]
    public async Task ValidateBuild_TooManyRamSticks_ReturnsError()
    {
        // Arrange
        List<Product> products =
        [
            CreateMotherboard("Board with 4 slots", "AM5", "DDR5", "ATX", memorySlots: 4),
            CreateRam("RAM Kit 1", "DDR5", 32, "2x16GB"),
            CreateRam("RAM Kit 2", "DDR5", 32, "2x16GB"),
            CreateRam("RAM Kit 3", "DDR5", 16, "2x8GB") // 6 sticks total
        ];

        // Act
        CompatibilityResult result = await _validator.ValidateBuild(products);

        // Assert
        Assert.False(result.IsCompatible);
        Assert.True(result.HasErrors);
        Assert.Contains(result.Issues, i => i.Category == "RAM/Motherboard" && i.Message.Contains("6") && i.Message.Contains("4"));
    }

    #endregion

    #region GPU Compatibility Tests

    [Fact]
    public async Task ValidateBuild_GpuFitsInCase_ReturnsCompatible()
    {
        // Arrange
        List<Product> products =
        [
            CreateGpu("RTX 4070", 285, 285),
            CreateCase("Lian Li O11", "ATX")
        ];

        // Act
        CompatibilityResult result = await _validator.ValidateBuild(products);

        // Assert
        Assert.True(result.IsCompatible);
        Assert.False(result.HasErrors);
    }





    #endregion

    #region Case Compatibility Tests

    [Fact]
    public async Task ValidateBuild_AtxMotherboardInAtxCase_ReturnsCompatible()
    {
        // Arrange
        List<Product> products =
        [
            CreateMotherboard("ATX Board", "AM5", "DDR5", "ATX"),
            CreateCase("ATX Case", "ATX")
        ];

        // Act
        CompatibilityResult result = await _validator.ValidateBuild(products);

        // Assert
        Assert.True(result.IsCompatible);
        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateBuild_AtxMotherboardInMicroAtxCase_ReturnsError()
    {
        // Arrange
        List<Product> products =
        [
            CreateMotherboard("ATX Board", "AM5", "DDR5", "ATX"),
            CreateCase("MicroATX Case", "MicroATX")
        ];

        // Act
        CompatibilityResult result = await _validator.ValidateBuild(products);

        // Assert
        Assert.False(result.IsCompatible);
        Assert.True(result.HasErrors);
        Assert.Contains(result.Issues, i => i.Category == "Case/Motherboard" && i.Message.Contains("ATX") && i.Message.Contains("MicroATX"));
    }

    [Fact]
    public async Task ValidateBuild_MicroAtxMotherboardInAtxCase_ReturnsCompatible()
    {
        // Arrange
        List<Product> products =
        [
            CreateMotherboard("MicroATX Board", "AM5", "DDR5", "MicroATX"),
            CreateCase("ATX Case", "ATX")
        ];

        // Act
        CompatibilityResult result = await _validator.ValidateBuild(products);

        // Assert
        Assert.True(result.IsCompatible);
        Assert.False(result.HasErrors);
    }

    #endregion

    #region PSU Compatibility Tests

    [Fact]
    public async Task ValidateBuild_SufficientPsuWattage_ReturnsCompatible()
    {
        // Arrange
        List<Product> products =
        [
            CreateCpu("Ryzen 7 7800X3D", "AM5", tdp: 120),
            CreateGpu("RTX 4070 Ti", 285, tdp: 285),
            CreatePsu("Corsair RM850x", 850)
        ];

        // Act
        CompatibilityResult result = await _validator.ValidateBuild(products);

        // Assert
        Assert.True(result.IsCompatible);
        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateBuild_InsufficientPsuWattage_ReturnsError()
    {
        // Arrange
        List<Product> products =
        [
            CreateCpu("Ryzen 9 7950X", "AM5", tdp: 170),
            CreateGpu("RTX 4090", 304, tdp: 450),
            CreatePsu("Seasonic 650W", 650) // Total need: 170+450+150 = 770W
        ];

        // Act
        CompatibilityResult result = await _validator.ValidateBuild(products);

        // Assert
        Assert.False(result.IsCompatible);
        Assert.True(result.HasErrors);
        Assert.Contains(result.Issues, i => i.Category == "PSU" && i.Severity == IssueSeverity.Error && i.Message.Contains("770"));
    }

    [Fact]
    public async Task ValidateBuild_BelowRecommendedPsuWattage_ReturnsWarning()
    {
        // Arrange
        List<Product> products =
        [
            CreateCpu("Ryzen 9 7950X", "AM5", tdp: 170),
            CreateGpu("RTX 4090", 304, tdp: 450),
            CreatePsu("Corsair RM850x", 850) // Sufficient but below 924W recommended
        ];

        // Act
        CompatibilityResult result = await _validator.ValidateBuild(products);

        // Assert
        Assert.True(result.IsCompatible);
        Assert.False(result.HasErrors);
        Assert.True(result.HasWarnings);
        Assert.Contains(result.Issues, i => i.Category == "PSU" && i.Severity == IssueSeverity.Warning);
    }

    #endregion

    #region Cooler Compatibility Tests

    [Fact]
    public async Task ValidateBuild_CoolerSupportsSocketAndTdp_ReturnsCompatible()
    {
        // Arrange
        List<Product> products =
        [
            CreateCpu("Ryzen 7 7800X3D", "AM5", tdp: 120),
            CreateCooler("Noctua NH-D15", "Air", 165, tdp: 250, sockets: ["AM5", "AM4", "LGA1700"])
        ];

        // Act
        CompatibilityResult result = await _validator.ValidateBuild(products);

        // Assert
        Assert.True(result.IsCompatible);
        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateBuild_CoolerDoesNotSupportSocket_ReturnsError()
    {
        // Arrange
        List<Product> products =
        [
            CreateCpu("Ryzen 7 7800X3D", "AM5", tdp: 120),
            CreateCooler("Old Cooler", "Air", 155, tdp: 200, sockets: ["AM4", "LGA1200"])
        ];

        // Act
        CompatibilityResult result = await _validator.ValidateBuild(products);

        // Assert
        Assert.False(result.IsCompatible);
        Assert.True(result.HasErrors);
        Assert.Contains(result.Issues, i => i.Category == "Cooler/CPU" && i.Message.Contains("AM5"));
    }

    [Fact]
    public async Task ValidateBuild_CoolerInsufficientTdp_ReturnsError()
    {
        // Arrange
        List<Product> products =
        [
            CreateCpu("Ryzen 9 7950X", "AM5", tdp: 170),
            CreateCooler("Weak Cooler", "Air", 120, tdp: 95, sockets: ["AM5"])
        ];

        // Act
        CompatibilityResult result = await _validator.ValidateBuild(products);

        // Assert
        Assert.False(result.IsCompatible);
        Assert.True(result.HasErrors);
        Assert.Contains(result.Issues, i => i.Category == "Cooler/CPU" && i.Message.Contains("95") && i.Message.Contains("170"));
    }



    #endregion

    #region Complex Multi-Issue Tests

    [Fact]
    public async Task ValidateBuild_MultipleIncompatibilities_ReturnsAllErrors()
    {
        // Arrange
        List<Product> products =
        [
            CreateCpu("Intel i9-14900K", "LGA1700", tdp: 125),
            CreateMotherboard("AMD Board", "AM5", "DDR5", "ATX"),
            CreateRam("DDR4 RAM", "DDR4", 32, "2x16GB"),
            CreateGpu("RTX 4090", 304, tdp: 450),
            CreateCase("Tiny Case", "MicroATX"),
            CreatePsu("Weak PSU", 500)
        ];

        // Act
        CompatibilityResult result = await _validator.ValidateBuild(products);

        // Assert
        Assert.False(result.IsCompatible);
        Assert.True(result.HasErrors);
        Assert.True(result.Issues.Count >= 4); // Socket, DDR, Case, PSU issues
        Assert.Contains(result.Issues, i => i.Category == "CPU/Motherboard");
        Assert.Contains(result.Issues, i => i.Category == "RAM/Motherboard");
        Assert.Contains(result.Issues, i => i.Category == "Case/Motherboard");
        Assert.Contains(result.Issues, i => i.Category == "PSU");
    }

    [Fact]
    public async Task ValidateBuild_PerfectBuild_ReturnsNoIssues()
    {
        // Arrange - A perfectly compatible build
        List<Product> products =
        [
            CreateCpu("AMD Ryzen 7 7800X3D", "AM5", tdp: 120),
            CreateMotherboard("MSI B650", "AM5", "DDR5", "ATX", maxMemory: 128, memorySlots: 4),
            CreateRam("G.Skill DDR5", "DDR5", 32, "2x16GB"),
            CreateGpu("RX 7900 XTX", 287, tdp: 355),
            CreateCase("Lian Li O11", "ATX"),
            CreatePsu("Corsair RM1000x", 1000),
            CreateCooler("Noctua NH-D15", "Air", 165, tdp: 250, sockets: ["AM5", "AM4"])
        ];

        // Act
        CompatibilityResult result = await _validator.ValidateBuild(products);

        // Assert
        Assert.True(result.IsCompatible);
        Assert.False(result.HasErrors);
        Assert.False(result.HasWarnings);
        Assert.Empty(result.Issues);
    }

    #endregion

    #region Helper Methods

    private CpuProduct CreateCpu(string name, string socket, int tdp = 0)
    {
        CpuSocket socketEnum = CpuSocketExtensions.Parse(socket);
        return new CpuProduct(
            Guid.NewGuid(),
            name,
            399.99m,
            "AMD",
            socketEnum,
            8,
            16,
            Frequency.FromGHz(4.2m),
            Frequency.FromGHz(5.0m),
            Power.FromWatts(tdp > 0 ? tdp : 120),
            false
        );
    }

    private Product CreateMotherboard(string name, string socket, string memoryType, string formFactor, int maxMemory = 128, int memorySlots = 4)
    {
        CpuSocket socketEnum = CpuSocketExtensions.Parse(socket);
        MemoryType memoryTypeEnum = Enum.Parse<MemoryType>(memoryType, ignoreCase: true);
        FormFactor formFactorEnum = Enum.Parse<FormFactor>(formFactor, ignoreCase: true);
        
        // Create RAM slots based on memorySlots parameter
        List<Slot> slots = [];
        for (int i = 0; i < memorySlots; i++)
        {
            slots.Add(new Slot(
                Guid.NewGuid(),
                $"RAM Slot {i + 1}",
                "RAM",
                new Vector3(i * 20, 0, 0),
                new Dimensions(150, 40, 10),
                null
            ));
        }
        
        return new MotherboardProduct(
            Guid.NewGuid(),
            name,
            299.99m,
            "ASUS",
            new Dimensions(305, 244, 50), // Standard ATX dimensions
            slots,
            socketEnum,
            "X670E",
            formFactorEnum,
            memoryTypeEnum,
            StorageCapacity.FromGB(maxMemory)
        );
    }

    private RamProduct CreateRam(string name, string type, int capacity, string configuration)
    {
        MemoryType memoryTypeEnum = Enum.Parse<MemoryType>(type, ignoreCase: true);
        return new RamProduct(
            Guid.NewGuid(),
            name,
            129.99m,
            "G.Skill",
            memoryTypeEnum,
            StorageCapacity.FromGB(capacity),
            configuration,
            Frequency.FromMHz(6000),
            "CL30",
            Voltage.FromVolts(1.35m)
        );
    }

    private GpuProduct CreateGpu(string name, int length, int tdp = 0)
    {
        return new GpuProduct(
            Guid.NewGuid(),
            name,
            999.99m,
            "NVIDIA",
            new Dimensions(length, 140, 61), // Typical GPU dimensions
            [], // No sub-slots for compatibility tests
            "NVIDIA",
            "RTX 4070",
            StorageCapacity.FromGB(12),
            MemoryType.GDDR6X,
            Frequency.FromMHz(2310),
            Frequency.FromMHz(2610),
            Power.FromWatts(tdp > 0 ? tdp : 300),
            Length.FromMm(length),
            GpuPowerConnector.Dual8Pin,
            true
        );
    }

    private PcCaseProduct CreateCase(string name, string formFactor)
    {
        return new PcCaseProduct(
            Guid.NewGuid(),
            name,
            169.99m,
            "Lian Li",
            new Dimensions(450, 220, 500), // Typical case dimensions
            [], // No chambers for compatibility tests
            formFactor,
            "Black",
            "Tempered Glass"
        );
    }

    private PsuProduct CreatePsu(string name, int wattage)
    {
        return new PsuProduct(
            Guid.NewGuid(),
            name,
            129.99m,
            "Corsair",
            Power.FromWatts(wattage),
            "80+ Gold",
            "Fully Modular",
            "ATX",
            Length.FromMm(160),
            6
        );
    }

    private CoolerProduct CreateCooler(string name, string type, int height, int tdp, string[] sockets)
    {
        CpuSocket[] socketEnums = sockets.Select(s => CpuSocketExtensions.Parse(s)).ToArray();
        CoolerType coolerType = Enum.Parse<CoolerType>(type, ignoreCase: true);
        return new CoolerProduct(
            Guid.NewGuid(),
            name,
            109.99m,
            "Noctua",
            new Dimensions(140, 150, height), // Typical cooler dimensions
            coolerType,
            Length.FromMm(height),
            Power.FromWatts(tdp),
            socketEnums
        );
    }

    #endregion
}
