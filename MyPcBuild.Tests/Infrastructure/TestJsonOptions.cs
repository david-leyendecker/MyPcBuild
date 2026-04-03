using System.Text.Json;
using System.Text.Json.Serialization;
using MyPcBuild.ApiService.Catalog.DTOs;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.Tests.Infrastructure;

/// <summary>
/// Provides JsonSerializerOptions for unit testing.
/// Note: Custom converters for internal domain models cannot be instantiated from test project
/// since they are marked as internal. This provides only the publicly accessible configuration.
/// For System tests, the AppHost provides the full production configuration.
/// </summary>
public static class TestJsonOptions
{
    public static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        // Register only publicly accessible converters
        options.Converters.Add(new ProductRequestJsonConverter());
        options.Converters.Add(new ProductCategoryJsonConverter());
        options.Converters.Add(new JsonStringEnumConverter());

        return options;
    }
}
