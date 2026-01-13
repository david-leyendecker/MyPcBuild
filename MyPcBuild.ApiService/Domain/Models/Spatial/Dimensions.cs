using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyPcBuild.ApiService.Domain.Models.Spatial;

/// <summary>
/// Represents 3D dimensions (Length, Width, Height) in millimeters.
/// </summary>
public record struct Dimensions(
    decimal Length,
    decimal Width,
    decimal Height
)
{
    public static readonly Dimensions Zero = new(0, 0, 0);

    /// <summary>
    /// Checks if these dimensions fit within the given container dimensions.
    /// </summary>
    public bool FitsWithin(Dimensions container)
    {
        return Length <= container.Length && Width <= container.Width && Height <= container.Height;
    }
}

/// <summary>
/// JSON converter for Dimensions that handles various input formats (object, array, string).
/// </summary>
internal class DimensionsConverter : JsonConverter<Dimensions>
{
    public override Dimensions Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return Dimensions.Zero;
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            decimal length = 0, width = 0, height = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString()!;
                    reader.Read();

                    decimal value = reader.TokenType == JsonTokenType.Number
                        ? reader.GetDecimal()
                        : 0;

                    if (propertyName.Equals(nameof(Dimensions.Length), StringComparison.OrdinalIgnoreCase))
                    {
                        length = value;
                    }
                    else if (propertyName.Equals(nameof(Dimensions.Width), StringComparison.OrdinalIgnoreCase))
                    {
                        width = value;
                    }
                    else if (propertyName.Equals(nameof(Dimensions.Height), StringComparison.OrdinalIgnoreCase))
                    {
                        height = value;
                    }
                }
            }

            return new Dimensions(length, width, height);
        }

        if (reader.TokenType == JsonTokenType.StartArray)
        {
            List<decimal> values = [];
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.TokenType == JsonTokenType.Number)
                {
                    values.Add(reader.GetDecimal());
                }
                else if (reader.TokenType == JsonTokenType.String && decimal.TryParse(reader.GetString(), out decimal val))
                {
                    values.Add(val);
                }
            }

            if (values.Count == 3)
            {
                return new Dimensions(values[0], values[1], values[2]);
            }
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            string? str = reader.GetString();
            if (!string.IsNullOrEmpty(str))
            {
                string[] parts = str.Split('x', 'X', '×');
                if (parts.Length == 3 &&
                    decimal.TryParse(parts[0].Trim(), out decimal length) &&
                    decimal.TryParse(parts[1].Trim(), out decimal width) &&
                    decimal.TryParse(parts[2].Trim(), out decimal height))
                {
                    return new Dimensions(length, width, height);
                }
            }
        }

        return Dimensions.Zero;
    }

    public override void Write(Utf8JsonWriter writer, Dimensions value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber(nameof(Dimensions.Length), value.Length);
        writer.WriteNumber(nameof(Dimensions.Width), value.Width);
        writer.WriteNumber(nameof(Dimensions.Height), value.Height);
        writer.WriteEndObject();
    }
}