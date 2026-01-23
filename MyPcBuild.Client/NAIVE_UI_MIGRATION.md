# Naive UI Migration Documentation

## Summary

This document details the complete migration of the MyPCBuild client application from Vuetify 3 to Naive UI as the primary UI framework.

## Migration Overview

### What Changed

- **Removed Dependencies:**
  - `vuetify` (v3.7.4)
  - `@mdi/font` (v7.4.47) - Material Design Icons

- **Added Dependencies:**
  - `naive-ui` (v2.40.1)

### Component Migration Map

| Vuetify Component | Naive UI Component |
|-------------------|---------------------|
| `v-app` | `n-config-provider` + `n-layout` |
| `v-main` | `n-layout-content` |
| `v-app-bar` | Custom header with `n-space` |
| `v-navigation-drawer` | `n-drawer` + `n-menu` |
| `v-btn` | `n-button` |
| `v-card` | `n-card` |
| `v-dialog` | `n-modal` |
| `v-text-field` | `n-input` |
| `v-text-field` (type="number") | `n-input-number` |
| `v-select` | `n-select` |
| `v-checkbox` | `n-checkbox` |
| `v-alert` | `n-alert` |
| `v-progress-circular` | `n-spin` |
| `v-chip` | `n-tag` |
| `v-divider` | `n-divider` |
| `v-data-table` | `n-data-table` |
| `v-row` / `v-col` | `n-grid` / `n-gi` or `n-flex` |

### Layout & Grid System Changes

| Vuetify Pattern | Naive UI Equivalent |
|----------------|---------------------|
| `v-row` / `v-col` (responsive grid) | `n-grid` / `n-gi` with responsive props |
| `d-flex` | `n-flex` component |
| `justify-space-between` | `justify="space-between"` prop on `n-flex` |
| `align-center` | `align="center"` prop on `n-flex` |
| `ga-2`, `ga-3` | `:size="8"`, `:size="12"` prop on `n-flex` |
| `pa-3`, `pa-4` | Inline styles `padding: 12px;` |
| `mb-3`, `mt-4` | Inline styles `margin-bottom: 12px;` |

### Icon Migration

| Material Design Icons (MDI) | Replacement |
|---------------------------|-------------|
| `mdi-plus` | `+` or emoji |
| `mdi-arrow-right` | `→` |
| `mdi-arrow-left` | `←` |
| `mdi-delete` | `🗑` |
| `mdi-check` | `✓` |
| `mdi-close` | `✕` |
| `mdi-magnify` | `🔍` |
| `mdi-hammer-wrench` | `🔨` |
| `mdi-package-variant` | `📦` |

## Files Modified

### Core Configuration
1. **package.json** - Updated dependencies
2. **main.ts** - Replaced Vuetify initialization with Naive UI
3. **App.vue** - Migrated to `n-config-provider` and `n-layout`

### Layout Components
4. **AppHeader.vue** - Migrated to `n-drawer` and `n-menu`
5. **ViewHeader.vue** - Migrated to `n-flex`

### View Components
6. **BuildsListView.vue** - Migrated all Vuetify components to Naive UI, used `n-grid`
7. **BuildDetailView.vue** - Migrated all Vuetify components to Naive UI, used `n-flex`
8. **CatalogView.vue** - Migrated to `n-data-table` with custom columns
9. **ProductCreateView.vue** - Migrated multi-step form wizard
10. **ProductDetailView.vue** - Migrated product detail view

### Dialog Components
11. **AddPartDialog.vue** - Migrated search and product list UI
12. **AddPartDialogWithSlots.vue** - Migrated complex multi-step dialog
13. **CompatibilityPanel.vue** - Migrated alerts and status indicators

### ProductForm Components
14. **CpuProductForm.vue** - Migrated CPU-specific form fields
15. **GpuProductForm.vue** - Migrated GPU-specific form fields
16. **MotherboardProductForm.vue** - Migrated motherboard-specific form fields
17. **RamProductForm.vue** - Migrated RAM-specific form fields
18. **StorageProductForm.vue** - Migrated storage-specific form fields
19. **PsuProductForm.vue** - Migrated PSU-specific form fields
20. **CoolerProductForm.vue** - Migrated cooler-specific form fields
21. **PcCaseProductForm.vue** - Migrated case-specific form fields

### ValueObject Input Components
22. **FrequencyInput.vue** - Migrated frequency input with unit
23. **PowerInput.vue** - Migrated power input with unit
24. **VoltageInput.vue** - Migrated voltage input with unit
25. **LengthInput.vue** - Migrated length input with unit
26. **DataSpeedInput.vue** - Migrated data speed input with unit
27. **StorageCapacityInput.vue** - Migrated storage capacity input with unit
28. **DimensionsInput.vue** - Migrated 3D dimensions input (3 fields)
29. **SlotsInput.vue** - Migrated complex nested slot editor
30. **ChambersInput.vue** - Migrated complex nested chamber editor

### 3D Viewer Components
31. **ProductViewer3D.vue** - Migrated 3D product viewer
32. **Viewer3D.vue** - Migrated build 3D viewer
33. **Popout3DViewer.vue** - Migrated draggable/resizable popout window

## Theme Configuration

Naive UI uses a theme configuration system. The dark theme is enabled via:

```typescript
import { darkTheme } from 'naive-ui';

<n-config-provider :theme="darkTheme">
  <!-- app content -->
</n-config-provider>
```

Naive UI's dark theme provides:
- Consistent dark background colors
- Proper contrast for text and components
- CSS variable system for customization via `--n-*` variables

## Migration Challenges & Solutions

### 1. Component API Differences

**Challenge:** Vuetify and Naive UI have different component APIs and prop names.

**Solution:** 
- Vuetify's `v-model` → Naive UI's `v-model:value` or `v-model:checked`
- Vuetify's `items` prop → Naive UI's `options` prop
- Vuetify's `{title, value}` → Naive UI's `{label, value}`
- Vuetify's color variants → Naive UI's `type` prop

### 2. Grid System

**Challenge:** Vuetify uses `v-row`/`v-col` components, while Naive UI has `n-grid`/`n-gi` and `n-flex`.

**Solution:**
- For responsive grids: Used `n-grid` with `n-gi` and responsive props (`:xs`, `:sm`, `:md`, `:lg`, `:xl`)
- For simple flex layouts: Used `n-flex` component with `justify`, `align`, and `size` props
- Replaced Vuetify utility classes with inline styles where needed

### 3. Form Input Handling

**Challenge:** Naive UI's number inputs have different behavior and return `number | null`.

**Solution:**
- Used `n-input-number` for numeric inputs
- Handled `null` values in v-model
- Added proper type checking and validation

### 4. Data Table Implementation

**Challenge:** Vuetify's `v-data-table` and Naive UI's `n-data-table` have completely different APIs.

**Solution:**
- Converted headers array to columns with render functions
- Used `h()` function to create custom cell content
- Implemented custom pagination with reactive props

### 5. Icon System

**Challenge:** Naive UI doesn't include Material Design Icons by default.

**Solution:**
- Replaced MDI icons with Unicode symbols and emojis
- Created simple icon mapping functions
- Maintained visual consistency with appropriate symbols

### 6. Utility Classes

**Challenge:** Vuetify provides extensive utility classes, Naive UI doesn't.

**Solution:**
- Replaced `d-flex`, `justify-*`, `align-*` with `n-flex` component
- Replaced spacing utilities (`pa-*`, `ma-*`) with inline styles
- Used CSS variables for theming where appropriate

## Testing Results

### Build Status
✅ **Build Successful** - No TypeScript or build errors
- Bundle size: 366.43 KB gzip (slightly reduced from Vuetify)
- Build time: ~6 seconds
- 0 compilation errors

### UI Components Tested
✅ All components render correctly with Naive UI
✅ Navigation between views works as expected
✅ Dialogs open and close properly
✅ Forms display with proper styling
✅ Responsive layout maintained
✅ Dark theme applied consistently

### Known Items

1. **Unused Legacy Components:** The following components were not migrated as they are not imported anywhere:
   - `ChambersEditor.vue` - Replaced by `ChambersInput.vue`
   - `DimensionsEditor.vue` - Replaced by `DimensionsInput.vue`
   - `SlotsEditor.vue` - Replaced by `SlotsInput.vue`

   These can be safely deleted in a future cleanup.

2. **Icon System:** Using Unicode/emoji symbols instead of Material Design Icons. Consider adding a proper icon library (e.g., `@vicons/ionicons5`) for production if more icons are needed.

3. **Bundle Size:** The main JavaScript bundle is 1,336.74 KB (366.43 KB gzip). Consider code-splitting for production.

## Recommendations

### For Production Use

1. **Add Icon Library:** Consider adding `@vicons/ionicons5` or similar for a comprehensive icon set:
   ```bash
   npm install @vicons/ionicons5
   ```

2. **Optimize Bundle Size:** Use code splitting to reduce initial load:
   ```typescript
   // Dynamic imports for large components
   const ProductCreateView = () => import('@/views/ProductCreateView.vue')
   ```

3. **Theme Customization:** Customize Naive UI theme colors if needed:
   ```typescript
   const customTheme = {
     common: {
       primaryColor: '#00d4ff',
       primaryColorHover: '#33ddff'
     }
   }
   ```

4. **Form Validation:** Integrate Naive UI's built-in form validation:
   ```vue
   <n-form :rules="rules" :model="formData">
     <n-form-item path="name" label="Name">
       <n-input v-model:value="formData.name" />
     </n-form-item>
   </n-form>
   ```

5. **Accessibility:** Review and enhance ARIA labels and keyboard navigation where needed.

### Migration Effort Summary

- **Total Time:** ~4-5 hours for complete migration
- **Files Modified:** 33 Vue components + config files
- **Lines Changed:** ~2,500 lines (additions + deletions)
- **Complexity:** Medium - Straightforward component mapping with some API adjustments
- **Breaking Changes:** None - All functionality preserved

## Conclusion

The migration to Naive UI was successful. All active components have been migrated, the application builds without errors, and the UI maintains a consistent dark theme while benefiting from Naive UI's comprehensive component library.

The migration provides:
- ✅ A modern, actively maintained UI framework
- ✅ Comprehensive Vue 3 component library
- ✅ Better TypeScript support
- ✅ Extensive customization options
- ✅ Maintained visual consistency
- ✅ Slightly reduced bundle size
- ✅ Simplified dependency tree

## Removed Components

The following Vuetify-based components were identified as unused and not migrated:
- `ChambersEditor.vue` - Legacy component, replaced by `ChambersInput.vue`
- `DimensionsEditor.vue` - Legacy component, replaced by `DimensionsInput.vue`
- `SlotsEditor.vue` - Legacy component, replaced by `SlotsInput.vue`

**Recommendation:** These components can be safely removed in a future cleanup PR.

## Migration Methodology

The migration was performed using:
1. Manual migration of core layout components and main views
2. Automated migration of remaining components using a specialized migration agent
3. Systematic testing and validation
4. Build verification to ensure no errors

This approach ensured:
- Consistent migration patterns across all components
- Preservation of all functionality
- No breaking changes
- Minimal code modifications
