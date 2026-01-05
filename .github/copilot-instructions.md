# GitHub Copilot Instructions for PCBuilder Project

## C# Code Style Guidelines

### Type Declarations
- **Use explicit types instead of `var` (IDE0008)**
  - ✅ `string name = "example";`
  - ✅ `List<Product> products = new();`
  - ❌ `var name = "example";`
  - ❌ `var products = new List<Product>();`
  - Exception: `var` is allowed within the MyPcBuild.AppHost (Aspire AppHost) project for host-specific code.

### Object Creation
- **Use target-typed `new()` expressions (IDE0090)**
  - ✅ `Product product = new();`
  - ✅ `List<string> items = new();`
  - ❌ `Product product = new Product();`
  - ❌ `List<string> items = new List<string>();`

### Collection Initializers
- **Use collection expressions `[]` for collections (IDE0028, C# 12+)**
  - ✅ `List<int> numbers = [1, 2, 3];`
  - ✅ `string[] names = ["Alice", "Bob"];`
  - ❌ `List<int> numbers = new() { 1, 2, 3 };`
  - ❌ `string[] names = new[] { "Alice", "Bob" };`

### Null Handling
- **Use null-coalescing assignment `??=`**
  - ✅ `_cache ??= new();`
  - ❌ `if (_cache == null) _cache = new();`

- **Use null-conditional operators `?.` and `?[]`**
  - ✅ `int? length = array?.Length;`
  - ✅ `string? value = dict?["key"];`

### Pattern Matching
- **Use pattern matching for type checks and casts**
  - ✅ `if (obj is Product product) { }`
  - ✅ `return value is >= 0 and < 100;`
  - ❌ `if (obj is Product) { var product = (Product)obj; }`

### String Handling
- **Use string interpolation over concatenation**
  - ✅ `string msg = $"Hello, {name}!";`
  - ❌ `string msg = "Hello, " + name + "!";`

- **Use raw string literals for multi-line or complex strings (C# 11+)**
  - ✅ `string json = """{"name": "value"}""";`

### LINQ and Collections
- **Prefer LINQ methods over manual loops when appropriate**
  - ✅ `bool hasError = issues.Any(i => i.Severity == IssueSeverity.Error);`
  - ❌ Manual foreach with flags

- **Use collection initializers and expression syntax**
  - ✅ `List<string> names = items.Select(i => i.Name).ToList();`

### Async/Await
- **Always use `async`/`await` for asynchronous operations**
  - Use `Task` and `Task<T>` return types
  - Avoid `async void` except for event handlers
  - Use `ConfigureAwait(false)` in library code when appropriate

### Naming Conventions
- **PascalCase**: Classes, methods, properties, public fields
- **camelCase**: Private fields, local variables, parameters
- **_camelCase**: Private instance fields (prefix with underscore)
- **UPPER_CASE**: Constants

### Access Modifiers
- **Always specify access modifiers explicitly**
  - ✅ `public class MyClass`
  - ✅ `private readonly IService _service;`
  - ❌ Omitting modifiers

### Records and Primary Constructors
- **Use records for immutable data models**
  - ✅ `public record Product(Guid Id, string Name, decimal Price);`

- **Use primary constructors for simple classes (C# 12+)**
  - ✅ `public class Service(ILogger logger) { }`

### Exception Handling
- **Use specific exception types**
- **Always provide meaningful error messages**
- **Don't catch exceptions you can't handle**

### Comments and Documentation
- **Use XML documentation comments for public APIs**
  ```csharp
  /// <summary>
  /// Validates PC component compatibility.
  /// </summary>
  /// <param name="products">List of products to validate.</param>
  /// <returns>Compatibility validation result.</returns>
  ```

- **Avoid obvious comments; code should be self-documenting**
- **Comment complex logic or business rules**

### Minimal API Best Practices
- **Use route groups for organizing endpoints**
- **Leverage endpoint filters for cross-cutting concerns**
- **Use `Results<T>` for typed responses**

### Dependency Injection
- **Register services with appropriate lifetime**
  - Singleton: Stateless services, caches
  - Scoped: Per-request services, DB contexts
  - Transient: Lightweight, stateless services

### Testing
- **Use Arrange-Act-Assert pattern**
- **Name tests descriptively: `MethodName_Scenario_ExpectedResult`**
- **Use explicit assertions with clear failure messages**

## Project-Specific Rules

### Domain Models
- Use record types for immutable entities
- Use dictionaries for flexible specifications
- Keep models in `Domain/Models` namespace

### Services
- Define interfaces for all services
- Keep business logic in service layer
- Use dependency injection for all dependencies

### API Design
- Follow REST conventions
- Use proper HTTP status codes
- Return consistent response shapes
- Include proper error handling middleware
