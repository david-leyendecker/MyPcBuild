using Microsoft.Extensions.AI;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Features.Catalog.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyPcBuild.ApiService.Features.Catalog;

/// <summary>
/// Implementation of IAiProductGenerator using OpenAI for product generation.
/// Uses API DTOs for schema extraction and parsing.
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
                new JsonStringEnumConverter()
            }
        };

        ProductDto dto = category switch
        {
            ProductCategory.CPU => JsonSerializer.Deserialize<CpuDto>(cleanedJson, options)!,
            ProductCategory.Motherboard => JsonSerializer.Deserialize<MotherboardDto>(cleanedJson, options)!,
            ProductCategory.GPU => JsonSerializer.Deserialize<GpuDto>(cleanedJson, options)!,
            ProductCategory.RAM => JsonSerializer.Deserialize<RamDto>(cleanedJson, options)!,
            ProductCategory.Case => JsonSerializer.Deserialize<PcCaseDto>(cleanedJson, options)!,
            ProductCategory.PowerSupply => JsonSerializer.Deserialize<PsuDto>(cleanedJson, options)!,
            ProductCategory.Storage => JsonSerializer.Deserialize<StorageDto>(cleanedJson, options)!,
            ProductCategory.Cooler => JsonSerializer.Deserialize<CoolerDto>(cleanedJson, options)!,
            _ => throw new ArgumentException($"Unknown category: {category}")
        };

        Guid id = Guid.NewGuid();
        string name = StripManufacturerPrefix(dto.Name, dto.Manufacturer);

        // Set DTO properties for draft status
        ProductDto draftDto = dto with
        {
            Id = id,
            Name = name,
            IsDraft = true,
            PublishedAt = null
        };

        // Convert DTO to domain model
        return ProductDtoMapper.ToDomain(draftDto, id);
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