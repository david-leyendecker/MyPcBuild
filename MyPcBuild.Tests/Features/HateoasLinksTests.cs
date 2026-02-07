using MyPcBuild.ApiService.Features.Builds;
using MyPcBuild.ApiService.Features.Catalog;
using MyPcBuild.ApiService.Features.Compatibility;
using MyPcBuild.ApiService.Features.Spatial;
using MyPcBuild.ApiService.Infrastructure;
using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.Tests.Features;

public class HateoasLinksTests
{
    // ==================== HateoasLink Model Tests ====================

    [Fact]
    public void HateoasLink_CreatesCorrectly()
    {
        // Arrange & Act
        HateoasLink link = new("https://api.example.com/api/builds/123", "self", "GET");

        // Assert
        Assert.Equal("https://api.example.com/api/builds/123", link.Href);
        Assert.Equal("self", link.Rel);
        Assert.Equal("GET", link.Method);
    }

    [Fact]
    public void HateoasLink_SupportsAllHttpMethods()
    {
        // Arrange & Act
        HateoasLink getLink = new("/api/builds", "self", "GET");
        HateoasLink postLink = new("/api/builds", "create", "POST");
        HateoasLink putLink = new("/api/products/1", "update", "PUT");
        HateoasLink deleteLink = new("/api/builds/1/parts/2", "remove", "DELETE");

        // Assert
        Assert.Equal("GET", getLink.Method);
        Assert.Equal("POST", postLink.Method);
        Assert.Equal("PUT", putLink.Method);
        Assert.Equal("DELETE", deleteLink.Method);
    }

    // ==================== Builds Response HATEOAS Tests ====================

    [Fact]
    public void GetBuildsResponse_ContainsLinks()
    {
        // Arrange & Act
        GetBuildsResponse response = new(
            [
                new GetBuildsResponseItem(
                    Guid.NewGuid(),
                    "Gaming PC",
                    1500m,
                    [new HateoasLink("/api/builds/1", "self", "GET")]
                )
            ],
            [
                new HateoasLink("/api/builds", "self", "GET"),
                new HateoasLink("/api/builds", "create-build", "POST")
            ]
        );

        // Assert
        Assert.NotEmpty(response.Links);
        Assert.Contains(response.Links, l => l.Rel == "self");
        Assert.Contains(response.Links, l => l.Rel == "create-build");
    }

    [Fact]
    public void GetBuildsResponseItem_ContainsSelfLink()
    {
        // Arrange & Act
        Guid buildId = Guid.NewGuid();
        GetBuildsResponseItem item = new(
            buildId,
            "My Build",
            999m,
            [
                new HateoasLink($"/api/builds/{buildId}", "self", "GET"),
                new HateoasLink($"/api/builds/{buildId}/parts", "add-part", "POST"),
                new HateoasLink($"/api/builds/{buildId}/compatibility", "validate", "GET")
            ]
        );

        // Assert
        Assert.Equal(3, item.Links.Count);
        Assert.Contains(item.Links, l => l.Rel == "self" && l.Method == "GET");
        Assert.Contains(item.Links, l => l.Rel == "add-part" && l.Method == "POST");
        Assert.Contains(item.Links, l => l.Rel == "validate" && l.Method == "GET");
    }

    [Fact]
    public void GetBuildResponse_ContainsHateoasLinks()
    {
        // Arrange
        Guid buildId = Guid.NewGuid();

        // Act
        GetBuildResponse response = new(
            buildId,
            "Gaming PC",
            Guid.NewGuid(),
            [],
            true,
            [],
            DateTimeOffset.UtcNow,
            [
                new HateoasLink($"/api/builds/{buildId}", "self", "GET"),
                new HateoasLink($"/api/builds/{buildId}/parts", "add-part", "POST"),
                new HateoasLink($"/api/builds/{buildId}/compatibility", "validate", "GET"),
                new HateoasLink($"/api/builds/{buildId}/slots", "available-slots", "GET"),
                new HateoasLink("/api/catalog/products", "catalog", "GET")
            ]
        );

        // Assert
        Assert.Equal(5, response.Links.Count);
        Assert.Contains(response.Links, l => l.Rel == "self");
        Assert.Contains(response.Links, l => l.Rel == "add-part");
        Assert.Contains(response.Links, l => l.Rel == "validate");
        Assert.Contains(response.Links, l => l.Rel == "available-slots");
        Assert.Contains(response.Links, l => l.Rel == "catalog");
    }

    [Fact]
    public void ProductDetails_ContainsHateoasLinks()
    {
        // Arrange & Act
        Guid productId = Guid.NewGuid();
        Guid buildId = Guid.NewGuid();

        ProductDetails details = new(
            productId,
            "AMD Ryzen 9 7950X",
            ProductCategory.CPU,
            "AMD",
            549.99m,
            null,
            null,
            null,
            null,
            null,
            null,
            [
                new HateoasLink($"/api/catalog/products/{productId}", "product", "GET"),
                new HateoasLink($"/api/builds/{buildId}/parts/{productId}", "remove", "DELETE")
            ]
        );

        // Assert
        Assert.Equal(2, details.Links.Count);
        Assert.Contains(details.Links, l => l.Rel == "product" && l.Method == "GET");
        Assert.Contains(details.Links, l => l.Rel == "remove" && l.Method == "DELETE");
    }

    [Fact]
    public void CreateBuildResponse_ContainsHateoasLinks()
    {
        // Arrange
        Guid buildId = Guid.NewGuid();

        // Act
        CreateBuildResponse response = new(
            buildId,
            "New Build",
            Guid.NewGuid(),
            [
                new HateoasLink($"/api/builds/{buildId}", "self", "GET"),
                new HateoasLink($"/api/builds/{buildId}/parts", "add-part", "POST"),
                new HateoasLink($"/api/builds/{buildId}/compatibility", "validate", "GET")
            ]
        );

        // Assert
        Assert.Equal(3, response.Links.Count);
        Assert.Contains(response.Links, l => l.Rel == "self");
        Assert.Contains(response.Links, l => l.Rel == "add-part");
        Assert.Contains(response.Links, l => l.Rel == "validate");
    }

    [Fact]
    public void AddPartResponse_ContainsHateoasLinks()
    {
        // Arrange & Act
        Guid buildId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();

        AddPartResponse response = new(
            "Part added successfully",
            [
                new HateoasLink($"/api/builds/{buildId}", "build", "GET"),
                new HateoasLink($"/api/builds/{buildId}/parts/{productId}", "remove", "DELETE"),
                new HateoasLink($"/api/builds/{buildId}/compatibility", "validate", "GET"),
                new HateoasLink($"/api/catalog/products/{productId}", "product", "GET")
            ]
        );

        // Assert
        Assert.Equal(4, response.Links.Count);
        Assert.Contains(response.Links, l => l.Rel == "build");
        Assert.Contains(response.Links, l => l.Rel == "remove");
        Assert.Contains(response.Links, l => l.Rel == "validate");
        Assert.Contains(response.Links, l => l.Rel == "product");
    }

    [Fact]
    public void RemovePartResponse_ContainsHateoasLinks()
    {
        // Arrange & Act
        RemovePartResponse response = new(
            "Part removed successfully",
            [
                new HateoasLink("/api/builds/1", "build", "GET"),
                new HateoasLink("/api/builds/1/parts", "add-part", "POST"),
                new HateoasLink("/api/builds/1/compatibility", "validate", "GET"),
                new HateoasLink("/api/catalog/products", "catalog", "GET")
            ]
        );

        // Assert
        Assert.Equal(4, response.Links.Count);
        Assert.Contains(response.Links, l => l.Rel == "build");
        Assert.Contains(response.Links, l => l.Rel == "add-part");
        Assert.Contains(response.Links, l => l.Rel == "catalog");
    }

    [Fact]
    public void GetAvailableSlotsResponse_ContainsHateoasLinks()
    {
        // Arrange & Act
        Guid buildId = Guid.NewGuid();

        GetAvailableSlotsResponse response = new(
            [],
            [
                new HateoasLink($"/api/builds/{buildId}/slots", "self", "GET"),
                new HateoasLink($"/api/builds/{buildId}", "build", "GET"),
                new HateoasLink($"/api/builds/{buildId}/parts/slot", "add-part-to-slot", "POST")
            ]
        );

        // Assert
        Assert.Equal(3, response.Links.Count);
        Assert.Contains(response.Links, l => l.Rel == "self");
        Assert.Contains(response.Links, l => l.Rel == "build");
        Assert.Contains(response.Links, l => l.Rel == "add-part-to-slot");
    }

    // ==================== Catalog Response HATEOAS Tests ====================

    [Fact]
    public void GetProductsResponse_ContainsHateoasLinks()
    {
        // Arrange & Act
        GetProductsResponse response = new(
            [],
            new PaginationMetadata { Total = 0, Page = 1, ItemsPerPage = 20 },
            [
                new HateoasLink("/api/catalog/products?page=1&itemsPerPage=20", "self", "GET"),
                new HateoasLink("/api/catalog/categories", "categories", "GET"),
                new HateoasLink("/api/catalog/products", "create-product", "POST")
            ]
        );

        // Assert
        Assert.Equal(3, response.Links.Count);
        Assert.Contains(response.Links, l => l.Rel == "self");
        Assert.Contains(response.Links, l => l.Rel == "categories");
        Assert.Contains(response.Links, l => l.Rel == "create-product");
    }

    [Fact]
    public void ProductSummary_ContainsHateoasLinks()
    {
        // Arrange & Act
        Guid productId = Guid.NewGuid();

        ProductSummary summary = new(
            productId,
            "AMD Ryzen 9 7950X",
            "CPU",
            549.99m,
            "AMD",
            false,
            DateTime.UtcNow,
            [
                new HateoasLink($"/api/catalog/products/{productId}", "self", "GET"),
                new HateoasLink("/api/catalog/products?filters=ProductCategory=CPU", "category", "GET")
            ]
        );

        // Assert
        Assert.Equal(2, summary.Links.Count);
        Assert.Contains(summary.Links, l => l.Rel == "self");
        Assert.Contains(summary.Links, l => l.Rel == "category");
    }

    [Fact]
    public void GetProductByIdResponse_ContainsHateoasLinks()
    {
        // Arrange & Act
        Guid productId = Guid.NewGuid();

        GetProductByIdResponse response = new(
            null!,
            [
                new HateoasLink($"/api/catalog/products/{productId}", "self", "GET"),
                new HateoasLink($"/api/catalog/products/{productId}", "update", "PUT"),
                new HateoasLink("/api/catalog/products?filters=ProductCategory=CPU", "category", "GET"),
                new HateoasLink("/api/catalog/products", "all-products", "GET"),
                new HateoasLink("/api/catalog/categories", "categories", "GET")
            ]
        );

        // Assert
        Assert.Equal(5, response.Links.Count);
        Assert.Contains(response.Links, l => l.Rel == "self");
        Assert.Contains(response.Links, l => l.Rel == "update");
        Assert.Contains(response.Links, l => l.Rel == "category");
        Assert.Contains(response.Links, l => l.Rel == "all-products");
        Assert.Contains(response.Links, l => l.Rel == "categories");
    }

    [Fact]
    public void GetCategoriesResponse_ContainsHateoasLinks()
    {
        // Arrange & Act
        GetCategoriesResponse response = new(
            [
                new CategoryInfo("CPU", "Processors", 4,
                [
                    new HateoasLink("/api/catalog/products?filters=ProductCategory=CPU", "products", "GET"),
                    new HateoasLink("/api/catalog/field-definitions/CPU", "field-definitions", "GET")
                ])
            ],
            [
                new HateoasLink("/api/catalog/categories", "self", "GET"),
                new HateoasLink("/api/catalog/products", "all-products", "GET")
            ]
        );

        // Assert
        Assert.Equal(2, response.Links.Count);
        Assert.Contains(response.Links, l => l.Rel == "self");
        Assert.Contains(response.Links, l => l.Rel == "all-products");

        // Category items also have links
        CategoryInfo category = response.Categories[0];
        Assert.Equal(2, category.Links.Count);
        Assert.Contains(category.Links, l => l.Rel == "products");
        Assert.Contains(category.Links, l => l.Rel == "field-definitions");
    }

    [Fact]
    public void CreateProductResponse_ContainsHateoasLinks()
    {
        // Arrange & Act
        Guid productId = Guid.NewGuid();

        CreateProductResponse response = new(
            productId,
            [
                new HateoasLink($"/api/catalog/products/{productId}", "self", "GET"),
                new HateoasLink($"/api/catalog/products/{productId}", "update", "PUT"),
                new HateoasLink($"/api/catalog/products/{productId}/publish", "publish", "POST"),
                new HateoasLink("/api/catalog/products", "all-products", "GET")
            ]
        );

        // Assert
        Assert.Equal(4, response.Links.Count);
        Assert.Contains(response.Links, l => l.Rel == "self");
        Assert.Contains(response.Links, l => l.Rel == "update");
        Assert.Contains(response.Links, l => l.Rel == "publish");
        Assert.Contains(response.Links, l => l.Rel == "all-products");
    }

    [Fact]
    public void SearchProductsResponse_ContainsHateoasLinks()
    {
        // Arrange & Act
        SearchProductsResponse response = new(
            [],
            [
                new HateoasLink("/api/catalog/search?query=ryzen&maxResults=10", "self", "GET"),
                new HateoasLink("/api/catalog/products", "all-products", "GET"),
                new HateoasLink("/api/catalog/categories", "categories", "GET")
            ]
        );

        // Assert
        Assert.Equal(3, response.Links.Count);
        Assert.Contains(response.Links, l => l.Rel == "self");
        Assert.Contains(response.Links, l => l.Rel == "all-products");
        Assert.Contains(response.Links, l => l.Rel == "categories");
    }

    [Fact]
    public void GetFieldDefinitionsResponse_ContainsHateoasLinks()
    {
        // Arrange & Act
        GetFieldDefinitionsResponse response = new(
            ProductCategory.CPU,
            [],
            [
                new HateoasLink("/api/catalog/field-definitions/CPU", "self", "GET"),
                new HateoasLink("/api/catalog/products?filters=ProductCategory=CPU", "products", "GET"),
                new HateoasLink("/api/catalog/categories", "categories", "GET"),
                new HateoasLink("/api/catalog/products", "create-product", "POST")
            ]
        );

        // Assert
        Assert.Equal(4, response.Links.Count);
        Assert.Contains(response.Links, l => l.Rel == "self");
        Assert.Contains(response.Links, l => l.Rel == "products");
        Assert.Contains(response.Links, l => l.Rel == "categories");
        Assert.Contains(response.Links, l => l.Rel == "create-product");
    }

    // ==================== Compatibility Response HATEOAS Tests ====================

    [Fact]
    public void ValidateCompatibilityResponse_ContainsHateoasLinks()
    {
        // Arrange & Act
        ValidateCompatibilityResponse response = new(
            true,
            false,
            false,
            [],
            [
                new ProductInfo(Guid.NewGuid(), "AMD Ryzen 9", ProductCategory.CPU,
                    [new HateoasLink("/api/catalog/products/1", "product", "GET")])
            ],
            [
                new HateoasLink("/api/compatibility/validate", "self", "POST"),
                new HateoasLink("/api/catalog/products", "catalog", "GET")
            ]
        );

        // Assert
        Assert.Equal(2, response.Links.Count);
        Assert.Contains(response.Links, l => l.Rel == "self");
        Assert.Contains(response.Links, l => l.Rel == "catalog");

        // Product items also have links
        ProductInfo product = response.ValidatedProducts[0];
        Assert.Single(product.Links);
        Assert.Contains(product.Links, l => l.Rel == "product");
    }

    [Fact]
    public void GetBuildCompatibilityResponse_ContainsHateoasLinks()
    {
        // Arrange
        Guid buildId = Guid.NewGuid();

        // Act
        GetBuildCompatibilityResponse response = new(
            buildId,
            "Gaming PC",
            true,
            false,
            false,
            [],
            [],
            [
                new HateoasLink($"/api/builds/{buildId}/compatibility", "self", "GET"),
                new HateoasLink($"/api/builds/{buildId}", "build", "GET"),
                new HateoasLink($"/api/builds/{buildId}/parts", "add-part", "POST"),
                new HateoasLink("/api/catalog/products", "catalog", "GET")
            ]
        );

        // Assert
        Assert.Equal(4, response.Links.Count);
        Assert.Contains(response.Links, l => l.Rel == "self");
        Assert.Contains(response.Links, l => l.Rel == "build");
        Assert.Contains(response.Links, l => l.Rel == "add-part");
        Assert.Contains(response.Links, l => l.Rel == "catalog");
    }

    // ==================== Spatial Response HATEOAS Tests ====================

    [Fact]
    public void ValidatePartInstallationResponse_ContainsHateoasLinks()
    {
        // Arrange
        Guid buildId = Guid.NewGuid();

        // Act
        ValidatePartInstallationResponse response = new(
            true,
            false,
            false,
            [],
            [
                new HateoasLink($"/api/builds/{buildId}/parts/validate", "self", "POST"),
                new HateoasLink($"/api/builds/{buildId}", "build", "GET"),
                new HateoasLink($"/api/builds/{buildId}/parts", "add-part", "POST"),
                new HateoasLink($"/api/builds/{buildId}/slots", "available-slots", "GET"),
                new HateoasLink($"/api/builds/{buildId}/validate", "validate-build", "POST")
            ]
        );

        // Assert
        Assert.Equal(5, response.Links.Count);
        Assert.Contains(response.Links, l => l.Rel == "self");
        Assert.Contains(response.Links, l => l.Rel == "build");
        Assert.Contains(response.Links, l => l.Rel == "add-part");
        Assert.Contains(response.Links, l => l.Rel == "available-slots");
        Assert.Contains(response.Links, l => l.Rel == "validate-build");
    }

    [Fact]
    public void ValidateBuildSpatialResponse_ContainsHateoasLinks()
    {
        // Arrange
        Guid buildId = Guid.NewGuid();

        // Act
        ValidateBuildSpatialResponse response = new(
            true,
            false,
            false,
            [],
            [
                new HateoasLink($"/api/builds/{buildId}/validate", "self", "POST"),
                new HateoasLink($"/api/builds/{buildId}", "build", "GET"),
                new HateoasLink($"/api/builds/{buildId}/parts", "add-part", "POST"),
                new HateoasLink($"/api/builds/{buildId}/slots", "available-slots", "GET"),
                new HateoasLink($"/api/builds/{buildId}/compatibility", "validate-compatibility", "GET")
            ]
        );

        // Assert
        Assert.Equal(5, response.Links.Count);
        Assert.Contains(response.Links, l => l.Rel == "self");
        Assert.Contains(response.Links, l => l.Rel == "build");
        Assert.Contains(response.Links, l => l.Rel == "add-part");
        Assert.Contains(response.Links, l => l.Rel == "available-slots");
        Assert.Contains(response.Links, l => l.Rel == "validate-compatibility");
    }

    // ==================== Link Relations Consistency Tests ====================

    [Fact]
    public void HateoasLinks_UseConsistentRelNames()
    {
        // Verify that standard rel names are used consistently
        string[] standardRels = ["self", "build", "catalog", "product", "category", "products",
            "add-part", "remove", "validate", "all-products", "categories", "prev", "next",
            "create-build", "create-product", "update", "publish", "available-slots",
            "add-part-to-slot", "validate-build", "validate-compatibility", "field-definitions"];

        // All standard rels should be non-empty strings
        foreach (string rel in standardRels)
        {
            Assert.False(string.IsNullOrWhiteSpace(rel));
            Assert.DoesNotContain(" ", rel);
        }
    }

    [Fact]
    public void HateoasLinks_UseValidHttpMethods()
    {
        // Arrange
        string[] validMethods = ["GET", "POST", "PUT", "DELETE", "PATCH"];

        List<HateoasLink> testLinks =
        [
            new HateoasLink("/api/builds", "self", "GET"),
            new HateoasLink("/api/builds", "create", "POST"),
            new HateoasLink("/api/builds/1", "update", "PUT"),
            new HateoasLink("/api/builds/1", "remove", "DELETE")
        ];

        // Assert
        foreach (HateoasLink link in testLinks)
        {
            Assert.Contains(link.Method, validMethods);
        }
    }
}
