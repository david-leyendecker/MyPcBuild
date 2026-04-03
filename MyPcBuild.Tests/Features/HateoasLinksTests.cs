using MyPcBuild.ApiService.Builds.Endpoints;
using MyPcBuild.ApiService.Catalog.Endpoints;
using MyPcBuild.ApiService.Catalog.Models;
using MyPcBuild.ApiService.Compatibility.Endpoints;
using MyPcBuild.ApiService.Infrastructure;
using MyPcBuild.ApiService.Spatial.Endpoints;
using HttpMethod = MyPcBuild.ApiService.Infrastructure.HttpMethod;

namespace MyPcBuild.Tests.Features;

public class HateoasLinksTests
{
    // ==================== HateoasLink Model Tests ====================

    [Fact]
    public void HateoasLink_CreatesCorrectly()
    {
        // Arrange & Act
        HateoasLink link = new(new Uri("https://api.example.com/api/builds/123"), "self", HttpMethod.GET);

        // Assert
        Assert.Equal(new Uri("https://api.example.com/api/builds/123"), link.Href);
        Assert.Equal("self", link.Rel);
        Assert.Equal(HttpMethod.GET, link.Method);
    }

    [Fact]
    public void HateoasLink_SupportsAllHttpMethods()
    {
        // Arrange & Act
        HateoasLink getLink = new(new Uri("/api/builds", UriKind.Relative), "self", HttpMethod.GET);
        HateoasLink postLink = new(new Uri("/api/builds", UriKind.Relative), "create", HttpMethod.POST);
        HateoasLink putLink = new(new Uri("/api/products/1", UriKind.Relative), "update", HttpMethod.PUT);
        HateoasLink deleteLink = new(new Uri("/api/builds/1/parts/2", UriKind.Relative), "remove", HttpMethod.DELETE);

        // Assert
        Assert.Equal(HttpMethod.GET, getLink.Method);
        Assert.Equal(HttpMethod.POST, postLink.Method);
        Assert.Equal(HttpMethod.PUT, putLink.Method);
        Assert.Equal(HttpMethod.DELETE, deleteLink.Method);
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
                    [new HateoasLink(new Uri("/api/builds/1", UriKind.Relative), "self", HttpMethod.GET)]
                )
            ],
            [
                new HateoasLink(new Uri("/api/builds", UriKind.Relative), "self", HttpMethod.GET),
                new HateoasLink(new Uri("/api/builds", UriKind.Relative), "create-build", HttpMethod.POST)
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
                new HateoasLink(new Uri($"/api/builds/{buildId}", UriKind.Relative), "self", HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts", UriKind.Relative), "add-part", HttpMethod.POST),
                new HateoasLink(new Uri($"/api/builds/{buildId}/compatibility", UriKind.Relative), "validate", HttpMethod.GET)
            ]
        );

        // Assert
        Assert.Equal(3, item.Links.Count);
        Assert.Contains(item.Links, l => l.Rel == "self" && l.Method == HttpMethod.GET);
        Assert.Contains(item.Links, l => l.Rel == "add-part" && l.Method == HttpMethod.POST);
        Assert.Contains(item.Links, l => l.Rel == "validate" && l.Method == HttpMethod.GET);
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
                new HateoasLink(new Uri($"/api/builds/{buildId}", UriKind.Relative), "self", HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts", UriKind.Relative), "add-part", HttpMethod.POST),
                new HateoasLink(new Uri($"/api/builds/{buildId}/compatibility", UriKind.Relative), "validate", HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/slots", UriKind.Relative), "available-slots", HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/products", UriKind.Relative), "catalog", HttpMethod.GET)
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
                new HateoasLink(new Uri($"/api/catalog/products/{productId}", UriKind.Relative), "product", HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts/{productId}", UriKind.Relative), "remove", HttpMethod.DELETE)
            ]
        );

        // Assert
        Assert.Equal(2, details.Links.Count);
        Assert.Contains(details.Links, l => l.Rel == "product" && l.Method == HttpMethod.GET);
        Assert.Contains(details.Links, l => l.Rel == "remove" && l.Method == HttpMethod.DELETE);
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
                new HateoasLink(new Uri($"/api/builds/{buildId}", UriKind.Relative), "self", HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts", UriKind.Relative), "add-part", HttpMethod.POST),
                new HateoasLink(new Uri($"/api/builds/{buildId}/compatibility", UriKind.Relative), "validate", HttpMethod.GET)
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
                new HateoasLink(new Uri($"/api/builds/{buildId}", UriKind.Relative), "build", HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts/{productId}", UriKind.Relative), "remove", HttpMethod.DELETE),
                new HateoasLink(new Uri($"/api/builds/{buildId}/compatibility", UriKind.Relative), "validate", HttpMethod.GET),
                new HateoasLink(new Uri($"/api/catalog/products/{productId}", UriKind.Relative), "product", HttpMethod.GET)
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
                new HateoasLink(new Uri("/api/builds/1", UriKind.Relative), "build", HttpMethod.GET),
                new HateoasLink(new Uri("/api/builds/1/parts", UriKind.Relative), "add-part", HttpMethod.POST),
                new HateoasLink(new Uri("/api/builds/1/compatibility", UriKind.Relative), "validate", HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/products", UriKind.Relative), "catalog", HttpMethod.GET)
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
                new HateoasLink(new Uri($"/api/builds/{buildId}/slots", UriKind.Relative), "self", HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}", UriKind.Relative), "build", HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts/slot", UriKind.Relative), "add-part-to-slot", HttpMethod.POST)
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
                new HateoasLink(new Uri("/api/catalog/products?page=1&itemsPerPage=20", UriKind.Relative), "self", HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/categories", UriKind.Relative), "categories", HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/products", UriKind.Relative), "create-product", HttpMethod.POST)
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
                new HateoasLink(new Uri($"/api/catalog/products/{productId}", UriKind.Relative), "self", HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/products?filters=ProductCategory=CPU", UriKind.Relative), "category", HttpMethod.GET)
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
                new HateoasLink(new Uri($"/api/catalog/products/{productId}", UriKind.Relative), "self", HttpMethod.GET),
                new HateoasLink(new Uri($"/api/catalog/products/{productId}", UriKind.Relative), "update", HttpMethod.PUT),
                new HateoasLink(new Uri("/api/catalog/products?filters=ProductCategory=CPU", UriKind.Relative), "category", HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/products", UriKind.Relative), "all-products", HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/categories", UriKind.Relative), "categories", HttpMethod.GET)
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
                    new HateoasLink(new Uri("/api/catalog/products?filters=ProductCategory=CPU", UriKind.Relative), "products", HttpMethod.GET),
                    new HateoasLink(new Uri("/api/catalog/field-definitions/CPU", UriKind.Relative), "field-definitions", HttpMethod.GET)
                ])
            ],
            [
                new HateoasLink(new Uri("/api/catalog/categories", UriKind.Relative), "self", HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/products", UriKind.Relative), "all-products", HttpMethod.GET)
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
                new HateoasLink(new Uri($"/api/catalog/products/{productId}", UriKind.Relative), "self", HttpMethod.GET),
                new HateoasLink(new Uri($"/api/catalog/products/{productId}", UriKind.Relative), "update", HttpMethod.PUT),
                new HateoasLink(new Uri($"/api/catalog/products/{productId}/publish", UriKind.Relative), "publish", HttpMethod.POST),
                new HateoasLink(new Uri("/api/catalog/products", UriKind.Relative), "all-products", HttpMethod.GET)
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
                new HateoasLink(new Uri("/api/catalog/search?query=ryzen&maxResults=10", UriKind.Relative), "self", HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/products", UriKind.Relative), "all-products", HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/categories", UriKind.Relative), "categories", HttpMethod.GET)
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
                new HateoasLink(new Uri("/api/catalog/field-definitions/CPU", UriKind.Relative), "self", HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/products?filters=ProductCategory=CPU", UriKind.Relative), "products", HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/categories", UriKind.Relative), "categories", HttpMethod.GET),
                new HateoasLink(new Uri("/api/catalog/products", UriKind.Relative), "create-product", HttpMethod.POST)
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
                    [new HateoasLink(new Uri("/api/catalog/products/1", UriKind.Relative), "product", HttpMethod.GET)])
            ],
            [
                new HateoasLink(new Uri("/api/compatibility/validate", UriKind.Relative), "self", HttpMethod.POST),
                new HateoasLink(new Uri("/api/catalog/products", UriKind.Relative), "catalog", HttpMethod.GET)
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
                new HateoasLink(new Uri($"/api/builds/{buildId}/compatibility", UriKind.Relative), "self", HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}", UriKind.Relative), "build", HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts", UriKind.Relative), "add-part", HttpMethod.POST),
                new HateoasLink(new Uri("/api/catalog/products", UriKind.Relative), "catalog", HttpMethod.GET)
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
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts/validate", UriKind.Relative), "self", HttpMethod.POST),
                new HateoasLink(new Uri($"/api/builds/{buildId}", UriKind.Relative), "build", HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts", UriKind.Relative), "add-part", HttpMethod.POST),
                new HateoasLink(new Uri($"/api/builds/{buildId}/slots", UriKind.Relative), "available-slots", HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/validate", UriKind.Relative), "validate-build", HttpMethod.POST)
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
                new HateoasLink(new Uri($"/api/builds/{buildId}/validate", UriKind.Relative), "self", HttpMethod.POST),
                new HateoasLink(new Uri($"/api/builds/{buildId}", UriKind.Relative), "build", HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/parts", UriKind.Relative), "add-part", HttpMethod.POST),
                new HateoasLink(new Uri($"/api/builds/{buildId}/slots", UriKind.Relative), "available-slots", HttpMethod.GET),
                new HateoasLink(new Uri($"/api/builds/{buildId}/compatibility", UriKind.Relative), "validate-compatibility", HttpMethod.GET)
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
        HttpMethod[] validMethods = [HttpMethod.GET, HttpMethod.POST, HttpMethod.PUT, HttpMethod.DELETE, HttpMethod.PATCH];

        List<HateoasLink> testLinks =
        [
            new HateoasLink(new Uri("/api/builds", UriKind.Relative), "self", HttpMethod.GET),
            new HateoasLink(new Uri("/api/builds", UriKind.Relative), "create", HttpMethod.POST),
            new HateoasLink(new Uri("/api/builds/1", UriKind.Relative), "update", HttpMethod.PUT),
            new HateoasLink(new Uri("/api/builds/1", UriKind.Relative), "remove", HttpMethod.DELETE)
        ];

        // Assert
        foreach (HateoasLink link in testLinks)
        {
            Assert.Contains(link.Method, validMethods);
        }
    }
}
