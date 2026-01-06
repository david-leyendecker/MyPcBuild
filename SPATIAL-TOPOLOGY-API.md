# Spatial Topology Engine API

## Overview
The Spatial Topology Engine implements ADR004 to provide 3D spatial validation for PC build compatibility. It uses a recursive chamber-and-slot design with Axis-Aligned Bounding Box (AABB) collision detection.

## Core Concepts

### Value Objects
- **Vector3**: 3D position coordinates (X, Y, Z) in millimeters
- **Dimensions**: 3D size (Length, Width, Height) in millimeters
- **BoundingBox**: AABB representation combining position and dimensions

### Entities
- **Chamber**: 3D container representing a PC case with defined boundaries
- **Slot**: Installation location for a component, can contain sub-slots
- **InstalledPart**: A component installed in a specific slot with position and dimensions

### Validation
- **Boundary Check**: Ensures parts fit within chamber boundaries
- **Collision Detection**: AABB-based collision detection between parts
- **Slot Compatibility**: Validates parts match slot dimensions

## API Endpoints

### POST /api/spatial/validate
Validate a part installation in a chamber slot.

**Request:**
```json
{
  "chamberId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "slotId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "position": {
    "x": 10,
    "y": 10,
    "z": 0
  },
  "dimensions": {
    "length": 305,
    "width": 244,
    "height": 50
  }
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

**Error Response:**
```json
{
  "isValid": false,
  "hasErrors": true,
  "hasWarnings": false,
  "issues": [
    {
      "message": "Part extends beyond chamber boundaries. Chamber: 400x200x450mm",
      "severity": "Error",
      "category": "Boundary/Exceeded"
    }
  ]
}
```

### POST /api/spatial/chambers
Create a new chamber configuration.

**Request:**
```json
{
  "name": "ATX PC Case",
  "dimensions": {
    "length": 400,
    "width": 200,
    "height": 450
  }
}
```

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "ATX PC Case",
  "dimensions": {
    "length": 400,
    "width": 200,
    "height": 450
  },
  "slots": [],
  "installedParts": []
}
```

### POST /api/spatial/chambers/{chamberId}/slots
Add a slot to a chamber.

**Request:**
```json
{
  "chamberId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "slotName": "Motherboard Slot",
  "allowedCategory": "Motherboard",
  "relativePosition": {
    "x": 10,
    "y": 10,
    "z": 0
  },
  "maxDimensions": {
    "length": 305,
    "width": 244,
    "height": 50
  },
  "parentSlotId": null
}
```

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Motherboard Slot",
  "allowedCategory": "Motherboard",
  "relativePosition": {
    "x": 10,
    "y": 10,
    "z": 0
  },
  "maxDimensions": {
    "length": 305,
    "width": 244,
    "height": 50
  },
  "installedPartId": null,
  "subSlots": []
}
```

### POST /api/spatial/chambers/{chamberId}/validate
Validate entire chamber configuration.

**Response:**
```json
{
  "isValid": false,
  "hasErrors": true,
  "hasWarnings": false,
  "issues": [
    {
      "message": "Collision detected between parts 3fa85f64-5717-4562-b3fc-2c963f66afa6 and 4fa85f64-5717-4562-b3fc-2c963f66afa6",
      "severity": "Error",
      "category": "Collision/PartConflict"
    }
  ]
}
```

### GET /api/spatial/chambers/{chamberId}
Get chamber configuration.

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "ATX PC Case",
  "dimensions": {
    "length": 400,
    "width": 200,
    "height": 450
  },
  "slots": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Motherboard Slot",
      "allowedCategory": "Motherboard",
      "relativePosition": {
        "x": 10,
        "y": 10,
        "z": 0
      },
      "maxDimensions": {
        "length": 305,
        "width": 244,
        "height": 50
      },
      "installedPartId": null,
      "subSlots": []
    }
  ],
  "installedParts": []
}
```

## Validation Categories

| Category | Description |
|----------|-------------|
| `Slot/NotFound` | Specified slot does not exist in the chamber |
| `Dimensions/Exceeded` | Part dimensions exceed slot maximum dimensions |
| `Boundary/Exceeded` | Part extends beyond chamber boundaries |
| `Collision/PartConflict` | Part collides with another installed part |

## Severity Levels

### Error (🔴)
- Part installation **will not work** or is **physically impossible**
- Examples: Boundary exceeded, collision detected, dimensions too large

### Warning (⚠️)
- Part installation **may have issues** but is technically possible
- Currently not used in spatial validation (reserved for future use)

## Usage Examples

### Example 1: Valid Installation
```json
POST /api/spatial/validate
{
  "chamberId": "chamber-1",
  "slotId": "motherboard-slot",
  "productId": "asus-x670e",
  "position": {"x": 10, "y": 10, "z": 0},
  "dimensions": {"length": 305, "width": 244, "height": 50}
}

Response: {"isValid": true, "hasErrors": false, "issues": []}
```

### Example 2: Part Too Large
```json
POST /api/spatial/validate
{
  "chamberId": "chamber-1",
  "slotId": "motherboard-slot",
  "productId": "oversized-board",
  "position": {"x": 10, "y": 10, "z": 0},
  "dimensions": {"length": 400, "width": 300, "height": 100}
}

Response: {
  "isValid": false,
  "hasErrors": true,
  "issues": [{
    "message": "Part dimensions (400x300x100mm) exceed slot maximum (305x244x50mm)",
    "severity": "Error",
    "category": "Dimensions/Exceeded"
  }]
}
```

### Example 3: Collision Detection
```json
POST /api/spatial/validate
{
  "chamberId": "chamber-with-parts",
  "slotId": "gpu-slot",
  "productId": "rtx-4090",
  "position": {"x": 50, "y": 50, "z": 0},
  "dimensions": {"length": 304, "width": 140, "height": 61}
}

Response: {
  "isValid": false,
  "hasErrors": true,
  "issues": [{
    "message": "Part collides with existing part (ProductId: existing-gpu-id) at position (40, 40, 0)",
    "severity": "Error",
    "category": "Collision/PartConflict"
  }]
}
```

## Recursive Slot Design

Slots can contain sub-slots to model hierarchical component relationships:

```
Chamber (PC Case)
├── Motherboard Slot
│   ├── CPU Slot (sub-slot)
│   ├── RAM Slot 1 (sub-slot)
│   ├── RAM Slot 2 (sub-slot)
│   ├── RAM Slot 3 (sub-slot)
│   └── RAM Slot 4 (sub-slot)
├── GPU Slot 1
└── GPU Slot 2
```

Positions are automatically flattened to global coordinates for collision detection.

## Event Sourcing

The following events are emitted for spatial operations:

- **ChamberConfigured**: Raised when a chamber is created or configured
- **SlotAddedToChamber**: Raised when a slot is added to a chamber
- **PartInstalledInSlot**: Raised when a part is installed in a slot
- **PartRemovedFromSlot**: Raised when a part is removed from a slot

## Future Enhancements

### Composite Dimensions
Support for complex part shapes (L-shaped, asymmetric):
```json
{
  "dimensions": {
    "primary": {"length": 304, "width": 140, "height": 61},
    "components": [
      {"offset": {"x": 0, "y": 0, "z": 61}, "dimensions": {"length": 50, "width": 50, "height": 20}}
    ]
  }
}
```

### Exclusion Zones
Define keep-out areas for clearance requirements:
```json
{
  "exclusionZones": [
    {
      "name": "RAM clearance zone",
      "position": {"x": 100, "y": 50, "z": 0},
      "dimensions": {"length": 150, "width": 40, "height": 60}
    }
  ]
}
```

### 3D Visualization Data
Export chamber and part data for 3D rendering:
```json
{
  "visualization": {
    "format": "gltf",
    "meshes": [...],
    "materials": [...]
  }
}
```

## Technical Details

### Coordinate System
- Origin (0, 0, 0) is at the front-bottom-left corner of the chamber
- X-axis: Length (front to back)
- Y-axis: Width (left to right)
- Z-axis: Height (bottom to top)
- All measurements in millimeters

### AABB Collision Detection
The engine uses Axis-Aligned Bounding Box collision detection:

```csharp
bool intersects = 
    box1.Min.X < box2.Max.X && box1.Max.X > box2.Min.X &&
    box1.Min.Y < box2.Max.Y && box1.Max.Y > box2.Min.Y &&
    box1.Min.Z < box2.Max.Z && box1.Max.Z > box2.Min.Z;
```

This provides fast, accurate collision detection for rectangular parts.

## Integration with Existing Compatibility Engine

The spatial topology engine complements the existing compatibility validator:

1. **Compatibility Validator**: Checks logical compatibility (socket, DDR type, wattage)
2. **Spatial Validator**: Checks physical compatibility (fit, clearance, collisions)

Both validators should be used together for comprehensive validation:

```csharp
// Check logical compatibility
var compatibilityResult = await compatibilityValidator.ValidateBuild(products);

// Check spatial compatibility (for parts with 3D data)
var spatialResult = spatialValidator.ValidateChamber(chamber);

// Combine results
bool isFullyCompatible = compatibilityResult.IsCompatible && spatialResult.IsValid;
```
