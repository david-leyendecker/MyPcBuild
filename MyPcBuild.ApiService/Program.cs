using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Marten.Pagination;
using Weasel.Core;
using MyPcBuild.ApiService.Data;
using MyPcBuild.ApiService.Domain.Events;
using MyPcBuild.ApiService.Domain.Models;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Marten for Event Sourcing
var connectionString = builder.Configuration.GetConnectionString("postgres") 
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
    opts.Projections.Snapshot<Build>(Marten.Events.Projections.SnapshotLifecycle.Inline);
}).UseLightweightSessions();

var app = builder.Build();

// Seed product catalog on startup
using (var scope = app.Services.CreateScope())
{
    var documentStore = scope.ServiceProvider.GetRequiredService<IDocumentStore>();
    await ProductSeeder.SeedProducts(documentStore);
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Build Management Endpoints
app.MapPost("/api/builds", async (CreateBuildRequest request, IDocumentSession session) =>
{
    var buildId = Guid.NewGuid();
    var @event = new BuildCreated { BuildId = buildId, Name = request.Name, UserId = request.UserId };
    
    session.Events.StartStream<Build>(buildId, @event);
    await session.SaveChangesAsync();
    
    return Results.Created($"/api/builds/{buildId}", new { BuildId = buildId });
})
.WithName("CreateBuild");

app.MapPost("/api/builds/{buildId:guid}/parts", async (Guid buildId, AddPartRequest request, IDocumentSession session) =>
{
    var @event = new PartAdded { BuildId = buildId, ProductId = request.ProductId, PricePaid = request.PricePaid };
    
    session.Events.Append(buildId, @event);
    await session.SaveChangesAsync();
    
    return Results.Ok();
})
.WithName("AddPart");

app.MapDelete("/api/builds/{buildId:guid}/parts/{productId:guid}", async (Guid buildId, Guid productId, IDocumentSession session) =>
{
    var @event = new PartRemoved { BuildId = buildId, ProductId = productId };
    
    session.Events.Append(buildId, @event);
    await session.SaveChangesAsync();
    
    return Results.Ok();
})
.WithName("RemovePart");

app.MapGet("/api/builds/{buildId:guid}", async (Guid buildId, IDocumentSession session) =>
{
    var build = await session.Events.AggregateStreamAsync<Build>(buildId);
    return build is null ? Results.NotFound() : Results.Ok(build);
})
.WithName("GetBuild");

// Product Catalog Endpoints
app.MapGet("/api/catalog/products", async (
    IDocumentSession session,
    ProductCategory? category = null,
    string? search = null,
    int page = 1,
    int pageSize = 20) =>
{
    IQueryable<Product> query = session.Query<Product>();

    if (category.HasValue)
    {
        query = query.Where(p => p.Category == category.Value);
    }

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(p => p.Name.Contains(search) || p.Manufacturer.Contains(search));
    }

    var products = await query
        .OrderBy(p => p.Category)
        .ThenBy(p => p.Name)
        .ToPagedListAsync(page, pageSize);

    return Results.Ok(new
    {
        products.Count,
        products.PageCount,
        products.PageNumber,
        products.PageSize,
        Items = products
    });
})
.WithName("GetProducts");

app.MapGet("/api/catalog/products/{id:guid}", async (Guid id, IDocumentSession session) =>
{
    var product = await session.LoadAsync<Product>(id);
    return product is null ? Results.NotFound() : Results.Ok(product);
})
.WithName("GetProduct");

app.MapGet("/api/catalog/categories", () =>
{
    var categories = Enum.GetValues<ProductCategory>()
        .Select(c => new { Value = (int)c, Name = c.ToString() });
    return Results.Ok(categories);
})
.WithName("GetCategories");

app.MapGet("/api/catalog/search", async (
    IDocumentSession session,
    string query,
    int maxResults = 10) =>
{
    if (string.IsNullOrWhiteSpace(query))
    {
        return Results.Ok(Array.Empty<Product>());
    }

    var results = await session.Query<Product>()
        .Where(p => p.Name.Contains(query) || p.Manufacturer.Contains(query))
        .Take(maxResults)
        .ToListAsync();

    return Results.Ok(results);
})
.WithName("SearchProducts");

app.MapDefaultEndpoints();

app.Run();

record CreateBuildRequest(string Name, Guid UserId);
record AddPartRequest(Guid ProductId, decimal PricePaid);
