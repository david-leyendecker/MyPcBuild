using System.Text.Json;
using System.Text.Json.Serialization;
using Marten;
using Marten.Events.Projections;
using Microsoft.Extensions.AI;
using MyPcBuild.ApiService.Builds.Endpoints;
using MyPcBuild.ApiService.Builds.Events;
using MyPcBuild.ApiService.Builds.Models;
using MyPcBuild.ApiService.Catalog.DTOs;
using MyPcBuild.ApiService.Catalog.Endpoints;
using MyPcBuild.ApiService.Catalog.Models;
using MyPcBuild.ApiService.Catalog.Services;
using MyPcBuild.ApiService.Compatibility.Endpoints;
using MyPcBuild.ApiService.Compatibility.Models;
using MyPcBuild.ApiService.Spatial.Endpoints;
using MyPcBuild.ApiService.Spatial.Models;
using MyPcBuild.ApiService.Infrastructure;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations
builder.AddServiceDefaults();
builder.Services.AddLogging();

// Add services to the container
builder.Services.AddProblemDetails();
builder.Services.AddHttpContextAccessor();

// Add OpenAPI
builder.Services.AddOpenApi();

// Add CORS for Vue.js client
string allowedOrigins = builder.Configuration["AllowedOrigins"]
    ?? throw new InvalidOperationException("AllowedOrigins configuration not found. Please set the AllowedOrigins environment variable.");

string[] origins = allowedOrigins.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddSingleton<ProductCategoryPromptFields>();

// Register AI product generator
builder.Services.AddScoped<IAiProductGenerator, OpenAiProductGenerator>();

// Register compatibility validator
builder.Services.AddScoped<ICompatibilityValidator, CompatibilityValidator>();

// Register spatial validator
builder.Services.AddScoped<ISpatialValidator, SpatialValidator>();

// Add OpenAPI
builder.AddAzureChatCompletionsClient("chat")
    .AddChatClient();

// Add Marten for Event Sourcing
string connectionString = builder.Configuration.GetConnectionString("mypcbuild") ?? throw new InvalidOperationException("Connection string 'mypcbuild' not found.");

builder.Services.AddMarten(opts =>
{
    opts.Connection(connectionString);

    // Configure event sourcing for Build aggregate
    opts.Events.AddEventTypes([
        typeof(BuildCreated),
        typeof(PartAdded),
        typeof(PartAddedToSlot),
        typeof(PartRemoved),
        typeof(BuildRenamed)
    ]);

    // Use Build as the aggregate with inline projection
    opts.Projections.Snapshot<Build>(SnapshotLifecycle.Inline);

    // Configure Product hierarchy for polymorphic storage
    opts.Schema.For<Product>()
        .AddSubClass<CpuProduct>()
        .AddSubClass<MotherboardProduct>()
        .AddSubClass<GpuProduct>()
        .AddSubClass<RamProduct>()
        .AddSubClass<PcCaseProduct>()
        .AddSubClass<PsuProduct>()
        .AddSubClass<StorageProduct>()
        .AddSubClass<CoolerProduct>();
}).UseLightweightSessions();

builder.Services.AddHostedService<ProductSeeder>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    // Add custom converter for ProductRequest to handle polymorphic deserialization
    options.SerializerOptions.Converters.Add(new ProductRequestJsonConverter());
    // Add custom converter for ProductCategory to handle case-insensitive deserialization
    options.SerializerOptions.Converters.Add(new ProductCategoryJsonConverter());
    // Register specific enum converters before JsonStringEnumConverter.
    // Global converters take precedence over type-level [JsonConverter] attributes,
    // so these must appear in the list first to handle non-standard string representations.
    options.SerializerOptions.Converters.Add(new ApiGpuPowerConnectorConverter());
    options.SerializerOptions.Converters.Add(new ApiGpuChipsetManufacturerConverter());
    options.SerializerOptions.Converters.Add(new ApiSidePanelTypeConverter());
    options.SerializerOptions.Converters.Add(new ApiPsuEfficiencyConverter());
    options.SerializerOptions.Converters.Add(new ApiPsuModularityConverter());
    options.SerializerOptions.Converters.Add(new ApiPsuFormFactorConverter());
    options.SerializerOptions.Converters.Add(new ApiStorageTypeConverter());
    options.SerializerOptions.Converters.Add(new ApiStorageInterfaceConverter());
    options.SerializerOptions.Converters.Add(new ApiStorageFormFactorConverter());
    options.SerializerOptions.Converters.Add(new DimensionsModelConverter());
    // Generic string enum converter for all remaining enums
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline
app.UseExceptionHandler();

// Map OpenAPI endpoint and Scalar UI
app.MapOpenApi();
app.MapScalarApiReference();

// Enable CORS
app.UseCors();

// Map feature endpoints
app.MapBuildEndpoints();
app.MapCatalogEndpoints();
app.MapCompatibilityEndpoints();
app.MapSpatialEndpoints();

app.MapDefaultEndpoints();

app.Run();
