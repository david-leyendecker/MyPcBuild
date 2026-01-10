using Microsoft.Extensions.AI;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;
using System.Text.Json;

namespace MyPcBuild.ApiService.Features.Catalog;

/// <summary>
/// Implementation of IAiProductGenerator using OpenAI for product generation.
/// </summary>
public class OpenAiProductGenerator : IAiProductGenerator
{
    private readonly IChatClient _chatClient;
    private readonly ILogger<OpenAiProductGenerator> _logger;

    public OpenAiProductGenerator(IChatClient chatClient, ILogger<OpenAiProductGenerator> logger)
    {
        _chatClient = chatClient;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Product> GenerateProductAsync(string category, string description, CancellationToken cancellationToken = default)
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

    private string BuildSystemPrompt(string category)
    {
        string basePrompt = @"You are a PC hardware expert assistant. Generate product specifications in valid JSON format.
Your response must be ONLY valid JSON with no markdown formatting, no code blocks, no additional text.

The JSON must follow this exact structure:";

        string categoryStructure = category switch
        {
            "CPU" => @"
{
  ""Name"": ""product name"",
  ""Manufacturer"": ""manufacturer name"",
  ""Price"": decimal_price,
  ""Socket"": ""AM5 or LGA1700 or AM4 or LGA1200"",
  ""Cores"": integer_cores,
  ""Threads"": integer_threads,
  ""BaseClock"": decimal_ghz,
  ""BoostClock"": decimal_ghz,
  ""TDP"": integer_watts,
  ""IntegratedGraphics"": boolean
}",
            "Motherboard" => @"
{
  ""Name"": ""product name"",
  ""Manufacturer"": ""manufacturer name"",
  ""Price"": decimal_price,
  ""Socket"": ""AM5 or LGA1700 or AM4 or LGA1200"",
  ""Chipset"": ""chipset name"",
  ""FormFactor"": ""ATX or MicroATX or MiniITX or EATX"",
  ""MemoryType"": ""DDR4 or DDR5"",
  ""MaxMemory"": integer_gb,
  ""Dimensions"": ""length,width,height in mm"",
  ""Slots"": []
}",
            "GPU" => @"
{
  ""Name"": ""product name"",
  ""Manufacturer"": ""manufacturer name"",
  ""Price"": decimal_price,
  ""ChipsetManufacturer"": ""NVIDIA or AMD or Intel"",
  ""Series"": ""series name"",
  ""VRAM"": integer_gb,
  ""MemoryType"": ""GDDR6 or GDDR6X or GDDR5"",
  ""CoreClock"": integer_mhz,
  ""BoostClock"": integer_mhz,
  ""TDP"": integer_watts,
  ""Length"": integer_mm,
  ""PowerConnectors"": ""1x16-pin or 2x8-pin or 3x8-pin"",
  ""RayTracing"": boolean,
  ""Dimensions"": ""length,width,height in mm"",
  ""Slots"": []
}",
            "RAM" => @"
{
  ""Name"": ""product name"",
  ""Manufacturer"": ""manufacturer name"",
  ""Price"": decimal_price,
  ""Type"": ""DDR4 or DDR5 or DDR3"",
  ""Capacity"": integer_gb,
  ""Configuration"": ""e.g., 2x16GB"",
  ""Speed"": integer_mhz,
  ""CASLatency"": ""e.g., CL16"",
  ""Voltage"": decimal_volts
}",
            "PCCase" => @"
{
  ""Name"": ""product name"",
  ""Manufacturer"": ""manufacturer name"",
  ""Price"": decimal_price,
  ""FormFactor"": ""ATX or MicroATX or MiniITX or EATX"",
  ""Color"": ""color name"",
  ""SidePanelWindow"": ""None or Acrylic or Tempered Glass"",
  ""Dimensions"": ""length,width,height in mm"",
  ""Chambers"": []
}",
            "PSU" => @"
{
  ""Name"": ""product name"",
  ""Manufacturer"": ""manufacturer name"",
  ""Price"": decimal_price,
  ""Wattage"": integer_watts,
  ""Efficiency"": ""80+ Bronze or 80+ Gold or 80+ Platinum or 80+ Titanium"",
  ""Modular"": ""Non-Modular or Semi-Modular or Fully Modular"",
  ""FormFactor"": ""ATX or SFX"",
  ""Length"": integer_mm,
  ""PCIe8Pin"": integer_count
}",
            "Storage" => @"
{
  ""Name"": ""product name"",
  ""Manufacturer"": ""manufacturer name"",
  ""Price"": decimal_price,
  ""Type"": ""SSD or HDD"",
  ""Interface"": ""NVMe or SATA or M.2"",
  ""StorageFormFactor"": ""M.2 2280 or 2.5 inch or 3.5 inch"",
  ""Capacity"": integer_gb,
  ""ReadSpeed"": integer_mbps,
  ""WriteSpeed"": integer_mbps
}",
            "Cooler" => @"
{
  ""Name"": ""product name"",
  ""Manufacturer"": ""manufacturer name"",
  ""Price"": decimal_price,
  ""CoolerType"": ""Air or AIO or CustomLoop"",
  ""Height"": integer_mm,
  ""TDP"": integer_watts,
  ""Sockets"": ""comma-separated list like AM5,LGA1700"",
  ""Dimensions"": ""length,width,height in mm""
}",
            _ => throw new ArgumentException($"Unknown category: {category}")
        };

        return $"{basePrompt}\n{categoryStructure}\n\nRespond with ONLY the JSON object, no other text.";
    }

    private Product ParseProductFromJson(string category, string jsonResponse)
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
        string name = GetStringProperty(root, "Name");
        decimal price = GetDecimalProperty(root, "Price");
        string manufacturer = GetStringProperty(root, "Manufacturer");

        Product product = category switch
        {
            "CPU" => ParseCpuProduct(id, name, price, manufacturer, root),
            "Motherboard" => ParseMotherboardProduct(id, name, price, manufacturer, root),
            "GPU" => ParseGpuProduct(id, name, price, manufacturer, root),
            "RAM" => ParseRamProduct(id, name, price, manufacturer, root),
            "PCCase" => ParsePcCaseProduct(id, name, price, manufacturer, root),
            "PSU" => ParsePsuProduct(id, name, price, manufacturer, root),
            "Storage" => ParseStorageProduct(id, name, price, manufacturer, root),
            "Cooler" => ParseCoolerProduct(id, name, price, manufacturer, root),
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
            ParseEnum<CpuSocket>(GetStringProperty(root, "Socket")),
            GetIntProperty(root, "Cores"),
            GetIntProperty(root, "Threads"),
            Frequency.FromGHz(GetDecimalProperty(root, "BaseClock")),
            Frequency.FromGHz(GetDecimalProperty(root, "BoostClock")),
            Power.FromWatts(GetIntProperty(root, "TDP")),
            GetBoolProperty(root, "IntegratedGraphics")
        );
    }

    private MotherboardProduct ParseMotherboardProduct(Guid id, string name, decimal price, string manufacturer, JsonElement root)
    {
        return new MotherboardProduct(
            id,
            name,
            price,
            manufacturer,
            ParseDimensions(GetStringProperty(root, "Dimensions", "305,244,50")),
            ParseSlots(GetStringProperty(root, "Slots", "[]")),
            ParseEnum<CpuSocket>(GetStringProperty(root, "Socket")),
            GetStringProperty(root, "Chipset"),
            ParseEnum<FormFactor>(GetStringProperty(root, "FormFactor")),
            ParseEnum<MemoryType>(GetStringProperty(root, "MemoryType")),
            StorageCapacity.FromGB(GetIntProperty(root, "MaxMemory"))
        );
    }

    private GpuProduct ParseGpuProduct(Guid id, string name, decimal price, string manufacturer, JsonElement root)
    {
        return new GpuProduct(
            id,
            name,
            price,
            manufacturer,
            ParseDimensions(GetStringProperty(root, "Dimensions", "300,120,50")),
            ParseSlots(GetStringProperty(root, "Slots", "[]")),
            GetStringProperty(root, "ChipsetManufacturer"),
            GetStringProperty(root, "Series"),
            StorageCapacity.FromGB(GetIntProperty(root, "VRAM")),
            ParseEnum<MemoryType>(GetStringProperty(root, "MemoryType")),
            Frequency.FromMHz(GetIntProperty(root, "CoreClock")),
            Frequency.FromMHz(GetIntProperty(root, "BoostClock")),
            Power.FromWatts(GetIntProperty(root, "TDP")),
            Length.FromMm(GetIntProperty(root, "Length")),
            ParseGpuPowerConnector(GetStringProperty(root, "PowerConnectors")),
            GetBoolProperty(root, "RayTracing")
        );
    }

    private RamProduct ParseRamProduct(Guid id, string name, decimal price, string manufacturer, JsonElement root)
    {
        return new RamProduct(
            id,
            name,
            price,
            manufacturer,
            ParseEnum<MemoryType>(GetStringProperty(root, "Type")),
            StorageCapacity.FromGB(GetIntProperty(root, "Capacity")),
            GetStringProperty(root, "Configuration"),
            Frequency.FromMHz(GetIntProperty(root, "Speed")),
            GetStringProperty(root, "CASLatency"),
            Voltage.FromVolts(GetDecimalProperty(root, "Voltage"))
        );
    }

    private PcCaseProduct ParsePcCaseProduct(Guid id, string name, decimal price, string manufacturer, JsonElement root)
    {
        return new PcCaseProduct(
            id,
            name,
            price,
            manufacturer,
            ParseDimensions(GetStringProperty(root, "Dimensions", "500,230,480")),
            ParseChambers(GetStringProperty(root, "Chambers", "[]")),
            GetStringProperty(root, "FormFactor"),
            GetStringProperty(root, "Color"),
            GetStringProperty(root, "SidePanelWindow")
        );
    }

    private PsuProduct ParsePsuProduct(Guid id, string name, decimal price, string manufacturer, JsonElement root)
    {
        return new PsuProduct(
            id,
            name,
            price,
            manufacturer,
            Power.FromWatts(GetIntProperty(root, "Wattage")),
            GetStringProperty(root, "Efficiency"),
            GetStringProperty(root, "Modular"),
            GetStringProperty(root, "FormFactor"),
            Length.FromMm(GetIntProperty(root, "Length")),
            GetIntProperty(root, "PCIe8Pin")
        );
    }

    private StorageProduct ParseStorageProduct(Guid id, string name, decimal price, string manufacturer, JsonElement root)
    {
        return new StorageProduct(
            id,
            name,
            price,
            manufacturer,
            GetStringProperty(root, "Type"),
            GetStringProperty(root, "Interface"),
            GetStringProperty(root, "StorageFormFactor"),
            StorageCapacity.FromGB(GetIntProperty(root, "Capacity")),
            DataSpeed.FromMBps(GetIntProperty(root, "ReadSpeed")),
            DataSpeed.FromMBps(GetIntProperty(root, "WriteSpeed"))
        );
    }

    private CoolerProduct ParseCoolerProduct(Guid id, string name, decimal price, string manufacturer, JsonElement root)
    {
        string socketsStr = GetStringProperty(root, "Sockets");
        string[] socketArr = socketsStr.Split(',', StringSplitOptions.RemoveEmptyEntries);
        CpuSocket[] sockets = socketArr.Select(s => ParseEnum<CpuSocket>(s.Trim())).ToArray();

        return new CoolerProduct(
            id,
            name,
            price,
            manufacturer,
            ParseDimensions(GetStringProperty(root, "Dimensions", "140,140,160")),
            ParseEnum<CoolerType>(GetStringProperty(root, "CoolerType")),
            Length.FromMm(GetIntProperty(root, "Height")),
            Power.FromWatts(GetIntProperty(root, "TDP")),
            sockets
        );
    }

    // Helper methods for JSON parsing
    private string GetStringProperty(JsonElement element, string propertyName, string defaultValue = "")
    {
        if (element.TryGetProperty(propertyName, out JsonElement property))
        {
            return property.GetString() ?? defaultValue;
        }
        return defaultValue;
    }

    private int GetIntProperty(JsonElement element, string propertyName, int defaultValue = 0)
    {
        if (element.TryGetProperty(propertyName, out JsonElement property))
        {
            if (property.ValueKind == JsonValueKind.Number)
            {
                return property.GetInt32();
            }
            if (property.ValueKind == JsonValueKind.String && int.TryParse(property.GetString(), out int result))
            {
                return result;
            }
        }
        return defaultValue;
    }

    private decimal GetDecimalProperty(JsonElement element, string propertyName, decimal defaultValue = 0m)
    {
        if (element.TryGetProperty(propertyName, out JsonElement property))
        {
            if (property.ValueKind == JsonValueKind.Number)
            {
                return property.GetDecimal();
            }
            if (property.ValueKind == JsonValueKind.String && decimal.TryParse(property.GetString(), out decimal result))
            {
                return result;
            }
        }
        return defaultValue;
    }

    private bool GetBoolProperty(JsonElement element, string propertyName, bool defaultValue = false)
    {
        if (element.TryGetProperty(propertyName, out JsonElement property))
        {
            if (property.ValueKind == JsonValueKind.True || property.ValueKind == JsonValueKind.False)
            {
                return property.GetBoolean();
            }
            if (property.ValueKind == JsonValueKind.String && bool.TryParse(property.GetString(), out bool result))
            {
                return result;
            }
        }
        return defaultValue;
    }

    private Dimensions ParseDimensions(string value)
    {
        string[] parts = value.Split(',');
        if (parts.Length != 3)
        {
            return new Dimensions(300, 200, 50); // Default dimensions
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
