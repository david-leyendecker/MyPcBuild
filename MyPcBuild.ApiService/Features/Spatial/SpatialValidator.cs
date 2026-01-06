using MyPcBuild.ApiService.Domain.Models.Spatial;

namespace MyPcBuild.ApiService.Features.Spatial;

public interface ISpatialValidator
{
    /// <summary>
    /// Validates that a part can be installed in a specific slot within a chamber.
    /// </summary>
    SpatialValidationResult ValidatePartInstallation(
        Chamber chamber,
        Guid slotId,
        Dimensions partDimensions,
        Vector3 partPosition);
    
    /// <summary>
    /// Validates the entire chamber configuration.
    /// </summary>
    SpatialValidationResult ValidateChamber(Chamber chamber);
}

public class SpatialValidator : ISpatialValidator
{
    public SpatialValidationResult ValidatePartInstallation(
        Chamber chamber,
        Guid slotId,
        Dimensions partDimensions,
        Vector3 partPosition)
    {
        List<SpatialIssue> issues = [];
        
        // Find the slot
        List<(Slot Slot, Vector3 GlobalPosition)> allSlots = chamber.GetAllSlots();
        (Slot Slot, Vector3 GlobalPosition) slotInfo = allSlots.FirstOrDefault(s => s.Slot.Id == slotId);
        
        if (slotInfo.Slot == null)
        {
            issues.Add(new SpatialIssue(
                $"Slot {slotId} not found in chamber",
                SpatialIssueSeverity.Error,
                "Slot/NotFound"
            ));
            return new SpatialValidationResult(false, issues);
        }
        
        Slot slot = slotInfo.Slot;
        Vector3 slotGlobalPosition = slotInfo.GlobalPosition;
        
        // Validate dimensions fit in slot
        if (!partDimensions.FitsWithin(slot.MaxDimensions))
        {
            issues.Add(new SpatialIssue(
                $"Part dimensions ({partDimensions.Length}x{partDimensions.Width}x{partDimensions.Height}mm) " +
                $"exceed slot maximum ({slot.MaxDimensions.Length}x{slot.MaxDimensions.Width}x{slot.MaxDimensions.Height}mm)",
                SpatialIssueSeverity.Error,
                "Dimensions/Exceeded"
            ));
        }
        
        // Create bounding box for the part at the given position
        BoundingBox partBox = new(partPosition, partDimensions);
        
        // Validate part fits within chamber boundaries
        BoundingBox chamberBox = chamber.GetBoundingBox();
        if (!partBox.IsContainedWithin(chamberBox))
        {
            issues.Add(new SpatialIssue(
                $"Part extends beyond chamber boundaries. Chamber: {chamber.Dimensions.Length}x{chamber.Dimensions.Width}x{chamber.Dimensions.Height}mm",
                SpatialIssueSeverity.Error,
                "Boundary/Exceeded"
            ));
        }
        
        // Check for collisions with existing parts
        foreach (InstalledPart existingPart in chamber.InstalledParts)
        {
            BoundingBox existingBox = existingPart.GetBoundingBox();
            if (partBox.Intersects(existingBox))
            {
                issues.Add(new SpatialIssue(
                    $"Part collides with existing part (ProductId: {existingPart.ProductId}) at position " +
                    $"({existingPart.Position.X}, {existingPart.Position.Y}, {existingPart.Position.Z})",
                    SpatialIssueSeverity.Error,
                    "Collision/PartConflict"
                ));
            }
        }
        
        return new SpatialValidationResult(!issues.Any(i => i.Severity == SpatialIssueSeverity.Error), issues);
    }
    
    public SpatialValidationResult ValidateChamber(Chamber chamber)
    {
        List<SpatialIssue> issues = [];
        
        // Validate all installed parts
        foreach (InstalledPart part in chamber.InstalledParts)
        {
            BoundingBox partBox = part.GetBoundingBox();
            BoundingBox chamberBox = chamber.GetBoundingBox();
            
            // Check boundary
            if (!partBox.IsContainedWithin(chamberBox))
            {
                issues.Add(new SpatialIssue(
                    $"Part {part.ProductId} extends beyond chamber boundaries",
                    SpatialIssueSeverity.Error,
                    "Boundary/Exceeded"
                ));
            }
        }
        
        // Check for collisions between all parts
        for (int i = 0; i < chamber.InstalledParts.Count; i++)
        {
            InstalledPart part1 = chamber.InstalledParts[i];
            BoundingBox box1 = part1.GetBoundingBox();
            
            for (int j = i + 1; j < chamber.InstalledParts.Count; j++)
            {
                InstalledPart part2 = chamber.InstalledParts[j];
                BoundingBox box2 = part2.GetBoundingBox();
                
                if (box1.Intersects(box2))
                {
                    issues.Add(new SpatialIssue(
                        $"Collision detected between parts {part1.ProductId} and {part2.ProductId}",
                        SpatialIssueSeverity.Error,
                        "Collision/PartConflict"
                    ));
                }
            }
        }
        
        return new SpatialValidationResult(!issues.Any(i => i.Severity == SpatialIssueSeverity.Error), issues);
    }
}
