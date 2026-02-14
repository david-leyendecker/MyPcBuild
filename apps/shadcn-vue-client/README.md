# MyPcBuild - shadcn-vue Client

Modern Vue 3 web client for the MyPcBuild application, built with TypeScript, shadcn-vue components, and Three.js for 3D visualization.

## Features

- 🎨 Modern UI with shadcn-vue components and Tailwind CSS
- 🎯 Type-safe development with TypeScript
- 🔄 State management with Pinia
- 🛣️ Client-side routing with Vue Router
- 📝 Form validation with vee-validate and Zod
- 🎭 3D visualization with Three.js
- 📱 Responsive design
- ♿ Accessibility focused

## Prerequisites

- Node.js 22 or higher
- npm 10 or higher

## Development Setup

1. **Install dependencies**
   ```bash
   npm install
   ```

2. **Configure environment**
   Create a `.env.local` file in the project root:
   ```env
   VITE_API_BASE_URL=http://localhost:5000
   ```

3. **Start development server**
   ```bash
   npm run dev
   ```
   The application will be available at `http://localhost:5173`

## Build Commands

### Development
```bash
npm run dev          # Start dev server
npm run build        # Build for production
npm run preview      # Preview production build
```

### Type Checking
```bash
npm run type-check   # Run TypeScript type checker
```

## Docker Commands

### Build Docker Image
```bash
docker build -t mypcbuild-shadcn-client .
```

### Run Docker Container
```bash
docker run -p 8080:80 mypcbuild-shadcn-client
```

The application will be available at `http://localhost:8080`

### Docker Compose (with API)
```bash
docker-compose up -d
```

## Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `VITE_API_BASE_URL` | Base URL for the API service | `http://localhost:5000` |

## Project Structure

```
src/
├── api/              # API client and type definitions
├── components/       # Vue components
│   ├── builds/       # Build-related components
│   ├── catalog/      # Catalog components
│   ├── compatibility/# Compatibility check components
│   ├── layout/       # Layout components
│   ├── products/     # Product components
│   ├── shared/       # Shared/common components
│   ├── spatial/      # 3D visualization components
│   └── ui/           # shadcn-vue UI components
├── composables/      # Vue composables
├── router/           # Vue Router configuration
├── stores/           # Pinia stores
├── types/            # TypeScript type definitions
├── utils/            # Utility functions
└── views/            # Page views/routes
```

## Key Technologies

- **Vue 3** - Progressive JavaScript framework
- **TypeScript** - Type-safe JavaScript
- **Vite** - Next generation frontend tooling
- **shadcn-vue** - Re-usable components built with Radix Vue and Tailwind CSS
- **Tailwind CSS** - Utility-first CSS framework
- **Three.js** - 3D graphics library
- **Pinia** - State management
- **Vue Router** - Official router for Vue.js
- **vee-validate** - Form validation
- **Zod** - Schema validation
- **Axios** - HTTP client

## Development Guidelines

- Use TypeScript for all new files
- Follow Vue 3 Composition API patterns
- Use explicit types instead of `var`
- Implement proper error handling
- Add ARIA labels for accessibility
- Write self-documenting code

## Production Deployment

The application is containerized and can be deployed using Docker. It uses a multi-stage build:

1. **Build stage**: Installs dependencies and builds the application
2. **Production stage**: Serves the static files with nginx

The nginx configuration includes:
- SPA routing support (redirect all routes to index.html)
- API proxy to `/api` endpoint
- Gzip compression for static assets

## License

Copyright © MyPcBuild Team
