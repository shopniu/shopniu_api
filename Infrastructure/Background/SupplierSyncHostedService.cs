using Shopniu_api.Aplication.Suppliers.Ports;
using Shopniu_api.Aplication.Suppliers.UseCases.SyncSupplierCatalog;
using Shopniu_api.Domain.Repositories;

namespace Shopniu_api.Infrastructure.Background;

/// <summary>Sincroniza el catálogo de los proveedores activos de forma
/// periódica (`DropShipping:SyncIntervalMinutes`). Se habilita con
/// `DropShipping:SyncEnabled=true`. Los productos creados por el job quedan
/// asignados al admin configurado (`Database:Seeding:AdminUserId`).</summary>
public class SupplierSyncHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SupplierSyncHostedService> _logger;

    public SupplierSyncHostedService(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<SupplierSyncHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_configuration.GetValue("DropShipping:SyncEnabled", false))
        {
            _logger.LogInformation("Supplier sync disabled (DropShipping:SyncEnabled=false).");
            return;
        }

        var intervalMinutes = _configuration.GetValue("DropShipping:SyncIntervalMinutes", 15);
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(Math.Max(1, intervalMinutes)));

        do
        {
            await SyncAllAsync(stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested);
    }

    private async Task SyncAllAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var supplierRepository = scope.ServiceProvider.GetRequiredService<ISupplierRepository>();
        var syncUseCase = scope.ServiceProvider.GetRequiredService<SyncSupplierCatalogUseCase>();
        var adminId = _configuration.GetValue("Database:Seeding:AdminUserId", 1);

        var suppliers = await supplierRepository.GetActiveAsync();
        foreach (var supplier in suppliers)
        {
            try
            {
                var result = await syncUseCase.ExecuteAsync(supplier.Id, adminId, cancellationToken);
                _logger.LogInformation(
                    "Supplier {SupplierId} sync finished: created={Created} updated={Updated} errors={Errors}",
                    supplier.Id,
                    result.Created,
                    result.Updated,
                    result.Errors.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Supplier sync failed for {SupplierId}", supplier.Id);
            }
        }
    }
}
