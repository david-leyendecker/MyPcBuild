using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using MyPcBuild.ApiService.Catalog.DTOs;
using MyPcBuild.ApiService.Infrastructure;

namespace MyPcBuild.Tests.Infrastructure;

/// <summary>
/// Provides JsonSerializerOptions for unit testing, matching the production configuration in Program.cs.
/// </summary>
public static class TestJsonOptions
{
    public static JsonSerializerOptions CreateOptions()
    {
        JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        // Register converters in same order as Program.cs to ensure correct priority
        options.Converters.Add(new ProductRequestJsonConverter());
        options.Converters.Add(new ProductCategoryJsonConverter());
        // Register specific enum converters before JsonStringEnumConverter
        options.Converters.Add(new ApiGpuPowerConnectorConverter());
        options.Converters.Add(new ApiGpuChipsetManufacturerConverter());
        options.Converters.Add(new ApiSidePanelTypeConverter());
        options.Converters.Add(new ApiPsuEfficiencyConverter());
        options.Converters.Add(new ApiPsuModularityConverter());
        options.Converters.Add(new ApiPsuFormFactorConverter());
        options.Converters.Add(new ApiStorageTypeConverter());
        options.Converters.Add(new ApiStorageInterfaceConverter());
        options.Converters.Add(new ApiStorageFormFactorConverter());
        options.Converters.Add(new DimensionsModelConverter());
        // Generic string enum converter for all remaining enums
        options.Converters.Add(new JsonStringEnumConverter());

        return options;
    }
}
