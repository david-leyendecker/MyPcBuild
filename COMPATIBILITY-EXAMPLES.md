# Compatibility Validation Examples

## Example 1: Socket Mismatch (Error)

**Components:**
- CPU: Intel Core i9-14900K (LGA1700)
- Motherboard: ASUS ROG Strix X670E (AM5)

**Validation Result:**
```json
{
  "isCompatible": false,
  "hasErrors": true,
  "hasWarnings": false,
  "issues": [
    {
      "message": "CPU socket LGA1700 is incompatible with motherboard socket AM5",
      "severity": "Error",
      "category": "CPU/Motherboard"
    }
  ]
}
```

## Example 2: DDR Type Mismatch (Error)

**Components:**
- Motherboard: ASUS ROG Strix X670E (AM5, DDR5)
- RAM: Corsair Vengeance LPX 16GB DDR4-3200

**Validation Result:**
```json
{
  "isCompatible": false,
  "hasErrors": true,
  "issues": [
    {
      "message": "Corsair Vengeance LPX 16GB (2x8GB) DDR4-3200 (DDR4) is incompatible with motherboard memory type (DDR5)",
      "severity": "Error",
      "category": "RAM/Motherboard"
    }
  ]
}
```

## Example 3: Insufficient PSU Wattage (Error)

**Components:**
- CPU: AMD Ryzen 9 7950X (170W TDP)
- GPU: NVIDIA RTX 4090 (450W TDP)
- PSU: Seasonic Focus GX-650 (650W)

**Calculation:**
- Total System Power: 170W + 450W + 150W = 770W
- PSU Wattage: 650W

**Validation Result:**
```json
{
  "isCompatible": false,
  "hasErrors": true,
  "issues": [
    {
      "message": "PSU wattage (650W) is insufficient for estimated system power draw (770W). Recommended: 924W+",
      "severity": "Error",
      "category": "PSU"
    }
  ]
}
```

## Example 4: GPU Too Long (Error)

**Components:**
- GPU: NVIDIA RTX 4090 (304mm length)
- Case: Fractal Design Meshify C (315mm max GPU)

**Validation Result:**
```json
{
  "isCompatible": true,
  "hasWarnings": true,
  "issues": [
    {
      "message": "GPU length (304mm) is close to case limit (315mm) - tight fit",
      "severity": "Warning",
      "category": "GPU/Case"
    }
  ]
}
```

## Example 5: Form Factor Mismatch (Error)

**Components:**
- Motherboard: ASUS ROG Strix X670E (ATX)
- Case: NZXT H510i (MicroATX)

**Validation Result:**
```json
{
  "isCompatible": false,
  "hasErrors": true,
  "issues": [
    {
      "message": "Case form factor (MicroATX) is incompatible with motherboard form factor (ATX)",
      "severity": "Error",
      "category": "Case/Motherboard"
    }
  ]
}
```

## Example 6: Cooler Socket Incompatibility (Error)

**Components:**
- CPU: Intel Core i9-14900K (LGA1700)
- Cooler: (Older cooler supporting only LGA1200)

**Validation Result:**
```json
{
  "isCompatible": false,
  "hasErrors": true,
  "issues": [
    {
      "message": "Cooler does not support CPU socket LGA1700. Supported sockets: AM4, LGA1200",
      "severity": "Error",
      "category": "Cooler/CPU"
    }
  ]
}
```

## Example 7: Multiple Issues

**Components:**
- CPU: Intel Core i9-14900K (LGA1700, 125W TDP)
- Motherboard: ASUS ROG Strix X670E (AM5, DDR5)
- RAM: Corsair Vengeance LPX DDR4-3200
- GPU: RTX 4090 (450W TDP, 304mm)
- Case: NZXT H510i (MicroATX, 381mm max GPU)
- PSU: Seasonic Focus GX-650 (650W)

**Validation Result:**
```json
{
  "isCompatible": false,
  "hasErrors": true,
  "hasWarnings": true,
  "issues": [
    {
      "message": "CPU socket LGA1700 is incompatible with motherboard socket AM5",
      "severity": "Error",
      "category": "CPU/Motherboard"
    },
    {
      "message": "Corsair Vengeance LPX 16GB (2x8GB) DDR4-3200 (DDR4) is incompatible with motherboard memory type (DDR5)",
      "severity": "Error",
      "category": "RAM/Motherboard"
    },
    {
      "message": "Case form factor (MicroATX) is incompatible with motherboard form factor (ATX)",
      "severity": "Error",
      "category": "Case/Motherboard"
    },
    {
      "message": "PSU wattage (650W) is insufficient for estimated system power draw (725W). Recommended: 870W+",
      "severity": "Error",
      "category": "PSU"
    }
  ]
}
```

## Example 8: Perfect AMD Build ✅

**Components:**
- CPU: AMD Ryzen 7 7800X3D (AM5, 120W TDP)
- Motherboard: MSI MAG B650 TOMAHAWK WiFi (AM5, DDR5, ATX)
- RAM: G.Skill Trident Z5 RGB 32GB DDR5-6000
- GPU: AMD Radeon RX 7900 XTX (355W TDP, 287mm)
- Case: Lian Li O11 Dynamic EVO (ATX, 420mm max GPU)
- PSU: Corsair RM850x (850W)
- Cooler: Noctua NH-D15 (165mm, supports AM5, 250W TDP)

**Validation Result:**
```json
{
  "isCompatible": true,
  "hasErrors": false,
  "hasWarnings": false,
  "issues": []
}
```

**Why it works:**
- ✅ CPU socket AM5 matches motherboard
- ✅ DDR5 RAM matches motherboard
- ✅ GPU (287mm) fits in case (420mm max)
- ✅ ATX motherboard fits in ATX case
- ✅ PSU (850W) exceeds requirement (~675W)
- ✅ Cooler supports AM5 and covers CPU TDP

## Example 9: Compatible with Warnings ⚠️

**Components:**
- CPU: AMD Ryzen 9 7950X (AM5, 170W TDP)
- Motherboard: ASUS ROG Strix X670E (AM5, DDR5, ATX)
- GPU: NVIDIA RTX 4090 (450W TDP, 304mm)
- Case: Lian Li O11 Dynamic EVO (ATX, 420mm max GPU)
- PSU: Corsair RM850x (850W)
- Cooler: Noctua NH-D15 (165mm, 250W TDP)

**Validation Result:**
```json
{
  "isCompatible": true,
  "hasErrors": false,
  "hasWarnings": true,
  "issues": [
    {
      "message": "PSU wattage (850W) is below recommended (924W) for optimal efficiency",
      "severity": "Warning",
      "category": "PSU"
    },
    {
      "message": "GPU requires 16-pin power connector (or adapter for 2x 8-pin), PSU has 6x 8-pin connectors",
      "severity": "Warning",
      "category": "GPU/PSU"
    }
  ]
}
```

**Notes:**
- Build will work but needs power adapter for GPU
- PSU has enough wattage but running close to capacity
- Consider upgrading to 1000W+ PSU for better efficiency

## Testing the API

### Test Compatible Build
```bash
curl -X POST http://localhost:5000/api/compatibility/validate \
  -H "Content-Type: application/json" \
  -d '{
    "productIds": [
      "ryzen-7-7800x3d-guid",
      "msi-b650-tomahawk-guid",
      "trident-z5-32gb-guid",
      "rx-7900-xtx-guid",
      "lian-li-o11-guid",
      "rm850x-guid"
    ]
  }'
```

### Test Incompatible Build
```bash
curl -X POST http://localhost:5000/api/compatibility/validate \
  -H "Content-Type: application/json" \
  -d '{
    "productIds": [
      "intel-i9-14900k-guid",
      "asus-x670e-guid"
    ]
  }'
```
