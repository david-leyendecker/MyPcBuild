using Microsoft.Extensions.AI;
using MyPcBuild.ApiService.Catalog.Models;
using MyPcBuild.ApiService.Catalog.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyPcBuild.ApiService.Catalog.Services;

/// <summary>
/// Implementation of IAiProductGenerator using OpenAI for product generation.
/// Uses API Request DTOs for schema extraction and parsing.
/// </summary>
public class OpenAiProductGenerator(ILogger<OpenAiProductGenerator> logger, IChatClient chatClient, ProductCategoryPromptFields productCategoryPromptFields) : IAiProductGenerator
{
    private readonly ILogger<OpenAiProductGenerator> _logger = logger;
    private readonly IChatClient _chatClient = chatClient;
    private readonly ProductCategoryPromptFields _productCategoryPromptFields = productCategoryPromptFields;

    /// <inheritdoc/>
    public async Task<Product> GenerateProductAsync(ProductCategory category, string description, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating {Category} product from description: {Description}", category, description);

        string systemPrompt = BuildSystemPrompt(category);
        string userPrompt = $"Generate a {category} product based on this description: {description}";

        try
        {
            ChatResponse response = await _chatClient.GetResponseAsync(
                [
                    new ChatMessage(ChatRole.System, systemPrompt),
                    new ChatMessage(ChatRole.User, userPrompt)
                ],
                cancellationToken: cancellationToken
            );

            string jsonResponse = response.Text ?? throw new InvalidOperationException("Empty response from AI");

            _logger.LogDebug("AI Response: {Response}", jsonResponse);

            Product product = ParseProductFromJson(category, jsonResponse);

            _logger.LogInformation("Successfully generated {Category} product: {Name}", category, product.Name);

            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate product using AI");
            throw new InvalidOperationException($"Failed to generate product: {ex.Message}", ex);
        }
    }

    private string BuildSystemPrompt(ProductCategory category)
    {
        List<SystemPromptCategoryField> fields = _productCategoryPromptFields.GetFieldsForCategory(category);

        string categoryStructure = string.Join($",{Environment.NewLine}  ",
            fields.Select(f => $"\"{f.Name}\": {f.values}"));

        return $"""
        You are a PC hardware expert assistant. Generate product specifications in valid JSON format.
        Your response must be ONLY valid JSON with no markdown formatting, no code blocks, no additional text.

        The JSON must follow this exact structure:

        {categoryStructure}

        Respond with ONLY the JSON object, no other text.
        """;
    }

    private Product ParseProductFromJson(ProductCategory category, string jsonResponse)
    {
        string cleanedJson = CleanJsonResponse(jsonResponse);

        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters =
            {
                new ApiGpuPowerConnectorConverter(),
                new DimensionsModelConverter(),
                new SlotModelListConverter(),
                new ChamberModelListConverter(),
                new JsonStringEnumConverter()
            }
        };

        ProductRequest request = category switch
        {
            ProductCategory.CPU => JsonSerializer.Deserialize<CpuProductRequest>(cleanedJson, options)!,
            ProductCategory.Motherboard => JsonSerializer.Deserialize<MotherboardProductRequest>(cleanedJson, options)!,
            ProductCategory.GPU => JsonSerializer.Deserialize<GpuProductRequest>(cleanedJson, options)!,
            ProductCategory.RAM => JsonSerializer.Deserialize<RamProductRequest>(cleanedJson, options)!,
            ProductCategory.Case => JsonSerializer.Deserialize<PcCaseProductRequest>(cleanedJson, options)!,
            ProductCategory.PowerSupply => JsonSerializer.Deserialize<PsuProductRequest>(cleanedJson, options)!,
            ProductCategory.Storage => JsonSerializer.Deserialize<StorageProductRequest>(cleanedJson, options)!,
            ProductCategory.Cooler => JsonSerializer.Deserialize<CoolerProductRequest>(cleanedJson, options)!,
            _ => throw new ArgumentException($"Unknown category: {category}")
        };

        Guid id = Guid.NewGuid();
        string name = StripManufacturerPrefix(request.Name, request.Manufacturer);

        // Create a new request with cleaned name
        ProductRequest cleanedRequest = request with
        {
            Name = name
        };

        // Convert request to domain model with draft status
        Product product = ProductDtoMapper.ToDomain(cleanedRequest, id);
        
        return product with
        {
            IsDraft = true,
            PublishedAt = null
        };
    }

    private static string CleanJsonResponse(string jsonResponse)
    {
        string cleanedJson = jsonResponse.Trim();
        if (cleanedJson.StartsWith("```json"))
        {
            cleanedJson = cleanedJson[7..];
        }
        if (cleanedJson.StartsWith("```"))
        {
            cleanedJson = cleanedJson[3..];
        }
        if (cleanedJson.EndsWith("```"))
        {
            cleanedJson = cleanedJson[..^3];
        }
        return cleanedJson.Trim();
    }

    private static string StripManufacturerPrefix(string name, string manufacturer)
    {
        if (string.IsNullOrWhiteSpace(manufacturer))
        {
            return name;
        }

        if (!name.StartsWith(manufacturer, StringComparison.OrdinalIgnoreCase))
        {
            return name;
        }

        string withoutPrefix = name[manufacturer.Length..].TrimStart();
        return string.IsNullOrWhiteSpace(withoutPrefix) ? name : withoutPrefix;
    }
}
