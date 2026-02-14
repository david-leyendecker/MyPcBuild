using Marten;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyPcBuild.ApiService.Catalog.Models;

namespace MyPcBuild.ApiService.Infrastructure;

public sealed class ProductSeeder(IDocumentStore documentStore, ILogger<ProductSeeder> logger) : BackgroundService
{
    private readonly IDocumentStore _documentStore = documentStore;
    private readonly ILogger<ProductSeeder> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await SeedProducts(stoppingToken);
    }

    private async Task SeedProducts(CancellationToken cancellationToken)
    {
        await using IDocumentSession session = _documentStore.LightweightSession();

        // Check if products already exist
        int existingCount = await session.Query<Product>().CountAsync();
        if (existingCount > 0)
        {
            _logger.LogInformation("Product catalog already seeded; skipping.");
            return; // Already seeded
        }

        // TODO: Update product seeding with proper product types
        // Products should be created using specific types:
        // - CpuProduct for CPUs
        // - MotherboardProduct for Motherboards (with Dimensions and Slots)
        // - GpuProduct for GPUs (with Dimensions and Slots)
        // - RamProduct for RAM
        // - PcCaseProduct for PC Cases (with Dimensions and Chambers)
        // - PsuProduct for PSUs
        // - StorageProduct for Storage
        // - CoolerProduct for Coolers (with Dimensions)
        
        _logger.LogInformation("Product seeding skipped - awaiting product data with new type system");
    }
}
