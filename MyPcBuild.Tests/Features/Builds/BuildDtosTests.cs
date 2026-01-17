using Xunit;
using MyPcBuild.ApiService.Features.Builds;
using MyPcBuild.ApiService.Domain.Models.Spatial;

namespace MyPcBuild.Tests.Features.Builds;

public class BuildDtosTests
{
    [Fact]
    public void Vector3Dto_CreatesCorrectly()
    {
        // Arrange & Act
        Vector3Dto vector = new(10.5m, 20.3m, 30.7m);

        // Assert
        Assert.Equal(10.5m, vector.X);
        Assert.Equal(20.3m, vector.Y);
        Assert.Equal(30.7m, vector.Z);
    }

    [Fact]
    public void DimensionsDto_CreatesCorrectly()
    {
        // Arrange & Act
        DimensionsDto dimensions = new(100, 200, 300);

        // Assert
        Assert.Equal(100, dimensions.Length);
        Assert.Equal(200, dimensions.Width);
        Assert.Equal(300, dimensions.Height);
    }

    [Fact]
    public void SlotDto_CreatesCorrectly()
    {
        // Arrange
        Guid slotId = Guid.NewGuid();
        Vector3Dto position = new(0, 0, 0);
        DimensionsDto dimensions = new(50, 50, 50);

        // Act
        SlotDto slot = new(
            slotId,
            "Test Slot",
            "CPU",
            position,
            dimensions
        );

        // Assert
        Assert.Equal(slotId, slot.Id);
        Assert.Equal("Test Slot", slot.Name);
        Assert.Equal("CPU", slot.AllowedCategory);
        Assert.Equal(position, slot.RelativePosition);
        Assert.Equal(dimensions, slot.MaxDimensions);
    }

    [Fact]
    public void ChamberDto_CreatesCorrectly()
    {
        // Arrange
        Guid chamberId = Guid.NewGuid();
        DimensionsDto dimensions = new(400, 260, 450);
        List<SlotDto> slots = [];

        // Act
        ChamberDto chamber = new(
            chamberId,
            "Main Chamber",
            dimensions,
            slots
        );

        // Assert
        Assert.Equal(chamberId, chamber.Id);
        Assert.Equal("Main Chamber", chamber.Name);
        Assert.Equal(dimensions, chamber.Dimensions);
        Assert.Empty(chamber.Slots);
    }

    [Fact]
    public void AvailableSlotDto_CreatesCorrectly()
    {
        // Arrange
        Guid slotId = Guid.NewGuid();
        Guid parentProductId = Guid.NewGuid();
        Vector3Dto position = new(10, 20, 30);
        DimensionsDto dimensions = new(50, 50, 50);

        // Act
        AvailableSlotDto availableSlot = new(
            slotId,
            "CPU Slot",
            "CPU",
            position,
            dimensions,
            false,
            parentProductId,
            "ASUS Motherboard"
        );

        // Assert
        Assert.Equal(slotId, availableSlot.Id);
        Assert.Equal("CPU Slot", availableSlot.Name);
        Assert.Equal("CPU", availableSlot.AllowedCategory);
        Assert.Equal(position, availableSlot.AbsolutePosition);
        Assert.Equal(dimensions, availableSlot.MaxDimensions);
        Assert.False(availableSlot.IsOccupied);
        Assert.Equal(parentProductId, availableSlot.ParentProductId);
        Assert.Equal("ASUS Motherboard", availableSlot.ParentProductName);
    }
}
