# Integration Test Template

Integration tests cover a **specific workflow involving multiple classes**. Mocking is
allowed for dependencies that are out-of-scope for the workflow under test (e.g. an
external HTTP client, a system clock). **No `AppHostFixture`** — no real infrastructure
is spun up.

## File Location

Mirror the path of the **entry-point class** (the class that drives the workflow) under
`MyPcBuild.Tests/Integration/`:

| Entry-point source file | Test file |
|---|---|
| `MyPcBuild.ApiService/Compatibility/Models/CompatibilityValidator.cs` | `MyPcBuild.Tests/Integration/Compatibility/Models/CompatibilityValidatorTests.cs` |
| `MyPcBuild.ApiService/Builds/Services/BuildService.cs` | `MyPcBuild.Tests/Integration/Builds/Services/BuildServiceTests.cs` |

When the workflow spans features, pick the outermost entry point that initiates the
workflow.

## Namespace Convention

```
MyPcBuild.Tests.Integration.{Feature}.{SubFolder}
```

## Template

```csharp
using NSubstitute;
using MyPcBuild.ApiService.{Feature}.{SubFolder};
// Add further using statements as needed

namespace MyPcBuild.Tests.Integration.{Feature}.{SubFolder};

public class {EntryPointClass}Tests
{
    // Substitute only out-of-scope external dependencies
    // private readonly IExternalService _externalSub = Substitute.For<IExternalService>();

    private readonly {EntryPointClass} _sut;

    public {EntryPointClass}Tests()
    {
        // Wire up real collaborators; substitute only what cannot be instantiated simply
        _sut = new();
    }

    [Fact]
    public void {MethodName}_{Scenario}_{ExpectedResult}()
    {

    }
}
```

## Rules

- **No `AppHostFixture`**, no `[Collection(...)]` — integration tests do not start the host.
- **Substitution is allowed** only for collaborators that are genuinely external to the workflow
  (e.g. third-party HTTP clients, system clock). Use `Substitute.For<T>()`. Prefer real implementations.
- Class name = `{EntryPointClass}Tests` where `{EntryPointClass}` is the class that drives
  the workflow.
- One workflow entry point per test file.
- Name test methods: `MethodName_Scenario_ExpectedResult`.
- Follow Arrange-Act-Assert sections.
