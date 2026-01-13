using System;
using System.Text.Json.Serialization;
using Marten;
using Marten.Events.Projections;
using Microsoft.Extensions.AI;
using MyPcBuild.ApiService.Domain.Events;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Features.Builds;
using MyPcBuild.ApiService.Features.Catalog;
using MyPcBuild.ApiService.Features.Compatibility;
using MyPcBuild.ApiService.Features.Spatial;
using MyPcBuild.ApiService.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations
builder.AddServiceDefaults();
builder.Services.AddLogging();

// Add services to the container
builder.Services.AddProblemDetails();
builder.Services.AddHttpContextAccessor();

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
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline
app.UseExceptionHandler();

// Enable CORS
app.UseCors();

// Map feature endpoints
app.MapBuildEndpoints();
app.MapCatalogEndpoints();
app.MapCompatibilityEndpoints();
app.MapSpatialEndpoints();

app.MapDefaultEndpoints();

app.Run();
