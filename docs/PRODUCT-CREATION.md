# Product Creation Feature

This feature allows users to create new products in the catalog with category-specific fields and spatial property definitions.

## Quick Start

1. Navigate to Catalog view (`/catalog`)
2. Click "Create Product" button
3. Fill basic info (category, name, manufacturer, price)
4. Fill category-specific fields
5. Click "Create Product"

## Architecture

### Backend Endpoints

- `POST /api/catalog/products` - Create new product
- `GET /api/catalog/field-definitions/{category}` - Get field metadata

### Frontend Components

- **ProductCreateView** - Main creation wizard
- **DynamicFieldRenderer** - Generic field renderer
- **DimensionsEditor** - 3D dimensions input
- **SlotsEditor** - Installation slots manager
- **ChambersEditor** - Chamber/compartment manager

## Field Types

The system supports:
- `text` - Text input
- `number` - Number input  
- `boolean` - Checkbox
- `select` - Dropdown
- `multi-select` - Multi-select dropdown
- `dimensions` - 3D dimensions (L×W×H)
- `slots` - Slot definitions
- `chambers` - Chamber definitions

## Design Approaches

Two implementation patterns included:

1. **Dynamic Field Renderer** (active) - Backend-driven, flexible
2. **Category-Specific Forms** (example) - Type-safe, customizable

See CpuForm.vue for category-specific example.

## Future Enhancements

- Image upload
- Client-side validation
- Draft saving
- Batch import
- Visual 3D editor
- Product templates
