# Compatibility Validation Engine

## Overview
Multi-level compatibility validation engine that checks PC component compatibility with errors and warnings.

## Validation Rules

### 1. CPU ↔ Motherboard Compatibility
**Error Level:**
- ❌ **Socket Mismatch**: CPU socket must match motherboard socket
  - Example: AMD Ryzen (AM5) requires AM5 motherboard
  - Example: Intel Core (LGA1700) requires LGA1700 motherboard

### 2. RAM ↔ Motherboard Compatibility
**Error Level:**
- ❌ **DDR Type Mismatch**: RAM type (DDR4/DDR5) must match motherboard
- ❌ **Capacity Exceeded**: Total RAM capacity exceeds motherboard maximum
- ❌ **Insufficient Slots**: More RAM sticks than available memory slots

**Examples:**
- DDR5 RAM with DDR4-only motherboard → Error
- 128GB total RAM on board with 64GB max → Error
- 6 RAM sticks on motherboard with 4 slots → Error

### 3. GPU Compatibility
**Error Level:**
- ❌ **Length Exceeded**: GPU length exceeds case maximum
- ❌ **Insufficient Power Connectors**: PSU lacks required GPU power connectors

**Warning Level:**
- ⚠️ **Tight Fit**: GPU length within 10% of case limit
- ⚠️ **Power Adapter Needed**: GPU needs 16-pin but PSU has 8-pin (adapter possible)

### 4. Case Compatibility
**Error Level:**
- ❌ **Form Factor Mismatch**: 
  - MicroATX case cannot fit ATX motherboard
  - Mini-ITX case can only fit Mini-ITX motherboard
- ❌ **Cooler Too Tall**: CPU cooler height exceeds case clearance
- ❌ **PSU Too Long**: PSU length exceeds case maximum

**Warning Level:**
- ⚠️ **Cooler Close to Limit**: Cooler height within 5% of case limit

**Form Factor Compatibility:**
```
ATX Case       → ATX, MicroATX, Mini-ITX ✓
MicroATX Case  → MicroATX, Mini-ITX ✓
Mini-ITX Case  → Mini-ITX only ✓
```

### 5. Power Supply (PSU) Validation
**Error Level:**
- ❌ **Insufficient Wattage**: PSU wattage below total system TDP + overhead

**Warning Level:**
- ⚠️ **Below Recommended**: PSU below 120% of estimated power draw

**Calculation:**
```
Total Power = CPU TDP + GPU TDP + 150W (overhead)
Recommended = Total Power × 1.2 (for efficiency)
```

**Examples:**
- CPU: 170W, GPU: 450W → ~620W minimum, ~744W recommended
- 650W PSU → Warning (below recommended)
- 500W PSU → Error (insufficient)

### 6. CPU Cooler Compatibility
**Error Level:**
- ❌ **Socket Incompatible**: Cooler doesn't support CPU socket
- ❌ **Insufficient TDP**: Cooler TDP rating below CPU TDP

**Warning Level:**
- ⚠️ **Minimal Headroom**: Cooler TDP less than 110% of CPU TDP
- ⚠️ **Verify Radiator Fit**: AIO radiator size compatibility with case

## API Endpoints

### POST /api/compatibility/validate
Validate a list of products for compatibility

**Request:**
```json
{
  "productIds": [
    "cpu-guid",
    "motherboard-guid",
    "gpu-guid",
    "ram-guid"
  ]
}
```

**Response:**
```json
{
  "isCompatible": false,
  "hasErrors": true,
  "hasWarnings": true,
  "issues": [
    {
      "message": "CPU socket AM5 is incompatible with motherboard socket LGA1700",
      "severity": "Error",
      "category": "CPU/Motherboard"
    },
    {
      "message": "GPU length (304mm) is close to case limit (315mm) - tight fit",
      "severity": "Warning",
      "category": "GPU/Case"
    }
  ]
}
```

### GET /api/builds/{buildId}/compatibility
Get compatibility validation for an existing build

**Response:** Same as validate endpoint

## Severity Levels

### Error (🔴)
Build **will not function** or components **physically incompatible**
- Requires immediate correction
- Examples: Socket mismatch, insufficient power, won't fit in case

### Warning (⚠️)
Build **may have issues** but is technically compatible
- Recommended to address but not critical
- Examples: Tight clearances, minimal PSU headroom, needs adapter

## Validation Categories

| Category | Components Checked |
|----------|-------------------|
| CPU/Motherboard | Socket compatibility |
| RAM/Motherboard | DDR type, capacity, slots |
| GPU/Case | Physical clearance |
| GPU/PSU | Power connectors, wattage |
| Cooler/CPU | Socket, TDP coverage |
| Cooler/Case | Height clearance |
| PSU/Case | Length clearance |
| PSU | Total system wattage |
| Case/Motherboard | Form factor |

## Real-World Example

**Incompatible Build:**
```
CPU: Intel Core i9-14900K (LGA1700)
Motherboard: ASUS ROG Strix X670E (AM5)
→ Error: Socket mismatch
```

**Compatible with Warning:**
```
CPU: AMD Ryzen 9 7950X (170W TDP)
GPU: RTX 4090 (450W TDP)
PSU: Corsair RM850x (850W)
→ Warning: Recommended 744W+ PSU for optimal efficiency
→ Compatible: 850W is sufficient
```

**Tight Fit Warning:**
```
GPU: RTX 4090 (304mm length)
Case: Case with 315mm max GPU length
→ Warning: Only 11mm clearance - tight fit
→ Compatible: Will physically fit
```

## Usage in Build Flow

1. **Real-time Validation**: Check compatibility when adding parts to a build
2. **Build Review**: Display compatibility report before finalizing
3. **Quick Validate**: Test compatibility of product combinations before purchasing
4. **Mobile Warning**: Show critical errors prominently on mobile UI

## Future Enhancements

- **Advanced Clearance**: RAM height vs CPU cooler clearance
- **Cable Management**: PSU cable length requirements
- **Airflow Analysis**: Fan placement and airflow validation
- **RGB Sync**: RGB ecosystem compatibility
- **BIOS Support**: CPU compatibility with motherboard BIOS version
- **Storage Slots**: M.2 slot lane sharing with SATA/PCIe
