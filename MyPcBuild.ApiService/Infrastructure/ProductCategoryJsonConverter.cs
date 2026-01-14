using System.Text.Json;
using System.Text.Json.Serialization;
using MyPcBuild.ApiService.Domain.Models;

namespace MyPcBuild.ApiService.Infrastructure;

/// <summary>
/// JSON converter for ProductCategory enum that handles case-insensitive deserialization.
/// This allows the frontend to send lowercase values like "gpu", "cpu" which will be
/// converted to the corresponding enum values GPU, CPU, etc.
/// </summary>
public class ProductCategoryJsonConverter : JsonConverter<ProductCategory>
{
    public override ProductCategory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string value for ProductCategory, got {reader.TokenType}");
        }

        string? value = reader.GetString();
        if (string.IsNullOrEmpty(value))
        {
            throw new JsonException("ProductCategory value cannot be null or empty");
        }

        // Try case-insensitive parsing
        if (Enum.TryParse<ProductCategory>(value, ignoreCase: true, out ProductCategory result))
        {
            return result;
        }

        throw new JsonException($"Unable to convert '{value}' to ProductCategory");
    }

    public override void Write(Utf8JsonWriter writer, ProductCategory value, JsonSerializerOptions options)
    {
        // Write as lowercase string to match frontend expectations
        writer.WriteStringValue(value.ToString().ToLowerInvariant());
    }
}
