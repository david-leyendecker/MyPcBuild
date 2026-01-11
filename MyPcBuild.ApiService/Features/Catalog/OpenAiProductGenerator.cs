using Microsoft.Extensions.AI;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;
using System.Text.Json;

namespace MyPcBuild.ApiService.Features.Catalog;

/// <summary>
/// Implementation of IAiProductGenerator using OpenAI for product generation.
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

            // Get the text content from the response message
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

        string categoryStructure = fields.Aggregate(string.Empty, (acc, field) =>
            acc + $"\n  \"{field.Name}\": {field.values},"
        ).TrimEnd(',');

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
        // Clean up the response in case it has markdown code blocks
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
        cleanedJson = cleanedJson.Trim();

        using JsonDocument doc = JsonDocument.Parse(cleanedJson);
        JsonElement root = doc.RootElement;

        Guid id = Guid.NewGuid();
        string name = root.GetStringProperty("Name");
        decimal price = root.GetDecimalProperty("Price");
        string manufacturer = root.GetStringProperty("Manufacturer");

        Product product = category switch
        {
            ProductCategory.CPU => ParseCpuProduct(id, name, price, manufacturer, root),
            ProductCategory.Motherboard => ParseMotherboardProduct(id, name, price, manufacturer, root),
            ProductCategory.GPU => ParseGpuProduct(id, name, price, manufacturer, root),
            ProductCategory.RAM => ParseRamProduct(id, name, price, manufacturer, root),
            ProductCategory.Case => ParsePcCaseProduct(id, name, price, manufacturer, root),
            ProductCategory.PowerSupply => ParsePsuProduct(id, name, price, manufacturer, root),
            ProductCategory.Storage => ParseStorageProduct(id, name, price, manufacturer, root),
            ProductCategory.Cooler => ParseCoolerProduct(id, name, price, manufacturer, root),
            _ => throw new ArgumentException($"Unknown category: {category}")
        };

        // Mark as draft and not yet published
        return product with { IsDraft = true, PublishedAt = null };
    }

    private CpuProduct ParseCpuProduct(Guid id, string name, decimal price, string manufacturer, JsonElement root)
    {
        return new CpuProduct(
            id,
            name,
            price,
            manufacturer,
            ParseEnum<CpuSocket>(root.GetStringProperty("Socket")),
            root.GetIntProperty("Cores"),
            root.GetIntProperty("Threads"),
            Frequency.FromGHz(root.GetDecimalProperty("BaseClock")),
            Frequency.FromGHz(root.GetDecimalProperty("BoostClock")),
            Power.FromWatts(root.GetIntProperty("TDP")),
            root.GetBoolProperty("IntegratedGraphics")
        );
    }

    private MotherboardProduct ParseMotherboardProduct(Guid id, string name, decimal price, string manufacturer, JsonElement root)
    {
        return new MotherboardProduct(
            id,
            name,
            price,
            manufacturer,
            ParseDimensions(root.GetStringProperty("Dimensions", "305,244,50")),
            ParseSlots(root.GetStringProperty("Slots", "[]")),
            ParseEnum<CpuSocket>(root.GetStringProperty("Socket")),
            root.GetStringProperty("Chipset"),
            ParseEnum<FormFactor>(root.GetStringProperty("FormFactor")),
            ParseEnum<MemoryType>(root.GetStringProperty("MemoryType")),
            StorageCapacity.FromGB(root.GetIntProperty("MaxMemory"))
        );
    }

    private GpuProduct ParseGpuProduct(Guid id, string name, decimal price, string manufacturer, JsonElement root)
    {
        return new GpuProduct(
            id,
            name,
            price,
            manufacturer,
            ParseDimensions(root.GetStringProperty("Dimensions", "300,120,50")),
            ParseSlots(root.GetStringProperty("Slots", "[]")),
            root.GetStringProperty("ChipsetManufacturer"),
            root.GetStringProperty("Series"),
            StorageCapacity.FromGB(root.GetIntProperty("VRAM")),
            ParseEnum<MemoryType>(root.GetStringProperty("MemoryType")),
            Frequency.FromMHz(root.GetIntProperty("CoreClock")),
            Frequency.FromMHz(root.GetIntProperty("BoostClock")),
            Power.FromWatts(root.GetIntProperty("TDP")),
            Length.FromMm(root.GetIntProperty("Length")),
            ParseGpuPowerConnector(root.GetStringProperty("PowerConnectors")),
            root.GetBoolProperty("RayTracing")
        );
    }

    private RamProduct ParseRamProduct(Guid id, string name, decimal price, string manufacturer, JsonElement root)
    {
        return new RamProduct(
            id,
            name,
            price,
            manufacturer,
            ParseEnum<MemoryType>(root.GetStringProperty("Type")),
            StorageCapacity.FromGB(root.GetIntProperty("Capacity")),
            root.GetStringProperty("Configuration"),
            Frequency.FromMHz(root.GetIntProperty("Speed")),
            root.GetStringProperty("CASLatency"),
            Voltage.FromVolts(root.GetDecimalProperty("Voltage"))
        );
    }

    private PcCaseProduct ParsePcCaseProduct(Guid id, string name, decimal price, string manufacturer, JsonElement root)
    {
        return new PcCaseProduct(
            id,
            name,
            price,
            manufacturer,
            ParseDimensions(root.GetStringProperty("Dimensions", "500,230,480")),
            ParseChambers(root.GetStringProperty("Chambers", "[]")),
            root.GetStringProperty("FormFactor"),
            root.GetStringProperty("Color"),
            root.GetStringProperty("SidePanelWindow")
        );
    }

    private PsuProduct ParsePsuProduct(Guid id, string name, decimal price, string manufacturer, JsonElement root)
    {
        return new PsuProduct(
            id,
            name,
            price,
            manufacturer,
            Power.FromWatts(root.GetIntProperty("Wattage")),
            root.GetStringProperty("Efficiency"),
            root.GetStringProperty("Modular"),
            root.GetStringProperty("FormFactor"),
            Length.FromMm(root.GetIntProperty("Length")),
            root.GetIntProperty("PCIe8Pin")
        );
    }

    private StorageProduct ParseStorageProduct(Guid id, string name, decimal price, string manufacturer, JsonElement root)
    {
        return new StorageProduct(
            id,
            name,
            price,
            manufacturer,
            root.GetStringProperty("Type"),
            root.GetStringProperty("Interface"),
            root.GetStringProperty("StorageFormFactor"),
            StorageCapacity.FromGB(root.GetIntProperty("Capacity")),
            DataSpeed.FromMBps(root.GetIntProperty("ReadSpeed")),
            DataSpeed.FromMBps(root.GetIntProperty("WriteSpeed"))
        );
    }

    private CoolerProduct ParseCoolerProduct(Guid id, string name, decimal price, string manufacturer, JsonElement root)
    {
        string socketsStr = root.GetStringProperty("Sockets");
        string[] socketArr = socketsStr.Split(',', StringSplitOptions.RemoveEmptyEntries);
        CpuSocket[] sockets = [.. socketArr.Select(s => ParseEnum<CpuSocket>(s.Trim()))];

        return new CoolerProduct(
            id,
            name,
            price,
            manufacturer,
            ParseDimensions(root.GetStringProperty("Dimensions", "140,140,160")),
            ParseEnum<CoolerType>(root.GetStringProperty("CoolerType")),
            Length.FromMm(root.GetIntProperty("Height")),
            Power.FromWatts(root.GetIntProperty("TDP")),
            sockets
        );
    }

    // Helper methods for JSON parsing

    private Dimensions ParseDimensions(string value)
    {
        string[] parts = value.Split(',');
        if (parts.Length != 3)
        {
            return Dimensions.Zero;
        }

        return new Dimensions(
            decimal.Parse(parts[0].Trim()),
            decimal.Parse(parts[1].Trim()),
            decimal.Parse(parts[2].Trim())
        );
    }

    private List<Slot> ParseSlots(string json)
    {
        // For AI-generated products, we'll keep slots empty for simplicity
        // Advanced spatial editing can be done after publishing
        return [];
    }

    private List<Chamber> ParseChambers(string json)
    {
        // For AI-generated products, we'll keep chambers empty for simplicity
        // Advanced spatial editing can be done after publishing
        return [];
    }

    private T ParseEnum<T>(string value) where T : struct, Enum
    {
        if (Enum.TryParse<T>(value, ignoreCase: true, out T result))
        {
            return result;
        }
        throw new ArgumentException($"Invalid enum value: {value} for type {typeof(T).Name}");
    }

    private GpuPowerConnector ParseGpuPowerConnector(string value)
    {
        string normalized = value.Replace(" ", string.Empty).Replace("-", string.Empty).ToLowerInvariant();

        return normalized switch
        {
            "1x16pin" or "16pin" => GpuPowerConnector.One16Pin,
            "2x8pin" or "dual8pin" => GpuPowerConnector.Dual8Pin,
            "3x8pin" or "triple8pin" => GpuPowerConnector.Triple8Pin,
            _ => GpuPowerConnector.Dual8Pin // Default
        };
    }
}

public static class JsonParseExtensions
{
    extension(JsonElement element)
    {
        public string GetStringProperty(string propertyName, string defaultValue = "")
        {
            if (element.TryGetProperty(propertyName, out JsonElement property))
            {
                return property.GetString() ?? defaultValue;
            }
            return defaultValue;
        }

        public int GetIntProperty(string propertyName, int defaultValue = 0)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement property))
            {
                return defaultValue;
            }

            if (property.ValueKind == JsonValueKind.Number)
            {
                return property.GetInt32();
            }

            if (property.ValueKind == JsonValueKind.String && int.TryParse(property.GetString(), out int result))
            {
                return result;
            }

            return defaultValue;
        }

        public decimal GetDecimalProperty(string propertyName, decimal defaultValue = 0m)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement property))
            {
                return defaultValue;
            }
            if (property.ValueKind == JsonValueKind.Number)
            {
                return property.GetDecimal();
            }
            if (property.ValueKind == JsonValueKind.String && decimal.TryParse(property.GetString(), out decimal result))
            {
                return result;
            }
            return defaultValue;
        }

        public bool GetBoolProperty(string propertyName, bool defaultValue = false)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement property))
            {
                return defaultValue;
            }
            if (property.ValueKind == JsonValueKind.True || property.ValueKind == JsonValueKind.False)
            {
                return property.GetBoolean();
            }
            if (property.ValueKind == JsonValueKind.String && bool.TryParse(property.GetString(), out bool result))
            {
                return result;
            }
            return defaultValue;
        }
    }
}