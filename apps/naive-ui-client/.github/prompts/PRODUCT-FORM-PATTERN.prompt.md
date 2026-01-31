---
agent: 'agent'
model: Auto (copilot)
description: 'This prompt provides a standardized pattern for creating product form components using Naive UI in a Vue 3 application with TypeScript. It outlines the structure, state management, and coding conventions to ensure consistency across different product forms.'
---
Your goal is to refactor or create a new product form component following the established pattern used in the Naive UI client application. The component should utilize Naive UI components, Vue 3 Composition API, and TypeScript for type safety.

# Product Form Component Pattern

## Structure & Layout
- **Grid-based Layout**: Use `<n-grid :cols="2">` for a 2-column responsive grid layout
- **Form Items**: Use `<n-form-item-gi>` (grid item) within the grid for automatic alignment
- **Spanning**: Use `:span="2"` on form items that should take full width across both columns
- **No Explicit Spacing**: Let Naive UI's grid system handle spacing automatically

## Form Elements
- **Text Inputs**: Use `<n-input>` with `v-model:value` binding
- **Placeholders**: Always provide helpful placeholder examples (e.g., `"e.g., Mid Tower, Full Tower"`)
- **Disabled State**: Bind `:disabled="!editable"` to respect the editable prop
- **Custom Components**: Use dedicated components for complex value objects (e.g., `DimensionsInput`, `ChambersInput`)

## Component Interface
- **Props**:
  - `modelValue`: Accept `Partial<ProductTypeRequest> | Partial<ProductTypeResponse>`
  - `editable`: Optional boolean, default to `true`
- **Emits**: 
  - `'update:modelValue'` with the request type
- **Composition API**: Use `<script setup lang="ts">` with TypeScript

## State Management
- **Local State**: Create `localProduct` ref with default values for all fields
- **Two-way Binding**: 
  - Watch `props.modelValue` to sync external changes to local state (with `{ deep: true }`)
  - Watch `localProduct` to emit changes upward (with `{ deep: true }`)
- **Default Values**: Provide sensible defaults in the initial ref and in the watch for `props.modelValue` using nullish coalescing (`??`)

## Imports Pattern
```typescript
import { ref, watch } from 'vue';
import { NForm, NFormItemGi, NGrid, NInput } from 'naive-ui';
import type { [ProductType]Request, [ProductType]Response } from '@/types/products';
// Import any custom value object components needed
```

## Field Organization
- Group related fields visually
- Use descriptive labels (simple text, no colons)
- Place simpler fields (text inputs) before complex ones (custom components)
- Full-width fields (span 2) typically go at the bottom

## Template Structure
```vue
<template>
  <n-form>
    <n-grid :cols="2">
      <!-- Simple fields in pairs -->
      <n-form-item-gi label="Field Name">
        <n-input v-model:value="localProduct.fieldName" :disabled="!editable" placeholder="e.g., Example" />
      </n-form-item-gi>
      
      <!-- Full-width complex fields -->
      <n-form-item-gi label="Complex Field" :span="2">
        <CustomComponent v-model="localProduct.complexField" :editable="editable" />
      </n-form-item-gi>
    </n-grid>
  </n-form>
</template>
```

## Code Pattern
```typescript
const localProduct = ref<Partial<[ProductType]Request>>({
  field1: props.modelValue.field1 ?? 'default',
  field2: props.modelValue.field2 ?? 'default',
  // ... all fields with defaults
});

watch(
  () => props.modelValue,
  (newValue) => {
    Object.assign(localProduct.value, {
      field1: newValue.field1 ?? 'default',
      field2: newValue.field2 ?? 'default',
      // ... mirror defaults from initial ref
    });
  },
  { deep: true }
);

watch(
  localProduct,
  (newValue) => {
    emit('update:modelValue', newValue);
  },
  { deep: true }
);
```

## Key Principles
- Keep the template clean and declarative
- No manual styling or classes needed (rely on Naive UI defaults)
- All fields should respect the `editable` prop
- Maintain type safety throughout with proper TypeScript types
- Use value object components for complex nested data structures

## Example Reference
See [PcCaseProductForm.vue](../../src/components/ProductForms/PcCaseProductForm.vue) for a complete implementation.
