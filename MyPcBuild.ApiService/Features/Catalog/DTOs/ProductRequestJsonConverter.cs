using System.Text.Json;
using System.Text.Json.Serialization;
using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Features.Catalog.DTOs;

/// <summary>
/// JSON converter for ProductRequest that handles polymorphic deserialization
/// based on the Category discriminator field.
/// </summary>
public class ProductRequestJsonConverter : JsonConverter<ProductRequest>
{
    public override ProductRequest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Read the JSON as a JsonDocument first
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        JsonElement root = doc.RootElement;

        // Get the Category property to determine which concrete type to deserialize
        if (!root.TryGetProperty("category", out JsonElement categoryElement) &&
            !root.TryGetProperty("Category", out categoryElement))
        {
            throw new JsonException("ProductRequest must have a 'category' property");
        }

        // Parse the category - it could be a string or a number
        ProductCategory category;
        if (categoryElement.ValueKind == JsonValueKind.String)
        {
            string? categoryString = categoryElement.GetString();
            if (string.IsNullOrEmpty(categoryString))
            {
                throw new JsonException("Category value cannot be null or empty");
            }

            // Try case-insensitive parsing
            if (!Enum.TryParse<ProductCategory>(categoryString, ignoreCase: true, out category))
            {
                throw new JsonException($"Invalid category value: {categoryString}");
            }
        }
        else if (categoryElement.ValueKind == JsonValueKind.Number)
        {
            category = (ProductCategory)categoryElement.GetInt32();
        }
        else
        {
            throw new JsonException($"Category must be a string or number, got {categoryElement.ValueKind}");
        }

        // Determine the concrete type based on category
        Type concreteType = category switch
        {
            ProductCategory.CPU => typeof(CpuProductRequest),
            ProductCategory.GPU => typeof(GpuProductRequest),
            ProductCategory.Motherboard => typeof(MotherboardProductRequest),
            ProductCategory.RAM => typeof(RamProductRequest),
            ProductCategory.Storage => typeof(StorageProductRequest),
            ProductCategory.PowerSupply => typeof(PsuProductRequest),
            ProductCategory.Cooler => typeof(CoolerProductRequest),
            ProductCategory.Case => typeof(PcCaseProductRequest),
            _ => throw new JsonException($"Unknown product category: {category}")
        };

        // Deserialize to the concrete type
        string json = root.GetRawText();
        ProductRequest? result = (ProductRequest?)JsonSerializer.Deserialize(json, concreteType, options);
        
        if (result == null)
        {
            throw new JsonException($"Failed to deserialize {concreteType.Name}");
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, ProductRequest value, JsonSerializerOptions options)
    {
        // Serialize using the concrete type
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
