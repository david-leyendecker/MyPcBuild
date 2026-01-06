using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Domain.Models.Spatial;

namespace MyPcBuild.ApiService.Features.Spatial;

public interface ISpatialValidator
{
    /// <summary>
    /// Validates that a part can be installed in a specific slot within a build.
    /// </summary>
    SpatialValidationResult ValidatePartInstallation(
        Build build,
        List<Product> allProducts,
        Guid productId,
        Guid slotId,
        Vector3 position);
    
    /// <summary>
    /// Validates the entire build spatial configuration.
    /// </summary>
    SpatialValidationResult ValidateBuild(Build build, List<Product> allProducts);
}

public class SpatialValidator : ISpatialValidator
{
    public SpatialValidationResult ValidatePartInstallation(
        Build build,
        List<Product> allProducts,
        Guid productId,
        Guid slotId,
        Vector3 position)
    {
        List<SpatialIssue> issues = [];
        
        // Find the product to install
        Product? product = allProducts.FirstOrDefault(p => p.Id == productId);
        if (product == null)
        {
            issues.Add(new SpatialIssue(
                $"Product {productId} not found",
                SpatialIssueSeverity.Error,
                "Product/NotFound"
            ));
            return new SpatialValidationResult(false, issues);
        }
        
        // Check if product has spatial properties
        if (product is not SpatialProduct spatialProduct)
        {
            issues.Add(new SpatialIssue(
                $"Product {product.Name} has no dimensions defined",
                SpatialIssueSeverity.Error,
                "Product/NoDimensions"
            ));
            return new SpatialValidationResult(false, issues);
        }
        
        // Find the slot in any product's chambers or slots
        (Product? slotOwner, Chamber? chamber, Slot? slot, Vector3 slotGlobalPosition) = FindSlot(build, allProducts, slotId);
        
        if (slot == null)
        {
            issues.Add(new SpatialIssue(
                $"Slot {slotId} not found in build",
                SpatialIssueSeverity.Error,
                "Slot/NotFound"
            ));
            return new SpatialValidationResult(false, issues);
        }
        
        // Validate dimensions fit in slot
        if (!spatialProduct.Dimensions.FitsWithin(slot.MaxDimensions))
        {
            issues.Add(new SpatialIssue(
                $"Part dimensions ({spatialProduct.Dimensions.Length}x{spatialProduct.Dimensions.Width}x{spatialProduct.Dimensions.Height}mm) " +
                $"exceed slot maximum ({slot.MaxDimensions.Length}x{slot.MaxDimensions.Width}x{slot.MaxDimensions.Height}mm)",
                SpatialIssueSeverity.Error,
                "Dimensions/Exceeded"
            ));
        }
        
        // Validate part category matches slot
        if (slot.AllowedCategory != product.Category)
        {
            issues.Add(new SpatialIssue(
                $"Product category {product.Category} does not match slot allowed category {slot.AllowedCategory}",
                SpatialIssueSeverity.Error,
                "Category/Mismatch"
            ));
        }
        
        // Create bounding box for the part at the given position
        BoundingBox partBox = new(position, spatialProduct.Dimensions);
        
        // If in a chamber, validate part fits within chamber boundaries
        if (chamber != null)
        {
            BoundingBox chamberBox = chamber.GetBoundingBox();
            if (!partBox.IsContainedWithin(chamberBox))
            {
                issues.Add(new SpatialIssue(
                    $"Part extends beyond chamber boundaries. Chamber: {chamber.Dimensions.Length}x{chamber.Dimensions.Width}x{chamber.Dimensions.Height}mm",
                    SpatialIssueSeverity.Error,
                    "Boundary/Exceeded"
                ));
            }
        }
        
        // Check for collisions with existing parts
        foreach (BuildPart existingPart in build.Parts)
        {
            if (existingPart.Position == null) continue; // Skip parts without spatial position
            
            Product? existingProduct = allProducts.FirstOrDefault(p => p.Id == existingPart.ProductId);
            if (existingProduct is not SpatialProduct existingSpatial) continue;
            
            BoundingBox existingBox = new(existingPart.Position, existingSpatial.Dimensions);
            if (partBox.Intersects(existingBox))
            {
                issues.Add(new SpatialIssue(
                    $"Part collides with existing part '{existingProduct.Name}' at position " +
                    $"({existingPart.Position.X}, {existingPart.Position.Y}, {existingPart.Position.Z})",
                    SpatialIssueSeverity.Error,
                    "Collision/PartConflict"
                ));
            }
        }
        
        return new SpatialValidationResult(!issues.Any(i => i.Severity == SpatialIssueSeverity.Error), issues);
    }
    
    public SpatialValidationResult ValidateBuild(Build build, List<Product> allProducts)
    {
        List<SpatialIssue> issues = [];
        
        // Get all parts with positions
        List<(BuildPart BuildPart, Product Product, BoundingBox BoundingBox)> spatialParts = [];
        
        foreach (BuildPart buildPart in build.Parts)
        {
            if (buildPart.Position == null) continue;
            
            Product? product = allProducts.FirstOrDefault(p => p.Id == buildPart.ProductId);
            if (product is not SpatialProduct spatialProduct) continue;
            
            BoundingBox box = new(buildPart.Position, spatialProduct.Dimensions);
            spatialParts.Add((buildPart, product, box));
        }
        
        // Check for collisions between all parts
        for (int i = 0; i < spatialParts.Count; i++)
        {
            (BuildPart part1, Product product1, BoundingBox box1) = spatialParts[i];
            
            for (int j = i + 1; j < spatialParts.Count; j++)
            {
                (BuildPart part2, Product product2, BoundingBox box2) = spatialParts[j];
                
                if (box1.Intersects(box2))
                {
                    issues.Add(new SpatialIssue(
                        $"Collision detected between '{product1.Name}' and '{product2.Name}'",
                        SpatialIssueSeverity.Error,
                        "Collision/PartConflict"
                    ));
                }
            }
        }
        
        // Validate parts are within their chamber boundaries
        foreach ((BuildPart buildPart, Product product, BoundingBox box) in spatialParts)
        {
            if (buildPart.SlotId.HasValue)
            {
                (Product? slotOwner, Chamber? chamber, Slot? slot, Vector3 slotPos) = FindSlot(build, allProducts, buildPart.SlotId.Value);
                
                if (chamber != null)
                {
                    BoundingBox chamberBox = chamber.GetBoundingBox();
                    if (!box.IsContainedWithin(chamberBox))
                    {
                        issues.Add(new SpatialIssue(
                            $"Part '{product.Name}' extends beyond chamber boundaries",
                            SpatialIssueSeverity.Error,
                            "Boundary/Exceeded"
                        ));
                    }
                }
            }
        }
        
        return new SpatialValidationResult(!issues.Any(i => i.Severity == SpatialIssueSeverity.Error), issues);
    }
    
    private (Product? SlotOwner, Chamber? Chamber, Slot? Slot, Vector3 GlobalPosition) FindSlot(
        Build build,
        List<Product> allProducts,
        Guid slotId)
    {
        // First search in products installed in the build
        foreach (BuildPart buildPart in build.Parts)
        {
            Product? product = allProducts.FirstOrDefault(p => p.Id == buildPart.ProductId);
            if (product == null) continue;
            
            // Check chambers (ChamberedProduct)
            if (product is ChamberedProduct chamberedProduct)
            {
                foreach (Chamber chamber in chamberedProduct.Chambers)
                {
                    List<(Slot Slot, Vector3 GlobalPosition)> slots = chamber.GetAllSlots();
                    (Slot Slot, Vector3 GlobalPosition) found = slots.FirstOrDefault(s => s.Slot.Id == slotId);
                    
                    if (found.Slot != null)
                    {
                        return (product, chamber, found.Slot, found.GlobalPosition);
                    }
                }
            }
            
            // Check direct slots on product (SlottedProduct)
            if (product is SlottedProduct slottedProduct)
            {
                foreach (Slot slot in slottedProduct.Slots)
                {
                    Vector3 basePosition = buildPart.Position ?? Vector3.Zero;
                    List<(Slot Slot, Vector3 GlobalPosition)> slots = slot.FlattenSlots(basePosition);
                    (Slot Slot, Vector3 GlobalPosition) found = slots.FirstOrDefault(s => s.Slot.Id == slotId);
                    
                    if (found.Slot != null)
                    {
                        return (product, null, found.Slot, found.GlobalPosition);
                    }
                }
            }
        }
        
        // If not found in build, search all products in catalog (for validation before installation)
        foreach (Product product in allProducts)
        {
            // Skip if already checked (in build)
            if (build.Parts.Any(bp => bp.ProductId == product.Id)) continue;
            
            // Check chambers
            if (product is ChamberedProduct chamberedProduct)
            {
                foreach (Chamber chamber in chamberedProduct.Chambers)
                {
                    List<(Slot Slot, Vector3 GlobalPosition)> slots = chamber.GetAllSlots();
                    (Slot Slot, Vector3 GlobalPosition) found = slots.FirstOrDefault(s => s.Slot.Id == slotId);
                    
                    if (found.Slot != null)
                    {
                        return (product, chamber, found.Slot, found.GlobalPosition);
                    }
                }
            }
            
            // Check direct slots on product
            if (product is SlottedProduct slottedProduct)
            {
                foreach (Slot slot in slottedProduct.Slots)
                {
                    List<(Slot Slot, Vector3 GlobalPosition)> slots = slot.FlattenSlots(Vector3.Zero);
                    (Slot Slot, Vector3 GlobalPosition) found = slots.FirstOrDefault(s => s.Slot.Id == slotId);
                    
                    if (found.Slot != null)
                    {
                        return (product, null, found.Slot, found.GlobalPosition);
                    }
                }
            }
        }
        
        return (null, null, null, Vector3.Zero);
    }
}
