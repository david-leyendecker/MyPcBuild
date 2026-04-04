using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyPcBuild.ApiService.Infrastructure;

/// <summary>
/// Base JSON converter for enum types that supports case-insensitive string deserialization.
/// Only enum member names defined in the enum are accepted; no aliases or alternate formats.
/// This ensures the OpenAPI schema exactly reflects the accepted values.
/// </summary>
public class EnumIgnoreCaseJsonConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string? value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new JsonException($"{typeof(T).Name} value cannot be null or empty");
            }

            if (Enum.TryParse<T>(value, ignoreCase: true, out T parsed))
            {
                return parsed;
            }

            throw new JsonException($"Unrecognized {typeof(T).Name} value: {value}. Expected one of: {string.Join(", ", Enum.GetNames<T>())}");
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            return (T)(object)reader.GetInt32();
        }

        throw new JsonException($"Cannot convert {reader.TokenType} to {typeof(T).Name}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
