# Implementation Plan: MyPcBuild shadcn-vue Frontend

> **Goal**: Build a modern alternative frontend for MyPcBuild using **TypeScript**, **Vue 3**, and **shadcn-vue**, located in `apps/shadcn-vue-client/`. This is **not** a 1:1 copy of the existing Naive UI client — it introduces redesigned workflows for part management and build management, supports light/dark mode, and follows best practices for component and view architecture.

---

## Table of Contents

1. [Current Application Analysis](#1-current-application-analysis)
2. [Technology Stack](#2-technology-stack)
3. [Project Structure](#3-project-structure)
4. [Design System & Theming](#4-design-system--theming)
5. [Layout & Navigation](#5-layout--navigation)
6. [Feature Modules](#6-feature-modules)
7. [API Integration Layer](#7-api-integration-layer)
8. [State Management](#8-state-management)
9. [Redesigned Workflows](#9-redesigned-workflows)
10. [Component Inventory](#10-component-inventory)
11. [Implementation Phases](#11-implementation-phases)
12. [Development & Build Configuration](#12-development--build-configuration)

---

## 1. Current Application Analysis

### 1.1 Existing Frontend (`apps/naive-ui-client`)

| Aspect | Details |
|--------|---------|
| **Framework** | Vue 3.5 + TypeScript 5.9 |
| **UI Library** | Naive UI 2.43 |
| **State** | Pinia 3.0 |
| **Router** | Vue Router 4.6 |
| **HTTP** | Axios |
| **3D** | Three.js |
| **Build** | Vite 7 |
| **Styling** | Scoped CSS + Naive UI theme system |

**Views (5 pages):**
- `BuildsListView` — Card grid of all builds with create dialog
- `BuildDetailView` — Single build with parts list, 3D viewer, compatibility panel
- `CatalogView` — Data table with category filter, search, pagination
- `ProductCreateView` — 3-step wizard (manual or AI mode)
- `ProductDetailView` — View/edit product with category-specific forms

**State stores:** `buildStore`, `catalogStore`, `themeStore`

**Limitations addressed by the redesign:**
- Build creation and part management use separate, disconnected dialogs
- No unified dashboard or overview page
- Product creation wizard is linear and rigid
- No breadcrumb navigation or contextual awareness
- Compatibility issues are shown only in the detail view, not during part selection
- No confirmation or undo for destructive actions

### 1.2 Backend API (`MyPcBuild.ApiService`)

| Area | Endpoints |
|------|-----------|
| **Builds** | `GET /api/builds`, `GET /api/builds/{id}`, `POST /api/builds`, `PUT /api/builds/{id}`, `POST /api/builds/{id}/parts`, `POST /api/builds/{id}/parts/slot`, `DELETE /api/builds/{id}/parts/{productId}`, `GET /api/builds/{id}/slots`, `GET /api/builds/{id}/compatibility` |
| **Catalog** | `GET /api/catalog/products`, `GET /api/catalog/products/{id}`, `POST /api/catalog/products`, `PUT /api/catalog/products/{id}`, `POST /api/catalog/products/{id}/publish`, `GET /api/catalog/categories`, `GET /api/catalog/products/fields/{category}`, `POST /api/catalog/products/search`, `POST /api/catalog/products/generate-ai` |
| **Compatibility** | `POST /api/compatibility/validate`, `GET /api/builds/{buildId}/compatibility` |
| **Spatial** | `POST /api/spatial/validate-installation`, `POST /api/spatial/validate-build` |

**8 product categories:** CPU, GPU, Motherboard, RAM, Storage, PSU, Cooler, PC Case

**Key domain concepts:**
- Event-sourced Build aggregate (BuildCreated, PartAdded, PartAddedToSlot, PartRemoved, BuildRenamed)
- Polymorphic product types with category-specific fields
- Strongly-typed value objects (Frequency, Power, StorageCapacity, Dimensions, etc.)
- Spatial model with Chambers → Slots → SubSlots hierarchy
- Draft/Published product lifecycle
- Real-time compatibility validation (Error/Warning severity)

---

## 2. Technology Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| **Framework** | Vue 3 (Composition API, `<script setup>`) | ^3.5 |
| **Language** | TypeScript (strict mode) | ^5.6 |
| **UI Components** | shadcn-vue | latest |
| **Styling** | Tailwind CSS 4 | ^4.0 |
| **Icons** | Lucide Vue Next | latest |
| **State Management** | Pinia | ^3.0 |
| **Routing** | Vue Router | ^4.5 |
| **HTTP Client** | Axios | ^1.7 |
| **Form Validation** | VeeValidate + Zod | latest |
| **3D Rendering** | Three.js | ^0.170 |
| **Date Formatting** | date-fns | ^4.0 |
| **Build Tool** | Vite | ^6.0 |
| **Linting** | ESLint + @vue/eslint-config-typescript | latest |

---

## 3. Project Structure

```
apps/shadcn-vue-client/
├── index.html
├── package.json
├── tsconfig.json
├── tsconfig.app.json
├── tsconfig.node.json
├── vite.config.ts
├── tailwind.config.ts              # Tailwind + shadcn theme tokens
├── components.json                 # shadcn-vue component registry
├── postcss.config.js
├── Dockerfile
├── nginx.conf
├── env.d.ts
├── .env.development
├── .env.production
│
├── public/
│   └── favicon.ico
│
└── src/
    ├── main.ts                     # App bootstrap
    ├── App.vue                     # Root layout
    │
    ├── assets/
    │   └── styles/
    │       ├── globals.css         # Tailwind directives + CSS variables
    │       └── themes.css          # Light/dark CSS variable overrides
    │
    ├── lib/
    │   └── utils.ts                # shadcn cn() utility
    │
    ├── api/                        # HTTP service layer
    │   ├── client.ts               # Axios instance & interceptors
    │   ├── builds.ts               # Build endpoints
    │   ├── catalog.ts              # Catalog endpoints
    │   ├── compatibility.ts        # Compatibility endpoints
    │   └── types.ts                # Shared API request/response types
    │
    ├── types/                      # Domain type definitions
    │   ├── build.ts                # Build, BuildPart, CompatibilityIssue
    │   ├── product.ts              # Product hierarchy (8 categories)
    │   └── spatial.ts              # Vector3, Dimensions, Slot, Chamber
    │
    ├── stores/                     # Pinia stores
    │   ├── buildStore.ts           # Build CRUD + validation
    │   ├── catalogStore.ts         # Product listing + search
    │   └── themeStore.ts           # Light/dark mode preference
    │
    ├── composables/                # Reusable composition functions
    │   ├── useTheme.ts             # Theme toggle logic
    │   ├── usePagination.ts        # Pagination state
    │   ├── useConfirmDialog.ts     # Confirm before destructive actions
    │   ├── use3DViewer.ts          # Three.js scene management
    │   └── useToast.ts             # Toast notification wrapper
    │
    ├── router/
    │   └── index.ts                # Route definitions
    │
    ├── layouts/                    # Layout wrappers
    │   └── DefaultLayout.vue       # Sidebar + header + content
    │
    ├── components/                 # Reusable app components
    │   ├── ui/                     # shadcn-vue components (auto-generated)
    │   │   ├── button/
    │   │   ├── card/
    │   │   ├── dialog/
    │   │   ├── dropdown-menu/
    │   │   ├── input/
    │   │   ├── label/
    │   │   ├── select/
    │   │   ├── table/
    │   │   ├── tabs/
    │   │   ├── badge/
    │   │   ├── alert/
    │   │   ├── sheet/
    │   │   ├── separator/
    │   │   ├── skeleton/
    │   │   ├── tooltip/
    │   │   ├── toast/
    │   │   ├── command/
    │   │   ├── popover/
    │   │   ├── progress/
    │   │   ├── switch/
    │   │   ├── checkbox/
    │   │   ├── form/
    │   │   ├── breadcrumb/
    │   │   ├── collapsible/
    │   │   └── scroll-area/
    │   │
    │   ├── layout/                 # App layout components
    │   │   ├── AppSidebar.vue      # Main navigation sidebar
    │   │   ├── AppHeader.vue       # Top bar with breadcrumbs & theme toggle
    │   │   ├── ThemeToggle.vue     # Dark/light mode switch
    │   │   └── MobileNav.vue       # Mobile hamburger navigation
    │   │
    │   ├── builds/                 # Build-specific components
    │   │   ├── BuildCard.vue       # Build summary card (grid item)
    │   │   ├── BuildPartsList.vue  # Parts table within a build
    │   │   ├── BuildSummaryBar.vue # Cost, power, compatibility summary
    │   │   ├── BuildCategorySlots.vue  # Category-based slot overview
    │   │   └── CreateBuildDialog.vue   # Create new build form
    │   │
    │   ├── catalog/                # Catalog components
    │   │   ├── ProductCard.vue     # Product summary card
    │   │   ├── ProductFilters.vue  # Category + search filters sidebar
    │   │   ├── ProductTable.vue    # Tabular product list
    │   │   └── CategoryIcon.vue    # Icon by product category
    │   │
    │   ├── products/               # Product form components
    │   │   ├── ProductFormShell.vue     # Common form wrapper
    │   │   ├── CpuForm.vue
    │   │   ├── GpuForm.vue
    │   │   ├── MotherboardForm.vue
    │   │   ├── RamForm.vue
    │   │   ├── StorageForm.vue
    │   │   ├── PsuForm.vue
    │   │   ├── CoolerForm.vue
    │   │   ├── PcCaseForm.vue
    │   │   └── AiGenerateForm.vue      # AI product generation form
    │   │
    │   ├── compatibility/          # Compatibility display components
    │   │   ├── CompatibilityBadge.vue  # Inline compat status indicator
    │   │   ├── CompatibilityPanel.vue  # Full issue list panel
    │   │   └── IssueCard.vue           # Single compatibility issue
    │   │
    │   ├── spatial/                # 3D visualization
    │   │   ├── BuildViewer3D.vue
    │   │   ├── ProductViewer3D.vue
    │   │   └── ViewerControls.vue
    │   │
    │   └── shared/                 # Common reusable components
    │       ├── EmptyState.vue      # "No data" placeholder
    │       ├── LoadingState.vue    # Loading skeleton/spinner
    │       ├── ErrorState.vue      # Error with retry action
    │       ├── ConfirmDialog.vue   # Generic confirmation dialog
    │       ├── PageHeader.vue      # Page title + actions
    │       ├── PriceDisplay.vue    # Formatted currency display
    │       └── StatusBadge.vue     # Draft/Published status
    │
    └── views/                      # Page-level views (1 per route)
        ├── DashboardView.vue       # NEW: Overview dashboard
        ├── builds/
        │   ├── BuildsListView.vue  # All builds grid/list
        │   └── BuildDetailView.vue # Single build editor
        ├── catalog/
        │   ├── CatalogView.vue     # Product browser
        │   ├── ProductCreateView.vue   # Product creation
        │   └── ProductDetailView.vue   # Product detail/edit
        └── NotFoundView.vue        # 404 page
```

---

## 4. Design System & Theming

### 4.1 shadcn-vue Setup

Initialize with the shadcn-vue CLI to generate `components.json` and the `ui/` directory. Use the **"New York"** style variant for a clean, modern feel.

### 4.2 Color Palette (CSS Variables)

Define CSS variables in `globals.css` following the shadcn convention. Both light and dark palettes:

```css
/* Light mode (`:root`) */
--background: 0 0% 100%;
--foreground: 240 10% 3.9%;
--card: 0 0% 100%;
--primary: 240 5.9% 10%;
--secondary: 240 4.8% 95.9%;
--muted: 240 4.8% 95.9%;
--accent: 240 4.8% 95.9%;
--destructive: 0 84.2% 60.2%;
--border: 240 5.9% 90%;

/* Dark mode (`.dark`) */
--background: 240 10% 3.9%;
--foreground: 0 0% 98%;
--card: 240 10% 3.9%;
--primary: 0 0% 98%;
--secondary: 240 3.7% 15.9%;
...
```

### 4.3 Theme Toggle

- Store preference in `localStorage` (key: `mypcbuild-theme`)
- Apply `.dark` class on `<html>` element (Tailwind dark mode strategy: `class`)
- Respect `prefers-color-scheme` for first-time visitors
- Provide `ThemeToggle.vue` component using shadcn `Switch` or a `Button` with sun/moon icons

### 4.4 Typography & Spacing

- Use Tailwind's default scale
- Body font: `Inter` (loaded via Google Fonts or bundled)
- Monospace for specs/technical values: `JetBrains Mono` or system mono

---

## 5. Layout & Navigation

### 5.1 Shell Layout (`DefaultLayout.vue`)

```
┌─────────────────────────────────────────────────┐
│  AppHeader (breadcrumbs, theme toggle, search)  │
├──────────┬──────────────────────────────────────┤
│          │                                      │
│ Sidebar  │         <RouterView />               │
│ (nav)    │         (main content)               │
│          │                                      │
│          │                                      │
├──────────┴──────────────────────────────────────┤
│  (optional footer)                              │
└─────────────────────────────────────────────────┘
```

- **Sidebar** (`AppSidebar.vue`): Collapsible, icon + label navigation
  - Dashboard (home icon)
  - My Builds (cpu icon)
  - Product Catalog (package icon)
  - Separator
  - Settings (gear icon — future)
- **Header** (`AppHeader.vue`): Dynamic breadcrumbs, global search (command palette), theme toggle
- **Mobile**: Sidebar collapses into a `Sheet` (slide-in drawer) triggered by a hamburger button

### 5.2 Routes

```typescript
const routes = [
  { path: '/',                    name: 'dashboard',       component: DashboardView },
  { path: '/builds',             name: 'builds',          component: BuildsListView },
  { path: '/builds/:id',         name: 'build-detail',    component: BuildDetailView, props: true },
  { path: '/catalog',            name: 'catalog',         component: CatalogView },
  { path: '/catalog/new',        name: 'product-create',  component: ProductCreateView },
  { path: '/catalog/:id',        name: 'product-detail',  component: ProductDetailView, props: true },
  { path: '/:pathMatch(.*)*',    name: 'not-found',       component: NotFoundView },
]
```

---

## 6. Feature Modules

### 6.1 Dashboard (NEW)

A landing page providing a quick overview:

- **Recent builds** — Last 3-5 builds as cards with quick-access links
- **Build stats** — Total builds, total parts across all builds, total investment
- **Compatibility alerts** — Builds with unresolved errors/warnings
- **Quick actions** — "New Build", "Browse Catalog", "Create Product"

### 6.2 Build Management (REDESIGNED)

**Builds List (`/builds`)**
- Toggle between **grid** (cards) and **list** (table) views
- Sort by: name, date created, total cost, part count
- Filter by: compatibility status (all, valid, has errors, has warnings)
- "New Build" button opens `CreateBuildDialog`
- Each card shows: name, part count, total cost, compatibility badge, quick actions (edit, delete)

**Build Detail (`/builds/:id`)**

Redesigned as a **tabbed workspace**:

| Tab | Content |
|-----|---------|
| **Overview** | Build summary, total cost, compatibility status, part count by category |
| **Parts** | Interactive parts management (see Workflow §9.1) |
| **3D Preview** | Three.js viewer for spatial builds |
| **Compatibility** | Full compatibility report with issue cards |

The **Parts tab** is the primary workspace:
- Category-based slot layout (one row per category: CPU, Motherboard, GPU, RAM, Storage, PSU, Cooler, Case)
- Each slot shows the installed product or an "Add" button
- Inline compatibility indicators per part
- Click a slot to open the part picker (command palette style)

### 6.3 Product Catalog (REDESIGNED)

**Catalog Browser (`/catalog`)**
- **Sidebar filters** panel (collapsible on mobile):
  - Category checkboxes with counts
  - Price range slider
  - Manufacturer multi-select
  - Status filter (draft/published/all)
- **Main area**: Toggle between card grid and table view
- **Search**: Debounced text search (300ms)
- **Sorting**: Dropdown for name, price, category, date
- **Pagination**: Bottom pagination bar with page size selector

**Product Create (`/catalog/new`)**
- Step 1: Select category (card grid with icons)
- Step 2: Choose mode (manual vs AI-assisted), enter base fields
- Step 3: Category-specific form (typed, validated with Zod)
- Step 4: Review & create as draft
- Use `Tabs` or `Stepper` pattern (not a rigid wizard)
- AI mode: textarea prompt → generates fields → user reviews & edits

**Product Detail (`/catalog/:id`)**
- Read-only view with edit toggle
- Category-specific fields rendered dynamically
- Draft banner with "Publish" action
- 3D spatial preview for products with dimensions/slots/chambers
- Delete with confirmation dialog

### 6.4 Compatibility Engine Integration

- `CompatibilityBadge`: Inline colored dot (green/yellow/red) with tooltip
- `CompatibilityPanel`: Collapsible panel with categorized issues
- `IssueCard`: Individual issue with severity icon, message, affected parts
- Auto-validate on build load and after part add/remove
- Show compatibility inline during part selection (in the part picker)

### 6.5 3D Spatial Viewer

- Reuse Three.js integration from existing client
- `BuildViewer3D`: Shows all parts in their slots/chambers
- `ProductViewer3D`: Shows single product with slot definitions
- `ViewerControls`: Grid toggle, reset camera, zoom controls
- Themed: respect dark/light mode for background and grid colors

---

## 7. API Integration Layer

### 7.1 HTTP Client (`api/client.ts`)

```typescript
import axios from 'axios'

const serviceUrl = import.meta.env.VITE_API_URL
  || import.meta.env.services__apiservice__https__0
  || import.meta.env.services__apiservice__http__0
  || 'http://localhost:5000'

const apiClient = axios.create({
  baseURL: `${serviceUrl}/api`,
  headers: { 'Content-Type': 'application/json' },
  timeout: 15000,
})

// Response interceptor for error toasts
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    // Global error handling
    return Promise.reject(error)
  },
)

export default apiClient
```

### 7.2 Service Modules

Each module exports typed async functions:

**`api/builds.ts`**
- `getBuilds()` → `Build[]`
- `getBuild(id: string)` → `GetBuildResponse`
- `createBuild(name: string)` → `CreateBuildResponse`
- `updateBuild(id: string, name: string)` → `void`
- `addPart(buildId: string, productId: string, pricePaid: number)` → `void`
- `addPartToSlot(buildId: string, request: AddPartToSlotRequest)` → `void`
- `removePart(buildId: string, productId: string)` → `void`
- `getAvailableSlots(buildId: string)` → `AvailableSlot[]`
- `getBuildCompatibility(buildId: string)` → `CompatibilityResult`

**`api/catalog.ts`**
- `getProducts(params: ProductQueryParams)` → `GetProductsResponse`
- `getProduct(id: string)` → `ProductResponse`
- `createProduct(request: ProductRequest)` → `ProductResponse`
- `updateProduct(id: string, request: ProductRequest)` → `ProductResponse`
- `publishProduct(id: string)` → `ProductResponse`
- `getCategories()` → `Category[]`
- `getFieldDefinitions(category: string)` → `FieldDefinition[]`
- `searchProducts(query: string)` → `ProductSummary[]`
- `generateWithAi(prompt: string)` → `GeneratedProduct`

**`api/compatibility.ts`**
- `validateCompatibility(productIds: string[])` → `CompatibilityResult`

### 7.3 Types (`api/types.ts` + `types/*.ts`)

Reuse and refine the TypeScript interfaces from the existing client:
- Product union types (request & response per category)
- Value object interfaces (Frequency, Power, StorageCapacity, etc.)
- Spatial types (Vector3, Dimensions, Slot, Chamber)
- Build and compatibility types

---

## 8. State Management

### 8.1 Build Store (`stores/buildStore.ts`)

```typescript
interface BuildState {
  builds: Build[]
  currentBuild: GetBuildResponse | null
  compatibilityIssues: CompatibilityIssue[]
  isLoading: boolean
  error: string | null
}
```

**Getters:** `errors`, `warnings`, `isValid`, `totalCost`, `partsByCategory`

**Actions:** `loadBuilds`, `loadBuild`, `createBuild`, `updateBuild`, `addPart`, `addPartToSlot`, `removePart`, `validateBuild`, `deleteBuild`

### 8.2 Catalog Store (`stores/catalogStore.ts`)

```typescript
interface CatalogState {
  products: ProductSummary[]
  totalProducts: number
  selectedCategories: string[]
  searchQuery: string
  currentPage: number
  itemsPerPage: number
  sortBy: string
  sortDesc: boolean
  isLoading: boolean
  error: string | null
}
```

**Actions:** `loadProducts`, `setCategories`, `setSearch`, `setPage`, `setSort`

### 8.3 Theme Store (`stores/themeStore.ts`)

```typescript
interface ThemeState {
  mode: 'light' | 'dark' | 'system'
}
```

**Actions:** `setMode`, `initTheme`

Applies `.dark` class to `document.documentElement` based on preference or system setting.

---

## 9. Redesigned Workflows

### 9.1 Build Part Management (NEW: Category-Slot Layout)

**Current approach**: A flat parts list with a generic "Add Component" dialog that lists all products.

**New approach**: A **category-slot board** that visually represents what a PC build needs:

```
┌─────────────────────────────────────────────┐
│  Build: "Gaming Rig 2025"                   │
│  Total: $2,450  │  8/8 parts  │  ✅ Valid   │
├─────────────────────────────────────────────┤
│                                             │
│  🔲 CPU         │ AMD Ryzen 9 7950X  $549   │ ✏️ 🗑️
│  🔲 Motherboard │ ASUS ROG X670E     $399   │ ✏️ 🗑️
│  🔲 GPU         │ RTX 4080 Super     $999   │ ✏️ 🗑️
│  🔲 RAM         │ [+ Add RAM]               │
│  🔲 Storage     │ Samsung 990 Pro    $159   │ ✏️ 🗑️
│  🔲 PSU         │ Corsair RM850x     $139   │ ✏️ 🗑️
│  🔲 Cooler      │ [+ Add Cooler]            │
│  🔲 Case        │ Fractal North      $179   │ ✏️ 🗑️
│                                             │
│  ⚠️ 2 compatibility warnings                │
└─────────────────────────────────────────────┘
```

**Interaction flow:**
1. Click **"+ Add"** on an empty category slot
2. Opens a **command palette / dialog** filtered to that category
3. Shows products with key specs, price, and compatibility status (pre-validated against current build)
4. User selects a product → immediately added to the build
5. Compatibility re-validates automatically
6. Toast notification confirms the action
7. Remove part: click trash icon → confirmation dialog → remove → re-validate

### 9.2 Product Creation (NEW: Streamlined Flow)

**Current approach**: A 3-step wizard with modal navigation.

**New approach**: A **single-page form with progressive disclosure**:

1. **Category selection** — 8 cards in a grid, click to select
2. **Creation mode** — Toggle between "Manual" and "AI-Assisted" (inline radio group, not a separate step)
3. **Form area** — Dynamically renders:
   - Common fields (name, manufacturer, price) always visible at top
   - Category-specific fields below, validated with Zod schemas
   - AI mode: shows a prompt textarea and "Generate" button, then populates the form fields for review
4. **Actions** — "Save as Draft" (primary), "Save & Publish" (secondary)
5. **Feedback** — Inline validation errors, toast on success, redirect to product detail

### 9.3 Catalog Browsing (NEW: Faceted Search)

**Current approach**: Category dropdown + text search + data table.

**New approach**: **Faceted filtering with a split layout**:

- Left sidebar: category checkboxes, price range, manufacturer filter, status filter
- Main area: card grid (default) or table view (toggle)
- Top bar: search input + sort dropdown + view mode toggle
- URL-synced filters (query params) for shareable links
- Skeleton loading states during fetch

### 9.4 Dashboard (NEW)

A new entry point replacing the direct builds list:

- **Build cards** — Last 3 recent builds with quick stats
- **Alerts section** — Builds with compatibility issues (click to navigate)
- **Quick actions** — Prominent buttons for "New Build", "Browse Catalog"
- **Stats** — Total builds, total investment, most-used category

---

## 10. Component Inventory

### 10.1 shadcn-vue Components to Install

```bash
npx shadcn-vue@latest add button card dialog dropdown-menu input label \
  select table tabs badge alert sheet separator skeleton tooltip toast \
  command popover progress switch checkbox form breadcrumb collapsible \
  scroll-area avatar sonner
```

### 10.2 Custom App Components

| Component | Location | Purpose |
|-----------|----------|---------|
| `AppSidebar` | `components/layout/` | Navigation sidebar with collapsible sections |
| `AppHeader` | `components/layout/` | Breadcrumbs, search trigger, theme toggle |
| `ThemeToggle` | `components/layout/` | Dark/light/system mode switch |
| `MobileNav` | `components/layout/` | Mobile sheet-based navigation |
| `BuildCard` | `components/builds/` | Build summary in grid view |
| `BuildPartsList` | `components/builds/` | Category-slot board for parts |
| `BuildSummaryBar` | `components/builds/` | Cost, power, compatibility stats |
| `BuildCategorySlots` | `components/builds/` | Per-category slot with add/remove |
| `CreateBuildDialog` | `components/builds/` | New build name dialog |
| `ProductCard` | `components/catalog/` | Product in grid view |
| `ProductFilters` | `components/catalog/` | Sidebar faceted filters |
| `ProductTable` | `components/catalog/` | Product table view |
| `CategoryIcon` | `components/catalog/` | Icon per product category |
| `ProductFormShell` | `components/products/` | Common form wrapper + validation |
| `CpuForm` | `components/products/` | CPU-specific fields |
| `GpuForm` | `components/products/` | GPU-specific fields |
| `MotherboardForm` | `components/products/` | Motherboard-specific fields |
| `RamForm` | `components/products/` | RAM-specific fields |
| `StorageForm` | `components/products/` | Storage-specific fields |
| `PsuForm` | `components/products/` | PSU-specific fields |
| `CoolerForm` | `components/products/` | Cooler-specific fields |
| `PcCaseForm` | `components/products/` | PC Case-specific fields |
| `AiGenerateForm` | `components/products/` | AI prompt and generation |
| `CompatibilityBadge` | `components/compatibility/` | Status dot with tooltip |
| `CompatibilityPanel` | `components/compatibility/` | Issue list panel |
| `IssueCard` | `components/compatibility/` | Single issue display |
| `BuildViewer3D` | `components/spatial/` | 3D build viewer |
| `ProductViewer3D` | `components/spatial/` | 3D product viewer |
| `ViewerControls` | `components/spatial/` | 3D camera controls |
| `EmptyState` | `components/shared/` | No-data placeholder |
| `LoadingState` | `components/shared/` | Loading skeletons |
| `ErrorState` | `components/shared/` | Error with retry |
| `ConfirmDialog` | `components/shared/` | Destructive action confirmation |
| `PageHeader` | `components/shared/` | Title + actions bar |
| `PriceDisplay` | `components/shared/` | Formatted currency |
| `StatusBadge` | `components/shared/` | Draft/Published badge |

---

## 11. Implementation Phases

### Phase 1: Project Scaffolding & Design System

**Tasks:**
- [ ] Scaffold Vite + Vue 3 + TypeScript project in `apps/shadcn-vue-client/`
- [ ] Install and configure Tailwind CSS 4
- [ ] Install and configure shadcn-vue (New York style)
- [ ] Set up CSS variables for light/dark themes in `globals.css`
- [ ] Install required shadcn-vue components (button, card, dialog, input, etc.)
- [ ] Create `ThemeToggle.vue` with localStorage persistence
- [ ] Configure Vite proxy for API (`/api` → `http://localhost:5000`)
- [ ] Set up ESLint + TypeScript strict mode
- [ ] Add Inter font (via Google Fonts or local)
- [ ] Create `.env.development` and `.env.production`

**Deliverable:** Empty app with theme toggle, Tailwind working, all shadcn components available.

### Phase 2: Layout & Navigation Shell

**Tasks:**
- [ ] Create `DefaultLayout.vue` with sidebar + header + content area
- [ ] Build `AppSidebar.vue` with navigation links and collapse functionality
- [ ] Build `AppHeader.vue` with breadcrumbs and theme toggle
- [ ] Build `MobileNav.vue` with sheet-based navigation
- [ ] Configure Vue Router with all routes
- [ ] Create `NotFoundView.vue` (404 page)
- [ ] Add page transition animations

**Deliverable:** Navigable app shell with responsive sidebar, breadcrumbs, and all routes rendering placeholder views.

### Phase 3: API Layer & State Management

**Tasks:**
- [ ] Create Axios client with Aspire service discovery support
- [ ] Implement `api/builds.ts` with all build endpoints
- [ ] Implement `api/catalog.ts` with all catalog endpoints
- [ ] Implement `api/compatibility.ts` with validation endpoints
- [ ] Define all TypeScript types in `types/` (build, product, spatial)
- [ ] Create `buildStore.ts` with full CRUD and validation actions
- [ ] Create `catalogStore.ts` with search, filter, pagination
- [ ] Create `themeStore.ts` with mode persistence
- [ ] Add error handling and loading state patterns

**Deliverable:** Fully typed API layer and Pinia stores, testable against the live backend.

### Phase 4: Shared Components

**Tasks:**
- [ ] Build `EmptyState.vue`, `LoadingState.vue`, `ErrorState.vue`
- [ ] Build `PageHeader.vue` with title and action slots
- [ ] Build `ConfirmDialog.vue` using shadcn `AlertDialog`
- [ ] Build `PriceDisplay.vue` (formatted currency)
- [ ] Build `StatusBadge.vue` (Draft/Published)
- [ ] Build `CategoryIcon.vue` (Lucide icon per category)
- [ ] Build `CompatibilityBadge.vue` (colored dot + tooltip)

**Deliverable:** All reusable shared components ready for use in feature views.

### Phase 5: Dashboard View

**Tasks:**
- [ ] Build `DashboardView.vue` as the home page
- [ ] Recent builds section with `BuildCard` components
- [ ] Quick actions section (New Build, Browse Catalog)
- [ ] Compatibility alerts section (builds with issues)
- [ ] Summary stats (total builds, total cost, etc.)
- [ ] Loading and empty states

**Deliverable:** Functional dashboard showing real data from the API.

### Phase 6: Build Management

**Tasks:**
- [ ] Build `BuildsListView.vue` with grid/list toggle
- [ ] Build `BuildCard.vue` with summary info and actions
- [ ] Build `CreateBuildDialog.vue` with name input
- [ ] Build `BuildDetailView.vue` with tabbed layout (Overview, Parts, 3D, Compatibility)
- [ ] Build `BuildCategorySlots.vue` — the category-slot board
- [ ] Build `BuildPartsList.vue` — parts table with actions
- [ ] Build `BuildSummaryBar.vue` — cost/power/compatibility stats
- [ ] Implement part picker (command palette filtered by category)
- [ ] Implement part removal with confirmation dialog
- [ ] Build `CompatibilityPanel.vue` and `IssueCard.vue`
- [ ] Wire up build store actions and API calls
- [ ] Handle slot-based part placement for spatial products

**Deliverable:** Complete build management workflow with create, edit, add/remove parts, compatibility display.

### Phase 7: Product Catalog

**Tasks:**
- [ ] Build `CatalogView.vue` with split layout (filters + grid/table)
- [ ] Build `ProductFilters.vue` with category, price, status filters
- [ ] Build `ProductCard.vue` for grid view
- [ ] Build `ProductTable.vue` for table view
- [ ] Implement debounced search
- [ ] Implement pagination with page size selector
- [ ] Implement sorting (name, price, category, date)
- [ ] URL-sync filters with query params
- [ ] Build `ProductDetailView.vue` with read/edit mode toggle
- [ ] Build `ProductCreateView.vue` with category → form flow
- [ ] Build all 8 category-specific forms with Zod validation
- [ ] Build `AiGenerateForm.vue` for AI-assisted creation
- [ ] Build `ProductFormShell.vue` as common wrapper
- [ ] Implement draft/publish workflow
- [ ] Implement product delete with confirmation

**Deliverable:** Complete catalog browsing, product creation (manual + AI), editing, and publishing.

### Phase 8: 3D Visualization

**Tasks:**
- [ ] Port `use3DViewer.ts` composable from existing client
- [ ] Build `BuildViewer3D.vue` for build spatial view
- [ ] Build `ProductViewer3D.vue` for product spatial view
- [ ] Build `ViewerControls.vue` (grid, reset, zoom)
- [ ] Theme-aware 3D backgrounds (light/dark)
- [ ] Collision detection highlighting

**Deliverable:** 3D visualization integrated into build detail and product detail views.

### Phase 9: Polish & Production Readiness

**Tasks:**
- [ ] Add toast notifications for all user actions (success, error)
- [ ] Add page transition animations
- [ ] Responsive testing (mobile, tablet, desktop)
- [ ] Keyboard navigation and accessibility audit
- [ ] Error boundary for uncaught errors
- [ ] Create `Dockerfile` and `nginx.conf` for production
- [ ] Update root `README.md` to document the new client
- [ ] Performance optimization (lazy-loaded routes, image optimization)

**Deliverable:** Production-ready frontend with Docker deployment support.

---

## 12. Development & Build Configuration

### 12.1 Vite Configuration (`vite.config.ts`)

```typescript
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { resolve } from 'path'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [vue(), tailwindcss()],
  resolve: {
    alias: {
      '@': resolve(__dirname, 'src'),
    },
  },
  server: {
    port: Number(process.env.PORT) || 5174,  // Different port from existing client
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
      },
    },
  },
})
```

### 12.2 TypeScript Configuration

- `strict: true`
- Path alias: `@/*` → `src/*`
- Target: ESNext
- Module: ESNext with bundler resolution

### 12.3 Scripts (`package.json`)

```json
{
  "scripts": {
    "dev": "vite",
    "build": "vue-tsc -b && vite build",
    "preview": "vite preview",
    "lint": "eslint . --ext .vue,.ts,.tsx",
    "type-check": "vue-tsc --noEmit"
  }
}
```

### 12.4 Docker Production Build

Multi-stage Dockerfile:
1. **Build stage**: `node:22-alpine`, install deps, run `npm run build`
2. **Serve stage**: `nginx:alpine`, copy dist + nginx.conf, expose port 80

### 12.5 Aspire Integration

Register the new client in `MyPcBuild.AppHost/Program.cs` alongside the existing client, with a different port and resource name.

---

## Notes for the Implementing Agent

1. **Do not copy-paste from the existing client.** Use it as a reference for API contracts and domain types, but build all components fresh with shadcn-vue patterns.
2. **Use `<script setup lang="ts">`** for all Vue components.
3. **Use shadcn-vue components** as the base — avoid custom CSS for things shadcn already provides.
4. **Validate forms with Zod** schemas and VeeValidate integration.
5. **Use Lucide icons** consistently (not mixing icon libraries).
6. **Test against the running API** — ensure the Aspire backend is available during development.
7. **Follow the phase order** — each phase builds on the previous one. Phases 1-4 are foundational and must be completed before feature phases 5-8.
8. **Dark mode must work out of the box** — test both themes during development.
9. **Use semantic HTML** and ARIA attributes for accessibility.
10. **Keep components small** — a component should do one thing. Extract sub-components when complexity grows.
