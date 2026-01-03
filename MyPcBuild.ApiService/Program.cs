using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Marten.Pagination;
using Weasel.Core;
using MyPcBuild.ApiService.Data;
using MyPcBuild.ApiService.Domain.Events;
using MyPcBuild.ApiService.Domain.Models;
using MyPcBuild.ApiService.Services;
using MyPcBuild.ApiService.Mappers;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddHttpContextAccessor();

// Register compatibility validator and response mapper
builder.Services.AddScoped<ICompatibilityValidator, CompatibilityValidator>();
builder.Services.AddScoped<IResponseMapper, ResponseMapper>();

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
app.MapPost("/api/builds", async (
    CreateBuildRequest request,
    IDocumentSession session,
    IResponseMapper mapper) =>
{
    var buildId = Guid.NewGuid();
    var @event = new BuildCreated { BuildId = buildId, Name = request.Name, UserId = request.UserId };
    
    session.Events.StartStream<Build>(buildId, @event);
    await session.SaveChangesAsync();
    
    var response = mapper.MapBuildCreated(buildId, request.Name, request.UserId);
    return Results.Created($"/api/builds/{buildId}", response);
})
.WithName("CreateBuild");

app.MapPost("/api/builds/{buildId:guid}/parts", async (
    Guid buildId,
    AddPartRequest request,
    IDocumentSession session) =>
{
    var @event = new PartAdded { BuildId = buildId, ProductId = request.ProductId, PricePaid = request.PricePaid };
    
    session.Events.Append(buildId, @event);
    await session.SaveChangesAsync();
    
    return Results.Ok(new { Message = "Part added successfully" });
})
.WithName("AddPart");

app.MapDelete("/api/builds/{buildId:guid}/parts/{productId:guid}", async (
    Guid buildId,
    Guid productId,
    IDocumentSession session) =>
{
    var @event = new PartRemoved { BuildId = buildId, ProductId = productId };
    
    session.Events.Append(buildId, @event);
    await session.SaveChangesAsync();
    
    return Results.Ok(new { Message = "Part removed successfully" });
})
.WithName("RemovePart");

app.MapGet("/api/builds/{buildId:guid}", async (
    Guid buildId,
    IDocumentSession session,
    IResponseMapper mapper,
    ICompatibilityValidator validator) =>
{
    var build = await session.Events.AggregateStreamAsync<Build>(buildId);
    if (build is null)
    {
        return Results.NotFound();
    }
    
    // Load products for the build
    var products = new List<Product>();
    foreach (var part in build.Parts)
    {
        var product = await session.LoadAsync<Product>(part.ProductId);
        if (product != null)
        {
            products.Add(product);
        }
    }
    
    // Run compatibility validation
    CompatibilityResult? compatibilityResult = null;
    if (products.Any())
    {
        compatibilityResult = await validator.ValidateBuild(products);
    }
    
    var response = mapper.MapBuild(build, products, compatibilityResult);
    return Results.Ok(response);
})
.WithName("GetBuild");

// Product Catalog Endpoints
app.MapGet("/api/catalog/products", async (
    IDocumentSession session,
    IResponseMapper mapper,
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

    var totalCount = await query.CountAsync();
    var products = await query
        .OrderBy(p => p.Category)
        .ThenBy(p => p.Name)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    var response = mapper.MapProductCatalog(products, totalCount, page, pageSize, category, search);

    return Results.Ok(response);
})
.WithName("GetProducts");

app.MapGet("/api/catalog/products/{id:guid}", async (
    Guid id,
    IDocumentSession session,
    IResponseMapper mapper) =>
{
    var product = await session.LoadAsync<Product>(id);
    if (product is null)
    {
        return Results.NotFound();
    }
    
    var response = mapper.MapProduct(product);
    return Results.Ok(response);
})
.WithName("GetProduct");

app.MapGet("/api/catalog/categories", async (
    IDocumentSession session,
    IResponseMapper mapper) =>
{
    // Get product counts per category
    var allProducts = await session.Query<Product>().ToListAsync();
    var productCounts = allProducts.GroupBy(p => p.Category)
        .ToDictionary(g => g.Key, g => g.Count());
    
    var response = mapper.MapCategories(productCounts);
    return Results.Ok(response);
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

// Compatibility Validation Endpoints
app.MapPost("/api/compatibility/validate", async (
    ValidateCompatibilityRequest request,
    IDocumentSession session,
    ICompatibilityValidator validator,
    IResponseMapper mapper) =>
{
    if (request.ProductIds == null || !request.ProductIds.Any())
    {
        return Results.BadRequest("Product IDs are required");
    }

    // Load all products
    var products = new List<Product>();
    foreach (var productId in request.ProductIds)
    {
        var product = await session.LoadAsync<Product>(productId);
        if (product != null)
        {
            products.Add(product);
        }
    }

    if (!products.Any())
    {
        return Results.BadRequest("No valid products found");
    }

    // Validate compatibility
    var result = await validator.ValidateBuild(products);
    
    // Map to response DTO
    var response = mapper.MapCompatibilityResult(result, products);

    return Results.Ok(response);
})
.WithName("ValidateCompatibility");

app.MapGet("/api/builds/{buildId:guid}/compatibility", async (
    Guid buildId,
    IDocumentSession session,
    ICompatibilityValidator validator,
    IResponseMapper mapper) =>
{
    // Load build
    var build = await session.Events.AggregateStreamAsync<Build>(buildId);
    if (build == null)
    {
        return Results.NotFound();
    }

    // Load all products in the build
    var products = new List<Product>();
    foreach (var part in build.Parts)
    {
        var product = await session.LoadAsync<Product>(part.ProductId);
        if (product != null)
        {
            products.Add(product);
        }
    }

    // Validate compatibility
    var result = await validator.ValidateBuild(products);
    
    // Map to response DTO with build context
    var response = mapper.MapCompatibilityResult(result, products, buildId.ToString());

    return Results.Ok(response);
})
.WithName("GetBuildCompatibility");

app.MapDefaultEndpoints();

app.Run();

record CreateBuildRequest(string Name, Guid UserId);
record AddPartRequest(Guid ProductId, decimal PricePaid);
record ValidateCompatibilityRequest(List<Guid> ProductIds);
