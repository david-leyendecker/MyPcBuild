---
name: test-structure
description: "Enforce test project structure and generate test files. Use when: writing tests, adding test files, deciding where to place a test, choosing between unit, integration and system test, structuring test namespaces, testing endpoints, testing services, testing models, creating AppHostFixture-based tests, organizing MyPcBuild.Tests project."
argument-hint: "Describe what you want to test (class name, endpoint, or workflow)"
---

# Test Structure

Enforces the three-tier split — **Unit / Integration / System** — and keeps test files
mirroring the source code layout.

## Decision: Which Tier?

| Scenario | Tier |
|---|---|
| Exactly one class, no infrastructure needed | **Unit** |
| Model / domain logic in isolation | **Unit** |
| Multiple classes exercised together in a workflow | **Integration** |
| Service wiring, pipeline logic needing some mocks | **Integration** |
| HTTP endpoint / resource backed by real infrastructure | **System** |
| Requires database, message queue, or AppHost | **System** |

### Mocking Rules

| Tier | Mocking |
|---|---|
| Unit | **Not allowed** — real objects only |
| Integration | **Allowed** for out-of-scope external dependencies only |
| System | **Not allowed** — all dependencies must be real |

## Folder Layout

```
MyPcBuild.Tests/
├── Infrastructure/        # Shared: AppHostFixture, AppHostCollection
├── Unit/                  # Single-class, no mocking
│   ├── Catalog/
│   ├── Builds/
│   ├── Compatibility/
│   └── Spatial/
├── Integration/           # Multi-class workflow, mocking allowed, no AppHost
│   ├── Catalog/
│   ├── Builds/
│   ├── Compatibility/
│   └── Spatial/
└── System/                # Real infrastructure via AppHostFixture, no mocking
    ├── Catalog/
    │   └── Endpoints/
    ├── Builds/
    │   └── Endpoints/
    ├── Compatibility/
    │   └── Endpoints/
    └── Spatial/
        └── Endpoints/
```

### Mirror Rule

Place the test file at the path that mirrors the **entry-point source file**:

- `MyPcBuild.ApiService/{path}` → `MyPcBuild.Tests/Unit/{path}`
- `MyPcBuild.ApiService/{path}` → `MyPcBuild.Tests/Integration/{path}`
- `MyPcBuild.ApiService/{path}` → `MyPcBuild.Tests/System/{path}`

For multi-class tests, use the **outermost entry-point** class to determine the path.

## Namespaces

```
MyPcBuild.Tests.Unit.{Feature}.{SubFolder}
MyPcBuild.Tests.Integration.{Feature}.{SubFolder}
MyPcBuild.Tests.System.{Feature}.{SubFolder}
```

## File Naming

`{ClassName}Tests.cs` — one entry-point class per file.

## Test Method Naming

```
MethodName_Scenario_ExpectedResult                    // unit & integration
GetProducts_ReturnsOk                                 // system (HTTP verb + resource + scenario)
PostBuild_InvalidPayload_ReturnsBadRequest
```

## Templates

- Unit → [unit-test-template.md](./references/unit-test-template.md)
- Integration → [integration-test-template.md](./references/integration-test-template.md)
- System → [system-test-template.md](./references/system-test-template.md)

## Procedure — Writing a New Test

1. **Classify**: Apply the Decision table to choose Unit / Integration / System.
2. **Locate the entry-point source file**: the class being tested (or that drives the workflow).
3. **Derive the test path**: apply the Mirror Rule.
4. **Load the template**: use the link above for the chosen tier.
5. **Set the namespace**: `MyPcBuild.Tests.{Tier}.…`
6. **Write the class**: follow the template — no `[Collection]` for Unit/Integration; primary
   constructor + `[Collection(AppHostCollection.Name)]` for System.
7. **Name test methods** per the convention.
8. **Validate**: run `dotnet test` and confirm the test is discovered and passes.

## Quick Examples

### Unit — single class

Source: `MyPcBuild.ApiService/Compatibility/Models/CompatibilityValidator.cs`
Test: `MyPcBuild.Tests/Unit/Compatibility/Models/CompatibilityValidatorTests.cs`

```csharp
namespace MyPcBuild.Tests.Unit.Compatibility.Models;

public class CompatibilityValidatorTests
{
    private readonly CompatibilityValidator _sut = new();

    [Fact]
    public void Validate_IncompatibleSocket_ReturnsError()
    {
        // Arrange … Act … Assert
    }
}
```

### Integration — multi-class workflow

Source entry-point: `MyPcBuild.ApiService/Builds/Services/BuildService.cs`
Test: `MyPcBuild.Tests/Integration/Builds/Services/BuildServiceTests.cs`

```csharp
namespace MyPcBuild.Tests.Integration.Builds.Services;

public class BuildServiceTests
{
    private readonly BuildService _sut = new(new CompatibilityValidator());

    [Fact]
    public void AddPart_IncompatibleComponent_ReturnsValidationError()
    {
        // Arrange … Act … Assert
    }
}
```

### System — endpoint with real infrastructure

Source: `MyPcBuild.ApiService/Catalog/Endpoints/ProductsEndpoints.cs`
Test: `MyPcBuild.Tests/System/Catalog/Endpoints/ProductsEndpointsTests.cs`

```csharp
using System.Net;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.System.Catalog.Endpoints;

[Collection(AppHostCollection.Name)]
public class ProductsEndpointsTests(AppHostFixture fixture)
{
    [Fact]
    public async Task GetProducts_ReturnsOk()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/catalog/products");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```
