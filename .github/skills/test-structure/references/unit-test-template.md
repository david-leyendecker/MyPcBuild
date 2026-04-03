# Unit Test Template

Unit tests cover **exactly one class** in isolation. **No mocking allowed** — if the class
under test needs collaborators, those must be supplied as real instances (e.g. a fresh
`new()` of a simple value object). If mocking feels necessary, reconsider whether the test
belongs in the Integration tier instead.

## File Location

Mirror the source path under `MyPcBuild.Tests/Unit/`:

| Source file | Test file |
|---|---|
| `MyPcBuild.ApiService/Catalog/Services/ProductService.cs` | `MyPcBuild.Tests/Unit/Catalog/Services/ProductServiceTests.cs` |
| `MyPcBuild.ApiService/Spatial/Models/SpatialValidator.cs` | `MyPcBuild.Tests/Unit/Spatial/Models/SpatialValidatorTests.cs` |
| `MyPcBuild.ApiService/Builds/Models/Build.cs` | `MyPcBuild.Tests/Unit/Builds/Models/BuildTests.cs` |

## Namespace Convention

```
MyPcBuild.Tests.Unit.{Feature}.{SubFolder}
```
Mirrors the source namespace with `Unit` inserted after the project root.

## Template

```csharp
using MyPcBuild.ApiService.{Feature}.{SubFolder};
// Add further using statements as needed

namespace MyPcBuild.Tests.Unit.{Feature}.{SubFolder};

public class {ClassName}Tests
{
    private readonly {ClassName} _sut = new();

    [Fact]
    public void {MethodName}_{Scenario}_{ExpectedResult}()
    {

    }
}
```

## Rules

- **No mocking** — no `Substitute.For<T>()`, no fakes, no stubs. Real objects only.
- **No `AppHostFixture`**, no `[Collection(...)]` — unit tests are self-contained.
- One class per test file. File name = `{ClassName}Tests.cs`.
- Name test methods: `MethodName_Scenario_ExpectedResult`.
