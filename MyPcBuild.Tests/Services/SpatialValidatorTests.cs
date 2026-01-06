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
        (Build build, List<Product> products) = CreateTestBuildWithCase();
        Product motherboard = CreateMotherboard();
        products.Add(motherboard);
        
        ChamberedProduct pcCase = (ChamberedProduct)products[0];
        Guid mbSlotId = pcCase.Chambers[0].Slots[0].Id;
        Vector3 position = new(10, 10, 0);

        // Act
        SpatialValidationResult result = _validator.ValidatePartInstallation(
            build,
            products,
            motherboard.Id,
            mbSlotId,
            position
        );

        // Assert
        Assert.True(result.IsValid, $"Validation failed with {result.Issues.Count} issues: {string.Join(", ", result.Issues.Select(i => i.Message))}");
        Assert.False(result.HasErrors);
        Assert.Empty(result.Issues);
    }

    [Fact]
    public void ValidatePartInstallation_PartTooLarge_ReturnsError()
    {
        // Arrange
        (Build build, List<Product> products) = CreateTestBuildWithCase();
        Product oversizedBoard = CreateOversizedMotherboard();
        products.Add(oversizedBoard);
        
        ChamberedProduct pcCase = (ChamberedProduct)products[0];
        Guid mbSlotId = pcCase.Chambers[0].Slots[0].Id;
        Vector3 position = new(10, 10, 0);

        // Act
        SpatialValidationResult result = _validator.ValidatePartInstallation(
            build,
            products,
            oversizedBoard.Id,
            mbSlotId,
            position
        );

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.Contains(result.Issues, i => i.Category == "Dimensions/Exceeded");
    }

    [Fact]
    public void ValidatePartInstallation_ProductNotFound_ReturnsError()
    {
        // Arrange
        (Build build, List<Product> products) = CreateTestBuildWithCase();
        Guid invalidProductId = Guid.NewGuid();
        ChamberedProduct pcCase = (ChamberedProduct)products[0];
        Guid mbSlotId = pcCase.Chambers[0].Slots[0].Id;
        Vector3 position = new(10, 10, 0);

        // Act
        SpatialValidationResult result = _validator.ValidatePartInstallation(
            build,
            products,
            invalidProductId,
            mbSlotId,
            position
        );

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.Contains(result.Issues, i => i.Category == "Product/NotFound");
    }

    [Fact]
    public void ValidateBuild_CollisionDetected_ReturnsError()
    {
        // Arrange
        (Build build, List<Product> products) = CreateTestBuildWithCase();
        
        // Add two overlapping parts to the build
        Product mb1 = CreateMotherboard();
        Product mb2 = CreateMotherboard();
        products.Add(mb1);
        products.Add(mb2);
        
        build.Parts.Add(new BuildPart(mb1.Id, 299.99m, Guid.NewGuid(), new Vector3(10, 10, 0)));
        build.Parts.Add(new BuildPart(mb2.Id, 299.99m, Guid.NewGuid(), new Vector3(20, 20, 0))); // Overlaps

        // Act
        SpatialValidationResult result = _validator.ValidateBuild(build, products);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.HasErrors);
        Assert.Contains(result.Issues, i => i.Category == "Collision/PartConflict");
    }

    #endregion

    #region Helper Methods

    private (Build, List<Product>) CreateTestBuildWithCase()
    {
        Build build = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test Build",
            UserId = Guid.NewGuid(),
            Parts = []
        };

        Product pcCase = CreatePCCase();
        
        // Add the case to the build
        build.Parts.Add(new BuildPart(pcCase.Id, 169.99m, null, null));

        return (build, [pcCase]);
    }

    private Product CreatePCCase()
    {
        Guid mbSlotId = Guid.NewGuid();
        
        return new ChamberedProduct(
            Guid.NewGuid(),
            "Test PC Case",
            ProductCategory.PCCase,
            169.99m,
            "Test Mfg",
            new Dictionary<string, object>
            {
                ["FormFactor"] = "ATX"
            },
            new Dimensions(450, 220, 500),
            [
                new Chamber(
                    Guid.NewGuid(),
                    "Main Chamber",
                    new Dimensions(400, 260, 450), // Big enough for ATX (305x244) at offset (10,10)
                    [
                        new Slot(
                            mbSlotId,
                            "Motherboard Slot",
                            ProductCategory.Motherboard,
                            new Vector3(10, 10, 0),
                            new Dimensions(305, 244, 50)
                        )
                    ]
                )
            ]
        );
    }

    private Product CreateMotherboard()
    {
        return new SpatialProduct(
            Guid.NewGuid(),
            "ASUS X670E",
            ProductCategory.Motherboard,
            299.99m,
            "ASUS",
            new Dictionary<string, object>
            {
                ["Socket"] = "AM5",
                ["FormFactor"] = "ATX"
            },
            new Dimensions(305, 244, 50)
        );
    }

    private Product CreateOversizedMotherboard()
    {
        return new SpatialProduct(
            Guid.NewGuid(),
            "Oversized Board",
            ProductCategory.Motherboard,
            399.99m,
            "Test",
            new Dictionary<string, object>
            {
                ["Socket"] = "AM5",
                ["FormFactor"] = "EATX"
            },
            new Dimensions(400, 300, 100) // Too large
        );
    }

    #endregion
}
