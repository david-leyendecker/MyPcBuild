dotnet run
# MyPCBuild

A modern web application for planning and building custom PC configurations with real-time compatibility validation.

## Tech Stack

### Frontend
- **Framework**: Vue.js 3 with TypeScript
- **UI Library**: PrimeVue 4
- **Styling**: PrimeFlex (utility-first CSS)
- **State Management**: Pinia
- **Build Tool**: Vite
- **Routing**: Vue Router

### Backend
- **Framework**: ASP.NET Core 10 (Minimal APIs)
- **Event Sourcing**: Marten
- **Database**: PostgreSQL
- **Orchestration**: .NET Aspire

## Features

✅ **Build Management**
- Create and manage multiple PC builds
- Add/remove components
- Real-time cost calculation
- Build history with event sourcing

✅ **Product Catalog**
- Search products by name or category
- Filter by component type (CPU, GPU, RAM, etc.)
- View detailed specifications
- Add products to builds

✅ **Compatibility Engine**
- Real-time validation of component compatibility
- Error/Warning severity levels
- Socket, chipset, and form factor validation
- Power supply and clearance checks

✅ **Modern UI**
- Dark theme with glassmorphic design
- Fully responsive (mobile-first)
- PrimeFlex utility classes for rapid development
- Smooth animations and transitions

## Quick Start

### Prerequisites
- .NET 10 SDK
- Node.js 18+
- Docker (for PostgreSQL container)

### Run with Aspire (recommended)

```bash
cd MyPcBuild.AppHost
dotnet run
```

This will start PostgreSQL in Docker, the .NET API on http://localhost:5000, the Vite dev server on http://localhost:5173, and the Aspire dashboard.

Access:
- Vue.js frontend: http://localhost:5173
- API: http://localhost:5000
- Aspire Dashboard: http://localhost:15000 (default)
- pgAdmin: http://localhost:5050

### Run manually (development)

1) Start PostgreSQL

```bash
docker run -d \
	--name pcbuild-postgres \
	-e POSTGRES_PASSWORD=postgres \
	-e POSTGRES_DB=mypcbuild \
	-p 5432:5432 \
	postgres:17
```

2) Start the API

```bash
cd MyPcBuild.ApiService
dotnet run
```

The API will be available at http://localhost:5000

3) Start the Vue.js client

```bash
cd MyPcBuild.Client
npm install
npm run dev
```

The client will be available at http://localhost:5173

## Project Structure

```
MyPcBuild/
├── MyPcBuild.AppHost/          # .NET Aspire orchestration
├── MyPcBuild.ApiService/       # ASP.NET Core API
│   ├── Domain/                 # Event sourcing models & events
│   ├── Features/               # Feature-based organization
│   │   ├── Builds/             # Build management endpoints
│   │   ├── Catalog/            # Product catalog endpoints
│   │   └── Compatibility/      # Validation engine
│   └── Infrastructure/         # Data seeding & utilities
├── MyPcBuild.Client/           # Vue.js frontend
│   ├── src/
│   │   ├── api/                # API client services
│   │   ├── components/         # Reusable Vue components
│   │   ├── stores/             # Pinia state management
│   │   ├── views/              # Page components
│   │   └── router/             # Vue Router configuration
│   ├── Dockerfile              # Production container
│   └── nginx.conf              # Nginx configuration
├── MyPcBuild.ServiceDefaults/  # Shared Aspire configuration
└── MyPcBuild.Tests/            # Unit tests
```

Removed projects: MyPcBuild.Web (Blazor) was replaced with the Vue.js client.

## Architecture

### Event Sourcing
The application uses Marten for event sourcing, storing all build changes as immutable events:
- `BuildCreated`
- `PartAdded`
- `PartRemoved`
- `BuildRenamed`

Events are projected into the current `Build` state for querying.

### API Design
RESTful API with feature-based organization:
- `GET /api/builds` - List all builds
- `POST /api/builds` - Create new build
- `GET /api/builds/{id}` - Get build details
- `POST /api/builds/{id}/parts` - Add component
- `DELETE /api/builds/{id}/parts/{productId}` - Remove component
- `GET /api/catalog/search` - Search products
- `GET /api/compatibility/validate/{buildId}` - Validate compatibility

### Frontend Architecture
- Pinia stores for builds and catalog
- Axios-based API clients
- PrimeVue component library
- PrimeFlex utility-first styling

## Configuration

### API
- CORS enabled for http://localhost:5173
- Event sourcing with Marten and PostgreSQL connection from Aspire
- Health checks at `/health`
- OpenAPI/Swagger in development mode

### Frontend
- Vite dev server with HMR
- `/api` proxy configured in vite.config.ts
- PrimeVue dark theme and PrimeFlex utilities
- Pinia for state management and Vue Router for navigation

## Development Workflow

1. Edit Vue components in `MyPcBuild.Client/src`; Vite HMR reloads automatically.
2. Edit API code in `MyPcBuild.ApiService`; use `dotnet watch run` for hot reload.
3. View logs and metrics in the Aspire dashboard.
4. Run tests with `dotnet test`.

## Troubleshooting

### API cannot connect to database
- Ensure the PostgreSQL container is running.
- Check the connection string in appsettings.json.
- Verify Aspire provides the correct connection string.

### CORS errors in browser
- Confirm the API is running on http://localhost:5000.
- Check CORS configuration in Program.cs.
- Ensure the frontend origin matches the CORS policy.

### Vue.js build fails
- Run `npm install` in MyPcBuild.Client.
- Delete node_modules and package-lock.json, then reinstall.
- Check Node.js version (18+ required).

### Aspire dashboard not accessible
- Default port is 15000; confirm the URL in terminal output.
- Ensure the .NET Aspire workload is installed: `dotnet workload install aspire`.

## Production Deployment

Build and run the frontend container:

```bash
cd MyPcBuild.Client
docker build -t mypcbuild-client .
docker run -p 80:80 mypcbuild-client
```

The multi-stage Dockerfile builds with Node.js, serves assets via Nginx (with gzip and caching), and proxies API requests.

## Documentation

- [MyPcBuild.Client/README.md](MyPcBuild.Client/README.md) - Frontend documentation
- [MyPcBuild.Client/REFACTORING.md](MyPcBuild.Client/REFACTORING.md) - PrimeFlex refactoring notes
- [CATALOG-API.md](CATALOG-API.md) - Catalog API documentation
- [COMPATIBILITY-ENGINE.md](COMPATIBILITY-ENGINE.md) - Compatibility validation logic
- [REST-API-DESIGN.md](REST-API-DESIGN.md) - API design principles
- [software-design-document.md](software-design-document.md) - Overall system design

## Contributing

When adding new features:

### Backend
1. Create a feature folder in `MyPcBuild.ApiService/Features/`.
2. Define events in `Domain/Events/`.
3. Create endpoint classes with `MapXXXEndpoint` methods.
4. Register endpoints in `Program.cs`.

### Frontend
1. Create API client methods in `src/api/`.
2. Add Pinia store actions in `src/stores/`.
3. Create components in `src/components/` or views in `src/views/`.
4. Use PrimeFlex utilities instead of custom CSS.
5. Follow TypeScript strict mode.

## Next Steps

- Implement remaining API endpoints.
- Add authentication and authorization.
- Configure production builds via Aspire.
- Set up CI/CD pipelines.
- Add integration tests.

## License

[Add your license here]

## Authors

[Add contributors here]
