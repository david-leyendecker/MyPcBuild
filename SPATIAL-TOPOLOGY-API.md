# Spatial Topology Engine API

## Overview
The Spatial Topology Engine implements ADR004 to provide 3D spatial validation for PC build compatibility. Chambers and slots are defined as properties of Products in the catalog, and validation operates on Build aggregates with the Product catalog.

## Core Concepts

### Product-Based Spatial Definitions
Spatial properties are embedded directly in the Product model:
- **PC Cases** have a `Chambers[]` property defining internal 3D spaces with slots
- **Motherboards, GPUs** can have a `Slots[]` property for attachable components (CPU, RAM, etc.)
- **All products** can have a `Dimensions` property for their physical size

### Value Objects
- **Vector3**: 3D position coordinates (X, Y, Z) in millimeters
- **Dimensions**: 3D size (Length, Width, Height) in millimeters
- **BoundingBox**: AABB representation combining position and dimensions
- **Chamber**: 3D container with boundaries and slots (embedded in Product)
- **Slot**: Installation location for components (embedded in Product/Chamber)

### Build Tracking
- **BuildPart**: Tracks installed products with optional `SlotId` and `Position` for spatial parts
- Non-spatial parts use regular `PartAdded` event
- Spatial parts use `PartAddedToSlot` event with position data

### Validation
- **Boundary Check**: Ensures parts fit within chamber boundaries
- **Collision Detection**: AABB-based collision detection between parts
- **Slot Compatibility**: Validates parts match slot dimensions and categories

## API Endpoints

### POST /api/builds/{buildId}/parts/validate
Validate adding a part to a build (optionally in a slot).

**Request:**
```json
{
  "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "slotId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "position": {"x": 10, "y": 10, "z": 0}
}
```

**Response:**
```json
{
  "isValid": true,
  "hasErrors": false,
  "hasWarnings": false,
  "issues": []
}
```

### POST /api/builds/{buildId}/validate
Validate entire build spatial configuration.

**Response:**
```json
{
  "isValid": false,
  "hasErrors": true,
  "hasWarnings": false,
  "issues": [{
    "message": "Collision detected between 'ASUS X670E' and 'MSI B650'",
    "severity": "Error",
    "category": "Collision/PartConflict"
  }]
}
```

## Validation Categories

| Category | Description |
|----------|-------------|
| `Product/NotFound` | Specified product does not exist in catalog |
| `Product/NoDimensions` | Product has no dimensions defined |
| `Slot/NotFound` | Specified slot does not exist |
| `Dimensions/Exceeded` | Part dimensions exceed slot maximum dimensions |
| `Category/Mismatch` | Product category doesn't match slot allowed category |
| `Boundary/Exceeded` | Part extends beyond chamber boundaries |
| `Collision/PartConflict` | Part collides with another installed part |

## Event Sourcing

**PartAdded** - For parts without spatial positioning:
```csharp
new PartAdded
{
    BuildId = buildId,
    ProductId = ramId,
    PricePaid = 129.99m
}
```

**PartAddedToSlot** - For parts installed in specific slots with position:
```csharp
new PartAddedToSlot
{
    BuildId = buildId,
    ProductId = motherboardId,
    PricePaid = 299.99m,
    SlotId = mbSlotId,
    Position = new Vector3(10, 10, 0)
}
```

## Product Examples

### PC Case with Chambers
```csharp
new Product(
    Id: caseId,
    Name: "NZXT H510",
    Category: ProductCategory.PCCase,
    Price: 89.99m,
    Manufacturer: "NZXT",
    Specifications: new Dictionary<string, object> { ["FormFactor"] = "ATX" },
    Chambers: [
        new Chamber(
            chamberId,
            "Main Chamber",
            new Dimensions(400, 260, 450),
            [
                new Slot(
                    mbSlotId,
                    "Motherboard Slot",
                    ProductCategory.Motherboard,
                    new Vector3(10, 10, 0),
                    new Dimensions(305, 244, 50)
                )
            ]
        )
    ],
    Slots: null,
    Dimensions: new Dimensions(450, 220, 500)
)
```

### Motherboard with Slots
```csharp
new Product(
    Id: motherboardId,
    Name: "ASUS ROG X670E",
    Category: ProductCategory.Motherboard,
    Price: 649.99m,
    Manufacturer: "ASUS",
    Specifications: new Dictionary<string, object>
    {
        ["Socket"] = "AM5",
        ["FormFactor"] = "ATX",
        ["MemoryType"] = "DDR5"
    },
    Chambers: null,
    Slots: [
        new Slot(
            cpuSlotId,
            "CPU Socket",
            ProductCategory.CPU,
            new Vector3(100, 120, 0),
            new Dimensions(40, 40, 20)
        )
    ],
    Dimensions: new Dimensions(305, 244, 50)
)
```

## Recursive Slot Design

Products can define slots that become available once installed. When a motherboard is installed in a case's motherboard slot, the motherboard's own slots (CPU, RAM) become available for further installation.

## Integration

The spatial topology engine complements the existing compatibility validator for comprehensive PC build validation.
