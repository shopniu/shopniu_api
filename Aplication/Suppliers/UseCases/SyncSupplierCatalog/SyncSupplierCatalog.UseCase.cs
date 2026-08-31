using FluentValidation;
using Shopniu_api.Aplication.Suppliers.Ports;
using Shopniu_api.Domain.Entities.ProductEntity;
using Shopniu_api.Domain.Entities.SupplierEntity;
using Shopniu_api.Domain.Exceptions.Common;
using Shopniu_api.Domain.Repositories;

namespace Shopniu_api.Aplication.Suppliers.UseCases.SyncSupplierCatalog;

public sealed record SyncSupplierCatalogResult(
    int Created,
    int Updated,
    List<string> Errors
);

/// <summary>Sincroniza el catálogo de un proveedor: crea productos externos
/// nuevos, actualiza stock/costo/precio de los existentes (matcheados por
/// SupplierId + SupplierSku) y marca a cero el stock cuando el proveedor
/// reporta 0. El precio se deriva del costo con el markup configurado.
/// `actingUserId` es el dueño asignado a los productos creados (null en el
/// job de fondo se resuelve al admin configurado).</summary>
public class SyncSupplierCatalogUseCase
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IProductRepository _productRepository;
    private readonly ISupplierSyncLogRepository _syncLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISupplierCatalogProvider _catalogProvider;
    private readonly IConfiguration _configuration;
    private readonly decimal _markupPercent;

    public SyncSupplierCatalogUseCase(
        ISupplierRepository supplierRepository,
        IProductRepository productRepository,
        ISupplierSyncLogRepository syncLogRepository,
        IUnitOfWork unitOfWork,
        ISupplierCatalogProvider catalogProvider,
        IConfiguration configuration)
    {
        _supplierRepository = supplierRepository;
        _productRepository = productRepository;
        _syncLogRepository = syncLogRepository;
        _unitOfWork = unitOfWork;
        _catalogProvider = catalogProvider;
        _configuration = configuration;
        _markupPercent = configuration.GetValue<decimal>("DropShipping:MarkupPercent", 30);
    }

    public async Task<SyncSupplierCatalogResult> ExecuteAsync(
        int supplierId,
        int? actingUserId,
        CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(supplierId)
            ?? throw new NotFoundException("Supplier", supplierId);

        var ownerId = actingUserId
            ?? _configuration.GetValue("Database:Seeding:AdminUserId", 1);

        try
        {
            var items = await _catalogProvider.FetchAsync(supplier, cancellationToken);
            var created = 0;
            var updated = 0;
            var errors = new List<string>();

            foreach (var item in items)
            {
                try
                {
                    var price = Math.Round(item.CostPrice * (1 + _markupPercent / 100m), 2);
                    var existing = await _productRepository.GetBySupplierAndSkuAsync(supplier.Id, item.Sku);

                    if (existing is null)
                    {
                        var product = await _productRepository.CreateAsync(new Product(
                            name: item.Name,
                            price: price,
                            imageUrl: item.ImageUrl,
                            description: item.Description,
                            stock: item.Stock,
                            userId: ownerId,
                            sourcing: ProductSourcing.External,
                            costPrice: item.CostPrice,
                            supplierName: supplier.Name,
                            leadTimeDays: item.LeadTimeDays,
                            supplierId: supplier.Id,
                            supplierSku: item.Sku,
                            markupPercent: _markupPercent
                        ));
                        await _productRepository.AddOwnerAsync(new ProductOwner
                        {
                            Product = product,
                            UserId = ownerId
                        });
                        created++;
                    }
                    else
                    {
                        existing.Update(
                            name: item.Name,
                            price: price,
                            imageUrl: item.ImageUrl,
                            description: item.Description,
                            stock: item.Stock,
                            costPrice: item.CostPrice,
                            leadTimeDays: item.LeadTimeDays,
                            markupPercent: _markupPercent
                        );
                        existing.Sourcing = ProductSourcing.External;
                        existing.SupplierId = supplier.Id;
                        existing.SupplierSku = item.Sku;
                        existing.SupplierName = supplier.Name;
                        await _productRepository.UpdateAsync(existing);
                        updated++;
                    }
                }
                catch (Exception ex) when (ex is ValidationsException or ValidationException)
                {
                    errors.Add($"{item.Sku} ({item.Name}): {ex.Message}");
                }
            }

            await _unitOfWork.SaveChangesAsync();

            await _syncLogRepository.CreateAsync(new SupplierSyncLog(
                supplier.Id,
                DateTime.UtcNow,
                succeeded: true,
                created,
                updated,
                errors));

            await _unitOfWork.SaveChangesAsync();
            return new SyncSupplierCatalogResult(created, updated, errors);
        }
        catch (Exception ex)
        {
            await _syncLogRepository.CreateAsync(new SupplierSyncLog(
                supplier.Id,
                DateTime.UtcNow,
                succeeded: false,
                0,
                0,
                new[] { ex.Message }));

            await _unitOfWork.SaveChangesAsync();
            throw;
        }
    }
}
