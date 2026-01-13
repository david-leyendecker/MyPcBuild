using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;
using MyPcBuild.ApiService.Features.Catalog;

namespace MyPcBuild.Tests.Services;

public class DraftProductTests
{
    [Fact]
    public void Product_DefaultIsDraft_IsFalse()
    {
        // Arrange & Act
        CpuProduct product = new CpuProduct(
            Guid.NewGuid(),
            "Test CPU",
            399.99m,
            "AMD",
            CpuSocket.AM5,
            8,
            16,
            Frequency.FromGHz(4.2m),
            Frequency.FromGHz(5.0m),
            Power.FromWatts(120),
            false
        );

        // Assert
        Assert.False(product.IsDraft);
        Assert.Null(product.PublishedAt);
    }

    [Fact]
    public void Product_CanBeMarkedAsDraft()
    {
        // Arrange
        CpuProduct product = new CpuProduct(
            Guid.NewGuid(),
            "Test CPU",
            399.99m,
            "AMD",
            CpuSocket.AM5,
            8,
            16,
            Frequency.FromGHz(4.2m),
            Frequency.FromGHz(5.0m),
            Power.FromWatts(120),
            false
        );

        // Act
        Product draftProduct = product with { IsDraft = true };

        // Assert
        Assert.True(draftProduct.IsDraft);
        Assert.Null(draftProduct.PublishedAt);
    }

    [Fact]
    public void Product_CanBePublished()
    {
        // Arrange
        CpuProduct draftProduct = new CpuProduct(
            Guid.NewGuid(),
            "Test CPU",
            399.99m,
            "AMD",
            CpuSocket.AM5,
            8,
            16,
            Frequency.FromGHz(4.2m),
            Frequency.FromGHz(5.0m),
            Power.FromWatts(120),
            false
        )
        { IsDraft = true };

        DateTime publishTime = DateTime.UtcNow;

        // Act
        Product publishedProduct = draftProduct with 
        { 
            IsDraft = false, 
            PublishedAt = publishTime 
        };

        // Assert
        Assert.False(publishedProduct.IsDraft);
        Assert.NotNull(publishedProduct.PublishedAt);
        Assert.Equal(publishTime, publishedProduct.PublishedAt);
    }

    [Fact]
    public void ProductSummary_IncludesDraftInformation()
    {
        // Arrange & Act
        DateTime publishTime = DateTime.UtcNow;
        ProductSummary summary = new ProductSummary(
            Guid.NewGuid(),
            "Test Product",
            "CPU",
            399.99m,
            "AMD",
            true,
            null
        );

        ProductSummary publishedSummary = new ProductSummary(
            Guid.NewGuid(),
            "Published Product",
            "GPU",
            999.99m,
            "NVIDIA",
            false,
            publishTime
        );

        // Assert
        Assert.True(summary.IsDraft);
        Assert.Null(summary.PublishedAt);
        
        Assert.False(publishedSummary.IsDraft);
        Assert.NotNull(publishedSummary.PublishedAt);
        Assert.Equal(publishTime, publishedSummary.PublishedAt);
    }
}
