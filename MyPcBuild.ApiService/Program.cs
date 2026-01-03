using Marten;
using Marten.Events.Projections;
using MyPcBuild.ApiService.Domain.Events;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Features.Builds;
using MyPcBuild.ApiService.Features.Catalog;
using MyPcBuild.ApiService.Features.Compatibility;
using MyPcBuild.ApiService.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddProblemDetails();
builder.Services.AddHttpContextAccessor();

// Register compatibility validator
builder.Services.AddScoped<ICompatibilityValidator, CompatibilityValidator>();

// Add OpenAPI
builder.Services.AddOpenApi();

// Add Marten for Event Sourcing
string connectionString = builder.Configuration.GetConnectionString("postgres") 
    ?? "Host=localhost;Database=mypcbuild;Username=postgres;Password=postgres";

builder.Services.AddMarten(opts =>
{
    opts.Connection(connectionString);
    
    // Configure event sourcing for Build aggregate
    opts.Events.AddEventTypes([
        typeof(BuildCreated),
        typeof(PartAdded),
        typeof(PartRemoved),
        typeof(BuildRenamed)
    ]);
    
    // Use Build as the aggregate with inline projection
    opts.Projections.Snapshot<Build>(SnapshotLifecycle.Inline);
}).UseLightweightSessions();

WebApplication app = builder.Build();

// Seed product catalog on startup
using (IServiceScope scope = app.Services.CreateScope())
{
    IDocumentStore documentStore = scope.ServiceProvider.GetRequiredService<IDocumentStore>();
    await ProductSeeder.SeedProducts(documentStore);
}

// Configure the HTTP request pipeline
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Map feature endpoints
app.MapBuildEndpoints();
app.MapCatalogEndpoints();
app.MapCompatibilityEndpoints();

app.MapDefaultEndpoints();

app.Run();
