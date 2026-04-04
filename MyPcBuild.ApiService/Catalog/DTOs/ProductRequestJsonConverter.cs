using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using MyPcBuild.ApiService.Catalog.Models;

namespace MyPcBuild.ApiService.Catalog.DTOs;

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
            throw new BadHttpRequestException("ProductRequest must have a 'category' property", StatusCodes.Status400BadRequest);
        }

        // Parse the category - it could be a string or a number
        ProductCategory category;
        if (categoryElement.ValueKind == JsonValueKind.String)
        {
            string? categoryString = categoryElement.GetString();
            if (string.IsNullOrEmpty(categoryString))
            {
                throw new BadHttpRequestException("Category value cannot be null or empty", StatusCodes.Status400BadRequest);
            }

            // Try case-insensitive parsing
            if (!Enum.TryParse<ProductCategory>(categoryString, ignoreCase: true, out category))
            {
                throw new BadHttpRequestException($"Invalid category value: {categoryString}", StatusCodes.Status400BadRequest);
            }
        }
        else if (categoryElement.ValueKind == JsonValueKind.Number)
        {
            category = (ProductCategory)categoryElement.GetInt32();
        }
        else
        {
            throw new BadHttpRequestException($"Category must be a string or number, got {categoryElement.ValueKind}", StatusCodes.Status400BadRequest);
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
            _ => throw new BadHttpRequestException($"Unknown product category: {category}", StatusCodes.Status400BadRequest)
        };

        // Deserialize to the concrete type
        string json = root.GetRawText();
        ProductRequest? result;
        try
        {
            result = (ProductRequest?)JsonSerializer.Deserialize(json, concreteType, options);
        }
        catch (JsonException ex)
        {
            throw new BadHttpRequestException(ex.Message, StatusCodes.Status400BadRequest);
        }

        if (result == null)
        {
            throw new BadHttpRequestException($"Failed to deserialize {concreteType.Name}", StatusCodes.Status400BadRequest);
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, ProductRequest value, JsonSerializerOptions options)
    {
        // Serialize using the concrete type
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
