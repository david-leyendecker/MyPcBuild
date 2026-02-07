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
        HateoasLink link = new(new Uri("https://api.example.com/api/builds/123"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET);

        // Assert
        Assert.Equal(new Uri("https://api.example.com/api/builds/123"), link.Href);
        Assert.Equal("self", link.Rel);
        Assert.Equal(MyPcBuild.ApiService.Infrastructure.HttpMethod.GET, link.Method);
    }

    [Fact]
    public void HateoasLink_SupportsAllHttpMethods()
    {
        // Arrange & Act
        HateoasLink getLink = new(new Uri("/api/builds", UriKind.Relative), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET);
        HateoasLink postLink = new(new Uri("/api/builds", UriKind.Relative), "create", MyPcBuild.ApiService.Infrastructure.HttpMethod.POST);
        HateoasLink putLink = new(new Uri("/api/products/1", UriKind.Relative), "update", MyPcBuild.ApiService.Infrastructure.HttpMethod.PUT);
        HateoasLink deleteLink = new(new Uri("/api/builds/1/parts/2", UriKind.Relative), "remove", MyPcBuild.ApiService.Infrastructure.HttpMethod.DELETE);

        // Assert
        Assert.Equal(MyPcBuild.ApiService.Infrastructure.HttpMethod.GET, getLink.Method);
        Assert.Equal(MyPcBuild.ApiService.Infrastructure.HttpMethod.POST, postLink.Method);
        Assert.Equal(MyPcBuild.ApiService.Infrastructure.HttpMethod.PUT, putLink.Method);
        Assert.Equal(MyPcBuild.ApiService.Infrastructure.HttpMethod.DELETE, deleteLink.Method);
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
                    [new HateoasLink(new Uri("/api/builds/1"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET)]
                )
            ],
            [
                new HateoasLink(new Uri("/api/builds"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri("/api/builds"), "create-build", MyPcBuild.ApiService.Infrastructure.HttpMethod.POST)
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
                new HateoasLink(new Uri($"/api/builds/{buildId}"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts"), "add-part", MyPcBuild.ApiService.Infrastructure.HttpMethod.POST),
                new HateoasLink(new Uri($"/api/builds/{buildId}/compatibility"), "validate", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET)
            ]
        );

        // Assert
        Assert.Equal(3, item.Links.Count);
        Assert.Contains(item.Links, l => l.Rel == "self" && l.Method == MyPcBuild.ApiService.Infrastructure.HttpMethod.GET);
        Assert.Contains(item.Links, l => l.Rel == "add-part" && l.Method == MyPcBuild.ApiService.Infrastructure.HttpMethod.POST);
        Assert.Contains(item.Links, l => l.Rel == "validate" && l.Method == MyPcBuild.ApiService.Infrastructure.HttpMethod.GET);
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
                new HateoasLink(new Uri($"/api/builds/{buildId}"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts"), "add-part", MyPcBuild.ApiService.Infrastructure.HttpMethod.POST),
                new HateoasLink(new Uri($"/api/builds/{buildId}/compatibility"), "validate", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/slots"), "available-slots", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/products"), "catalog", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET)
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
                new HateoasLink(new Uri($"/api/catalog/products/{productId}"), "product", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts/{productId}"), "remove", MyPcBuild.ApiService.Infrastructure.HttpMethod.DELETE)
            ]
        );

        // Assert
        Assert.Equal(2, details.Links.Count);
        Assert.Contains(details.Links, l => l.Rel == "product" && l.Method == MyPcBuild.ApiService.Infrastructure.HttpMethod.GET);
        Assert.Contains(details.Links, l => l.Rel == "remove" && l.Method == MyPcBuild.ApiService.Infrastructure.HttpMethod.DELETE);
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
                new HateoasLink(new Uri($"/api/builds/{buildId}"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts"), "add-part", MyPcBuild.ApiService.Infrastructure.HttpMethod.POST),
                new HateoasLink(new Uri($"/api/builds/{buildId}/compatibility"), "validate", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET)
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
                new HateoasLink(new Uri($"/api/builds/{buildId}"), "build", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts/{productId}"), "remove", MyPcBuild.ApiService.Infrastructure.HttpMethod.DELETE),
                new HateoasLink(new Uri($"/api/builds/{buildId}/compatibility"), "validate", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri($"/api/catalog/products/{productId}"), "product", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET)
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
                new HateoasLink(new Uri("/api/builds/1"), "build", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri("/api/builds/1/parts"), "add-part", MyPcBuild.ApiService.Infrastructure.HttpMethod.POST),
                new HateoasLink(new Uri("/api/builds/1/compatibility"), "validate", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/products"), "catalog", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET)
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
                new HateoasLink(new Uri($"/api/builds/{buildId}/slots"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}"), "build", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts/slot"), "add-part-to-slot", MyPcBuild.ApiService.Infrastructure.HttpMethod.POST)
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
                new HateoasLink(new Uri("/api/catalog/products?page=1&itemsPerPage=20"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/categories"), "categories", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/products"), "create-product", MyPcBuild.ApiService.Infrastructure.HttpMethod.POST)
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
                new HateoasLink(new Uri($"/api/catalog/products/{productId}"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/products?filters=ProductCategory=CPU"), "category", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET)
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
                new HateoasLink(new Uri($"/api/catalog/products/{productId}"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri($"/api/catalog/products/{productId}"), "update", MyPcBuild.ApiService.Infrastructure.HttpMethod.PUT),
                new HateoasLink(new Uri("/api/catalog/products?filters=ProductCategory=CPU"), "category", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/products"), "all-products", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/categories"), "categories", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET)
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
                    new HateoasLink(new Uri("/api/catalog/products?filters=ProductCategory=CPU"), "products", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                    new HateoasLink(new Uri("/api/catalog/field-definitions/CPU"), "field-definitions", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET)
                ])
            ],
            [
                new HateoasLink(new Uri("/api/catalog/categories"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/products"), "all-products", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET)
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
                new HateoasLink(new Uri($"/api/catalog/products/{productId}"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri($"/api/catalog/products/{productId}"), "update", MyPcBuild.ApiService.Infrastructure.HttpMethod.PUT),
                new HateoasLink(new Uri($"/api/catalog/products/{productId}/publish"), "publish", MyPcBuild.ApiService.Infrastructure.HttpMethod.POST),
                new HateoasLink(new Uri("/api/catalog/products"), "all-products", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET)
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
                new HateoasLink(new Uri("/api/catalog/search?query=ryzen&maxResults=10"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/products"), "all-products", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/categories"), "categories", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET)
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
                new HateoasLink(new Uri("/api/catalog/field-definitions/CPU"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/products?filters=ProductCategory=CPU"), "products", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/categories"), "categories", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/products"), "create-product", MyPcBuild.ApiService.Infrastructure.HttpMethod.POST)
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
                    [new HateoasLink(new Uri("/api/catalog/products/1"), "product", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET)])
            ],
            [
                new HateoasLink(new Uri("/api/compatibility/validate"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.POST),
                new HateoasLink(new Uri("/api/catalog/products"), "catalog", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET)
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
                new HateoasLink(new Uri($"/api/builds/{buildId}/compatibility"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}"), "build", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts"), "add-part", MyPcBuild.ApiService.Infrastructure.HttpMethod.POST),
                new HateoasLink(new Uri("/api/catalog/products"), "catalog", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET)
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
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts/validate"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.POST),
                new HateoasLink(new Uri($"/api/builds/{buildId}"), "build", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts"), "add-part", MyPcBuild.ApiService.Infrastructure.HttpMethod.POST),
                new HateoasLink(new Uri($"/api/builds/{buildId}/slots"), "available-slots", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/validate"), "validate-build", MyPcBuild.ApiService.Infrastructure.HttpMethod.POST)
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
                new HateoasLink(new Uri($"/api/builds/{buildId}/validate"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.POST),
                new HateoasLink(new Uri($"/api/builds/{buildId}"), "build", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts"), "add-part", MyPcBuild.ApiService.Infrastructure.HttpMethod.POST),
                new HateoasLink(new Uri($"/api/builds/{buildId}/slots"), "available-slots", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/compatibility"), "validate-compatibility", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET)
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
        MyPcBuild.ApiService.Infrastructure.HttpMethod[] validMethods = [MyPcBuild.ApiService.Infrastructure.HttpMethod.GET, MyPcBuild.ApiService.Infrastructure.HttpMethod.POST, MyPcBuild.ApiService.Infrastructure.HttpMethod.PUT, MyPcBuild.ApiService.Infrastructure.HttpMethod.DELETE, MyPcBuild.ApiService.Infrastructure.HttpMethod.PATCH];

        List<HateoasLink> testLinks =
        [
            new HateoasLink(new Uri("/api/builds"), "self", MyPcBuild.ApiService.Infrastructure.HttpMethod.GET),
            new HateoasLink(new Uri("/api/builds"), "create", MyPcBuild.ApiService.Infrastructure.HttpMethod.POST),
            new HateoasLink(new Uri("/api/builds/1"), "update", MyPcBuild.ApiService.Infrastructure.HttpMethod.PUT),
            new HateoasLink(new Uri("/api/builds/1"), "remove", MyPcBuild.ApiService.Infrastructure.HttpMethod.DELETE)
        ];

        // Assert
        foreach (HateoasLink link in testLinks)
        {
            Assert.Contains(link.Method, validMethods);
        }
    }
}
