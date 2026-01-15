# Product Management UI Rework - Integration Guide

## Overview

This document provides guidance for integrating the new modular product management components into the existing views.

## What Has Been Built

### 1. TypeScript Contracts (`/src/types/products.ts`)
Complete type definitions for all product categories matching the backend API DTOs:
- `CpuProductRequest/Response`
- `GpuProductRequest/Response`
- `MotherboardProductRequest/Response`
- `RamProductRequest/Response`
- `StorageProductRequest/Response`
- `PsuProductRequest/Response`
- `CoolerProductRequest/Response`
- `PcCaseProductRequest/Response`

### 2. Value Object Components (`/src/components/ValueObjects/`)
Reusable input components for API value types:
- `FrequencyInput.vue` - For GHz values (base clock, boost clock, memory speed)
- `PowerInput.vue` - For Watt values (TDP, wattage)
- `LengthInput.vue` - For millimeter values (height, length)
- `StorageCapacityInput.vue` - For GB values (RAM, VRAM, storage, max memory)
- `VoltageInput.vue` - For Volt values (RAM voltage)
- `DataSpeedInput.vue` - For MB/s values (read/write speeds)
- `DimensionsInput.vue` - For 3D dimensions (length × width × height)

All components support:
- `editable` prop for readonly/edit modes
- Proper unit display
- Type-safe v-model binding

### 3. Product Form Components (`/src/components/ProductForms/`)
Category-specific form components:
- `CpuProductForm.vue`
- `GpuProductForm.vue`
- `MotherboardProductForm.vue`
- `RamProductForm.vue`
- `StorageProductForm.vue`
- `PsuProductForm.vue`
- `CoolerProductForm.vue`
- `PcCaseProductForm.vue`

Features:
- Side-by-side layouts for related fields
- Full type safety
- Both edit and readonly modes
- Minimal layout logic (relies on parent containers)

### 4. ProductFormSelector (`/src/components/ProductFormSelector.vue`)
Smart wrapper component that selects the appropriate form based on category.

Props:
- `modelValue`: Partial<ProductRequest> | Partial<ProductResponse>
- `category`: string
- `editable`: boolean (default: true)

### 5. Typed API Client (`/src/api/catalogTyped.ts`)
Functions for working with typed product DTOs:
- `createTypedProduct(request: ProductRequest)`
- `getTypedProduct(id: string): ProductResponse`
- `updateTypedProduct(id: string, request: ProductRequest)`

### 6. Field Converters (`/src/utils/productFieldConverters.ts`)
Utility functions for converting between old field-based format and new typed format:
- `fieldsToTypedProduct(fields, category)` - Converts flat fields to typed product
- `typedProductToFields(product)` - Converts typed product to fields

## Integration Steps

### Step 1: Update ProductCreateView

Replace the DynamicFieldRenderer usage with ProductFormSelector:

**Before:**
```vue
<DynamicFieldRenderer 
  v-model="formData.fields"
  :field-definitions="fieldDefinitions"
/>
```

**After:**
```vue
<ProductFormSelector 
  v-model="productFormData"
  :category="formData.category"
  :editable="true"
/>
```

Changes needed:
1. Add `productFormData` ref to hold typed product data
2. Use `fieldsToTypedProduct` to convert AI-generated fields to typed format
3. Use `typedProductToFields` when submitting to API (or better: use `createTypedProduct` directly)

### Step 2: Update ProductDetailView

Replace the DynamicFieldRenderer usage with ProductFormSelector:

**Before:**
```vue
<DynamicFieldRenderer 
  v-else
  v-model="formData.fields"
  :field-definitions="fieldDefinitions"
/>
```

**After:**
```vue
<ProductFormSelector 
  v-model="productFormData"
  :category="formData.category"
  :editable="isEditMode"
/>
```

Changes needed:
1. Add `productFormData` ref for typed product data
2. Load product using `getTypedProduct` and map to form data
3. Save using `updateTypedProduct` with typed data

### Step 3: Remove Old Components

After successful integration:
1. Delete `/src/components/DynamicFieldRenderer.vue`
2. Delete `/src/components/CategoryForms/CpuForm.vue` (replaced by ProductForms/CpuProductForm.vue)

### Step 4: Update API Integration

The backend API already expects typed DTOs (ProductRequest/ProductResponse). Update the frontend to use these directly:

1. Replace `catalogApi.createProduct(request)` with `createTypedProduct(typedRequest)`
2. Replace `catalogApi.getProduct(id)` with `getTypedProduct(id)` where typed responses are needed
3. Replace `catalogApi.updateProduct(id, request)` with `updateTypedProduct(id, typedRequest)`

## Testing Checklist

- [ ] Create new CPU product - manual entry
- [ ] Create new CPU product - AI generation
- [ ] Create products for all 8 categories
- [ ] View product details in readonly mode
- [ ] Edit existing product
- [ ] Publish draft product
- [ ] Verify side-by-side layouts work correctly
- [ ] Verify all value objects display correct units
- [ ] Test readonly mode for all components
- [ ] Verify TypeScript compilation succeeds
- [ ] Test frontend builds successfully

## Notes

- The new components use typed objects directly, which aligns with the backend API design
- Value object components automatically handle the conversion between display values and API format
- The ProductFormSelector provides a single integration point, making it easy to swap implementations
- Field converters are provided for backward compatibility but direct typed approach is preferred
- All components follow the project's C# code style guidelines for consistency
