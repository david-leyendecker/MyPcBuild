# Product Catalog API

## Overview
The product catalog is automatically seeded on application startup with a comprehensive selection of PC components.

## Seeded Products

### CPUs (4 products)
- AMD Ryzen 9 7950X (AM5, 16C/32T)
- AMD Ryzen 7 7800X3D (AM5, 8C/16T, 3D V-Cache)
- Intel Core i9-14900K (LGA1700, 24C/32T)
- Intel Core i5-14600K (LGA1700, 14C/20T)

### Motherboards (4 products)
- ASUS ROG Strix X670E-E Gaming WiFi (AM5, DDR5, ATX)
- MSI MAG B650 TOMAHAWK WiFi (AM5, DDR5, ATX)
- ASUS ROG Maximus Z790 Hero (LGA1700, DDR5, ATX)
- Gigabyte B760M DS3H (LGA1700, DDR4, MicroATX)

### GPUs (3 products)
- NVIDIA GeForce RTX 4090 (24GB GDDR6X)
- NVIDIA GeForce RTX 4070 Ti (12GB GDDR6X)
- AMD Radeon RX 7900 XTX (24GB GDDR6)

### RAM (3 products)
- G.Skill Trident Z5 RGB 32GB DDR5-6000
- Corsair Vengeance 64GB DDR5-5600
- Corsair Vengeance LPX 16GB DDR4-3200

### PC Cases (3 products)
- Lian Li O11 Dynamic EVO (ATX)
- Fractal Design Meshify C (ATX)
- NZXT H510i (MicroATX)

### PSUs (3 products)
- Corsair RM850x 850W 80+ Gold
- EVGA SuperNOVA 1000W 80+ Gold
- Seasonic Focus GX-650 650W 80+ Gold

### Storage (2 products)
- Samsung 990 Pro 2TB NVMe Gen4
- WD Black SN850X 1TB NVMe Gen4

### Coolers (2 products)
- Noctua NH-D15 (Air Cooler)
- Corsair iCUE H150i Elite LCD XT (360mm AIO)

**Total: 24 products across 8 categories**

## API Endpoints

### GET /api/catalog/products
Get paginated list of products with optional filtering, searching, and sorting.

**Query Parameters:**
All query parameters are validated using DataAnnotations.
- `category` (optional): Filter by category name (e.g., "CPU", "GPU", "Motherboard")
- `search` (optional): Search in product name or manufacturer
- `page` (default: 1, min: 1): Page number - validated with `[Range(1, int.MaxValue)]`
- `itemsPerPage` (default: 10, min: 1, max: 100): Items per page - validated with `[Range(1, 100)]`
- `sortBy` (default: "name"): Sort field - one of: "name", "category", "categoryName", "price", "manufacturer"
- `sortDesc` (default: false): Sort in descending order

**Response:**
```json
{
  "items": [
    {
      "id": "guid",
      "name": "AMD Ryzen 9 7950X",
      "categoryName": "CPU",
      "price": 549.99,
      "manufacturer": "AMD"
    }
  ],
  "total": 24,
  "page": 1,
  "itemsPerPage": 10,
  "sortBy": "name",
  "sortDesc": false,
  "category": null,
  "search": null
}
```

**Notes:**
- Results are always ordered to ensure consistent pagination
- Query parameters use the shared `QueryParameters` class with DataAnnotations validation
- Invalid page numbers (< 1) return HTTP 400 with validation error message
- Invalid itemsPerPage (< 1 or > 100) returns HTTP 400 with validation error message
- The `total` field indicates the total number of items matching the filters

### GET /api/catalog/products/{id}
Get single product by GUID

**Response:**
```json
{
  "id": "guid",
  "name": "AMD Ryzen 9 7950X",
  "category": 0,
  "price": 549.99,
  "manufacturer": "AMD",
  "specifications": {
    "Socket": "AM5",
    "Cores": 16,
    "Threads": 32,
    ...
  }
}
```

### GET /api/catalog/categories
Get list of all product categories

**Response:**
```json
[
  { "value": 0, "name": "CPU" },
  { "value": 1, "name": "Motherboard" },
  { "value": 2, "name": "GPU" },
  ...
]
```

### GET /api/catalog/search
Quick search endpoint

**Query Parameters:**
- `query` (required): Search term
- `maxResults` (default: 10): Maximum results to return

**Response:**
```json
[
  { "id": "...", "name": "AMD Ryzen...", ... }
]
```

## Product Categories Enum

```csharp
public enum ProductCategory
{
    CPU = 0,
    Motherboard = 1,
    GPU = 2,
    RAM = 3,
    PCCase = 4,
    PSU = 5,
    Storage = 6,
    Cooler = 7
}
```

## Product Specifications

Each product includes a `Specifications` dictionary with category-specific fields:

**CPU:**
- Socket, Cores, Threads, BaseClock, BoostClock, TDP, IntegratedGraphics

**Motherboard:**
- Socket, Chipset, FormFactor, MemoryType, MaxMemory, MemorySlots, PCIeSlots, M2Slots, WiFi, RGB

**GPU:**
- ChipsetManufacturer, Series, VRAM, MemoryType, CoreClock, BoostClock, TDP, Length, Slots, PowerConnectors

**RAM:**
- Type (DDR4/DDR5), Capacity, Configuration, Speed, CASLatency, Voltage, RGB, HeatSpreader

**PC Case:**
- FormFactor, Color, DriveBays, FanMounts

**PSU:**
- Wattage, Efficiency, Modular, FormFactor, PCIe8Pin, SATA, Molex

**Storage:**
- Type, Interface, FormFactor, Capacity, ReadSpeed, WriteSpeed, TBW, DRAM, PCIeGen

**Cooler:**
- Type (Air/AIO), Height/RadiatorSize, FanSize, FanCount, TDP, Sockets, RGB, NoiseLevel
