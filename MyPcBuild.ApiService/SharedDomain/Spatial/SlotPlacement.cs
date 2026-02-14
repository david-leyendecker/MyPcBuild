namespace MyPcBuild.ApiService.SharedDomain.Spatial;

/// <summary>
/// Represents a slot with its absolute position in 3D space.
/// </summary>
public record SlotPlacement(Slot Slot, Vector3 GlobalPosition);
