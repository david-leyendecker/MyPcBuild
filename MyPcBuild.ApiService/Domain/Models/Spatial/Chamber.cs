using MyPcBuild.ApiService.Domain.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyPcBuild.ApiService.Domain.Models.Spatial;

/// <summary>
/// Represents a 3D chamber (container) that can hold parts via slots.
/// Used as a value object within Product definitions (e.g., PC Case).
/// </summary>
public record Chamber(
    Guid Id,
    string Name,
    Dimensions Dimensions,
    List<Slot> Slots
)
{
    /// <summary>
    /// Gets the bounding box for this chamber (always at origin).
    /// </summary>
    public BoundingBox GetBoundingBox()
    {
        return new BoundingBox(Vector3.Zero, Dimensions);
    }

    /// <summary>
    /// Gets all slots flattened to global coordinates.
    /// </summary>
    public List<SlotPlacement> GetAllSlots()
    {
        List<SlotPlacement> result = [];

        foreach (Slot slot in Slots)
        {
            result.AddRange(slot.FlattenSlots(Vector3.Zero));
        }

        return result;
    }
}

/// <summary>
/// JSON converter for Chamber lists (always returns empty list for AI-generated products).
/// </summary>
internal class ChamberListConverter : JsonConverter<List<Chamber>>
{
    public override List<Chamber> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Skip the value (could be array or null)
        reader.Skip();
        // Always return empty list for AI-generated draft products
        return [];
    }

    public override void Write(Utf8JsonWriter writer, List<Chamber> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
