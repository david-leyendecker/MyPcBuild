using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;
using MyPcBuild.ApiService.Features.Spatial;

namespace MyPcBuild.Tests.Services;

public class SpatialValidatorTests
{
    private readonly ISpatialValidator _validator;

    public SpatialValidatorTests()
    {
        _validator = new SpatialValidator();
    }

    #region Vector3 Tests

    [Fact]
    public void Vector3_Addition_CalculatesCorrectly()
    {
        // Arrange
        Vector3 a = new(10, 20, 30);
        Vector3 b = new(5, 15, 25);

        // Act
        Vector3 result = a + b;

        // Assert
        Assert.Equal(15, result.X);
        Assert.Equal(35, result.Y);
        Assert.Equal(55, result.Z);
    }

    [Fact]
    public void Vector3_Subtraction_CalculatesCorrectly()
    {
        // Arrange
        Vector3 a = new(10, 20, 30);
        Vector3 b = new(5, 15, 25);

        // Act
        Vector3 result = a - b;

        // Assert
        Assert.Equal(5, result.X);
        Assert.Equal(5, result.Y);
        Assert.Equal(5, result.Z);
    }

    #endregion

    #region Dimensions Tests

    [Fact]
    public void Dimensions_FitsWithin_ReturnsTrueWhenFits()
    {
        // Arrange
        Dimensions part = new(100, 50, 75);
        Dimensions container = new(150, 100, 100);

        // Act
        bool fits = part.FitsWithin(container);

        // Assert
        Assert.True(fits);
    }

    [Fact]
    public void Dimensions_FitsWithin_ReturnsFalseWhenTooLarge()
    {
        // Arrange
        Dimensions part = new(200, 50, 75);
        Dimensions container = new(150, 100, 100);

        // Act
        bool fits = part.FitsWithin(container);

        // Assert
        Assert.False(fits);
    }

    #endregion

    #region BoundingBox Tests

    [Fact]
    public void BoundingBox_Intersects_ReturnsTrueWhenOverlapping()
    {
        // Arrange
        BoundingBox box1 = new(new Vector3(0, 0, 0), new Dimensions(100, 100, 100));
        BoundingBox box2 = new(new Vector3(50, 50, 50), new Dimensions(100, 100, 100));

        // Act
        bool intersects = box1.Intersects(box2);

        // Assert
        Assert.True(intersects);
    }

    [Fact]
    public void BoundingBox_Intersects_ReturnsFalseWhenNotOverlapping()
    {
        // Arrange
        BoundingBox box1 = new(new Vector3(0, 0, 0), new Dimensions(100, 100, 100));
        BoundingBox box2 = new(new Vector3(200, 200, 200), new Dimensions(100, 100, 100));

        // Act
        bool intersects = box1.Intersects(box2);

        // Assert
        Assert.False(intersects);
    }

    [Fact]
    public void BoundingBox_IsContainedWithin_ReturnsTrueWhenFullyInside()
    {
        // Arrange
        BoundingBox inner = new(new Vector3(10, 10, 10), new Dimensions(50, 50, 50));
        BoundingBox outer = new(new Vector3(0, 0, 0), new Dimensions(100, 100, 100));

        // Act
        bool contained = inner.IsContainedWithin(outer);

        // Assert
        Assert.True(contained);
    }

    [Fact]
    public void BoundingBox_IsContainedWithin_ReturnsFalseWhenPartiallyOutside()
    {
        // Arrange
        BoundingBox inner = new(new Vector3(80, 80, 80), new Dimensions(50, 50, 50));
        BoundingBox outer = new(new Vector3(0, 0, 0), new Dimensions(100, 100, 100));

        // Act
        bool contained = inner.IsContainedWithin(outer);

        // Assert
        Assert.False(contained);
    }

    #endregion

    #region Spatial Validation Tests

    [Fact]
    public void ValidatePartInstallation_ValidPart_ReturnsSuccess()
    {
        // Arrange
        Chamber chamber = CreateTestChamber();
        Slot slot = chamber.Slots[0];
        Dimensions partDimensions = new(200, 150, 40); // Fits within ATX motherboard slot
        Vector3 partPosition = new(10, 10, 0);

        // Act
        SpatialValidationResult result = _validator.ValidatePartInstallation(
            chamber,
            slot.Id,
            partDimensions,
            partPosition
        );

        // Assert
        Assert.True(result.IsValid);
        Assert.False(result.HasErrors);
        Assert.Empty(result.Issues);
    }

    [Fact]
    public void ValidatePartInstallation_PartTooLarge_ReturnsError()
    {
        // Arrange
        Chamber chamber = CreateTestChamber();
        Slot slot = chamber.Slots[0];
        Dimensions partDimensions = new(400, 300, 100); // Too large for slot
        Vector3 partPosition = new(10, 10, 0);

        // Act
        SpatialValidationResult result = _validator.ValidatePartInstallation(
            chamber,
            slot.Id,
            partDimensions,
            partPosition
        );

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.Contains(result.Issues, i => i.Category == "Dimensions/Exceeded");
    }

    [Fact]
    public void ValidatePartInstallation_PartExceedsChamberBounds_ReturnsError()
    {
        // Arrange
        Chamber chamber = CreateTestChamber();
        Slot slot = chamber.Slots[0];
        Dimensions partDimensions = new(300, 200, 40);
        Vector3 partPosition = new(200, 100, 0); // Position + dimensions exceed chamber

        // Act
        SpatialValidationResult result = _validator.ValidatePartInstallation(
            chamber,
            slot.Id,
            partDimensions,
            partPosition
        );

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.Contains(result.Issues, i => i.Category == "Boundary/Exceeded");
    }

    [Fact]
    public void ValidatePartInstallation_CollidesWithExistingPart_ReturnsError()
    {
        // Arrange
        Chamber chamber = CreateTestChamber();
        Slot slot = chamber.Slots[0];
        
        // Add an existing part
        chamber.InstalledParts.Add(new InstalledPart
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            SlotId = slot.Id,
            Position = new Vector3(50, 50, 0),
            Dimensions = new Dimensions(100, 100, 50)
        });

        // Try to install a part that overlaps
        Dimensions partDimensions = new(100, 100, 50);
        Vector3 partPosition = new(100, 100, 0); // Overlaps with existing part

        // Act
        SpatialValidationResult result = _validator.ValidatePartInstallation(
            chamber,
            slot.Id,
            partDimensions,
            partPosition
        );

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.Contains(result.Issues, i => i.Category == "Collision/PartConflict");
    }

    [Fact]
    public void ValidatePartInstallation_InvalidSlotId_ReturnsError()
    {
        // Arrange
        Chamber chamber = CreateTestChamber();
        Guid invalidSlotId = Guid.NewGuid();
        Dimensions partDimensions = new(200, 150, 40);
        Vector3 partPosition = new(10, 10, 0);

        // Act
        SpatialValidationResult result = _validator.ValidatePartInstallation(
            chamber,
            invalidSlotId,
            partDimensions,
            partPosition
        );

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.Contains(result.Issues, i => i.Category == "Slot/NotFound");
    }

    [Fact]
    public void ValidateChamber_AllPartsValid_ReturnsSuccess()
    {
        // Arrange
        Chamber chamber = CreateTestChamber();
        Slot slot = chamber.Slots[0];
        
        chamber.InstalledParts.Add(new InstalledPart
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            SlotId = slot.Id,
            Position = new Vector3(10, 10, 0),
            Dimensions = new Dimensions(200, 150, 40)
        });

        // Act
        SpatialValidationResult result = _validator.ValidateChamber(chamber);

        // Assert
        Assert.True(result.IsValid);
        Assert.False(result.HasErrors);
        Assert.Empty(result.Issues);
    }

    [Fact]
    public void ValidateChamber_PartOutOfBounds_ReturnsError()
    {
        // Arrange
        Chamber chamber = CreateTestChamber();
        
        chamber.InstalledParts.Add(new InstalledPart
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            SlotId = chamber.Slots[0].Id,
            Position = new Vector3(350, 150, 0),
            Dimensions = new Dimensions(100, 100, 50) // Extends beyond 400mm chamber length
        });

        // Act
        SpatialValidationResult result = _validator.ValidateChamber(chamber);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.Contains(result.Issues, i => i.Category == "Boundary/Exceeded");
    }

    [Fact]
    public void ValidateChamber_PartsCollide_ReturnsError()
    {
        // Arrange
        Chamber chamber = CreateTestChamber();
        Slot slot = chamber.Slots[0];
        
        chamber.InstalledParts.Add(new InstalledPart
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            SlotId = slot.Id,
            Position = new Vector3(10, 10, 0),
            Dimensions = new Dimensions(100, 100, 50)
        });
        
        chamber.InstalledParts.Add(new InstalledPart
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            SlotId = slot.Id,
            Position = new Vector3(50, 50, 0), // Overlaps with first part
            Dimensions = new Dimensions(100, 100, 50)
        });

        // Act
        SpatialValidationResult result = _validator.ValidateChamber(chamber);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.Contains(result.Issues, i => i.Category == "Collision/PartConflict");
    }

    #endregion

    #region Slot Flattening Tests

    [Fact]
    public void Slot_FlattenSlots_IncludesAllSubSlots()
    {
        // Arrange
        Slot motherboardSlot = new()
        {
            Id = Guid.NewGuid(),
            Name = "Motherboard Slot",
            AllowedCategory = ProductCategory.Motherboard,
            RelativePosition = new Vector3(10, 10, 0),
            MaxDimensions = new Dimensions(305, 244, 50),
            SubSlots =
            [
                new Slot
                {
                    Id = Guid.NewGuid(),
                    Name = "CPU Slot",
                    AllowedCategory = ProductCategory.CPU,
                    RelativePosition = new Vector3(100, 100, 0),
                    MaxDimensions = new Dimensions(40, 40, 20)
                },
                new Slot
                {
                    Id = Guid.NewGuid(),
                    Name = "RAM Slot 1",
                    AllowedCategory = ProductCategory.RAM,
                    RelativePosition = new Vector3(150, 50, 0),
                    MaxDimensions = new Dimensions(133, 31, 40)
                }
            ]
        };

        // Act
        List<(Slot Slot, Vector3 GlobalPosition)> flattened = motherboardSlot.FlattenSlots(Vector3.Zero);

        // Assert
        Assert.Equal(3, flattened.Count); // Motherboard + 2 sub-slots
        Assert.Contains(flattened, s => s.Slot.Name == "Motherboard Slot");
        Assert.Contains(flattened, s => s.Slot.Name == "CPU Slot");
        Assert.Contains(flattened, s => s.Slot.Name == "RAM Slot 1");
    }

    [Fact]
    public void Chamber_GetAllSlots_FlattensSlotsCorrectly()
    {
        // Arrange
        Chamber chamber = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test Chamber",
            Dimensions = new Dimensions(400, 200, 450),
            Slots =
            [
                new Slot
                {
                    Id = Guid.NewGuid(),
                    Name = "Motherboard Slot",
                    AllowedCategory = ProductCategory.Motherboard,
                    RelativePosition = new Vector3(10, 10, 0),
                    MaxDimensions = new Dimensions(305, 244, 50),
                    SubSlots =
                    [
                        new Slot
                        {
                            Id = Guid.NewGuid(),
                            Name = "CPU Slot",
                            AllowedCategory = ProductCategory.CPU,
                            RelativePosition = new Vector3(100, 100, 0),
                            MaxDimensions = new Dimensions(40, 40, 20)
                        }
                    ]
                }
            ]
        };

        // Act
        List<(Slot Slot, Vector3 GlobalPosition)> allSlots = chamber.GetAllSlots();

        // Assert
        Assert.Equal(2, allSlots.Count); // Motherboard + CPU slot
    }

    #endregion

    #region Helper Methods

    private Chamber CreateTestChamber()
    {
        return new Chamber
        {
            Id = Guid.NewGuid(),
            Name = "Test PC Case",
            Dimensions = new Dimensions(400, 200, 450), // Typical ATX case dimensions
            Slots =
            [
                new Slot
                {
                    Id = Guid.NewGuid(),
                    Name = "Motherboard Slot",
                    AllowedCategory = ProductCategory.Motherboard,
                    RelativePosition = new Vector3(10, 10, 0),
                    MaxDimensions = new Dimensions(305, 244, 50) // ATX motherboard
                }
            ],
            InstalledParts = []
        };
    }

    #endregion
}
