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
        string manufacturer = root.GetStringProperty("Manufacturer");
        string name = root.GetStringProperty("Name");
        decimal price = root.GetDecimalProperty("Price");

        // Remove manufacturer from product name if it starts with it
        if (!string.IsNullOrEmpty(manufacturer) && name.StartsWith(manufacturer, StringComparison.OrdinalIgnoreCase))
        {
            name = name[manufacturer.Length..].Trim();
        }

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
            root.GetIntegerProperty("Cores"),
            root.GetIntegerProperty("Threads"),
            Frequency.FromGHz(root.GetDecimalProperty("BaseClock")),
            Frequency.FromGHz(root.GetDecimalProperty("BoostClock")),
            Power.FromWatts(root.GetIntegerProperty("TDP")),
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
            root.ParseDimensionsFromJson("Dimensions", new Dimensions(305, 244, 50)),
            root.ParseSlotsFromJson("Slots"),
            ParseEnum<CpuSocket>(root.GetStringProperty("Socket")),
            root.GetStringProperty("Chipset"),
            ParseEnum<FormFactor>(root.GetStringProperty("FormFactor")),
            ParseEnum<MemoryType>(root.GetStringProperty("MemoryType")),
            StorageCapacity.FromGB(root.GetIntegerProperty("MaxMemory"))
        );
    }

    private GpuProduct ParseGpuProduct(Guid id, string name, decimal price, string manufacturer, JsonElement root)
    {
        return new GpuProduct(
            id,
            name,
            price,
            manufacturer,
            root.ParseDimensionsFromJson("Dimensions", new Dimensions(300, 120, 50)),
            root.ParseSlotsFromJson("Slots"),
            root.GetStringProperty("ChipsetManufacturer"),
            root.GetStringProperty("Series"),
            StorageCapacity.FromGB(root.GetIntegerProperty("VRAM")),
            ParseEnum<MemoryType>(root.GetStringProperty("MemoryType")),
            Frequency.FromMHz(root.GetIntegerProperty("CoreClock")),
            Frequency.FromMHz(root.GetIntegerProperty("BoostClock")),
            Power.FromWatts(root.GetIntegerProperty("TDP")),
            Length.FromMm(root.GetIntegerProperty("Length")),
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
            StorageCapacity.FromGB(root.GetIntegerProperty("Capacity")),
            root.GetStringProperty("Configuration"),
            Frequency.FromMHz(root.GetIntegerProperty("Speed")),
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
            root.ParseDimensionsFromJson(nameof(PcCaseProduct.Dimensions), Dimensions.Zero),
            root.ParseChambersFromJson(nameof(PcCaseProduct.Chambers)),
            root.GetStringProperty(nameof(PcCaseProduct.FormFactor)),
            root.GetStringProperty(nameof(PcCaseProduct.Color)),
            root.GetStringProperty(nameof(PcCaseProduct.SidePanelWindow))
        );
    }

    private PsuProduct ParsePsuProduct(Guid id, string name, decimal price, string manufacturer, JsonElement root)
    {
        return new PsuProduct(
            id,
            name,
            price,
            manufacturer,
            Power.FromWatts(root.GetIntegerProperty("Wattage")),
            root.GetStringProperty("Efficiency"),
            root.GetStringProperty("Modular"),
            root.GetStringProperty("FormFactor"),
            Length.FromMm(root.GetIntegerProperty("Length")),
            root.GetIntegerProperty("PCIe8Pin")
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
            StorageCapacity.FromGB(root.GetIntegerProperty("Capacity")),
            DataSpeed.FromMBps(root.GetIntegerProperty("ReadSpeed")),
            DataSpeed.FromMBps(root.GetIntegerProperty("WriteSpeed"))
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
            root.ParseDimensionsFromJson("Dimensions", Dimensions.Zero),
            ParseEnum<CoolerType>(root.GetStringProperty("CoolerType")),
            Length.FromMm(root.GetIntegerProperty("Height")),
            Power.FromWatts(root.GetIntegerProperty("TDP")),
            sockets
        );
    }

    // Helper methods for JSON parsing

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

        public int GetIntegerProperty(string propertyName, int defaultValue = 0)
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


        public Dimensions ParseDimensionsFromJson(string propertyName, Dimensions defaultValue)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement dimensionsElement))
            {
                return defaultValue;
            }

            try
            {
                // Handle object format: {"length": 320, "width": 140, "height": 50}
                if (dimensionsElement.ValueKind == JsonValueKind.Object)
                {
                    JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
                    Dimensions? des = dimensionsElement.Deserialize<Dimensions?>(options);

                    if (des.HasValue && IsValidDimensions(des.Value))
                    {
                        return des.Value;
                    }
                }

                // Handle array format: [320, 140, 50]
                if (dimensionsElement.ValueKind == JsonValueKind.Array && dimensionsElement.GetArrayLength() == 3)
                {
                    JsonElement[] elements = dimensionsElement.EnumerateArray().ToArray();
                    if (TryParseDecimal(elements[0], out decimal length) &&
                        TryParseDecimal(elements[1], out decimal width) &&
                        TryParseDecimal(elements[2], out decimal height))
                    {
                        Dimensions dimensions = new(length, width, height);
                        if (IsValidDimensions(dimensions))
                        {
                            return dimensions;
                        }
                    }
                }

                // Handle string format: "320,140,50" or "320x140x50"
                if (dimensionsElement.ValueKind == JsonValueKind.String)
                {
                    string value = dimensionsElement.GetString() ?? string.Empty;
                    string[] parts = value.Split([',', 'x', 'X', '*'], StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length == 3 &&
                        decimal.TryParse(parts[0].Trim(), out decimal length) &&
                        decimal.TryParse(parts[1].Trim(), out decimal width) &&
                        decimal.TryParse(parts[2].Trim(), out decimal height))
                    {
                        Dimensions dimensions = new(length, width, height);
                        if (IsValidDimensions(dimensions))
                        {
                            return dimensions;
                        }
                    }
                }
            }
            catch
            {
                // If parsing fails for any reason, return default value
            }

            return defaultValue;
        }

        private static bool TryParseDecimal(JsonElement jsonElement, out decimal result)
        {
            result = 0m;

            if (jsonElement.ValueKind == JsonValueKind.Number)
            {
                result = jsonElement.GetDecimal();
                return true;
            }

            if (jsonElement.ValueKind == JsonValueKind.String)
            {
                return decimal.TryParse(jsonElement.GetString(), out result);
            }

            return false;
        }

        private static bool IsValidDimensions(Dimensions dimensions)
        {
            // Validate that dimensions are positive and within reasonable bounds (0-10000mm)
            return dimensions.Length > 0 && dimensions.Length <= 10000 &&
                   dimensions.Width > 0 && dimensions.Width <= 10000 &&
                   dimensions.Height > 0 && dimensions.Height <= 10000;
        }

        public List<Slot> ParseSlotsFromJson(string propertyName)
        {
            // For AI-generated products, we'll keep slots empty for simplicity
            // Advanced spatial editing can be done after publishing
            // The AI might return slots data, but we ignore it for draft products
            return [];
        }

        public List<Chamber> ParseChambersFromJson(string propertyName)
        {
            // For AI-generated products, we'll keep chambers empty for simplicity
            // Advanced spatial editing can be done after publishing
            return [];
        }
    }
}