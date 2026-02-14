using MyPcBuild.ApiService.Catalog.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyPcBuild.ApiService.SharedDomain.Spatial;

/// <summary>
/// Represents a slot where a part can be installed.
/// Slots can contain sub-slots for recursive part installation (e.g., motherboard has CPU/RAM slots).
/// Used as a value object within Product/Chamber definitions.
/// </summary>
public record Slot(
    Guid Id,
    string Name,
    ProductCategory AllowedProductCategory,
    Vector3 RelativePosition,
    Dimensions MaxDimensions,
    Rotation? Rotation = null,
    List<Slot>? SubSlots = null
)
{
    public Rotation Rotation { get; init; } = Rotation ?? Spatial.Rotation.Identity;
    public List<Slot> SubSlots { get; init; } = SubSlots ?? [];

    /// <summary>
    /// Gets the bounding box for this slot at the given position.
    /// </summary>
    public BoundingBox GetBoundingBox(Vector3 absolutePosition)
    {
        return new BoundingBox(absolutePosition + RelativePosition, MaxDimensions);
    }

    /// <summary>
    /// Flattens all sub-slots to global coordinates relative to the given base position.
    /// </summary>
    public List<SlotPlacement> FlattenSlots(Vector3 basePosition)
    {
        List<SlotPlacement> result = [new SlotPlacement(this, basePosition + RelativePosition)];

        foreach (Slot subSlot in SubSlots)
        {
            result.AddRange(subSlot.FlattenSlots(basePosition + RelativePosition));
        }

        return result;
    }
}

/// <summary>
/// JSON converter for Slot lists (always returns empty list for AI-generated products).
/// </summary>
internal class SlotListConverter : JsonConverter<List<Slot>>
{
    public override List<Slot> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Skip the value (could be array or null)
        reader.Skip();
        // Always return empty list for AI-generated draft products
        return [];
    }

    public override void Write(Utf8JsonWriter writer, List<Slot> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
