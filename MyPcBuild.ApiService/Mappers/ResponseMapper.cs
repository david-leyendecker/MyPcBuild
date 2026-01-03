using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Models.Responses;
using MyPcBuild.ApiService.Services;

namespace MyPcBuild.ApiService.Mappers;

public interface IResponseMapper
{
    CompatibilityValidationResponse MapCompatibilityResult(
        CompatibilityResult result,
        IEnumerable<Product> products,
        string? buildId = null);
    
    ProductCatalogResponse MapProductCatalog(
        IEnumerable<Product> products,
        int totalCount,
        int currentPage,
        int pageSize,
        ProductCategory? categoryFilter = null,
        string? searchTerm = null);
    
    ProductResponse MapProduct(Product product);
    
    CategoryListResponse MapCategories(Dictionary<ProductCategory, int> productCounts);
    
    BuildCreatedResponse MapBuildCreated(Guid buildId, string name, Guid userId);
    
    BuildResponse MapBuild(Build build, List<Product> products, CompatibilityResult? compatibilityResult = null);
}

public class ResponseMapper : IResponseMapper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ResponseMapper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CompatibilityValidationResponse MapCompatibilityResult(
        CompatibilityResult result,
        IEnumerable<Product> products,
        string? buildId = null)
    {
        var productList = products.ToList();
        var componentSummary = CreateComponentSummary(productList);
        var issues = result.Issues.Select(i => new CompatibilityIssueDto
        {
            Message = i.Message,
            Severity = i.Severity.ToString(),
            Category = i.Category,
            Recommendation = GetRecommendation(i)
        }).ToList();

        var links = new List<LinkDto>();
        
        if (!string.IsNullOrEmpty(buildId))
        {
            links.Add(new LinkDto
            {
                Href = GetAbsoluteUrl($"/api/builds/{buildId}"),
                Rel = "build",
                Method = "GET"
            });
        }
        
        links.Add(new LinkDto
        {
            Href = GetAbsoluteUrl("/api/catalog/products"),
            Rel = "catalog",
            Method = "GET"
        });

        return new CompatibilityValidationResponse
        {
            IsCompatible = result.IsCompatible,
            HasErrors = result.HasErrors,
            HasWarnings = result.HasWarnings,
            Issues = issues,
            ComponentSummary = componentSummary,
            Links = links
        };
    }

    public ProductCatalogResponse MapProductCatalog(
        IEnumerable<Product> products,
        int totalCount,
        int currentPage,
        int pageSize,
        ProductCategory? categoryFilter = null,
        string? searchTerm = null)
    {
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Category = p.Category.ToString(),
            CategoryId = (int)p.Category,
            Price = p.Price,
            Manufacturer = p.Manufacturer,
            Specifications = p.Specifications,
            Links = new List<LinkDto>
            {
                new() { Href = GetAbsoluteUrl($"/api/catalog/products/{p.Id}"), Rel = "self", Method = "GET" },
                new() { Href = GetAbsoluteUrl($"/api/catalog/products?category={p.Category}"), Rel = "category", Method = "GET" }
            }
        }).ToList();

        var links = new List<LinkDto>
        {
            new() { Href = GetAbsoluteUrl($"/api/catalog/products?page={currentPage}&pageSize={pageSize}"), Rel = "self", Method = "GET" }
        };

        if (currentPage > 1)
        {
            links.Add(new LinkDto
            {
                Href = GetAbsoluteUrl($"/api/catalog/products?page={currentPage - 1}&pageSize={pageSize}"),
                Rel = "prev",
                Method = "GET"
            });
        }

        if (currentPage < totalPages)
        {
            links.Add(new LinkDto
            {
                Href = GetAbsoluteUrl($"/api/catalog/products?page={currentPage + 1}&pageSize={pageSize}"),
                Rel = "next",
                Method = "GET"
            });
        }

        return new ProductCatalogResponse
        {
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPage = currentPage,
            PageSize = pageSize,
            Products = productDtos,
            Filters = new FilterInfoDto
            {
                Category = categoryFilter?.ToString(),
                SearchTerm = searchTerm
            },
            Links = links
        };
    }

    public ProductResponse MapProduct(Product product)
    {
        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category.ToString(),
            CategoryId = (int)product.Category,
            Price = product.Price,
            Manufacturer = product.Manufacturer,
            Specifications = product.Specifications,
            Links = new List<LinkDto>
            {
                new() { Href = GetAbsoluteUrl($"/api/catalog/products/{product.Id}"), Rel = "self", Method = "GET" },
                new() { Href = GetAbsoluteUrl($"/api/catalog/products?category={product.Category}"), Rel = "category", Method = "GET" }
            }
        };

        return new ProductResponse
        {
            Product = productDto,
            Links = new List<LinkDto>
            {
                new() { Href = GetAbsoluteUrl("/api/catalog/products"), Rel = "catalog", Method = "GET" },
                new() { Href = GetAbsoluteUrl("/api/catalog/categories"), Rel = "categories", Method = "GET" }
            }
        };
    }

    public CategoryListResponse MapCategories(Dictionary<ProductCategory, int> productCounts)
    {
        var categories = Enum.GetValues<ProductCategory>()
            .Select(c => new CategoryDto
            {
                Id = (int)c,
                Name = c.ToString(),
                DisplayName = FormatCategoryName(c),
                ProductCount = productCounts.GetValueOrDefault(c, 0),
                Links = new List<LinkDto>
                {
                    new() { Href = GetAbsoluteUrl($"/api/catalog/products?category={c}"), Rel = "products", Method = "GET" }
                }
            }).ToList();

        return new CategoryListResponse
        {
            Categories = categories,
            Links = new List<LinkDto>
            {
                new() { Href = GetAbsoluteUrl("/api/catalog/categories"), Rel = "self", Method = "GET" },
                new() { Href = GetAbsoluteUrl("/api/catalog/products"), Rel = "all-products", Method = "GET" }
            }
        };
    }

    public BuildCreatedResponse MapBuildCreated(Guid buildId, string name, Guid userId)
    {
        return new BuildCreatedResponse
        {
            BuildId = buildId,
            Name = name,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Links = new List<LinkDto>
            {
                new() { Href = GetAbsoluteUrl($"/api/builds/{buildId}"), Rel = "self", Method = "GET" },
                new() { Href = GetAbsoluteUrl($"/api/builds/{buildId}/parts"), Rel = "add-part", Method = "POST" },
                new() { Href = GetAbsoluteUrl($"/api/builds/{buildId}/compatibility"), Rel = "validate", Method = "GET" }
            }
        };
    }

    public BuildResponse MapBuild(Build build, List<Product> products, CompatibilityResult? compatibilityResult = null)
    {
        var parts = build.Parts.Select(p =>
        {
            var product = products.FirstOrDefault(prod => prod.Id == p.ProductId);
            return new BuildPartDto
            {
                ProductId = p.ProductId,
                ProductName = product?.Name ?? "Unknown Product",
                Category = product?.Category.ToString() ?? "Unknown",
                PricePaid = p.PricePaid,
                Manufacturer = product?.Manufacturer ?? "Unknown",
                Links = new List<LinkDto>
                {
                    new() { Href = GetAbsoluteUrl($"/api/catalog/products/{p.ProductId}"), Rel = "product", Method = "GET" },
                    new() { Href = GetAbsoluteUrl($"/api/builds/{build.Id}/parts/{p.ProductId}"), Rel = "remove", Method = "DELETE" }
                }
            };
        }).ToList();

        CompatibilityStatusDto? compatStatus = null;
        if (compatibilityResult != null)
        {
            compatStatus = new CompatibilityStatusDto
            {
                IsCompatible = compatibilityResult.IsCompatible,
                ErrorCount = compatibilityResult.Issues.Count(i => i.Severity == IssueSeverity.Error),
                WarningCount = compatibilityResult.Issues.Count(i => i.Severity == IssueSeverity.Warning)
            };
        }

        return new BuildResponse
        {
            Id = build.Id,
            Name = build.Name,
            UserId = build.UserId,
            Parts = parts,
            TotalPrice = parts.Sum(p => p.PricePaid),
            Version = build.Version,
            CompatibilityStatus = compatStatus,
            Links = new List<LinkDto>
            {
                new() { Href = GetAbsoluteUrl($"/api/builds/{build.Id}"), Rel = "self", Method = "GET" },
                new() { Href = GetAbsoluteUrl($"/api/builds/{build.Id}/parts"), Rel = "add-part", Method = "POST" },
                new() { Href = GetAbsoluteUrl($"/api/builds/{build.Id}/compatibility"), Rel = "validate", Method = "GET" },
                new() { Href = GetAbsoluteUrl("/api/catalog/products"), Rel = "catalog", Method = "GET" }
            }
        };
    }

    private ComponentSummaryDto CreateComponentSummary(List<Product> products)
    {
        var categoryGroups = products.GroupBy(p => p.Category)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        return new ComponentSummaryDto
        {
            TotalComponents = products.Count,
            ComponentsByCategory = categoryGroups,
            HasCpu = products.Any(p => p.Category == ProductCategory.CPU),
            HasMotherboard = products.Any(p => p.Category == ProductCategory.Motherboard),
            HasGpu = products.Any(p => p.Category == ProductCategory.GPU),
            HasRam = products.Any(p => p.Category == ProductCategory.RAM),
            HasCase = products.Any(p => p.Category == ProductCategory.PCCase),
            HasPsu = products.Any(p => p.Category == ProductCategory.PSU),
            HasCooler = products.Any(p => p.Category == ProductCategory.Cooler),
            HasStorage = products.Any(p => p.Category == ProductCategory.Storage)
        };
    }

    private string? GetRecommendation(Services.CompatibilityIssue issue)
    {
        return issue.Category switch
        {
            "CPU/Motherboard" when issue.Severity == IssueSeverity.Error => 
                "Choose a motherboard with matching CPU socket or select a different CPU",
            "RAM/Motherboard" when issue.Message.Contains("DDR") => 
                "Select RAM that matches your motherboard's memory type",
            "GPU/Case" when issue.Severity == IssueSeverity.Error => 
                "Choose a larger case or a smaller GPU",
            "PSU" when issue.Severity == IssueSeverity.Error => 
                "Upgrade to a higher wattage power supply",
            _ => null
        };
    }

    private string FormatCategoryName(ProductCategory category)
    {
        return category switch
        {
            ProductCategory.CPU => "Processors",
            ProductCategory.Motherboard => "Motherboards",
            ProductCategory.GPU => "Graphics Cards",
            ProductCategory.RAM => "Memory",
            ProductCategory.PCCase => "Cases",
            ProductCategory.PSU => "Power Supplies",
            ProductCategory.Storage => "Storage",
            ProductCategory.Cooler => "CPU Coolers",
            _ => category.ToString()
        };
    }

    private string GetAbsoluteUrl(string relativePath)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null) return relativePath;

        return $"{request.Scheme}://{request.Host}{relativePath}";
    }
}
