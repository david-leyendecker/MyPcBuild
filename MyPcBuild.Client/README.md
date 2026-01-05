# MyPCBuild.Client - Vue.js Frontend

A modern TypeScript + Vue.js 3 web application for planning and building custom PC configurations with real-time compatibility validation.

## Technology Stack

- **Framework**: Vue.js 3 (Composition API)
- **Language**: TypeScript
- **Build Tool**: Vite
- **UI Components**: PrimeVue 4
- **Responsive Design**: PrimeFlex
- **State Management**: Pinia
- **Routing**: Vue Router
- **HTTP Client**: Axios
- **Styling**: SCSS with PrimeFlex utilities

## Project Structure

```
src/
├── api/                    # API client services
│   ├── client.ts          # Axios instance configuration
│   ├── builds.ts          # Build management API
│   └── catalog.ts         # Product catalog API
├── components/            # Reusable Vue components
│   ├── AppHeader.vue
│   ├── CompatibilityPanel.vue
│   └── AddPartDialog.vue
├── stores/               # Pinia state management
│   ├── buildStore.ts
│   └── catalogStore.ts
├── views/                # Page components
│   ├── BuildsListView.vue
│   ├── BuildDetailView.vue
│   └── CatalogView.vue
├── router/               # Vue Router configuration
│   └── index.ts
├── App.vue              # Root component
├── main.ts              # Application entry point
└── style.css            # Global styles
```

## Getting Started

### Prerequisites

- Node.js 18+ 
- npm or yarn

### Installation

```bash
cd MyPcBuild.Client
npm install
```

### Development

Start the development server on `http://localhost:5173`:

```bash
npm run dev
```

The application will proxy API requests to `http://localhost:5000` by default.

### Build

Create an optimized production build:

```bash
npm run build
```

### Preview

Preview the production build locally:

```bash
npm run preview
```

## Configuration

### API Base URL

The API client is configured in `src/api/client.ts`. By default:
- **Development**: `http://localhost:5000/api`
- **Production**: `/api`

Update the base URL if your API server is running on a different port.

### PrimeVue Theme

PrimeVue is configured with the Aura theme in `src/main.ts`. Global styles are defined in `src/style.css` with a dark theme optimized for gaming/tech aesthetics.

### Vue Router

Routes are configured in `src/router/index.ts`:
- `/` - My Builds (list view)
- `/builds/:id` - Build Detail
- `/catalog` - Product Catalog

## Features

### Build Management
- ✅ Create new PC builds
- ✅ Add/remove components
- ✅ Real-time compatibility validation
- ✅ Total cost calculation
- ✅ Build persistence via API

### Product Catalog
- ✅ Search products by name
- ✅ Filter by category (CPU, GPU, RAM, etc.)
- ✅ View product specifications
- ✅ Add products to builds

### Compatibility Engine
- ✅ Real-time validation with error/warning severity
- ✅ Visual feedback for component compatibility
- ✅ Performance and form-factor checks

### Design
- ✅ Dark theme with glassmorphic elements
- ✅ Responsive grid layouts with PrimeFlex
- ✅ Smooth animations and transitions
- ✅ Mobile-friendly interface

## API Integration

The application communicates with the `MyPcBuild.ApiService` backend via REST APIs:

### Builds API
```typescript
GET    /api/builds                        // List all builds
GET    /api/builds/:id                   // Get build details
POST   /api/builds                       // Create new build
PUT    /api/builds/:id                   // Update build
DELETE /api/builds/:id                   // Delete build
POST   /api/builds/:id/parts             // Add component
DELETE /api/builds/:id/parts/:productId  // Remove component
GET    /api/builds/:id/validate          // Validate compatibility
```

### Catalog API
```typescript
GET /api/catalog/search              // Search products
GET /api/catalog/category/:category  // Get products by category
GET /api/catalog/:id                 // Get product details
```

## State Management (Pinia)

### Build Store (`buildStore`)
Manages PC build state and operations:
- `builds` - Array of user builds
- `currentBuild` - Currently selected build
- `validationIssues` - Compatibility validation results
- `errors` / `warnings` - Filtered issues by severity

### Catalog Store (`catalogStore`)
Manages product catalog state:
- `products` - Array of products
- `selectedCategory` - Currently filtered category
- `searchQuery` - Current search term

## Component Architecture

### AppHeader
Navigation header with logo and menu links.

### CompatibilityPanel
Displays validation results with error/warning indicators.

### AddPartDialog
Modal dialog for searching and adding components to a build.

## Styling Guide

### Color Scheme
- **Primary**: `#00d4ff` (Cyan)
- **Success**: `#00ff00` (Green)
- **Error**: `#ff4444` (Red)
- **Warning**: `#ffaa00` (Orange)
- **Background**: `#1a1a2e` → `#16213e` (Dark gradient)

### PrimeFlex Utilities
- `.w-full` - Full width
- `.m-0` - No margin
- `.p-3` - Padding 1rem

## Contributing

When adding new features:

1. Create API client methods in `src/api/`
2. Add Pinia store actions in `src/stores/`
3. Create components in `src/components/` or `src/views/`
4. Follow the TypeScript strict mode requirements
5. Use PrimeVue components for consistency
6. Apply dark theme color variables

## Troubleshooting

### API Connection Issues
- Ensure the .NET API is running on port 5000
- Check CORS configuration in the API (`Program.cs`)
- Verify proxy settings in `vite.config.ts`

### PrimeVue Styling Not Applied
- Verify PrimeVue CSS imports in `src/main.ts`
- Check that `src/style.css` is imported before components
- Clear browser cache and rebuild with `npm run build`

### Build Errors
- Delete `node_modules` and `package-lock.json`, then run `npm install`
- Ensure TypeScript version matches: `5.9.3`
- Check for strict mode compilation errors

## Performance Optimization

- Lazy load routes in `src/router/index.ts` for SPA code splitting
- Use PrimeVue's lazy loading components
- Implement virtual scrolling for large product lists
- Cache API responses in Pinia stores

## Future Enhancements

- [ ] PWA offline support
- [ ] Build sharing and comparison
- [ ] Advanced filtering options
- [ ] Performance benchmarking indicators
- [ ] User accounts and cloud sync
- [ ] Dark/Light theme toggle
- [ ] Build templates and suggestions
