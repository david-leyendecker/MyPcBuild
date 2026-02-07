# REST API Design - HATEOAS Pattern & REPR Convention

## Overview
The API follows REpresentational State Transfer (REST) principles with HATEOAS (Hypermedia as the Engine of Application State) for discoverability.

All endpoints follow the **REPR (Request-Endpoint-Response)** pattern where each endpoint is defined in its own file containing all related models (request, handler, response, and HATEOAS link generation).

## Key Principles

### 1. **Resource-Oriented**
Every endpoint represents a resource or collection of resources:
- `/api/catalog/products` - Product collection
- `/api/builds/{id}` - Single build resource
- `/api/compatibility/validate` - Compatibility validation resource

### 2. **REPR Pattern (One File Per Endpoint)**
Each endpoint has its own dedicated file containing:
- **Request model** - Input DTOs/records
- **Endpoint handler** - Static class with the `Map*Endpoint` extension method
- **Response model** - Output DTOs/records with HATEOAS links

Example file structure:
```
Features/
├── Builds/
│   ├── BuildEndpoints.cs          # Routing orchestrator
│   ├── GetBuilds.cs               # GET /api/builds
│   ├── GetBuild.cs                # GET /api/builds/{id}
│   ├── CreateBuild.cs             # POST /api/builds
│   ├── AddPartToBuild.cs          # POST /api/builds/{id}/parts
│   ├── AddPartToSlot.cs           # POST /api/builds/{id}/parts/slot
│   ├── RemovePartFromBuild.cs     # DELETE /api/builds/{id}/parts/{productId}
│   ├── GetAvailableSlots.cs       # GET /api/builds/{id}/slots
│   └── BuildDtos.cs               # Shared spatial DTOs
├── Catalog/
│   ├── CatalogEndpoints.cs        # Routing orchestrator
│   ├── GetProducts.cs             # GET /api/catalog/products
│   ├── GetProductById.cs          # GET /api/catalog/products/{id}
│   ├── GetCategories.cs           # GET /api/catalog/categories
│   ├── SearchProducts.cs          # GET /api/catalog/search
│   ├── CreateProduct.cs           # POST /api/catalog/products
│   ├── UpdateProduct.cs           # PUT /api/catalog/products/{id}
│   ├── PublishProduct.cs          # POST /api/catalog/products/{id}/publish
│   ├── GenerateProductWithAi.cs   # POST /api/catalog/products/generate-with-ai
│   └── GetFieldDefinitions.cs     # GET /api/catalog/field-definitions/{category}
├── Compatibility/
│   ├── CompatibilityEndpoints.cs  # Routing orchestrator
│   ├── ValidateCompatibility.cs   # POST /api/compatibility/validate
│   └── GetBuildCompatibility.cs   # GET /api/builds/{id}/compatibility
└── Spatial/
    ├── SpatialEndpoints.cs        # Routing orchestrator
    ├── ValidatePartInstallation.cs # POST /api/builds/{id}/parts/validate
    └── ValidateBuildSpatial.cs    # POST /api/builds/{id}/validate
```

### 3. **HATEOAS Links**
Every response includes hypermedia links to related resources using the `HateoasLink` record:

```json
{
  "id": "abc-123",
  "name": "My Gaming PC",
  "links": [
    {
      "href": "https://api.example.com/api/builds/abc-123",
      "rel": "self",
      "method": "GET"
    },
    {
      "href": "https://api.example.com/api/builds/abc-123/parts",
      "rel": "add-part",
      "method": "POST"
    },
    {
      "href": "https://api.example.com/api/builds/abc-123/compatibility",
      "rel": "validate",
      "method": "GET"
    }
  ]
}
```

### 4. **Link Relations (rel)**

| Relation | Description |
|----------|-------------|
| `self` | Link to current resource |
| `build` | Link to parent build |
| `catalog` | Link to product catalog |
| `product` | Link to product details |
| `category` | Link to product category |
| `products` | Link to products in category |
| `add-part` | Action to add part to build |
| `add-part-to-slot` | Action to add part to a specific slot |
| `remove` | Action to remove resource |
| `validate` | Action to validate compatibility |
| `validate-build` | Action to validate build spatial configuration |
| `validate-compatibility` | Action to validate compatibility |
| `prev` | Previous page |
| `next` | Next page |
| `all-products` | Link to all products |
| `categories` | Link to category list |
| `create-build` | Action to create a new build |
| `create-product` | Action to create a new product |
| `update` | Action to update resource |
| `publish` | Action to publish a draft product |
| `available-slots` | Link to available slots in a build |
| `field-definitions` | Link to field definitions for a category |

## Response DTOs

### Compatibility Validation Response

```json
{
  "isCompatible": false,
  "hasErrors": true,
  "hasWarnings": true,
  "issues": [
    {
      "message": "CPU socket AM5 is incompatible with motherboard socket LGA1700",
      "severity": "Error",
      "category": "CPU/Motherboard",
      "recommendation": "Choose a motherboard with matching CPU socket or select a different CPU"
    }
  ],
  "componentSummary": {
    "totalComponents": 5,
    "componentsByCategory": {
      "CPU": 1,
      "Motherboard": 1,
      "GPU": 1,
      "RAM": 2
    },
    "hasCpu": true,
    "hasMotherboard": true,
    "hasGpu": true,
    "hasRam": true,
    "hasCase": false,
    "hasPsu": false,
    "hasCooler": false,
    "hasStorage": false
  },
  "links": [
    {
      "href": "https://api.example.com/api/catalog/products",
      "rel": "catalog",
      "method": "GET"
    }
  ]
}
```

### Product Catalog Response

```json
{
  "totalCount": 24,
  "totalPages": 2,
  "currentPage": 1,
  "pageSize": 20,
  "products": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "name": "AMD Ryzen 9 7950X",
      "category": "CPU",
      "categoryId": 0,
      "price": 549.99,
      "manufacturer": "AMD",
      "specifications": {
        "Socket": "AM5",
        "Cores": 16,
        "Threads": 32
      },
      "links": [
        {
          "href": "https://api.example.com/api/catalog/products/550e8400-e29b-41d4-a716-446655440000",
          "rel": "self",
          "method": "GET"
        },
        {
          "href": "https://api.example.com/api/catalog/products?category=CPU",
          "rel": "category",
          "method": "GET"
        }
      ]
    }
  ],
  "filters": {
    "category": "CPU",
    "searchTerm": "ryzen"
  },
  "links": [
    {
      "href": "https://api.example.com/api/catalog/products?page=1&pageSize=20",
      "rel": "self",
      "method": "GET"
    },
    {
      "href": "https://api.example.com/api/catalog/products?page=2&pageSize=20",
      "rel": "next",
      "method": "GET"
    }
  ]
}
```

### Category List Response

```json
{
  "categories": [
    {
      "id": 0,
      "name": "CPU",
      "displayName": "Processors",
      "productCount": 4,
      "links": [
        {
          "href": "https://api.example.com/api/catalog/products?category=CPU",
          "rel": "products",
          "method": "GET"
        }
      ]
    }
  ],
  "links": [
    {
      "href": "https://api.example.com/api/catalog/categories",
      "rel": "self",
      "method": "GET"
    },
    {
      "href": "https://api.example.com/api/catalog/products",
      "rel": "all-products",
      "method": "GET"
    }
  ]
}
```

### Build Response

```json
{
  "id": "build-guid",
  "name": "My Gaming PC",
  "userId": "user-guid",
  "parts": [
    {
      "productId": "product-guid",
      "productName": "AMD Ryzen 9 7950X",
      "category": "CPU",
      "pricePaid": 549.99,
      "manufacturer": "AMD",
      "links": [
        {
          "href": "https://api.example.com/api/catalog/products/product-guid",
          "rel": "product",
          "method": "GET"
        },
        {
          "href": "https://api.example.com/api/builds/build-guid/parts/product-guid",
          "rel": "remove",
          "method": "DELETE"
        }
      ]
    }
  ],
  "totalPrice": 1549.99,
  "version": 3,
  "compatibilityStatus": {
    "isCompatible": true,
    "errorCount": 0,
    "warningCount": 1
  },
  "links": [
    {
      "href": "https://api.example.com/api/builds/build-guid",
      "rel": "self",
      "method": "GET"
    },
    {
      "href": "https://api.example.com/api/builds/build-guid/parts",
      "rel": "add-part",
      "method": "POST"
    },
    {
      "href": "https://api.example.com/api/builds/build-guid/compatibility",
      "rel": "validate",
      "method": "GET"
    },
    {
      "href": "https://api.example.com/api/catalog/products",
      "rel": "catalog",
      "method": "GET"
    }
  ]
}
```

### Build Created Response

```json
{
  "buildId": "new-build-guid",
  "name": "My New PC",
  "userId": "user-guid",
  "createdAt": "2026-01-03T14:23:00Z",
  "links": [
    {
      "href": "https://api.example.com/api/builds/new-build-guid",
      "rel": "self",
      "method": "GET"
    },
    {
      "href": "https://api.example.com/api/builds/new-build-guid/parts",
      "rel": "add-part",
      "method": "POST"
    },
    {
      "href": "https://api.example.com/api/builds/new-build-guid/compatibility",
      "rel": "validate",
      "method": "GET"
    }
  ]
}
```

## Benefits of HATEOAS

### 1. **Client Discoverability**
Clients don't need to hardcode URLs:
```javascript
// Instead of:
const url = `/api/builds/${buildId}/compatibility`;

// Client can discover:
const validateLink = response.links.find(l => l.rel === 'validate');
fetch(validateLink.href);
```

### 2. **API Evolution**
URLs can change without breaking clients:
- Old: `/api/builds/{id}/parts`
- New: `/api/v2/builds/{id}/components`
- Client follows `add-part` link regardless

### 3. **Self-Documenting**
API responses indicate available actions:
- Product has `category` link → Can browse similar products
- Build has `validate` link → Can check compatibility
- Missing `remove` link → No permission to delete

### 4. **Workflow Guidance**
Links guide user through process:
1. Create build → Response includes `add-part` link
2. Add parts → Each part has `remove` link
3. View build → Includes `validate` link
4. Validate → Response includes `catalog` link to fix issues

## Implementation Details

### HateoasLink Record
All HATEOAS links use the shared `HateoasLink` record from `Infrastructure/HateoasLink.cs`:
```csharp
public record HateoasLink(string Href, string Rel, string Method);
```

### REPR Pattern Example
Each endpoint file follows this structure:
```csharp
// CreateBuild.cs - Single file for the POST /api/builds endpoint
public static class CreateBuild
{
    public static IEndpointRouteBuilder MapCreateBuildEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/builds", async (
            CreateBuildRequest request,
            IDocumentSession session,
            IHttpContextAccessor httpContextAccessor) =>
        {
            // ... handler logic ...

            string baseUrl = GetBaseUrl(httpContextAccessor);
            CreateBuildResponse response = new(buildId, request.Name, request.UserId,
            [
                new HateoasLink($"{baseUrl}/api/builds/{buildId}", "self", "GET"),
                new HateoasLink($"{baseUrl}/api/builds/{buildId}/parts", "add-part", "POST"),
                new HateoasLink($"{baseUrl}/api/builds/{buildId}/compatibility", "validate", "GET")
            ]);
            return Results.Created($"/api/builds/{buildId}", response);
        })
        .WithName("CreateBuild")
        .WithTags("Builds");

        return app;
    }
}

// Request model - in the same file
public record CreateBuildRequest(string Name, Guid UserId);

// Response model - in the same file, includes HATEOAS links
public record CreateBuildResponse(Guid Id, string Name, Guid UserId, List<HateoasLink> Links);
```

### URL Generation
Uses `IHttpContextAccessor` to generate absolute URLs:
```csharp
private static string GetBaseUrl(IHttpContextAccessor httpContextAccessor)
{
    HttpRequest request = httpContextAccessor.HttpContext!.Request;
    return $"{request.Scheme}://{request.Host}";
}
```

### Conditional Links
Links appear based on state:
- Build with compatibility link only if build has parts
- Pagination links (prev/next) only when applicable
- Remove link only if user has permission

## Versioning Strategy

URLs remain stable, DTO structure can evolve:
```json
{
  "id": "...",
  "name": "...",
  "_links": { ... },    // Can move to top level
  "links": [ ... ],     // Or keep as array
  "_embedded": { ... }  // Can add embedded resources
}
```

## Content Negotiation

Support multiple formats:
- `Accept: application/json` - Default JSON
- `Accept: application/hal+json` - HAL format
- `Accept: application/vnd.api+json` - JSON:API format

## Error Responses

Errors also follow HATEOAS:
```json
{
  "status": 404,
  "title": "Build Not Found",
  "detail": "Build with ID abc-123 does not exist",
  "links": [
    {
      "href": "https://api.example.com/api/builds",
      "rel": "create-build",
      "method": "POST"
    },
    {
      "href": "https://api.example.com/api/catalog/products",
      "rel": "catalog",
      "method": "GET"
    }
  ]
}
```
