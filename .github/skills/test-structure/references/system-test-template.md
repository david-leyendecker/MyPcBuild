# System Test Template

System tests exercise **resources that require real infrastructure** — HTTP endpoints,
databases, message queues — running via the Aspire `AppHostFixture`. **No mocking
allowed**: every dependency must be real.

## File Location

Mirror the path of the **endpoint class** (or outermost entry point) under
`MyPcBuild.Tests/System/`:

| Source file | Test file |
|---|---|
| `MyPcBuild.ApiService/Catalog/Endpoints/ProductsEndpoints.cs` | `MyPcBuild.Tests/System/Catalog/Endpoints/ProductsEndpointsTests.cs` |
| `MyPcBuild.ApiService/Builds/Endpoints/BuildsEndpoints.cs` | `MyPcBuild.Tests/System/Builds/Endpoints/BuildsEndpointsTests.cs` |

## Namespace Convention

```
MyPcBuild.Tests.System.{Feature}.{SubFolder}
```

## Template

```csharp
using System.Net;
using MyPcBuild.Tests.Infrastructure;

namespace MyPcBuild.Tests.System.{Feature}.{SubFolder};

[Collection(AppHostCollection.Name)]
public class {EntryPointClass}Tests(AppHostFixture fixture)
{
    [Fact]
    public async Task {HttpMethod}{Resource}_{Scenario}_{ExpectedResult}()
    {
        HttpClient client = fixture.CreateApiServiceClient();
        HttpResponseMessage response = await client.GetAsync("/{route}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

## Rules

- **Always** use `[Collection(AppHostCollection.Name)]` and inject `AppHostFixture` via primary constructor.
- **Never** instantiate `AppHostFixture` directly — it is shared across all tests in the collection.
- **No mocking** — all dependencies must be real (real DB, real services). If mocking feels
  necessary, the test belongs in the Integration tier instead.
- Class name = `{EntryPointClass}Tests` where `{EntryPointClass}` is the endpoint class.
- One endpoint class (or resource entry point) per test file.
- Name test methods: `{HttpMethod}{Resource}_{Scenario}_{ExpectedResult}`
  e.g. `GetProducts_ReturnsOk`, `PostBuild_InvalidPayload_ReturnsBadRequest`.
- Deserialize response bodies with `System.Text.Json` — do not couple to internal domain types
  unless the type is already a shared DTO.
